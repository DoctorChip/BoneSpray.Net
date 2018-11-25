using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using BoneSpray.Net.Models.Events;
using BoneSpray.Net.Services;
using BoneSpray.Net.Visuals.Models.Models.Attributes;
using BoneSpray.Net.Visuals.Scenes;
using JackSharp.Ports;
using JackSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace BoneSpray.Net.Visuals
{
    public static class VisualsControlService
    {
        /// <summary>
        /// The Veldrid rendering GraphicsDevice.
        /// </summary>
        public static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// The ResourceFactory for our GraphicsDevice.
        /// </summary>
        public static ResourceFactory ResourceFactory = GraphicsDevice.ResourceFactory;

        /// <summary>
        /// If we should run the window in Debug mode. This will render in a windowed view, smaller than the native
        /// resolution, useful for debugging and checking different window outputs.
        /// </summary>
        public const bool DebugMode = true;

        /// <summary>
        /// The dimensions of our window.
        /// </summary>
        public const int WindowX = 3840;
        public const int WindowY = 2160;
        public const int WindowX_Debug = 1000;
        public const int WindowY_Debug = 500;

        /// <summary>
        /// A container for all Renderers.
        /// </summary>
        private static Dictionary<string, BaseRenderer> Renderers { get; set; } = new Dictionary<string, BaseRenderer>();

        /// <summary>
        /// A map between the BoneSpray Scene objects and their renderers.
        /// </summary>
        public static Dictionary<Type, Type> SceneToRendererMap { get; set; } = new Dictionary<Type, Type>();

        /// <summary>
        /// The startup state of the graphics window. E.g. windowed, fullscreen, etc.
        /// </summary>
        private const WindowState StartupWindowState = WindowState.BorderlessFullScreen;

        /// <summary>
        /// If the cursor is visible in the graphic window.
        /// </summary>
        private const bool CursorVisible = false;

        /// <summary>
        /// The active renderer
        /// </summary>
        private static BaseRenderer ActiveRenderer { get; set; }

        /// <summary>
        /// The underlying graphics API to use.
        /// DirectX is good for windows, although not cross platform. Maybe we'll be
        /// mental one day and run on Linux, in which case OpenGL will be good.
        /// </summary>
        private const GraphicsBackend WindowGraphicsBackend = GraphicsBackend.Direct3D11;

        /// <summary>
        /// The SDL2 Graphics Window.
        /// </summary>
        private static Sdl2Window GraphicsWindow; 

        /// <summary>
        /// The main entry point for our visual app.
        /// This class will manage all of the Scenes we need to render, and switching between them,
        /// as well as building and maintaining the graphics window.
        /// </summary>
        public static void Run()
        {
            // Build our graphics window
            CreateWindow();

            // Find and Load all of our Renderers
            FindAllRenderers();

            // Bind all the ports required for our Visual Scenes
            BindAllPortsFromAttributes();

            // Listen to any events from BoneSpray, such as Scene Changed
            BindToEvents();

            // Enter our render loop
            while (GraphicsWindow.Exists)
            {
                GraphicsWindow.PumpEvents();
                ActiveRenderer.Draw();
            }
        }

        /// <summary>
        /// Sets the active renderer.
        /// </summary>
        public static void SetActiveScene(string sceneKey)
        {
            // Assign a Scene from Renderers to the ActiveScene.
            var sceneFound = Renderers.TryGetValue(sceneKey, out var scene);
            if (!sceneFound) throw new Exception("Renderer not found. Is the KEY correct?");
            ActiveRenderer = scene;
        }

        /// <summary>
        /// Creates our native graphics window.
        /// </summary>
        private static void CreateWindow()
        {
            // Build our window config.
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowHeight = DebugMode ? WindowY_Debug : WindowY,
                WindowWidth = DebugMode ? WindowX_Debug : WindowX,
                WindowInitialState = DebugMode ? WindowState.Normal : StartupWindowState,
                WindowTitle = "Bone Spray"
            };

            // Create and assign the GraphicsDevice to our window.
            GraphicsWindow = VeldridStartup.CreateWindow(ref windowCI);
            GraphicsWindow.CursorVisible = CursorVisible;
            GraphicsDevice = VeldridStartup.CreateGraphicsDevice(GraphicsWindow, WindowGraphicsBackend);
        }

        /// <summary>
        /// Similar to our Scene resolver for the JACK port service, this will build all of our Scenes and store them for
        /// later loading and switching.
        /// </summary>
        private static void FindAllRenderers()
        {
            // Find all Renderer types for construction
            var sceneTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(ass => ass.GetTypes())
                .Where(type =>
                    typeof(BaseRenderer).IsAssignableFrom(type) &&
                    type.IsClass && !type.IsAbstract);

            foreach (var type in sceneTypes)
            {
                string key = null;

                // Build our Renderer.
                var instance = (BaseRenderer)Activator.CreateInstance(type);
               
                var attr = type.GetCustomAttributes();
                if (attr != null)
                {
                    // Find SceneKey value.
                    var keyAttr = (SceneKeyAttribute)attr.SingleOrDefault(x => (x as SceneKeyAttribute) != null);
                    if (keyAttr != null) {
                        key = keyAttr.Key;
                    }

                    // Find SceneSource value.
                    var sourceAttr = (SceneSourceAttribute)attr.SingleOrDefault(x => (x as SceneSourceAttribute) != null);
                    if (sourceAttr != null)
                    {
                        SceneToRendererMap.Add(sourceAttr.Type, instance.GetType());
                    }

                    // Check if the scene is StartupScene.
                    var startUpSceneAttr = (StartupSceneAttribute)attr.SingleOrDefault(x => (x as StartupSceneAttribute) != null);
                    if (startUpSceneAttr != null)
                    {
                        if (ActiveRenderer != null) throw new Exception("Only one scene should have the StartUpScene attribute!");
                        ActiveRenderer = instance;
                    }
                }

                // If we dont have a scene key, we cant register it.
                if (key == null) throw new Exception($"Unable to find a SceneKey attribute for type: {type.FullName}.");

                // Create each renderer's resources.
                instance.CreateResources();

                // Add the Renderer to our Dict.
                Renderers.Add(key, instance);
            }

            // If, after processing all Renderers, we haven't found one that is set as StartupScene, we can't continue.
            if (ActiveRenderer == null)
            {
                throw new Exception("No scene was found with the StartupScene attribute. Please register one!");
            }
        }

        /// <summary>
        /// Bind to any events across BoneSpray, such as the SceneChanged event from our MIDI.
        /// </summary>
        private static void BindToEvents()
        {
            // Register SceneChange event handler.
            SceneOrchestrator.SceneChanged += OnSceneChangedEventHandler;
        }

        /// <summary>
        /// Handle SceneChangeEvents, mapping the passed Scene type into a Render type.
        /// </summary>
        public static void OnSceneChangedEventHandler(object sender, OnSceneChangedEventArgs e)
        {
            // Try to find the Renderer for our new Scene that we are changing to.
            var rendererFound = SceneToRendererMap.TryGetValue(e.ActiveScene, out var rendererType);
            if (!rendererFound) return;

            // Fetch the renderer.
            var renderer = Renderers.SingleOrDefault(x => x.Value.GetType() == rendererType);

            // Assign as Active.
            SetActiveScene(renderer.Key);
        }


        /// <summary>
        /// Bind all our required ports in the visual scenes to the callback methods specified in the BindPort attributes.
        /// </summary>
        private static void BindAllPortsFromAttributes()
        {
            foreach (var renderer in Renderers)
            {
                // Fetch all the reqired MIDI and Audio ports for each Renderer
                var midiBind = GetPortAttributes(PortType.Midi, renderer.Value);
                var audioBind = GetPortAttributes(PortType.Audio, renderer.Value);

                // Build up a delegate on the Renderer of a certain name (see midi.Callback) and assign it to the Event on the incoming stream, for both Midi and Audio
                foreach (var midi in midiBind)
                {
                    var action = PortConnectionHelper.GetMidiPortContainer(midi.Scene, midi.PortName);
                    var portCallbackMethodInfo = renderer.Value.GetType().GetMethod(midi.Callback);
                    var portCallback = (Action<IEnumerable<SimpleMidiEvent>>)Delegate.CreateDelegate(typeof(Action<IEnumerable<SimpleMidiEvent>>), renderer.Value, portCallbackMethodInfo);
                    action.MidiStream += portCallback;
                }

                foreach (var audio in audioBind)
                {
                    var action = PortConnectionHelper.GetAudioPortContainer(audio.Scene, audio.PortName);
                    var portCallbackMethodInfo = renderer.Value.GetType().GetMethod(audio.Callback);
                    var portCallback = (Action<AudioBuffer[]>)Delegate.CreateDelegate(typeof(Action<AudioBuffer[]>), renderer.Value, portCallbackMethodInfo);
                    action.AudioStream+= portCallback;
                } 
            }
        }

        /// <summary>
        /// Gets all the <see cref="BindPortAttribute"/> on a given BaseRenderer.
        /// </summary>
        private static IEnumerable<BindPortAttribute> GetPortAttributes(PortType type, BaseRenderer scene)
        {
            // Find all BindPort attributes on our Renderers
            var attrs = scene.GetType().GetCustomAttributes(typeof(BindPortAttribute), true);

            // Cast and return as an IEnumerable<BindPortAttribute>
            foreach (var attr in attrs)
            {
                var castedAttr = (BindPortAttribute)attr;
                if (castedAttr.Type != type) continue;

                yield return castedAttr;
            }
        }

        /// <summary>
        /// Dispose all of the render resources.
        /// </summary>
        public static void DisposeResources()
        {
            // Dispose from this Service
            GraphicsDevice.Dispose();

            // Dispose from all Renderers
            foreach (var renderer in Renderers)
            {
                renderer.Value.DisposeResources();
            }
        }
    }
}