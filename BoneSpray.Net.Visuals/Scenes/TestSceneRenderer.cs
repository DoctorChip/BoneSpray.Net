using BoneSpray.Net.Models;
using BoneSpray.Net.Models.Attributes;
using BoneSpray.Net.Scenes.Implementations;
using BoneSpray.Net.Services;
using JackSharp.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoneSpray.Net.Visuals.Scenes
{
    [SceneSource(nameof(TestScene))]
    public class TestSceneRenderer : BaseRenderer
    {
        //private static void Draw()
        //{
        //    _commandList.Begin();
        //    _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
        //    _commandList.ClearColorTarget(0, RgbaFloat.Black);

        //    _commandList.SetVertexBuffer(0, _vertexBuffer);
        //    _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
        //    _commandList.SetPipeline(_pipeline);
        //    _commandList.DrawIndexed(
        //        indexCount: 4,
        //        instanceCount: 1,
        //        indexStart: 0,
        //        vertexOffset: 0,
        //        instanceStart: 0);

        //    _commandList.End();
        //    _graphicsDevice.SubmitCommands(_commandList);
        //    _graphicsDevice.SwapBuffers();
        //}

        private void HandleCallback(IEnumerable<SimpleMidiEvent> events)
        {
            /// FUCK YESSSSSS
        }

        public override void CreateResources()
        {
            var scenePorts = SceneOrchestrator.GetPortsByKey("TEST_SCENE");
            var midiOne = (OutMidiPortContainer)scenePorts.SingleOrDefault(x => x.Name == "1" && x.Type == PortType.Midi);
            midiOne.MidiStream += HandleCallback;

        //    ResourceFactory factory = _graphicsDevice.ResourceFactory;

        //    VertexPositionColor[] quadVertices =
        //    {
        //        new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
        //        new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
        //        new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
        //        new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
        //    };

        //    ushort[] quadIndices = { 0, 1, 2, 3 };

        //    _vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
        //    _indexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

        //    _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);
        //    _graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

        //    VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
        //        new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float2),
        //        new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4));

        //    _vertexShader = LoadShader(ShaderStages.Vertex);
        //    _fragmentShader = LoadShader(ShaderStages.Fragment);

        //    GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription
        //    {
        //        BlendState = BlendStateDescription.SingleOverrideBlend,
        //        DepthStencilState = new DepthStencilStateDescription(
        //        depthTestEnabled: true,
        //        depthWriteEnabled: true,
        //        comparisonKind: ComparisonKind.LessEqual)
        //    };

        //    pipelineDescription.RasterizerState = new RasterizerStateDescription(
        //        cullMode: FaceCullMode.Back,
        //        fillMode: PolygonFillMode.Solid,
        //        frontFace: FrontFace.Clockwise,
        //        depthClipEnabled: true,
        //        scissorTestEnabled: false);

        //    pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
        //    pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();
        //    pipelineDescription.ShaderSet = new ShaderSetDescription(
        //        vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
        //        shaders: new Shader[] { _vertexShader, _fragmentShader });

        //    pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;

        //    _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
        //    _commandList = factory.CreateCommandList();
        //}

        //private static Shader LoadShader(ShaderStages stage)
        //{
        //    string extension = null;
        //    switch (_graphicsDevice.BackendType)
        //    {
        //        case GraphicsBackend.Direct3D11:
        //            extension = "hlsl.bytes";
        //            break;
        //        case GraphicsBackend.Vulkan:
        //            extension = "spv";
        //            break;
        //        case GraphicsBackend.OpenGL:
        //            extension = "glsl";
        //            break;
        //        case GraphicsBackend.Metal:
        //            extension = "metallib";
        //            break;
        //        default: throw new System.InvalidOperationException();
        //    }

        //    string entryPoint = stage == ShaderStages.Vertex ? "VS" : "FS";
        //    string path = Path.Combine(System.AppContext.BaseDirectory, "Shaders", $"{stage.ToString()}.{extension}");
        //    byte[] shaderBytes = File.ReadAllBytes(path);
        //    return _graphicsDevice.ResourceFactory.CreateShader(new ShaderDescription(stage, shaderBytes, entryPoint));
        }

        public override void DisposeResources()
        {
            //_pipeline.Dispose();
            //_vertexShader.Dispose();
            //_fragmentShader.Dispose();
            //_commandList.Dispose();
            //_vertexBuffer.Dispose();
            //_indexBuffer.Dispose();
            //_graphicsDevice.Dispose();
        }
    }
}