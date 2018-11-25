using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using BoneSpray.Net.Scenes.Implementations;
using BoneSpray.Net.Visuals.Models.Models.Attributes;
using BoneSpray.Net.Visuals.Models.RenderObjects;
using JackSharp.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace BoneSpray.Net.Visuals.Scenes
{
    [SceneKey("TEST_SCENE2")]
    [SceneSource(typeof(TestScene2))]
    [BindPort(PortType.Midi, typeof(TestScene2), "TestScene2Midi", nameof(HandleCallback))]
    public class TestScene2Renderer : BaseRenderer
    {
        private DeviceBuffer VertexBuffer;
        private DeviceBuffer IndexBuffer;
        private Pipeline Pipeline;

        private Shader VertexShader;
        private Shader FragmentShader;

        private string _resourceDirectory = "";
        protected override string ResourceDirectory { get { return _resourceDirectory; } set { _resourceDirectory = value; } }

        public override void Draw()
        {
            CommandList.Begin();
            CommandList.SetFramebuffer(VisualsControlService.GraphicsDevice.SwapchainFramebuffer);
            CommandList.ClearColorTarget(0, RgbaFloat.Black);

            CommandList.SetVertexBuffer(0, VertexBuffer);
            CommandList.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);
            CommandList.SetPipeline(Pipeline);
            CommandList.DrawIndexed(
                indexCount: 4,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

            CommandList.End();
            VisualsControlService.GraphicsDevice.SubmitCommands(CommandList);
            VisualsControlService.GraphicsDevice.SwapBuffers();
        }

        public void HandleCallback(IEnumerable<SimpleMidiEvent> events)
        {
            throw new Exception($"{events.Count().ToString()} EVENTS REC'D.");
        }

        public override void CreateResources()
        {
            ResourceFactory factory = VisualsControlService.GraphicsDevice.ResourceFactory;

            VertexPositionColor[] quadVertices =
            {
                new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Red)
            };

            ushort[] quadIndices = { 0, 1, 2, 3 };

            VertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            IndexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

            VisualsControlService.GraphicsDevice.UpdateBuffer(VertexBuffer, 0, quadVertices);
            VisualsControlService.GraphicsDevice.UpdateBuffer(IndexBuffer, 0, quadIndices);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4));

            VertexShader = ShaderService.LoadShader(ShaderStages.Vertex);
            FragmentShader = ShaderService.LoadShader(ShaderStages.Fragment);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual)
            };

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            pipelineDescription.ResourceLayouts = Array.Empty<ResourceLayout>();
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: new Shader[] { VertexShader, FragmentShader });

            pipelineDescription.Outputs = VisualsControlService.GraphicsDevice.SwapchainFramebuffer.OutputDescription;

            Pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            CommandList = factory.CreateCommandList();
        }
    }
}