using JackSharp.Ports;
using System.Collections.Generic;

namespace BoneSpray.Net.Scenes
{
    public abstract class BaseScene : IBaseScene
    {
        /// <summary>
        /// Scene Key. The key used to access and find scenes from the Orchestrator.
        /// </summary>
        protected string Key { get; }

        /// <summary>
        /// Live MIDI ports;
        /// </summary>
        public List<MidiInPort> MidiPorts { get; protected set; }

        /// <summary>
        /// The live Audio ports.
        /// </summary>
        public List<AudioInPort> AudioPorts { get; protected set; }
    }
}