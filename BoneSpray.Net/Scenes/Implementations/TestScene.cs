using BoneSpray.Net.Models;
using JackSharp.Ports;
using JackSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoneSpray.Net.Scenes.Implementations
{
    public class TestScene : BaseScene
    {
        #region CONFIGURATION
        /// <summary>
        /// Scene Key. The key used to access and find scenes from the Orchestrator.
        /// </summary>
        protected override string Key => "TEST_SCENE";

        /// <summary>
        /// The MIDI ports that this scene would like to subscribe to and their handlers.
        /// </summary>
        public override Dictionary<string, Action<ProcessBuffer>> RequiredMidiPortNames => 
            new Dictionary<string, Action<ProcessBuffer>>
            {
                { "1", ProcessMidiOne },
            };

        /// <summary>
        /// The live MIDI ports.
        /// </summary>
        private List<MidiInPort> _midi;
        public override List<MidiInPort> MidiPorts
        {
            get { return _midi; }
            protected set { _midi = value; }
        }

        /// <summary>
        /// The Audio ports that this scene would like to subscribe to and their handlers.
        /// </summary>
        public override Dictionary<string, Action<ProcessBuffer>> RequiredAudioPortNames =>
            new Dictionary<string, Action<ProcessBuffer>>
            {   
                { "1", ProcessAudioOne },
            };

        /// <summary>
        /// The live Audio ports.
        /// </summary>
        private List<AudioInPort> _audio;
        public override List<AudioInPort> AudioPorts
        {
            get { return _audio; }
            protected set { _audio = value; }
        }
        #endregion

        /// <summary>
        /// Action handler for Midi port 1.
        /// </summary>
        /// See: https://www.midi.org/specifications-old/item/table-2-expanded-messages-list-status-bytes
        public Action<ProcessBuffer> ProcessMidiOne => ProcessMidiOneEvent;
        public void ProcessMidiOneEvent(ProcessBuffer buffer)
        {
            if (buffer.MidiIn.Length == 0) return;
            foreach (MidiEventCollection<MidiInEvent> midi in buffer.MidiIn)
            {
                var notes = new List<SimpleMidiEvent>();

                foreach (var val in midi)
                {
                    var note = val.MidiData;

                    // 144 = Cha1 ON, 128 = Cha1 OFF
                    notes.Add(new SimpleMidiEvent
                    {
                        Type = note[0] == 144 ? MidiEventType.ON : MidiEventType.OFF,
                        Note = note[1],
                        Velocity = note[2],
                    });
                }

                MidiBroadcastBuffer?.Invoke(notes);
            }
        }

        /// <summary>
        /// Our simple midi output buffer stream wooooo.
        /// Subscribe for more!
        /// </summary>
        public Action<IEnumerable<SimpleMidiEvent>> MidiBroadcastBuffer { get; set; }

        /// <summary>
        /// Action handler for Audio port 1.
        /// </summary>
        public Action<ProcessBuffer> ProcessAudioOne => ProcessAudioOneEvent;
        public void ProcessAudioOneEvent(ProcessBuffer buffer)
        {
            // To complete  :)
            if (buffer.AudioIn.Length == 0) return;
            Console.WriteLine(buffer.AudioIn.GetValue(0));
        }
    }
}