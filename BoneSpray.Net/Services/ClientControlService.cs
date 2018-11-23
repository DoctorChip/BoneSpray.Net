using JackSharp;
using JackSharp.Ports;

namespace Bonespray.Net
{
    /// <summary>
    /// Controls the JACK connection
    /// </summary>
    public class ClientControlService 
    {
        private static Controller controller;

        public ClientControlService(string controllerName)
        {
            if (controller == null)
            {
                controller = new Controller(controllerName);
            }
        }
        
        /// <summary>
        /// If the controller is connected to the JACK server.
        /// </summary>
        public bool IsConnected => controller.IsConnectedToJack;

        /// <summary>
        /// Gets the sample rate set for the JACK server.
        /// </summary>
        public int SampleRate => controller.SampleRate;

        /// <summary>
        /// Starts the JACK connection, and optionally the Server.
        /// </summary>
        /// <param name="startJack">Wether to start the JACK server. Default: false.</param>
        /// <returns>Success</returns>
        public bool Start(bool startJack = false)
        {
            return controller.Start(startJack);
        }

        /// <summary>
        /// Connect two ports together.
        /// </summary>
        /// <param name="inPort">The in port to connect.</param>
        /// <param name="outPort">The out port to connect.</param>
        public bool Connect(PortReference inPort, PortReference outPort)
        {
            return controller.Connect(inPort, outPort);
        }

        /// <summary>
        /// Disconnect two ports.
        /// </summary>
        public bool Disconnect(PortReference inPort, PortReference outPort)
        {
            return controller.Disconnect(outPort, inPort);
        }

        /// <summary>
        /// Stops the JACK connection.
        /// </summary>
        /// <returns>Success</returns>
        public bool Stop()
        {
            return controller.Stop();
        }
    }
}