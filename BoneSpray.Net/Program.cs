using System;
using Bonespray.Net;
using BoneSpray.Net.Scenes;
using BoneSpray.Net.Services;

namespace BoneSpray.Net
{
    class Program
    {
        private const string CLIENT_NAME = "JackServerBoneSpray";
        public const bool FORCE_START = false;

        public static ClientControlService clientController = new ClientControlService(CLIENT_NAME);

        static void Main(string[] args)
        {
            PortControlService.Initalise();
            FindAndRegisterScenes();

            InitaliseJack();
            HangForInput();
        }

        static void InitaliseJack()
        {
            var start = clientController.Start(FORCE_START);
            var startedPorts = PortControlService.ConnectAllPorts();
            if (!start) throw new Exception("Unable to connect to JACK server with C# Client.");
            if (!startedPorts) throw new Exception("Unable to connect some or all of the ports to the client.");

        }

        static void FindAndRegisterScenes()
        {
            // Find and instantiate our scenes
            var scenesCount = SceneOrchestrator.FindScenes();

            // Hook our scenes up to the jack ports they need
            var canRegisterMidi = SceneOrchestrator.ConnectScenesMidiPorts();
            var canRegisterAudio = SceneOrchestrator.ConnectScenesAudioPorts();

            if (!canRegisterAudio || !canRegisterMidi) throw new Exception("Unable to connect ports for scenes.");

            if (scenesCount == 0) throw new Exception("Unable to find any Scenes.");
        }

        static void HangForInput()
        {
            Console.WriteLine(">> Init");
            Console.ReadLine();
        }
    }
}