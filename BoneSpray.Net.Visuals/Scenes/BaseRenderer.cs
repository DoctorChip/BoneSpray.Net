using Veldrid;

namespace BoneSpray.Net.Visuals.Scenes
{
    public abstract class BaseRenderer
    {
        protected CommandList _commandList;
        protected DeviceBuffer _vertexBuffer;
        protected DeviceBuffer _indexBuffer;
        protected Shader _vertexShader;
        protected Shader _fragmentShader;
        protected Pipeline _pipeline;

        public abstract void Draw();

        public abstract void CreateResources();

        /// <summary>
        /// Dispose of the resources this scene created on startup.
        /// </summary>
        public void DisposeResources()
        {
            _commandList.Dispose();
            _fragmentShader.Dispose();
            _indexBuffer.Dispose();
            _pipeline.Dispose();
            _vertexBuffer.Dispose();
            _vertexShader.Dispose();
        }
    }
}