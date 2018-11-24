using JackSharp;
using JackSharp.Ports;
using System;

namespace BoneSpray.Net.Services
{
    /// <summary>
    /// Controls the JACK connection
    /// </summary>
    public static class ClientControlService 
    {
        private static Controller controller;

        /// <summary>
        /// The name of the JACK client we are going to create;
        /// </summary>
        public static string ClientName = null;
        
        /// <summary>
        /// If the controller is connected to the JACK server.
        /// </summary>
        public static bool IsConnected => controller.IsConnectedToJack;

        /// <summary>
        /// Gets the sample rate set for the JACK server.
        /// </summary>
        public static int SampleRate => controller.SampleRate;

        /// <summary>
        /// Set our ClientName, prior to initalising the client
        /// </summary>
        public static void SetName(string name) => ClientName = name;

        /// <summary>
        /// Starts the JACK connection, and optionally the Server.
        /// </summary>
        /// <param name="startJack">Wether to start the JACK server. Default: false.</param>
        /// <returns>Success</returns>
        public static bool Start(bool startJack = false)
        {
            return controller.Start(startJack);
        }

        /// <summary>
        /// Check all of our configuration is correct, and if so, create our Client.
        /// </summary>
        public static void Create()
        {
            if (controller != null) return;

            if (ClientName == null) throw new Exception(
                "Client Name not set prior to creating Controller. Set ClientName first.");

            controller = new Controller(ClientName);
        }

        /// <summary>
        /// Connect two ports together.
        /// </summary>
        /// <param name="inPort">The in port to connect.</param>
        /// <param name="outPort">The out port to connect.</param>
        public static bool Connect(PortReference inPort, PortReference outPort)
        {
            return controller.Connect(inPort, outPort);
        }

        /// <summary>
        /// Disconnect two ports.
        /// </summary>
        public static bool Disconnect(PortReference inPort, PortReference outPort)
        {
            return controller.Disconnect(outPort, inPort);
        }

        /// <summary>
        /// Stops the JACK connection.
        /// </summary>
        /// <returns>Success</returns>
        public static bool Stop()
        {
            return controller.Stop();
        }
    }
}