using JackSharp.Ports;
using JackSharp.Processing;
using System;
using System.Collections.Generic;

namespace BoneSpray.Net.Models
{
    public class OutPortContainer
    {
        public string Name { get; set; }

        public PortType Type { get; set; }
    }

    public class OutMidiPortContainer : OutPortContainer
    {
        public Action<IEnumerable<SimpleMidiEvent>> MidiStream { get; set; }
    }

    public class OutAudioPortContainer : OutPortContainer
    {
        public Action<AudioBuffer[]> AudioStream { get; set; }
    }
}