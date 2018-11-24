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
    [SceneKey("TEST_SCENE")]
    [StartupScene]
    [SceneSource(typeof(TestScene))]
    [BindPort(PortType.Midi, typeof(TestScene), "TestSceneMidi", nameof(HandleCallback))]
    public class TestSceneRenderer : BaseRenderer
    {
        public override void Draw()
        {
            _commandList.Begin();
            _commandList.SetFramebuffer(VisualsControlService.GraphicsDevice.SwapchainFramebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);

            _commandList.SetVertexBuffer(0, _vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _commandList.SetPipeline(_pipeline);
            _commandList.DrawIndexed(
                indexCount: 4,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

            _commandList.End();
            VisualsControlService.GraphicsDevice.SubmitCommands(_commandList);
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
                new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
                new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
            };

            ushort[] quadIndices = { 0, 1, 2, 3 };

            _vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

            VisualsControlService.GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);
            VisualsControlService.GraphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4));

            _vertexShader = ShaderService.LoadShader(ShaderStages.Vertex);
            _fragmentShader = ShaderService.LoadShader(ShaderStages.Fragment);

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
            pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: new Shader[] { _vertexShader, _fragmentShader });

            pipelineDescription.Outputs = VisualsControlService.GraphicsDevice.SwapchainFramebuffer.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            _commandList = factory.CreateCommandList();
        }
    }
}