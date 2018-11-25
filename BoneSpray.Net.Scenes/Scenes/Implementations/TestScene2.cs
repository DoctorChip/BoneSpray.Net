using BoneSpray.Net.Scenes.Extensions;
using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using JackSharp.Ports;
using JackSharp.Processing;
using System.Linq;

namespace BoneSpray.Net.Scenes.Implementations
{
    [Keybind(2)]
    [SceneKey("TEST_SCENE2")]
    [RequiredPort(Type: PortType.Midi, PortName: "TestScene2Midi", CallbackName: nameof(ProcessMidi))]
    public class TestScene2 : BaseScene
    {
        /// <summary>
        /// Action handler for Midi port 1.
        /// </summary>
        public void ProcessMidi(ProcessBuffer buffer)
        {
            var notes = buffer.GetSimpleEvents(out var name);
            if (notes.Count != 0)
            {
                ((OutMidiPortContainer)OutPorts.SingleOrDefault(x => x.Name == name && x.Type == PortType.Midi)).MidiStream?.Invoke(notes);
            }
        }
    }
}