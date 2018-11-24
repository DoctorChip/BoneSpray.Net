using BoneSpray.Net.Visuals.Scenes;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace BoneSpray.Net.Visuals
{
    public static class VisualsControlService
    {
        private static BaseRenderer ActiveRenderer { get; set; }

        private static GraphicsDevice _graphicsDevice;
        private static CommandList _commandList;
        private static DeviceBuffer _vertexBuffer;
        private static DeviceBuffer _indexBuffer;
        private static Shader _vertexShader;
        private static Shader _fragmentShader;
        private static Pipeline _pipeline;

        /// <summary>
        /// The main entry point for our visual app.
        /// This class will manage all of the Scenes we need to render, and switching between them,
        /// as well as building and maintaining the graphics window.
        /// </summary>
        public static void Run()
        {
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowHeight = 500,
                WindowWidth = 500,
                //WindowInitialState = WindowState.BorderlessFullScreen,
                WindowTitle = "Bone Spray"
            };

            Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

           _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, GraphicsBackend.OpenGL);

            while (window.Exists)
            {
                window.PumpEvents();
            }
        }
    }
}
