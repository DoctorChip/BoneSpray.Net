using BoneSpray.Net.Scenes.Extensions;
using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using JackSharp.Ports;
using JackSharp.Processing;
using System.Linq;

namespace BoneSpray.Net.Scenes.Implementations
{
    [Keybind('1')]
    [StartupScene]
    [SceneKey("TEST_SCENE")]
    [RequiredPort(Type: PortType.Midi, PortName: "TestSceneMidi", CallbackName: nameof(ProcessMidiOneEvent))]
    [RequiredPort(Type: PortType.Audio, PortName: "TestSceneAudio", CallbackName: nameof(ProcessAudioOneEvent))]
    public class TestScene : BaseScene
    {
        /// <summary>
        /// Action handler for Midi port 1.
        /// </summary>
        public void ProcessMidiOneEvent(ProcessBuffer buffer)
        {
            var notes = buffer.GetSimpleEvents(out var name);
            if (notes.Count != 0)
            {
                ((OutMidiPortContainer)OutPorts.SingleOrDefault(x => x.Name == name && x.Type == PortType.Midi)).MidiStream?.Invoke(notes);
            }
        }

        /// <summary>
        /// Action handler for Audio port 1.
        /// </summary>
        public void ProcessAudioOneEvent(ProcessBuffer buffer)
        {
            //if (buffer.AudioIn.Length == 0) return;
            //var name = buffer.AudioIn[0].Port.Name;
            //((OutAudioPortContainer)OutPorts.SingleOrDefault(x => x.Name == name)).AudioStream?.Invoke(buffer.AudioIn);
        }
    }
}