using BoneSpray.Net.Models;
using BoneSpray.Net.Services;
using JackSharp.Ports;
using System;
using System.Linq;

namespace BoneSpray.Net.Visuals
{
    public static class PortConnectionHelper
    {


        /// <summary>
        /// Find a given PortContainer for a scene and port number.
        /// This container holds an Action for the given scene and port, which delegates can be attached
        /// to.
        /// </summary>
        public static OutMidiPortContainer GetMidiPortContainer(Type sceneType, string portName)
        {
            var scenePorts = SceneOrchestrator.GetPortsBySceneType(sceneType);
            var midiPortContainer = (OutMidiPortContainer)scenePorts.SingleOrDefault(x => x.Name == portName && x.Type == PortType.Midi);
            return midiPortContainer;
        }

        /// <summary>
        /// Find a given PortContainer for a scene and port number.
        /// This container holds an Action for the given scene and port, which delegates can be attached
        /// to.
        /// </summary>
        public static OutAudioPortContainer GetAudioPortContainer(Type sceneType, string portName)
        {
            var scenePorts = SceneOrchestrator.GetPortsBySceneType(sceneType);
            var audioPortContainer = (OutAudioPortContainer)scenePorts.SingleOrDefault(x => x.Name == portName && x.Type == PortType.Audio);
            return audioPortContainer;
        }
    }
}
