/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MelenitasDev.SoundsGood.Domain
{
    public class OutputCollection : ScriptableObject
    {
        [SerializeField] private OutputData[] outputs = Array.Empty<OutputData>();

        private Dictionary<string, OutputData> outputsDictionary = new Dictionary<string, OutputData>();

        public OutputData[] Outputs => outputs;

        public void Init ()
        {
            foreach (OutputData outputData in outputs)
            {
                outputsDictionary.Add(outputData.Name, outputData);
            }
        }

        public void LoadOutputs()
        {
            AudioMixer mixer = Resources.Load<AudioMixer>("Melenitas Dev/Sounds Good/Outputs/Master");
            AudioMixerGroup[] mixerGroups = mixer.FindMatchingGroups(null);
            OutputData[] loadedOutputs = new OutputData[mixerGroups.Length];
            for (int i = 0; i < loadedOutputs.Length; i++)
            {
                OutputData newOutputData = new OutputData(mixerGroups[i].name.Replace(" ", ""), mixerGroups[i]);
                loadedOutputs[i] = newOutputData;
                Debug.Log($"Output {i} '{newOutputData.Name}' saved!");
            }
            outputs = loadedOutputs;
        }
        
        public AudioMixerGroup GetOutput (string name)
        {
            if (outputsDictionary.TryGetValue(name.Replace(" ", ""), out OutputData outputData)) return outputData.Output;
            
            Debug.LogWarning($"Output with tag '{name}' don't exist");
            return null;
        }
    }
}