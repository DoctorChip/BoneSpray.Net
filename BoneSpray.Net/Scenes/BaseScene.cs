using JackSharp.Ports;
using JackSharp.Processing;
using System;
using System.Collections.Generic;

namespace BoneSpray.Net.Scenes
{
    public abstract class BaseScene : IBaseScene
    {
        /// <summary>
        /// Scene Key. The key used to access and find scenes from the Orchestrator.
        /// </summary>
        protected abstract string Key { get; }
        public string GetKey() => Key;

        /// <summary>
        /// The MIDI ports that this scene would like to subscribe to.
        /// </summary>
        public abstract Dictionary<string, Action<ProcessBuffer>> RequiredMidiPortNames { get; }

        /// <summary>
        /// The live MIDI ports.
        /// </summary>
        public abstract List<MidiInPort> MidiPorts { get; protected set; }

        /// <summary>
        /// The Audio ports that this scene would like to subscribe to.
        /// </summary>
        public abstract Dictionary<string, Action<ProcessBuffer>> RequiredAudioPortNames { get; }

        /// <summary>
        /// The live Audio ports.
        /// </summary>
        public abstract List<AudioInPort> AudioPorts { get; protected set; }
    }
}