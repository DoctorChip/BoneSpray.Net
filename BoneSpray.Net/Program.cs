using System;
using Bonespray.Net;
using BoneSpray.Net.Scenes;
using BoneSpray.Net.Services;
using JackSharp.Ports;

namespace BoneSpray.Net
{
    class Program
    {
        /// <summary>
        /// Our JACK Client!
        /// </summary>
        public static ClientControlService clientController = new ClientControlService("BONE_SPRAY");

        static void Main(string[] args)
        {
            // Process all of our scenes, connecting up their ports.
            FindAndRegisterScenes();

            // Fire up the JACK server, and start all of the defined ports from above.
            InitaliseJack();

            // Keep Alive
            HangForInput();
        }

        /// <summary>
        /// Starts the JACK service and connects and defined ports.
        /// </summary>
        static void InitaliseJack()
        {
            // Start JACK
            var start = clientController.Start();
            if (!start) throw new Exception("Unable to connect to JACK server with C# Client.");

            // Connect all of our defined ports from the RequiredPortAttributes
            var startedPorts = PortControlService.ConnectAllPorts();
            if (!startedPorts) throw new Exception("Unable to connect ports to JACK client.");

        }

        /// <summary>
        /// Hunt through our Assembly for any <see cref="IBaseScene"/> types and create them.
        /// If a scene has requested any ports, hook up the port to the scene too.
        /// </summary>
        static void FindAndRegisterScenes()
        {
            // Find and instantiate our scenes
            var scenesCount = SceneOrchestrator.FindScenes();
            if (scenesCount == 0) throw new Exception("Unable to find any Scenes.");

            // Hook our scenes up to the jack ports they need
            var canRegisterMidi = SceneOrchestrator.ConnectScenesPorts(PortType.Midi);
            var canRegisterAudio = SceneOrchestrator.ConnectScenesPorts(PortType.Audio);
            if (!canRegisterAudio || !canRegisterMidi) throw new Exception("Unable to connect ports for scenes.");
        }

        /// <summary>
        /// Keep the console app alive
        /// </summary>
        static void HangForInput()
        {
            Console.WriteLine(">> Init");
            Console.ReadLine();
        }
    }
}