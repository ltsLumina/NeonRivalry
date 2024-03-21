/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace MelenitasDev.SoundsGood
{
    public partial class DynamicMusic // Fields
    {
        private SourcePoolElement referenceSourcePoolElement = null;
        private Dictionary<string, SourcePoolElement> sourcePoolElementDictionary 
            = new Dictionary<string, SourcePoolElement>();

        private Dictionary<string, float> volumeDictionary = new Dictionary<string, float>();
        private Dictionary<string, Vector2> hearDistanceDictionary = new Dictionary<string, Vector2>();
        private Vector2 defaultHearDistance = new Vector2(3, 500);
        private float pitch = 1;
        private Vector2 pitchRange = new Vector2(0.85f, 1.15f);
        private string id = null;
        private Vector3 position = Vector3.zero;
        private Transform followTarget = null;
        private bool loop = false;
        private bool spatialSound = false;
        private float fadeOutTime = 0;
        private bool forgetSourcePoolOnStop = false;
        private AudioClip[] clips;
        private AudioMixerGroup output = null;
    }

    public partial class DynamicMusic // Fields (Callbacks)
    {
        private Action onPlay;
        private Action onComplete;
        private Action onLoopCycleComplete;
        private Action onPause;
        private Action onPauseComplete;
        private Action onResume;
    }

    public partial class DynamicMusic // Properties
    {
        /// <summary>It's true when it's being used. When it's paused, it's true as well</summary>
        public bool Using => referenceSourcePoolElement != null;
        /// <summary>It's true when audio is playing.</summary>
        public bool Playing => Using && referenceSourcePoolElement.Playing;
        /// <summary>It's true when audio paused (it ignore the fade out time).</summary>
        public bool Paused => Using && referenceSourcePoolElement.Paused;
        /// <summary>Total time in seconds that it have been playing.</summary>
        public float PlayingTime => Using ? referenceSourcePoolElement.PlayingTime : 0;
        /// <summary>Reproduced time in seconds of current loop cycle.</summary>
        public float CurrentLoopCycleTime => Using ? referenceSourcePoolElement.CurrentLoopCycleTime : 0;
        /// <summary>Times it has looped.</summary>
        public int CompletedLoopCycles => Using ? referenceSourcePoolElement.CompletedLoopCycles : 0;
        /// <summary>Duration in seconds of matched clip (use the first clip of the array because they should have the same duration).</summary>
        public float ClipDuration => clips.Length > 0 ? clips[0].length : 0;
        /// <summary>Matched clip.</summary>
        public AudioClip[] Clips => clips;
    }

    public partial class DynamicMusic // Public Methods
    {
        /// <summary>
        /// Create new Dynamic Music object given a Tracks array.
        /// </summary>
        /// <param name="tracks">Track array with all music tracks that you want to reproduce at the same time</param>
        public DynamicMusic (Track[] tracks)
        {
            clips = new AudioClip[tracks.Length];
            int i = 0;
            foreach (Track track in tracks)
            {
                clips[i] = AudioManager.GetTrack(track.ToString());
                sourcePoolElementDictionary.Add(track.ToString(), null);
                volumeDictionary.Add(track.ToString(), 0.5f);
                hearDistanceDictionary.Add(track.ToString(), defaultHearDistance);
                i++;
            }
        }
        
        /// <summary>
        /// Create new Dynamic Music object given a tags array.
        /// </summary>
        /// <param name="tag">Track array with all music tracks tags that you want to reproduce at the same time</param>
        public DynamicMusic (string[] tags)
        {
            clips = new AudioClip[tags.Length];
            int i = 0;
            foreach (string tag in tags)
            {
                clips[i] = AudioManager.GetTrack(tag);
                sourcePoolElementDictionary.Add(tag, null);
                volumeDictionary.Add(tag, 0.5f);
                hearDistanceDictionary.Add(tag, defaultHearDistance);
                i++;
            }
        }
        
        /// <summary>
        /// Store volume parameters of all tracks BEFORE play Dynamic Music.
        /// </summary>
        /// <param name="volume">Volume: min 0, Max 1</param>
        public DynamicMusic SetAllVolumes (float volume)
        {
            foreach (var tagSourcePair in sourcePoolElementDictionary)
            {
                volumeDictionary[tagSourcePair.Key] = volume;
                hearDistanceDictionary[tagSourcePair.Key] = defaultHearDistance;
            }
            return this;
        }
        
        /// <summary>
        /// Store volume parameters of all tracks BEFORE play Dynamic Music.
        /// </summary>
        /// <param name="volume">Volume: min 0, Max 1.</param>
        /// <param name="hearDistance">Distance range to hear music</param>
        public DynamicMusic SetAllVolumes (float volume, Vector2 hearDistance)
        {
            foreach (var tagSourcePair in sourcePoolElementDictionary)
            {
                volumeDictionary[tagSourcePair.Key] = volume;
                hearDistanceDictionary[tagSourcePair.Key] = hearDistance;
            }
            return this;
        }
        
        /// <summary>
        /// Store volume parameters of specific track BEFORE play Dynamic Music.
        /// </summary>
        /// /// <param name="track">Track you want modify.</param>
        /// <param name="volume">Volume: min 0, Max 1.</param>
        /// <param name="hearDistance">min and Max distance to hear sound.</param>
        public DynamicMusic SetTrackVolume (Track track, float volume)
        {
            return SetTrackVolume(track.ToString(), volume);
        }
        
        /// <summary>
        /// Store volume parameters of specific track BEFORE play Dynamic Music.
        /// </summary>
        /// /// <param name="tag">Track you want modify.</param>
        /// <param name="volume">Volume: min 0, Max 1.</param>
        public DynamicMusic SetTrackVolume (string tag, float volume)
        {
            foreach (var tagSourcePair in sourcePoolElementDictionary)
            {
                if (!tagSourcePair.Key.Equals(tag)) continue;
                
                volumeDictionary[tagSourcePair.Key] = volume;
                hearDistanceDictionary[tagSourcePair.Key] = defaultHearDistance;
                break;
            }
            return this;
        }
        
        /// <summary>
        /// Set volume parameters BEFORE play music.
        /// </summary>
        /// /// <param name="track">Track you want modify.</param>
        /// <param name="volume">Volume: min 0, Max 1.</param>
        /// <param name="hearDistance">min and Max distance to hear music</param>
        public DynamicMusic SetTrackVolume (Track track, float volume, Vector2 hearDistance)
        {
            return SetTrackVolume(track.ToString(), volume, hearDistance);
        }
        
        /// <summary>
        /// Set volume parameters BEFORE play music.
        /// </summary>
        /// /// <param name="tag">Track you want modify.</param>
        /// <param name="volume">Volume: min 0, Max 1.</param>
        /// <param name="hearDistance">min and Max distance to hear music</param>
        public DynamicMusic SetTrackVolume (string tag, float volume, Vector2 hearDistance)
        {
            foreach (var tagSourcePair in sourcePoolElementDictionary)
            {
                if (!tagSourcePair.Key.Equals(tag)) continue;
                
                volumeDictionary[tagSourcePair.Key] = volume;
                hearDistanceDictionary[tagSourcePair.Key] = hearDistance;
                break;
            }
            return this;
        }

        /// <summary>
        /// Change all tracks volume while music is reproducing.
        /// </summary>
        /// <param name="newVolume">New volume: min 0, Max 1</param>
        /// <param name="lerpTime">Time to lerp current to new volume</param>
        public void ChangeAllVolumes (float newVolume, float lerpTime = 0)
        {
            foreach (var tagSourcePair in sourcePoolElementDictionary)
            {
                if (volumeDictionary[tagSourcePair.Key] == newVolume) continue;
                volumeDictionary[tagSourcePair.Key] = newVolume;
            
                if (!Using) return;
            
                tagSourcePair.Value.SetVolume(newVolume, hearDistanceDictionary[tagSourcePair.Key], lerpTime);
            }
        }

        /// <summary>
        /// Change volume while music is reproducing.
        /// </summary>
        /// <param name="track">Track you want modify.</param>
        /// <param name="newVolume">New volume: min 0, Max 1.</param>
        /// <param name="lerpTime">Time to lerp current to new volume.</param>
        public void ChangeTrackVolume (Track track, float newVolume, float lerpTime = 0)
        {
            ChangeTrackVolume(track.ToString(), newVolume, lerpTime);
        }
        
        /// <summary>
        /// Change volume while music is reproducing.
        /// </summary>
        /// <param name="track">Track you want modify.</param>
        /// <param name="newVolume">New volume: min 0, Max 1.</param>
        /// <param name="lerpTime">Time to lerp current to new volume.</param>
        public void ChangeTrackVolume (string tag, float newVolume, float lerpTime = 0)
        {
            foreach (var tagSourcePair in sourcePoolElementDictionary)
            {
                if (!tagSourcePair.Key.Equals(tag)) continue;
                
                if (volumeDictionary[tagSourcePair.Key] == newVolume) return;
                volumeDictionary[tagSourcePair.Key] = newVolume;
            
                if (!Using) return;
            
                tagSourcePair.Value.SetVolume(newVolume, hearDistanceDictionary[tagSourcePair.Key], lerpTime);
                return;
            }
        }

        /// <summary>
        /// Set given pitch. Make your music sound different :)
        /// </summary>
        public DynamicMusic SetPitch (float pitch)
        {
            this.pitch = pitch;
            return this;
        }

        /// <summary>
        /// Set an id to identify this music on AudioManager static methods.
        /// </summary>
        public DynamicMusic SetId (string id)
        {
            this.id = id;
            return this;
        }
        
        /// <summary>
        /// Make your music loops for infinite time. If you need to stop it, use Stop() method.
        /// </summary>
        public DynamicMusic SetLoop (bool loop)
        {
            this.loop = loop;
            return this;
        }

        /// <summary>
        /// Set the position of the sound emitter.
        /// </summary>
        public DynamicMusic SetPosition (Vector3 position)
        {
            this.position = position;
            return this;
        }
        
        /// <summary>
        /// Set a target to follow. Audio source will update its position every frame.
        /// </summary>
        /// <param name="followTarget">Transform to follow. Null to follow Main Camera transform.</param>
        public DynamicMusic SetFollowTarget (Transform followTarget)
        {
            this.followTarget = followTarget;
            return this;
        }
        
        /// <summary>
        /// Set spatial sound.
        /// </summary>
        /// <param name="true">Your sound will be 3D</param>
        /// <param name="false">Your sound will be global / 2D</param>
        public DynamicMusic SetSpatialSound (bool activate = true)
        {
            spatialSound = activate;
            return this;
        }
        
        /// <summary>
        /// Set fade out duration. It'll be used when sound ends.
        /// </summary>
        /// <param name="fadeOutTime">Seconds that fade out will last</param>
        public DynamicMusic SetFadeOut (float fadeOutTime)
        {
            this.fadeOutTime = fadeOutTime;
            return this;
        }
        
        /// <summary>
        /// Set the audio output to manage the volume using the Audio Mixers.
        /// </summary>
        /// <param name="output">Output you've created before inside Master AudioMixer
        /// (Remember reload the outputs database on Output Manager Window)</param>
        public DynamicMusic SetOutput (Output output)
        {
            this.output = AudioManager.GetOutput(output);
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on music start playing.
        /// </summary>
        /// <param name="onPlay">Method will be invoked</param>
        public DynamicMusic OnPlay (Action onPlay)
        {
            this.onPlay = onPlay;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on music complete.
        /// If "loop" is active, it'll be called when you Stop the sound manually.
        /// </summary>
        /// <param name="onComplete">Method will be invoked</param>
        public DynamicMusic OnComplete (Action onComplete)
        {
            this.onComplete = onComplete;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on loop cycle complete.
        /// You need to set loop on true to use it.
        /// </summary>
        /// <param name="onLoopCycleComplete">Method will be invoked</param>
        public DynamicMusic OnLoopCycleComplete (Action onLoopCycleComplete)
        {
            this.onLoopCycleComplete = onLoopCycleComplete;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on music pause.
        /// It will ignore the fade out time.
        /// </summary>
        /// <param name="onPause">Method will be invoked</param>
        public DynamicMusic OnPause (Action onPause)
        {
            this.onPause = onPause;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on music pause and fade out ends.
        /// </summary>
        /// <param name="onPauseComplete">Method will be invoked</param>
        public DynamicMusic OnPauseComplete (Action onPauseComplete)
        {
            this.onPauseComplete = onPauseComplete;
            return this;
        }
        
        /// <summary>
        /// Define a callback that will be invoked on resume/unpause music.
        /// </summary>
        /// <param name="onResume">Method will be invoked</param>
        public DynamicMusic OnResume (Action onResume)
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
            
            for (int i = 0; i < sourcePoolElementDictionary.Count; i++)
            {
                string tag = sourcePoolElementDictionary.ElementAt(i).Key;
                
                SourcePoolElement newSourcePoolElement = AudioManager.GetSource();
                sourcePoolElementDictionary[tag] = newSourcePoolElement;
                if (i == 0)
                {
                    referenceSourcePoolElement = newSourcePoolElement;
                    referenceSourcePoolElement
                        .SetVolume(volumeDictionary[tag], hearDistanceDictionary[tag])
                        .SetPitch(pitch)
                        .SetLoop(loop)
                        .SetClip(clips[i])
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
                    continue;
                }
                
                newSourcePoolElement
                    .SetVolume(volumeDictionary[tag], hearDistanceDictionary[tag])
                    .SetPitch(pitch)
                    .SetLoop(loop)
                    .SetClip(clips[i])
                    .SetPosition(position)
                    .SetFollowTarget(followTarget)
                    .SetSpatialSound(spatialSound)
                    .SetFadeOut(fadeOutTime)
                    .SetId(id)
                    .SetOutput(output)
                    .Play(fadeInTime);
            }
        }

        /// <summary>
        /// Pause music.
        /// </summary>
        /// <param name="fadeOutTime">Seconds that fade out will last before pause</param>
        public void Pause (float fadeOutTime = 0)
        {
            if (!Using) return;
            
            foreach (var source in sourcePoolElementDictionary.Values)
            {
                source.Pause(fadeOutTime);
            }
        }

        /// <summary>
        /// Resume/Unpause music.
        /// </summary>
        /// <param name="fadeInTime">Seconds that fade in will last</param>
        public void Resume (float fadeInTime = 0)
        {
            if (!Using) return;
            
            foreach (var source in sourcePoolElementDictionary.Values)
            {
                source.Resume(fadeInTime);
            }
        }

        /// <summary>
        /// Stop music.
        /// </summary>
        /// <param name="fadeOutTime">Seconds that fade out will last before stop</param>
        public void Stop (float fadeOutTime = 0)
        {
            if (!Using) return;

            for (int i = 0; i < sourcePoolElementDictionary.Count; i++)
            {
                string tag = sourcePoolElementDictionary.ElementAt(i).Key;

                if (forgetSourcePoolOnStop)
                {
                    sourcePoolElementDictionary[tag].Stop(fadeOutTime, () =>
                    {
                        sourcePoolElementDictionary[tag] = null;
                        referenceSourcePoolElement = null;
                    });
                    continue;
                }
                sourcePoolElementDictionary[tag].Stop(fadeOutTime, () => sourcePoolElementDictionary[tag] = null);
            }
            referenceSourcePoolElement = null;
            forgetSourcePoolOnStop = false;
        }
    }
}
