using BoneSpray.Net.Services;
using JackSharp.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoneSpray.Net.Scenes
{
    public static class SceneOrchestrator
    {
        /// <summary>
        /// A list of all scenes currently activate. Built on startup using reflection.
        /// Can be accessed by Key.
        /// </summary>
        private static Dictionary<string, IBaseScene> _scenes = new Dictionary<string, IBaseScene>();

        /// <summary>
        /// The scene type that all of our scenes extend from.
        /// </summary>
        private static readonly Type sceneBase = typeof(BaseScene);

        /// <summary>
        /// Use reflection to find all of our scenes and add them to this static class for easier access,
        /// and to keep them in memory. :)
        /// </summary>
        /// <returns>Scene count</returns>
        public static int FindScenes()
        {
            var sceneTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(ass => ass.GetTypes())
                .Where(type =>
                    sceneBase.IsAssignableFrom(type) &&
                    type.IsClass && !type.IsAbstract);

            foreach (var type in sceneTypes)
            {
                IBaseScene instance = (IBaseScene)Activator.CreateInstance(type);
                var key = instance.GetKey();
                _scenes.Add(key, instance);
            }

            return _scenes.Count();
        }

        /// <summary>
        /// Finds the RequiredMidiPortNames property on each scene and hooks up the ports it asks for.
        /// </summary>
        /// <returns>If successful, true</returns>
        public static bool ConnectScenesMidiPorts()
        {
            if (_scenes == null || _scenes.Count() == 0) return false;

            foreach (var scene in _scenes)
            {
                var requiredPorts = scene.Value.RequiredMidiPortNames;
                foreach (var port in requiredPorts)
                {
                    if (!PortControlService.PortExists(port.Key, PortType.Midi))
                    {
                        var createdPort = PortControlService.CreatePort(port.Key, PortType.Midi);
                        if (!createdPort) throw new Exception($"Unable to creat port: {port.Key}.");
                    }

                    var existingPort = PortControlService.MidiPorts.Single(x => x.Key == port.Key);
                    existingPort.Value.PortProcessor.ProcessFunc += port.Value;
                }
            }

            return true;
        }
        /// <summary>
        /// Finds the RequiredAudioPortNames property on each scene and hooks up the ports it asks for.
        /// TODO: Can this be combined generically with the above method?
        /// </summary>
        /// <returns>If successful, true</returns>
        public static bool ConnectScenesAudioPorts()
        {
            if (_scenes == null || _scenes.Count() == 0) return false;

            foreach (var scene in _scenes)
            {
                var requiredPorts = scene.Value.RequiredMidiPortNames;
                foreach (var port in requiredPorts)
                {
                    if (!PortControlService.PortExists(port.Key, PortType.Audio))
                    {
                        var createdPort = PortControlService.CreatePort(port.Key, PortType.Audio);
                        if (!createdPort) throw new Exception($"Unable to creat port: {port.Key}.");
                    }

                    var existingPort = PortControlService.AudioPorts.Single(x => x.Key == port.Key);
                    existingPort.Value.PortProcessor.ProcessFunc += port.Value;
                }
            }

            return true;
        }
    }
}