using System;
using System.Collections.Generic;

namespace BoneSpray.Net.Services.Services
{
    public static class KeybindOrchestrator
    {
        /// <summary>
        /// A map of 0-127 MIDI note value to a scene.
        /// When the MidiKeybinding channel receives a value, it will search this map for a scene.
        /// If a scene is found, it will be marked active.
        /// </summary>
        public static Dictionary<int, Type> KeybindSceneMap { get; set; } = new Dictionary<int, Type>();

        public static void AddKeybindingMap(int key, Type scene)
        {
            KeybindSceneMap.Add(key, scene);
        }

        /// <summary>
        /// Accepts a 0-127 MIDI note to attempt to switch to a scene bound to that value.
        /// </summary>
        public static void SwitchScene(int midiNote)
        {
            var sceneFound = KeybindSceneMap.TryGetValue(midiNote, out var scene);
            if (sceneFound)
            {
                SceneOrchestrator.SetActiveSceneByType(scene);
            }
        }
    }
}
