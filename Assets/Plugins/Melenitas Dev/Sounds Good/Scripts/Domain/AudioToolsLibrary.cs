/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using System;
using UnityEngine;

namespace MelenitasDev.SoundsGood.Domain
{
    public partial class AudioToolsLibrary // Properties
    {
        public SoundDataCollection SoundDataCollection { get; private set; }
        public MusicDataCollection MusicDataCollection { get; private set; }
        public OutputCollection OutputCollection { get; private set; }
    }

    public partial class AudioToolsLibrary
    {
        public AudioToolsLibrary ()
        {
            Init();
        }

        private void Init ()
        {
            try
            {
                SoundDataCollection = Resources.Load<SoundDataCollection>("Melenitas Dev/Sounds Good/SoundCollection");
                MusicDataCollection = Resources.Load<MusicDataCollection>("Melenitas Dev/Sounds Good/MusicCollection");
                OutputCollection = Resources.Load<OutputCollection>("Melenitas Dev/Sounds Good/Outputs/OutputCollection");
            }
            catch (Exception e)
            {
                Debug.LogError("You're trying to initialize a Sound, Music, Playlist, Dynamic Music or Output in its declaration. " +
                               "Please, move the initialization to Start or Awake methods\n\n" + e);
                return;
            }

            if (SoundDataCollection == null || MusicDataCollection == null)
            {
                Debug.LogError("Audio resources haven't found. " +
                               "Open [Tools > Audio System > Audio Creator] to solve this problem.");
                return;
            }

            SoundDataCollection.Init();
            MusicDataCollection.Init();
            OutputCollection.Init();
        }
    }
}