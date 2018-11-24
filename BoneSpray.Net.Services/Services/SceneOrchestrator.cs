using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using BoneSpray.Net.Scenes;
using JackSharp.Ports;
using JackSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BoneSpray.Net.Services
{
    public static class SceneOrchestrator
    {
        /// <summary>
        /// A list of all scenes currently activate. Built on startup using reflection.
        /// Can be accessed by Key.
        /// </summary>
        private static Dictionary<string, IBaseScene> _scenes = new Dictionary<string, IBaseScene>();

        /// <summary>
        /// The active scene. Initally this will be populated by a scene that has the StartupScene attribute.
        /// </summary>
        public static IBaseScene ActiveScene;

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
                    typeof(BaseScene).IsAssignableFrom(type) &&
                    type.IsClass && !type.IsAbstract);

            foreach (var type in sceneTypes)
            {
                string key = null;
                IBaseScene instance = (IBaseScene)Activator.CreateInstance(type);
               
                var attr = type.GetCustomAttributes();

                if (attr != null)
                {
                    var keyAttr = (SceneKeyAttribute)attr.SingleOrDefault(x => (x as SceneKeyAttribute) != null);
                    if (keyAttr != null) {
                        key = keyAttr.Key;
                    }

                    var startUpSceneAttr = (StartupSceneAttribute)attr.SingleOrDefault(x => (x as StartupSceneAttribute) != null);
                    if (startUpSceneAttr != null)
                    {
                        if (ActiveScene != null) throw new Exception("Only one scene should have the StartUpScene attribute!");
                        ActiveScene = instance;
                    }
                }

                if (key == null) throw new Exception($"Unable to find a SceneKey attribute for type: {type.FullName}.");

                _scenes.Add(key, instance);
            }

            if (ActiveScene == null)
            {
                throw new Exception("No scene was found with the StartupScene attribute. Please register one!");
            }

            return _scenes.Count();
        }

        /// <summary>
        /// Assign the active scene.
        /// </summary>
        public static bool SetActiveSceneByType(Type sceneType)
        {
            var scene = _scenes.SingleOrDefault(x => x.Value.GetType() == sceneType).Value;
            if (scene == null) return false;

            ActiveScene = scene;
            return true;
        }

        /// <summary>
        /// Finds the required ports on each scene and hooks up the ports it asks for.
        /// </summary>
        /// <returns>If successful, true</returns>
        public static bool ConnectScenesPorts(PortType type)
        {
            if (_scenes == null || _scenes.Count() == 0) return false;

            foreach (var scene in _scenes)
            {
                var attrs = GetPortAttributes(type, scene.Value);
                var requiredPorts = attrs.Select(x => new
                {
                    Key = x.PortName,
                    Callback = x.CallbackName,
                });

                foreach (var port in requiredPorts)
                {
                    if (!PortControlService.PortExists(port.Key, type))
                    {
                        var createdPort = PortControlService.CreatePort(port.Key, type);
                        if (!createdPort) throw new Exception($"Unable to creat port: {port.Key}.");
                    }

                    KeyValuePair<string, PortContainer> existingPort;
                    if (type == PortType.Midi)
                    {
                        existingPort = PortControlService.MidiPorts.Single(x => x.Key == port.Key);
                    }
                    else
                    {
                        existingPort = PortControlService.AudioPorts.Single(x => x.Key == port.Key);
                    }

                    var portCallbackMethodInfo = scene.Value.GetType().GetMethod(port.Callback);
                    var portCallback = (Action<ProcessBuffer>)Delegate.CreateDelegate(typeof(Action<ProcessBuffer>), scene.Value, portCallbackMethodInfo);
                    existingPort.Value.PortProcessor.ProcessFunc += portCallback;

                    if (type == PortType.Midi)
                    {
                        scene.Value.OutPorts.Add(new OutMidiPortContainer
                        {
                            Name = port.Key,
                            Type = type,
                            MidiStream = null,
                        });
                    }
                    else
                    {
                        scene.Value.OutPorts.Add(new OutAudioPortContainer
                        {
                            Name = port.Key,
                            Type = type,
                            AudioStream = null,
                        });
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Get the Out Ports for a given scene, by type.
        /// </summary>
        public static IEnumerable<OutPortContainer> GetPortsBySceneType(Type sceneType)
        {
            return _scenes.SingleOrDefault(x => x.Value.GetType() == sceneType).Value.OutPorts;
        }

        /// <summary>
        /// Gets all the <see cref="RequiredPortAttribute"/> on a given IBaseScene.
        /// </summary>
        private static IEnumerable<RequiredPortAttribute> GetPortAttributes(PortType type, IBaseScene scene)
        {
            var attrs = scene.GetType().GetCustomAttributes(typeof(RequiredPortAttribute), true);
            foreach (var attr in attrs)
            {
                var castedAttr = (RequiredPortAttribute)attr;
                if (castedAttr.Type != type) continue;

                yield return castedAttr;
            }
        }
    }
}