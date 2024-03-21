/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace MelenitasDev.SoundsGood
{
    public partial class Music // Fields
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
        private bool randomClip = true;
        private bool forgetSourcePoolOnStop = false;
        private AudioClip clip = null;
        private AudioMixerGroup output = null;
        private string cachedSoundTag;
    }

    public partial class Music // Fields (Callbacks)
    {
        private Action onPlay;
        private Action onComplete;
        private Action onLoopCycleComplete;
        private Action onPause;
        private Action onPauseComplete;
        private Action onResume;
    }

    public partial class Music // Properties
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
        /// <summary>Duration in seconds of matched clip.</summary>
        public float ClipDuration => clip != null ? clip.length : 0;
        /// <summary>Matched clip.</summary>
        public AudioClip Clip => clip;
    }

    public partial class Music // Public Methods
    {
        /// <summary>
        /// Create new Music object given a Track.
        /// </summary>
        /// <param name="track">Music track you've created before on Audio Creator window</param>
        public Music (Track track)
        {
            SetClip(track.ToString());
        }
        
        /// <summary>
        /// Create new Music object given a tag.
        /// </summary>
        /// <param name="tag">The tag you've used to create the sound on Audio Creator</param>
        public Music (string tag)
        {
            SetClip(tag);
        }
        
        /// <summary>
        /// Store volume parameters BEFORE play music.
        /// </summary>
        /// <param name="volume">Volume: min 0, Max 1</param>
        public Music SetVolume (float volume)
        {
            this.volume = volume;
            return this;
        }
        
        /// <summary>
        /// Set volume parameters BEFORE play music.
        /// </summary>
        /// <param name="volume">Volume: min 0, Max 1</param>
        /// <param name="hearDistance">Distance range to hear music</param>
        public Music SetVolume (float volume, Vector2 hearDistance)
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
        public Music SetPitch (float pitch)
        {
            this.pitch = pitch;
            return this;
        }

        /// <summary>
        /// Set an id to identify this music on AudioManager static methods.
        /// </summary>
        public Music SetId (string id)
        {
            this.id = id;
            return this;
        }
        
        /// <summary>
        /// Make your music loops for infinite time. If you need to stop it, use Stop() method.
        /// </summary>
        public Music SetLoop (bool loop)
        {
            this.loop = loop;
            return this;
        }
        
        /// <summary>
        /// Change the AudioClip of this Music.
        /// </summary>
        /// <param name="tag">The tag you've used to save the music track on Audio Creator window</param>
        public Music SetClip (string tag)
        {
            cachedSoundTag = tag;
            clip = AudioManager.GetTrack(tag);
            return this;
        }
        
        /// <summary>
        /// Set a new track BEFORE play it.
        /// </summary>
        /// <param name="track">Music track you've created before on Audio Creator window</param>
        public Music SetClip (Track track)
        {
            SetClip(track.ToString());
            return this;
        }
        
        /// <summary>
        /// Make the music clip change with each new Play().
        /// A random clip from those you have added together in the Audio Creator will be played.
        /// </summary>
        /// <param name="random">Use random clip</param>
        public Music SetRandomClip (bool random)
        {
            randomClip = random;
            return this;
        }

        /// <summary>
        /// Set the position of the sound emitter.
        /// </summary>
        public Music SetPosition (Vector3 position)
        {
            this.position = position;
            return this;
        }
        
        /// <summary>
        /// Set a target to follow. Audio source will update its position every frame.
        /// </summary>
        /// <param name="followTarget">Transform to follow</param>
        public Music SetFollowTarget (Transform followTarget)
        {
            this.followTarget = followTarget;
            return this;
        }
        
        /// <summary>
        /// Set spatial sound.
        /// </summary>
        /// <param name="true">Your sound will be 3D</param>
        /// <param name="false">Your sound will be global / 2D</param>
        public Music SetSpatialSound (bool activate = true)
        {
            spatialSound = activate;
            return this;
        }
        
        /// <summary>
        /// Set fade out duration. It'll be used when music ends.
        /// </summary>
        /// <param name="fadeOutTime">Seconds that fade out will last</param>
        public Music SetFadeOut (float fadeOutTime)
        {
            this.fadeOutTime = fadeOutTime;
            return this;
        }
        
        /// <summary>
        /// Set the audio output to manage the volume using the Audio Mixers.
        /// </summary>
        /// <param name="output">Output you've created before inside Master AudioMixer
        /// (Remember reload the outputs database on Output Manager Window)</param>
        public Music SetOutput (Output output)
        {
            this.output = AudioManager.GetOutput(output);
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on music start playing.
        /// </summary>
        /// <param name="onPlay">Method will be invoked</param>
        public Music OnPlay (Action onPlay)
        {
            this.onPlay = onPlay;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on music complete.
        /// If "loop" is active, it'll be called when you Stop the music manually.
        /// </summary>
        /// <param name="onComplete">Method will be invoked</param>
        public Music OnComplete (Action onComplete)
        {
            this.onComplete = onComplete;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on loop cycle complete.
        /// You need to set loop on true to use it.
        /// </summary>
        /// <param name="onLoopCycleComplete">Method will be invoked</param>
        public Music OnLoopCycleComplete (Action onLoopCycleComplete)
        {
            this.onLoopCycleComplete = onLoopCycleComplete;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on music pause.
        /// It will ignore the fade out time.
        /// </summary>
        /// <param name="onPause">Method will be invoked</param>
        public Music OnPause (Action onPause)
        {
            this.onPause = onPause;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on music pause and fade out ends.
        /// </summary>
        /// <param name="onPauseComplete">Method will be invoked</param>
        public Music OnPauseComplete (Action onPauseComplete)
        {
            this.onPauseComplete = onPauseComplete;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on resume/unpause music.
        /// </summary>
        /// <param name="onResume">Method will be invoked</param>
        public Music OnResume (Action onResume)
        {
            this.onResume = onResume;
            return this;
        }

        /// <summary>
        /// Reproduce music.
        /// </summary>
        /// <param name="fadeInTime">Seconds that fade in will last</param>
        public void Play (float fadeInTime = 0)
        {
            if (Using && Playing)
            {
                Stop();
                forgetSourcePoolOnStop = true;
            }

            if (randomClip) SetClip(cachedSoundTag);
            
            sourcePoolElement = AudioManager.GetSource();
            sourcePoolElement
                .SetVolume(volume, hearDistance)
                .SetPitch(pitch)
                .SetLoop(loop)
                .SetClip(clip)
                .SetPosition(position)
                .SetFollowTarget(followTarget)
                .SetSpatialSound(spatialSound)
                .SetFadeOut(fadeOutTime)
                .SetId(id)
                .SetOutput(output)
                .OnPlay(onPlay)
                .OnComplete(onComplete)
                .OnLoopCycleComplete(onLoopCycleComplete)
                .OnPause(onPause)
                .OnPauseComplete(onPauseComplete)
                .OnResume(onResume)
                .Play(fadeInTime);
        }

        /// <summary>
        /// Pause music.
        /// </summary>
        /// <param name="fadeOutTime">Seconds that fade out will last before pause</param>
        public void Pause (float fadeOutTime = 0)
        {
            if (!Using) return;
            
            sourcePoolElement.Pause(fadeOutTime);
        }

        /// <summary>
        /// Resume/Unpause music.
        /// </summary>
        /// <param name="fadeInTime">Seconds that fade in will last</param>
        public void Resume (float fadeInTime = 0)
        {
            if (!Using) return;
            
            sourcePoolElement.Resume(fadeInTime);
        }

        /// <summary>
        /// Stop music.
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