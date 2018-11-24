using System;
using Bonespray.Net.Services;
using BoneSpray.Net.Scenes;
using BoneSpray.Net.Services;
using BoneSpray.Net.Visuals;
using JackSharp.Ports;

namespace BoneSpray.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            // #Style
            PrettyPrint();

            // Launch our JACK Client
            LaunchJackClient();

            // Process all of our scenes, connecting up their ports.
            FindAndRegisterScenes();

            // Fire up the JACK server, and start all of the defined ports from above.
            InitaliseJack();

            // Hang for user confirmation before launching visuals
            Console.WriteLine("> All functions complete. Press any key to continue.");
            Console.ReadKey();

            // Fire up our visuals woo
            Console.WriteLine("> Starting VELDRID window...");
            VisualsControlService.Run();
        }

        /// <summary>
        /// Assigns a name for our jack client and creates it.
        /// </summary>
        static void LaunchJackClient()
        {
            ClientControlService.SetName("BoneSpray");
            ClientControlService.Create();
            Console.WriteLine("> JACK Service Created.");
        }

        /// <summary>
        /// Starts the JACK service and connects and defined ports.
        /// </summary>
        static void InitaliseJack()
        {
            // Start JACK
            var start = ClientControlService.Start();
            if (!start) throw new Exception("Unable to connect to JACK server with C# Client.");
            Console.WriteLine("> JACK service started.");

            // Connect all of our defined ports from the RequiredPortAttributes
            var startedPorts = PortControlService.ConnectAllPorts();
            if (!startedPorts) throw new Exception("Unable to connect ports to JACK client.");
            Console.WriteLine("> All JACK ports opened and connected to client.");
            Console.WriteLine();
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
            Console.WriteLine($"> Found {scenesCount} scene(s) to register.");

            // Hook our scenes up to the jack ports they need
            var canRegisterMidi = SceneOrchestrator.ConnectScenesPorts(PortType.Midi);
            var canRegisterAudio = SceneOrchestrator.ConnectScenesPorts(PortType.Audio);
            if (!canRegisterAudio || !canRegisterMidi) throw new Exception("Unable to connect ports for scenes.");
            Console.WriteLine("> Connected all scene ports to JACK.");
        }

        /// <summary>
        /// Because #Style.
        /// </summary>
        static void PrettyPrint()
        {
            Console.WriteLine("#####################################################");
            Console.WriteLine("##                                                 ##");
            Console.WriteLine("##               BONE                              ##");
            Console.WriteLine("##                              SPRAY              ##");
            Console.WriteLine("##                                                 ##");
            Console.WriteLine("#####################################################");
            Console.WriteLine();
        }
    }
}