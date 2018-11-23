using BoneSpray.Net.Models;
using BoneSpray.Net.Scenes.Attributes;
using JackSharp.Ports;
using JackSharp.Processing;
using System;
using System.Collections.Generic;

namespace BoneSpray.Net.Scenes.Implementations
{
    [SceneKey("TEST_SCENE")]
    [RequiredPort(Type: PortType.Midi, PortName: "1", CallbackName: nameof(ProcessMidiOne))]
    [RequiredPort(Type: PortType.Audio, PortName: "1", CallbackName: nameof(ProcessAudioOne))]
    public class TestScene : BaseScene
    {
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