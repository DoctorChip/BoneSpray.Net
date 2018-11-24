using BoneSpray.Net.Models.Attributes;
using BoneSpray.Net.Scenes;
using BoneSpray.Net.Scenes.Extensions;
using BoneSpray.Net.Services.Services;
using JackSharp.Ports;
using JackSharp.Processing;

namespace BoneSpray.Net.Services
{
    /// <summary>
    /// Binds to a KeybindPort MIDI port, and passes any midi events
    /// to the KeybindOrchestrator, which will handle scene
    /// switching.
    /// </summary>
    [SceneKey("KEYBIND_SCENE")]
    [RequiredPort(Type: PortType.Midi, PortName: "KeybindPort", CallbackName: nameof(HandleMidiForSceneSwitch))]
    class MidiSceneSwitcher : BaseScene
    {
        public void HandleMidiForSceneSwitch(ProcessBuffer buffer)
        {
            var midiEvents = buffer.GetSimpleEvents(out var name);
            foreach (var midi in midiEvents)
            {
                KeybindOrchestrator.SwitchScene(midi.Note);
            }
        }
    }
}