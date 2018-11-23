using BoneSpray.Net.Models;
using JackSharp;
using JackSharp.Ports;
using System.Collections.Generic;
using System.Linq;

namespace BoneSpray.Net.Services
{
    public static class PortControlService
    {
        /// <summary>
        /// Hold onto all existing ports.
        /// </summary>
        public static Dictionary<string, PortContainer> AudioPorts = new Dictionary<string, PortContainer>();
        public static Dictionary<string, PortContainer> MidiPorts = new Dictionary<string, PortContainer>();

        /// <summary>
        /// Loop through all of our ports and connect them to JACK.
        /// </summary>
        public static bool ConnectAllPorts()
        {
            var result = true;

            foreach(var port in MidiPorts)
            {
                var success = port.Value.PortProcessor.Start();
                if (!success) result = false;
            }

            foreach(var port in AudioPorts)
            {
                var success = port.Value.PortProcessor.Start();
                if (!success) result = false;
            }

            return result;
        }

        /// <summary>
        /// Check if we have an existing port of a certain type by a specific name.
        /// </summary>
        public static bool PortExists(string name, PortType type)
        {
            return type == PortType.Midi ? MidiPorts.ContainsKey(name) : AudioPorts.ContainsKey(name);
        }

        /// <summary>
        /// Create a port of a certain type and add it to our ports list.
        /// </summary>
        public static bool CreatePort(string name, PortType type)
        {
            if (type == PortType.Midi)
            {
                var processor = new Processor(name, midiInPorts: 1, autoconnect: true);

                var container = new MidiPortContainer
                {
                    Name = name,
                    PortProcessor = processor,
                    Ports = processor.MidiInPorts.ToList(),
                    
                };

                MidiPorts.Add(name, container);
            }
            else
            {
                var processor = new Processor(name, audioInPorts: 1, autoconnect: true);

                var container = new AudioPortContainer
                {
                    Name = name,
                    PortProcessor = processor,
                    Ports = processor.AudioInPorts.ToList(),
                };

                AudioPorts.Add(name, container);
            }

            return true;
        }
    }
}