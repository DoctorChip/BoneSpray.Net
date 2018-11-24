using JackSharp;
using JackSharp.Ports;
using System.Collections.Generic;

namespace BoneSpray.Net.Models
{
    public class PortContainer
    {
        public string Name { get; set; }

        public Processor PortProcessor { get; set; }
    }

    public class AudioPortContainer : PortContainer
    {
        public List<AudioInPort> Ports { get; set; }
    }

    public class MidiPortContainer : PortContainer
    {
        public List<MidiInPort> Ports { get; set; }
    }
}
