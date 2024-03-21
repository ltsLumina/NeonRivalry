/*
 * All rights to the Sounds Good plugin, © Created by Melenitas Dev, are reserved.
 * Distribution of the standalone asset is strictly prohibited.
 */
using UnityEngine;
using UnityEngine.Audio;

namespace MelenitasDev.SoundsGood.Domain
{
    public partial class OutputData // Serialized Fields
    {
        [SerializeField] private string name;
        [SerializeField] private AudioMixerGroup output;
    }
    
    public partial class OutputData // Properties
    {
        public string Name { get => name; set => name = value; }
        public AudioMixerGroup Output { get => output; set => output = value; }
    }
    
    [System.Serializable]
    public partial class OutputData // Public Methods
    {
        public OutputData (string name, AudioMixerGroup output)
        {
            this.name = name;
            this.output = output;
        }
    }
}