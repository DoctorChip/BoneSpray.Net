using System;
using System.Collections.Generic;
using BoneSpray.Net.Scenes.Extensions;
using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using JackSharp.Ports;
using JackSharp.Processing;

namespace BoneSpray.Net.Scenes.Implementations
{
    [Keybind('1')]
    [StartupScene]
    [SceneKey("TEST_SCENE")]
    [RequiredPort(Type: PortType.Midi, PortName: "1", CallbackName: nameof(ProcessMidiOneEvent))]
    [RequiredPort(Type: PortType.Audio, PortName: "1", CallbackName: nameof(ProcessAudioOneEvent))]
    public class TestScene : BaseScene
    {
        /// <summary>
        /// Action handler for Midi port 1.
        /// </summary>
        public void ProcessMidiOneEvent(ProcessBuffer buffer)
        {
            var notes = buffer.GetSimpleEvents();
            if (notes.Count != 0)
            {
                MidiBroadcastBuffer?.Invoke(notes);
            }
        }

        /// <summary>
        /// Action handler for Audio port 1.
        /// </summary>
        public void ProcessAudioOneEvent(ProcessBuffer buffer)
        {
            if (buffer.AudioIn.Length == 0) return;
            AudioBroadcastBuffer?.Invoke(buffer.AudioIn);
        }

        /// <summary>
        /// Our midi output buffer stream.
        /// </summary>
        public Action<IEnumerable<SimpleMidiEvent>> MidiBroadcastBuffer { get; set; }


        /// <summary>
        /// Our audio output buffer stream.
        /// </summary>
        public Action<AudioBuffer[]> AudioBroadcastBuffer { get; set; }
    }
}