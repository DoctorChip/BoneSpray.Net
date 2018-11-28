using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using BoneSpray.Net.Scenes.Implementations;
using BoneSpray.Net.Visuals.Models.Models.Attributes;
using BoneSpray.Net.Visuals.Models.Models.Models;
using BoneSpray.Net.Visuals.Models.Models.RenderObjects;
using JackSharp.Ports;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;

namespace BoneSpray.Net.Visuals.Scenes
{
    [SceneKey("TEST_SCENE")]
    [StartupScene]
    [SceneSource(typeof(TestScene))]
    [BindPort(PortType.Midi, typeof(TestScene), "TestSceneMidi", nameof(HandleCallback))]
    public class TestSceneRenderer : BaseRenderer
    {
        /// <summary>
        /// The number of particles to render.
        /// </summary>
        private const int ParticleCount = 1024;

        private DeviceBuffer ParticleBuffer;
        private DeviceBuffer ScreenSizeBuffer;
        private Pipeline ComputePipeline;
        private Pipeline GraphicsPipeline;
        private Shader ComputeShader;
        private Shader VertexShader;
        private Shader FragmentShader;
        private ResourceSet ScreenSizeResourceSet;
        private ResourceSet ComputeResourceSet;
        private ResourceSet ComputeScreenSizeResourceSet;
        private ResourceSet GraphicsResourceSet;

        private string _resourceDirectory = "Particles";
        protected override string ResourceDirectory { get { return _resourceDirectory; } set { _resourceDirectory = value; } }

        public override void Draw(float deltaSeconds)
        {
            if (!Initialised) { return; }

            CommandList.Begin();

            CommandList.SetPipeline(ComputePipeline);
            CommandList.SetComputeResourceSet(0, ComputeResourceSet);
            CommandList.SetComputeResourceSet(1, ComputeScreenSizeResourceSet);
            CommandList.Dispatch(1024, 1, 1);

            CommandList.SetFramebuffer(MainSwapchain.Framebuffer);
            CommandList.SetFullViewports();
            CommandList.SetFullScissorRects();
            CommandList.ClearColorTarget(0, RgbaFloat.Black);
            CommandList.SetPipeline(GraphicsPipeline);
            CommandList.SetGraphicsResourceSet(0, GraphicsResourceSet);
            CommandList.SetGraphicsResourceSet(1, ScreenSizeResourceSet);
            CommandList.Draw(ParticleCount, 1, 0, 0);
            CommandList.End();

            VisualsControlService.GraphicsDevice.SubmitCommands(CommandList);
            VisualsControlService.GraphicsDevice.SwapBuffers(MainSwapchain);
        }

        public void HandleCallback(IEnumerable<SimpleMidiEvent> events)
        {
        }

        public override void CreateResources()
        {
            ParticleBuffer = Factory.CreateBuffer(
                new BufferDescription(
                    (uint)Unsafe.SizeOf<ParticleInfo>() * ParticleCount,
                    BufferUsage.StructuredBufferReadWrite,
                    (uint)Unsafe.SizeOf<ParticleInfo>()));

            ScreenSizeBuffer = Factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));

            ComputeShader = Factory.CreateShader(new ShaderDescription(
                ShaderStages.Compute,
                ReadAssetBytes(AssetType.Shader, "Compute"),
                "CS"));

            var particleStorageLayout = Factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ParticlesBuffer", ResourceKind.StructuredBufferReadWrite, ShaderStages.Compute)));

            var screenSizeLayout = Factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ScreenSizeBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            var computePipelineDesc = new ComputePipelineDescription(
                ComputeShader,
                new[] { particleStorageLayout, screenSizeLayout },
                1, 1, 1);

            ComputePipeline = Factory.CreateComputePipeline(ref computePipelineDesc);

            ComputeResourceSet = Factory.CreateResourceSet(new ResourceSetDescription(particleStorageLayout, ParticleBuffer));

            ComputeScreenSizeResourceSet = Factory.CreateResourceSet(new ResourceSetDescription(screenSizeLayout, ScreenSizeBuffer));

            VertexShader = Factory.CreateShader(new ShaderDescription(
                ShaderStages.Vertex,
                ReadAssetBytes(AssetType.Shader, "Vertex"),
                "VS"));

            FragmentShader = Factory.CreateShader(new ShaderDescription(
                ShaderStages.Fragment,
                ReadAssetBytes(AssetType.Shader, "Fragment"),
                "FS"));

            var shaderSet = new ShaderSetDescription(
                Array.Empty<VertexLayoutDescription>(),
                new[]
                {
                    VertexShader,
                    FragmentShader
                });

            particleStorageLayout = Factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ParticlesBuffer", ResourceKind.StructuredBufferReadOnly, ShaderStages.Vertex)));

            screenSizeLayout = Factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ScreenSizeBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            var particleDrawPipelineDesc = new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.Disabled,
                RasterizerStateDescription.Default,
                PrimitiveTopology.PointList,
                shaderSet,
                new[] { particleStorageLayout, screenSizeLayout },
                MainSwapchain.Framebuffer.OutputDescription);

            GraphicsPipeline = Factory.CreateGraphicsPipeline(ref particleDrawPipelineDesc);

            GraphicsResourceSet = Factory.CreateResourceSet(new ResourceSetDescription(
                particleStorageLayout,
                ParticleBuffer));

            ScreenSizeResourceSet = Factory.CreateResourceSet(new ResourceSetDescription(
                screenSizeLayout,
                ScreenSizeBuffer));

            CommandList = Factory.CreateCommandList();

            CommandList.Begin();

            CommandList.UpdateBuffer(ScreenSizeBuffer, 0, new Vector4(
                VisualsControlService.DebugMode ? VisualsControlService.WindowX_Debug : VisualsControlService.WindowX,
                VisualsControlService.DebugMode ? VisualsControlService.WindowY_Debug : VisualsControlService.WindowY,
                0, 0));

            var initialParticles = new ParticleInfo[ParticleCount];
            var r = new Random();

            for (int i = 0; i < ParticleCount; i++)
            {
                var pi = new ParticleInfo(
                    new Vector2((float)(r.NextDouble() * (VisualsControlService.DebugMode ? VisualsControlService.WindowX_Debug : VisualsControlService.WindowX)), 
                                (float)(r.NextDouble() * (VisualsControlService.DebugMode ? VisualsControlService.WindowY_Debug : VisualsControlService.WindowY))),
                    new Vector2((float)(r.NextDouble() * 3), (float)(r.NextDouble() * 3)),
                    new Vector4(0.4f + (float)r.NextDouble() * .6f, 0.4f + (float)r.NextDouble() * .6f, 0.4f + (float)r.NextDouble() * .6f, 1));
                initialParticles[i] = pi;
            }

            CommandList.UpdateBuffer(ParticleBuffer, 0, initialParticles);
            CommandList.End();

            VisualsControlService.GraphicsDevice.SubmitCommands(CommandList);
            VisualsControlService.GraphicsDevice.WaitForIdle();

            Initialised = true;
        }
    }
}