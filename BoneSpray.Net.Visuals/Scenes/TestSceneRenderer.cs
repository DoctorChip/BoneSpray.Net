using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using BoneSpray.Net.Scenes.Implementations;
using BoneSpray.Net.Visuals.Models.Models.Attributes;
using BoneSpray.Net.Visuals.Models.Models.RenderObjects;
using BoneSpray.Net.Visuals.Models.RenderObjects;
using JackSharp.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private Pipeline ComputePipeline;
        private Shader ComputeShader;
        private Shader VertexShader;
        private Shader FragmentShader;
        private ResourceSet ComputeResourceSet;

        private Pipeline GraphicsPipeline;
        private ResourceSet GraphicsResourceSet;

        public override void Draw()
        {
            if (!Initialised) { return; }

            CommandList.Begin();
            CommandList.SetPipeline(ComputePipeline);
            CommandList.SetComputeResourceSet(0, ComputeResourceSet);
            CommandList.Dispatch(1024, 1, 1);
            CommandList.SetFramebuffer(MainSwapchain.Framebuffer);
            CommandList.SetFullViewports();
            CommandList.SetFullScissorRects();
            CommandList.ClearColorTarget(0, RgbaFloat.Black);
            CommandList.SetPipeline(GraphicsPipeline);
            CommandList.SetGraphicsResourceSet(0, GraphicsResourceSet);
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
            ParticleBuffer = VisualsControlService.ResourceFactory.CreateBuffer(
                new BufferDescription(
                    (uint)Unsafe.SizeOf<ParticleStruct>() * ParticleCount,
                    BufferUsage.StructuredBufferReadWrite,
                    (uint)Unsafe.SizeOf<ParticleStruct>()));

            ComputeShader = VisualsControlService.ResourceFactory.CreateShader(new ShaderDescription(
                ShaderStages.Compute,
                ReadEmbeddedAssetBytes($"ParticlesCompute.{GetExtension(VisualsControlService.GraphicsDevice.BackendType)}"),
                "CS"));

            ResourceLayout particleStorageLayout = VisualsControlService.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ParticlesBuffer", ResourceKind.StructuredBufferReadWrite, ShaderStages.Compute)));

            ComputePipelineDescription computePipelineDesc = new ComputePipelineDescription(
                ComputeShader,
                new[] { particleStorageLayout },
                1, 1, 1);

            ComputePipeline = VisualsControlService.ResourceFactory.CreateComputePipeline(ref computePipelineDesc);

            ComputeResourceSet = VisualsControlService.ResourceFactory.CreateResourceSet(new ResourceSetDescription(particleStorageLayout, ParticleBuffer));

            VertexShader = VisualsControlService.ResourceFactory.CreateShader(new ShaderDescription(
                ShaderStages.Vertex,
                ReadEmbeddedAssetBytes($"ParticlesVertex.{GetExtension(VisualsControlService.ResourceFactory.BackendType)}"),
                "VS"));
            FragmentShader = VisualsControlService.ResourceFactory.CreateShader(new ShaderDescription(
                ShaderStages.Fragment,
                ReadEmbeddedAssetBytes($"ParticlesFragment.{GetExtension(VisualsControlService.ResourceFactory.BackendType)}"),
                "FS"));

            ShaderSetDescription shaderSet = new ShaderSetDescription(
                Array.Empty<VertexLayoutDescription>(),
                new[]
                {
                    VertexShader,
                    FragmentShader
                });

            particleStorageLayout = VisualsControlService.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ParticlesBuffer", ResourceKind.StructuredBufferReadOnly, ShaderStages.Vertex)));

            GraphicsPipelineDescription particleDrawPipelineDesc = new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.Disabled,
                RasterizerStateDescription.Default,
                PrimitiveTopology.PointList,
                shaderSet,
                new[] { particleStorageLayout },
                MainSwapchain.Framebuffer.OutputDescription);

            GraphicsPipeline = VisualsControlService.ResourceFactory.CreateGraphicsPipeline(ref particleDrawPipelineDesc);

            GraphicsResourceSet = VisualsControlService.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                particleStorageLayout,
                ParticleBuffer));

            CommandList = VisualsControlService.ResourceFactory.CreateCommandList();

            InitResources();

            Initialised = true;
        }

        private void InitResources()
        {
            CommandList.Begin();

            ParticleStruct[] initialParticles = new ParticleStruct[ParticleCount];
            Random r = new Random();

            for (int i = 0; i < ParticleCount; i++)
            {
                ParticleStruct pi = new ParticleStruct(
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
}
    }
}