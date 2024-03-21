/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MelenitasDev.SoundsGood
{
    public partial class Playlist // Fields
    {
        private SourcePoolElement sourcePoolElement;

        private float volume = 1;
        private Vector2 hearDistance = new Vector2(3, 500);
        private float pitch = 1;
        private Vector2 pitchRange = new Vector2(0.85f, 1.15f);
        private string id = null;
        private Vector3 position = Vector3.zero;
        private Transform followTarget = null;
        private bool loop = false;
        private bool spatialSound = false;
        private float fadeOutTime = 0;
        private float fadeInTime = 0;
        private bool forgetSourcePoolOnStop = false;
        private Queue<AudioClip> playlist = new Queue<AudioClip>();
        private AudioMixerGroup output = null;
    }

    public partial class Playlist // Fields (Callbacks)
    {
        private Action onPlay;
        private Action onComplete;
        private Action onLoopCycleComplete;
        private Action onNextTrackStart;
        private Action onPause;
        private Action onPauseComplete;
        private Action onResume;
    }

    public partial class Playlist // Properties
    {
        /// <summary>It's true when it's being used. When it's paused, it's true as well</summary>
        public bool Using => sourcePoolElement != null;
        /// <summary>It's true when audio is playing.</summary>
        public bool Playing => Using && sourcePoolElement.Playing;
        /// <summary>It's true when audio paused (it ignore the fade out time).</summary>
        public bool Paused => Using && sourcePoolElement.Paused;
        /// <summary>Volume level between [0,1].</summary>
        public float Volume => volume;
        /// <summary>Total time in seconds that it have been playing.</summary>
        public float PlayingTime => Using ? sourcePoolElement.PlayingTime : 0;
        /// <summary>Reproduced time in seconds of current loop cycle.</summary>
        public float CurrentLoopCycleTime => Using ? sourcePoolElement.CurrentLoopCycleTime : 0;
        /// <summary>Times it has looped.</summary>
        public int CompletedLoopCycles => Using ? sourcePoolElement.CompletedLoopCycles : 0;
        /// <summary>Duration in seconds of current playing clip.</summary>
        public float CurrentClipDuration => Using ? sourcePoolElement.CurrentClipDuration : 0;
        /// <summary>Total duration in seconds of entire playlist.</summary>
        public float PlayListDuration
        {
            get
            {
                float playlistDuration = 0;
                foreach (AudioClip clip in playlist) playlistDuration += clip.length;
                return playlistDuration;
            }
        }
        /// <summary>Reproduced tracks in this playlist.</summary>
        public float ReproducedTracks => Using ? sourcePoolElement.ReproducedTracks : 0;
        /// <summary>Current clip that is playing.</summary>
        public AudioClip CurrentPlaylistClip => sourcePoolElement.CurrentClip;
        /// <summary>The next clip of current playlist</summary>
        public AudioClip NextPlaylistClip => sourcePoolElement.NextPlaylistClip;
    }

    public partial class Playlist // Public Methods
    {
        /// <summary>
        /// Create new Playlist object given a Tracks array.
        /// </summary>
        /// <param name="playlistTracks">Track array with all music tracks that you want to reproduce in order</param>
        public Playlist (Track[] playlistTracks)
        {
            SetPlaylist(playlistTracks);
        }
        
        /// <summary>
        /// Create new Playlist object given a tags array.
        /// </summary>
        /// <param name="playlistTags">A music tracks tags array that you want to reproduce in order</param>
        public Playlist (string[] playlistTags)
        {
            SetPlaylist(playlistTags);
        }

        /// <summary>
        /// Store volume parameters BEFORE play playlist.
        /// </summary>
        /// <param name="volume">Volume: min 0, Max 1</param>
        public Playlist SetVolume (float volume)
        {
            this.volume = volume;
            return this;
        }
        
        /// <summary>
        /// Store volume parameters BEFORE play playlist.
        /// </summary>
        /// <param name="volume">Volume: min 0, Max 1</param>
        /// <param name="hearDistance">min and Max distance to hear music</param>
        public Playlist SetVolume (float volume, Vector2 hearDistance)
        {
            this.volume = volume;
            this.hearDistance = hearDistance;
            return this;
        }
        
        /// <summary>
        /// Change volume while music is reproducing.
        /// </summary>
        /// <param name="newVolume">New volume: min 0, Max 1</param>
        /// <param name="lerpTime">Time to lerp current to new volume</param>
        public void ChangeVolume (float newVolume, float lerpTime = 0)
        {
            if (volume == newVolume) return;
            
            volume = newVolume;
            
            if (!Using) return;
            
            sourcePoolElement.SetVolume(newVolume, hearDistance, lerpTime);
        }
        
        /// <summary>
        /// Set given pitch. Make your music sound different :)
        /// </summary>
        public Playlist SetPitch (float pitch)
        {
            this.pitch = pitch;
            return this;
        }

        /// <summary>
        /// Set an id to identify this music on AudioManager static methods.
        /// </summary>
        public Playlist SetId (string id)
        {
            this.id = id;
            return this;
        }
        
        /// <summary>
        /// Make your playlist loops for infinite time. If you need to stop it, use Stop() method.
        /// </summary>
        public Playlist SetLoop (bool loop)
        {
            this.loop = loop;
            return this;
        }

        /// <summary>
        /// Set a new playlist BEFORE play it.
        /// </summary>
        /// <param name="playlistTags">A music tracks tags array in order</param>
        public Playlist SetPlaylist (string[] playlistTags)
        {
            playlist.Clear();
            foreach (string tag in playlistTags)
                playlist.Enqueue(AudioManager.GetTrack(tag));
            return this;
        }
        
        /// <summary>
        /// Set a new playlist BEFORE play it.
        /// </summary>
        /// <param name="playlistTracks">A music tracks array in order</param>
        public Playlist SetPlaylist (Track[] playlistTracks)
        {
            playlist.Clear();
            foreach (Track track in playlistTracks)
                playlist.Enqueue(AudioManager.GetTrack(track.ToString()));
            return this;
        }
        
        /// <summary>
        /// Enqueue a new track to the existing playlist.
        /// </summary>
        /// <param name="addedTrackTag">The new track's tag you want to add at the end of the playlist</param>
        public void AddToPlaylist (string addedTrackTag)
        {
            playlist.Enqueue(AudioManager.GetTrack(addedTrackTag));
            sourcePoolElement.AddToPlaylist(AudioManager.GetTrack(addedTrackTag));
        }
        
        /// <summary>
        /// Enqueue a new track to the existing playlist.
        /// </summary>
        /// <param name="addedTrack">The new track you want to add at the end of the playlist</param>
        public void AddToPlaylist (Track addedTrack)
        {
            playlist.Enqueue(AudioManager.GetTrack(addedTrack.ToString()));
            sourcePoolElement.AddToPlaylist(AudioManager.GetTrack(addedTrack.ToString()));
        }

        /// <summary>
        /// Set the position of the sound emitter.
        /// </summary>
        public Playlist SetPosition (Vector3 position)
        {
            this.position = position;
            return this;
        }
        
        /// <summary>
        /// Set a target to follow. Audio source will update its position every frame.
        /// </summary>
        /// <param name="followTarget">Transform to follow. Null to follow Main Camera transform.</param>
        public Playlist SetFollowTarget (Transform followTarget)
        {
            this.followTarget = followTarget;
            return this;
        }
        
        /// <summary>
        /// Set spatial sound.
        /// </summary>
        /// <param name="true">Your sound will be 3D</param>
        /// <param name="false">Your sound will be global / 2D</param>
        public Playlist SetSpatialSound (bool activate = true)
        {
            spatialSound = activate;
            return this;
        }
        
        /// <summary>
        /// Set fade out duration for all tracks.
        /// </summary>
        /// <param name="fadeOutTime">Seconds that fade out will last</param>
        public Playlist SetFadeOut (float fadeOutTime)
        {
            this.fadeOutTime = fadeOutTime;
            return this;
        }
        
        /// <summary>
        /// Set fade in duration for all tracks.
        /// </summary>
        /// <param name="fadeInTime">Seconds that fade in will last</param>
        public Playlist SetFadeIn (float fadeInTime)
        {
            this.fadeInTime = fadeInTime;
            return this;
        }
        
        /// <summary>
        /// Set the audio output to manage the volume using the Audio Mixers.
        /// </summary>
        /// <param name="output">Output you've created before inside Master AudioMixer
        /// (Remember reload the outputs database on Output Manager Window)</param>
        public Playlist SetOutput (Output output)
        {
            this.output = AudioManager.GetOutput(output);
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on playlist starts.
        /// </summary>
        /// <param name="onPlay">Method will be invoked</param>
        public Playlist OnPlay (Action onPlay)
        {
            this.onPlay = onPlay;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on playlist complete.
        /// If "loop" is active, it'll be called when you Stop the playlist manually.
        /// </summary>
        /// <param name="onComplete">Method will be invoked</param>
        public Playlist OnComplete (Action onComplete)
        {
            this.onComplete = onComplete;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on playlist loop cycle complete.
        /// You need to set loop on true to use it.
        /// </summary>
        /// <param name="onLoopCycleComplete">Method will be invoked</param>
        public Playlist OnLoopCycleComplete (Action onLoopCycleComplete)
        {
            this.onLoopCycleComplete = onLoopCycleComplete;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on finish track and start the next one.
        /// </summary>
        /// <param name="onNextTrackStart">Method will be invoked</param>
        public Playlist OnNextTrackStart (Action onNextTrackStart)
        {
            this.onNextTrackStart = onNextTrackStart;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on playlist pause.
        /// It will ignore the fade out time.
        /// </summary>
        /// <param name="onPause">Method will be invoked</param>
        public Playlist OnPause (Action onPause)
        {
            this.onPause = onPause;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on playlist pause and fade out ends.
        /// </summary>
        /// <param name="onPauseComplete">Method will be invoked</param>
        public Playlist OnPauseComplete (Action onPauseComplete)
        {
            this.onPauseComplete = onPauseComplete;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on resume/unpause playlist.
        /// </summary>
        /// <param name="onResume">Method will be invoked</param>
        public Playlist OnResume (Action onResume)
        {
            this.onResume = onResume;
            return this;
        }

        /// <summary>
        /// Reproduce playlist.
        /// </summary>
        public void Play ()
        {
            if (Using && Playing)
            {
                Stop();
                forgetSourcePoolOnStop = true;
            }

            sourcePoolElement = AudioManager.GetSource();
            sourcePoolElement
                .MarkAsPlaylist()
                .SetVolume(volume, hearDistance)
                .SetPitch(pitch)
                .SetLoop(loop)
                .SetPlaylist(playlist)
                .SetPosition(position)
                .SetFollowTarget(followTarget)
                .SetSpatialSound(spatialSound)
                .SetFadeIn(fadeInTime)
                .SetFadeOut(fadeOutTime)
                .SetId(id)
                .SetOutput(output)
                .OnPlay(onPlay)
                .OnComplete(onComplete)
                .OnLoopCycleComplete(onLoopCycleComplete)
                .OnNextTrackStart(onNextTrackStart)
                .OnPause(onPause)
                .OnPauseComplete(onPauseComplete)
                .OnResume(onResume)
                .PlayPlaylist(fadeInTime);
        }

        /// <summary>
        /// Pause playlist.
        /// </summary>
        /// <param name="fadeOutTime">Seconds that fade out will last before pause</param>
        public void Pause (float fadeOutTime = 0)
        {
            if (!Using) return;
            
            sourcePoolElement.Pause(fadeOutTime);
        }

        /// <summary>
        /// Resume/Unpause playlist.
        /// </summary>
        /// <param name="fadeInTime">Seconds that fade in will last</param>
        public void Resume (float fadeInTime = 0)
        {
            if (!Using) return;
            
            sourcePoolElement.Resume(fadeInTime);
        }

        /// <summary>
        /// Stop playlist.
        /// </summary>
        /// <param name="fadeOutTime">Seconds that fade out will last before stop</param>
        public void Stop (float fadeOutTime = 0)
        {
            if (!Using) return;

            if (forgetSourcePoolOnStop)
            {
                sourcePoolElement.Stop(fadeOutTime);
                sourcePoolElement = null;
                forgetSourcePoolOnStop = false;
                return;
            }
            sourcePoolElement.Stop(fadeOutTime, () => sourcePoolElement = null);
        }
    }
}