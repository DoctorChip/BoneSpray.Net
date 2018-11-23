using BoneSpray.Net.Models;
using JackSharp.Processing;
using System.Collections.Generic;

namespace BoneSpray.Net.Extensions
{
    public static class ProcessBufferExtensions
    {
        /// <summary>
        /// Accept a ProcessBuffer object from Jack client, and transform it into a simple
        /// list of MIDI events we can more easily work with.
        /// See: https://www.midi.org/specifications-old/item/table-2-expanded-messages-list-status-bytes
        /// </summary>
        public static List<SimpleMidiEvent> GetSimpleEvents(this ProcessBuffer buffer)
        {
            // If we dont have any data in the buffer, just return an empty list
            var notes = new List<SimpleMidiEvent>();
            if (buffer.MidiIn.Length == 0) return notes;

            foreach (MidiEventCollection<MidiInEvent> midi in buffer.MidiIn)
            {
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
            }

            return notes;
        }
    }
}
