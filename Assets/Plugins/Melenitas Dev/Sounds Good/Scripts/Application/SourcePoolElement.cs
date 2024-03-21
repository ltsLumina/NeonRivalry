/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MelenitasDev.SoundsGood
{
    public partial class SourcePoolElement // Fields
    {
        private AudioSource source;
        private Transform followTarget = null;
        private float volume = 1;
        private Vector2 hearDistance = new Vector2(3, 500);
        private float fadeOutTime = 0;
        private float fadeInTime = 0;
        private bool loop = false;
        private bool stopping = false;
        private bool changingTrack = false;
        private bool isPlaylist;
        private Queue<AudioClip> playlist = new Queue<AudioClip>();
        private float playingTimeForNextSong = 0;
        private float playlistDuration;
        private AudioClip cachedClipOnPause = null;
        
        private Coroutine lerpVolumeCor;
        private Coroutine fadeInOnChangeTrackCor;

        private SourceState currentState = SourceState.Stopped;
        
        private enum SourceState
        {
            Playing,
            Paused,
            Pausing,
            FadingIn,
            ChangingTrack,
            Stopping,
            Stopped
        }
    }

    public partial class SourcePoolElement // Fields (Callbacks)
    {
        private Action onPlay;
        private Action onComplete;
        private Action onLoopCycleComplete;
        private Action onNextTrackStart;
        private Action onPause;
        private Action onPauseComplete;
        private Action onResume;
    }
    
    public partial class SourcePoolElement // Properties
    {
        internal bool Using { get; private set; }
        internal bool Playing => source.isPlaying;
        internal bool Paused { get; private set; }
        internal string Id { get; private set; }
        internal float PlayingTime { get; private set; }
        internal float CurrentLoopCycleTime => source.time;
        internal int CompletedLoopCycles { get; private set; }
        internal int ReproducedTracks { get; private set; }
        internal float CurrentClipDuration => source.clip.length;
        internal AudioClip CurrentClip => source.clip;
        internal AudioClip NextPlaylistClip => playlist.Peek();
    }

    public partial class SourcePoolElement : MonoBehaviour
    {
        void Update ()
        {
            if (!Using) return;

            if (!loop)
            {
                if (!isPlaylist) HandleSoundStop();
                else HandlePlaylistStop();
            }

            if (Playing)
            {
                if (!isPlaylist) HandleSoundPlaying();
                else HandlePlaylistPlaying();
            }

            if (followTarget == null) return;
            
            transform.position = followTarget.position;
        }
    }

    public partial class SourcePoolElement // Internal Methods
    {
        internal SourcePoolElement Init (AudioSource source)
        {
            this.source = source;
            return this;
        }

        internal SourcePoolElement MarkAsPlaylist ()
        {
            isPlaylist = true;
            return this;
        }
        
        internal SourcePoolElement SetVolume (float volume, Vector2 hearDistance, float lerpTime = 0)
        {
            this.volume = volume;
            this.hearDistance = hearDistance;
            source.minDistance = hearDistance.x;
            source.maxDistance = hearDistance.y;
            
            if (currentState == SourceState.Paused || currentState == SourceState.Stopping ||
                currentState == SourceState.ChangingTrack)
            {
                Debug.LogWarning($"There's a volume fade out taking place at this moment, " +
                                 "so volume won't change right now, but on the next fade in it will go " +
                                 $"up until the new volume of {volume}");
                return this;
            }

            if (lerpTime <= 0) source.volume = volume;
            else
            {
                lerpVolumeCor = StartCoroutine(LerpVolume(volume, lerpTime));
            }
            return this;
        }

        internal SourcePoolElement SetPitch (float pitch)
        {
            source.pitch = pitch;
            return this;
        }

        internal SourcePoolElement SetClip (AudioClip audioClip)
        {
            source.clip = audioClip;
            return this;
        }
        
        internal SourcePoolElement SetPlaylist (Queue<AudioClip> playlist)
        {
            this.playlist = new Queue<AudioClip>(playlist);
            playlistDuration = 0;
            foreach (AudioClip clip in playlist) playlistDuration += clip != null ? clip.length : 0;
            return this;
        }
        
        internal void AddToPlaylist (AudioClip addedClip)
        {
            playlistDuration += addedClip.length;
            playlist.Enqueue(addedClip);
        }

        internal SourcePoolElement SetId (string id)
        {
            Id = id;
            return this;
        }

        internal SourcePoolElement SetSpatialSound (bool activate)
        {
            source.spatialBlend = activate ? 1 : 0;
            return this;
        }
        
        internal SourcePoolElement SetPosition (Vector3 position)
        {
            transform.position = position;
            return this;
        }
        
        internal SourcePoolElement SetFollowTarget (Transform followTarget)
        {
            this.followTarget = followTarget;
            return this;
        }
        
        internal SourcePoolElement SetFadeIn (float fadeInTime)
        {
            this.fadeInTime = fadeInTime;
            return this;
        }
        
        internal SourcePoolElement SetFadeOut (float fadeOutTime)
        {
            this.fadeOutTime = fadeOutTime;
            return this;
        }
        
        internal SourcePoolElement SetLoop (bool loop)
        {
            this.loop = loop;
            source.loop = loop;
            return this;
        }

        internal SourcePoolElement SetOutput (AudioMixerGroup output)
        {
            source.outputAudioMixerGroup = output;
            return this;
        }

        internal SourcePoolElement OnPlay (Action onPlay)
        {
            this.onPlay = onPlay;
            return this;
        }
        
        internal SourcePoolElement OnComplete (Action onComplete)
        {
            this.onComplete = onComplete;
            return this;
        }
        
        internal SourcePoolElement OnLoopCycleComplete (Action onLoopCycleComplete)
        {
            this.onLoopCycleComplete = onLoopCycleComplete;
            return this;
        }
        
        internal SourcePoolElement OnNextTrackStart (Action onNextTrackStart)
        {
            this.onNextTrackStart = onNextTrackStart;
            return this;
        }
        
        internal SourcePoolElement OnPause (Action onPause)
        {
            this.onPause = onPause;
            return this;
        }
        
        internal SourcePoolElement OnPauseComplete (Action onPauseComplete)
        {
            this.onPauseComplete = onPauseComplete;
            return this;
        }
        
        internal SourcePoolElement OnResume (Action onResume)
        {
            this.onResume = onResume;
            return this;
        }
        
        internal void Play (float fadeInTime = 0)
        {
            if (source.clip == null)
            {
                Debug.LogError("No audio clip found, make sure you have initialized it in a method (not in the declaration)");
                return;
            }
            
            Using = true;
            Paused = false;
            PlayingTime = 0;
            CompletedLoopCycles = 0;
            
            onPlay?.Invoke();
            
            source.Play();
            ChangeState(SourceState.Playing);
            enabled = true;
            
            if (fadeInTime > 0)
            {
                ChangeState(SourceState.FadingIn);
                source.volume = 0;
                lerpVolumeCor = StartCoroutine(LerpVolume(volume, fadeInTime,
                    (() => ChangeState(SourceState.Playing))));
            }
        }
        
        internal void PlayPlaylist (float fadeInTime)
        {
            bool validClips = true;
            for (int i = 0; i < playlist.Count; i++)
            {
                if (playlist.ToArray()[i] != null) continue;
                validClips = false;
            }
            if (!validClips)
            {
                Debug.LogError("There are invalid audio clips in playlist, make sure you have " +
                                 "initialized it in a method (not in the declaration)");
                return;
            }
            
            Using = true;
            Paused = false;
            PlayingTime = 0;
            ReproducedTracks = 0;
            CompletedLoopCycles = 0;
            playingTimeForNextSong = 0;
            if (loop) source.loop = false;
            changingTrack = false;

            PlayNextSong();
            enabled = true;
            
            if (fadeInTime > 0)
            {
                ChangeState(SourceState.FadingIn);
                source.volume = 0;
                lerpVolumeCor = StartCoroutine(LerpVolume(volume, fadeInTime,
                    (() => ChangeState(SourceState.Playing))));
            }
        }

        internal void Pause (float fadeOutTime = 0)
        {
            if (!Using) return;
            if (Paused) return;
            
            Paused = true;
            cachedClipOnPause = CurrentClip;
            
            onPause?.Invoke();

            void CompletePause ()
            {
                onPauseComplete?.Invoke();
                source.Pause();
                ChangeState(SourceState.Paused);
            }

            if (changingTrack)
            {
                CompletePause();
                return;
            }
            
            if (fadeOutTime > 0)
            {
                if (currentState == SourceState.FadingIn)
                {
                    StopCoroutine(fadeInOnChangeTrackCor);
                    fadeInOnChangeTrackCor = null;
                }
                StopLerpCoroutine();
                ChangeState(SourceState.Pausing);
                lerpVolumeCor = StartCoroutine(LerpVolume(0, fadeOutTime, CompletePause, true));
                return;
            }
            CompletePause();
        }
        
        internal void Resume (float fadeInTime = 0)
        {
            if (!Paused) return;

            onResume?.Invoke();
            
            Paused = false;
            source.UnPause();
            ChangeState(SourceState.Playing);

            if (changingTrack) return;

            if (fadeInTime > 0)
            {
                StopLerpCoroutine();
                ChangeState(SourceState.FadingIn);
                lerpVolumeCor = StartCoroutine(LerpVolume(volume, fadeInTime,
                    (() => ChangeState(SourceState.Playing))));
            }
        }
        
        internal void Stop (float fadeOutTime = 0, Action onStop = null)
        {
            if (fadeOutTime > 0)
            {
                stopping = true;
                ChangeState(SourceState.Stopping);
                lerpVolumeCor = StartCoroutine(LerpVolume(0, fadeOutTime, () => Stop(0, onStop)));
                return;
            }
            
            onComplete?.Invoke();
            onStop?.Invoke();
            
            if (!source) return;
            source.Stop();
            ChangeState(SourceState.Stopped);
            source.clip = null;
            
            followTarget = null;
            
            onComplete = null;
            onLoopCycleComplete = null;
            onPause = null;
            onPauseComplete = null;
            onResume = null;

            Id = null;

            changingTrack = false;
            stopping = false;
            Using = false;
            enabled = false;
        }
    }

    public partial class SourcePoolElement // Private Methods
    {
        private void HandleSoundPlaying ()
        {
            PlayingTime += Time.deltaTime;

            if (!loop) return;
            if (PlayingTime > CurrentClipDuration * CompletedLoopCycles + 1) return;
            
            CompletedLoopCycles++;
                
            onLoopCycleComplete?.Invoke();
        }

        private void HandlePlaylistPlaying ()
        {
            PlayingTime += Time.deltaTime;
            
            void CompleteLoopCycle ()
            {
                CompletedLoopCycles++;
                
                onLoopCycleComplete?.Invoke();
            }

            if (loop)
            {
                if (CurrentLoopCycleTime >= playlistDuration - fadeOutTime)
                {
                    CompleteLoopCycle();
                }
            }

            if (PlayingTime >= playingTimeForNextSong - fadeOutTime && !changingTrack)
            {
                changingTrack = true;
                ChangeState(SourceState.ChangingTrack);
                if (fadeOutTime > 0) 
                    lerpVolumeCor = StartCoroutine(LerpVolume(0, fadeOutTime, () => PlayNextSong()));
                else PlayNextSong();
            }
        }

        private void HandleSoundStop ()
        {
            if (stopping) return;
            
            if (fadeOutTime > 0)
            {
                if (PlayingTime >= CurrentClipDuration - 0.05f - fadeOutTime)
                {
                    Stop(fadeOutTime);
                }
            }

            if (!Playing && !Paused)
            {
                Stop();
            }
        }

        private void HandlePlaylistStop ()
        {
            if (playlist.Count > 0) return;
            if (stopping) return;
            
            if (fadeOutTime > 0)
            {
                if (CurrentLoopCycleTime >= CurrentClipDuration - 0.05f - fadeOutTime)
                {
                    Stop(fadeOutTime);
                }
            }

            if (!Playing && !Paused)
            {
                Stop();
            }
        }

        private void PlayNextSong (bool firstTrack = false)
        {
            if (playlist.Count == 0) return;
            
            source.clip = playlist.Dequeue();
            if (loop) playlist.Enqueue(source.clip);
            playingTimeForNextSong += CurrentClipDuration;
            if (!firstTrack)
            {
                ReproducedTracks++;
                onNextTrackStart?.Invoke();
            }
            changingTrack = false;
            
            if (Paused) return;
            
            source.Play();
            if (fadeInTime > 0)
            {
                ChangeState(SourceState.FadingIn);
                fadeInOnChangeTrackCor = StartCoroutine(LerpVolume(volume, fadeInTime, 
                    () => ChangeState(SourceState.Playing)));
            }
            else
            {
                ChangeState(SourceState.Playing);
                source.volume = volume;
            }
        }

        private void StopLerpCoroutine ()
        {
            if (lerpVolumeCor == null) return;
            StopCoroutine(lerpVolumeCor);
            lerpVolumeCor = null;
        }
        
        private void ChangeState (SourceState newState)
        {
            currentState = newState;
        }
    }

    public partial class SourcePoolElement // Private Methods (Coroutines)
    {
        private IEnumerator LerpVolume (float newVolume, float lerpTime, Action onFinishLerp = null, bool ignorePause = false)
        {
            float currentVolume = source.volume;
            for (float t = 0.0f; t < lerpTime; t += Time.deltaTime)
            {
                if (!ignorePause)
                {
                    while (Paused)
                    {
                        yield return null;
                    }
                }

                source.volume = Mathf.Lerp(currentVolume, newVolume, t / lerpTime);
                yield return null;
            }
            source.volume = newVolume;

            onFinishLerp?.Invoke();
            
            lerpVolumeCor = null;
        }
    }
}