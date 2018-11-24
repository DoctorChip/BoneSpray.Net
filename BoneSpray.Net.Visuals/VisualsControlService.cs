using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using BoneSpray.Net.Models.Events;
using BoneSpray.Net.Scenes;
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
        /// A container for all Renderers.
        /// </summary>
        private static Dictionary<string, BaseRenderer> Renderers { get; set; } = new Dictionary<string, BaseRenderer>();

        /// <summary>
        /// A map between the BoneSpray Scene objects and their renderers.
        /// </summary>
        public static Dictionary<Type, Type> SceneToRendererMap { get; set; } = new Dictionary<Type, Type>();

        /// <summary>
        /// The active renderer
        /// </summary>
        private static BaseRenderer ActiveRenderer { get; set; }

        public static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// The main entry point for our visual app.
        /// This class will manage all of the Scenes we need to render, and switching between them,
        /// as well as building and maintaining the graphics window.
        /// </summary>
        public static void Run()
        {
            // Build our graphics window
            var window = CreateWindow();

            // Find and Load all of our Renderers
            FindAllRenderers();

            // Bind all the ports required for our Visual Scenes
            BindAllPortsFromAttributes();

            // Listen to any events from BoneSpray, such as Scene Changed
            BindToEvents();

            // Enter our render loop
            while (window.Exists)
            {
                window.PumpEvents();
                ActiveRenderer.Draw();
            }
        }

        /// <summary>
        /// Sets the active renderer.
        /// </summary>
        public static void SetActiveScene(string sceneKey)
        {
            var sceneFound = Renderers.TryGetValue(sceneKey, out var scene);
            if (!sceneFound) throw new Exception("Renderer not found. Is the KEY correct?");
            ActiveRenderer = scene;
        }

        /// <summary>
        /// Creates our native graphics window.
        /// </summary>
        private static Sdl2Window CreateWindow()
        {
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowHeight = 500,
                WindowWidth = 500,
                WindowTitle = "Bone Spray"
            };

            Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);
            GraphicsDevice = VeldridStartup.CreateGraphicsDevice(window);

            return window;
        }

        /// <summary>
        /// Similar to our Scene resolver for the JACK port service, this will build all of our Scenes and store them for
        /// later loading and switching.
        /// </summary>
        private static void FindAllRenderers()
        {
            var sceneTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(ass => ass.GetTypes())
                .Where(type =>
                    typeof(BaseRenderer).IsAssignableFrom(type) &&
                    type.IsClass && !type.IsAbstract);

            foreach (var type in sceneTypes)
            {
                string key = null;
                var instance = (BaseRenderer)Activator.CreateInstance(type);
               
                var attr = type.GetCustomAttributes();

                if (attr != null)
                {
                    var keyAttr = (SceneKeyAttribute)attr.SingleOrDefault(x => (x as SceneKeyAttribute) != null);
                    if (keyAttr != null) {
                        key = keyAttr.Key;
                    }

                    var sourceAttr = (SceneSourceAttribute)attr.SingleOrDefault(x => (x as SceneSourceAttribute) != null);
                    if (sourceAttr != null)
                    {
                        SceneToRendererMap.Add(sourceAttr.Type, instance.GetType());
                    }

                    var startUpSceneAttr = (StartupSceneAttribute)attr.SingleOrDefault(x => (x as StartupSceneAttribute) != null);
                    if (startUpSceneAttr != null)
                    {
                        if (ActiveRenderer != null) throw new Exception("Only one scene should have the StartUpScene attribute!");
                        ActiveRenderer = instance;
                    }
                }

                if (key == null) throw new Exception($"Unable to find a SceneKey attribute for type: {type.FullName}.");

                // Create each renderer's resources
                instance.CreateResources();

                Renderers.Add(key, instance);
            }

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
            SceneOrchestrator.SceneChanged += OnSceneChangedEventHandler;
        }

        /// <summary>
        /// Handle SceneChangeEvents, mapping the passed Scene type into a Render type.
        /// </summary>
        public static void OnSceneChangedEventHandler(object sender, OnSceneChangedEventArgs e)
        {
            var rendererFound = SceneToRendererMap.TryGetValue(e.ActiveScene, out var rendererType);
            if (!rendererFound) return;

            var renderer = Renderers.SingleOrDefault(x => x.Value.GetType() == rendererType);

            SetActiveScene(renderer.Key);
        }


        /// <summary>
        /// Bind all our required ports in the visual scenes to the callback methods specified in the BindPort attributes.
        /// </summary>
        private static void BindAllPortsFromAttributes()
        {
            foreach (var renderer in Renderers)
            {
                var midiBind = GetPortAttributes(PortType.Midi, renderer.Value);
                var audioBind = GetPortAttributes(PortType.Audio, renderer.Value);

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
        /// Gets all the <see cref="BindPortAttribute"/> on a given IBaseScene.
        /// </summary>
        private static IEnumerable<BindPortAttribute> GetPortAttributes(PortType type, BaseRenderer scene)
        {
            var attrs = scene.GetType().GetCustomAttributes(typeof(BindPortAttribute), true);
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
            GraphicsDevice.Dispose();
        }
    }
}
