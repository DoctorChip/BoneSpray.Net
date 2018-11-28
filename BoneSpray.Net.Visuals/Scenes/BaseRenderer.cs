using BoneSpray.Net.Visuals.Models.Models.Models;
using BoneSpray.Net.Visuals.Models.Models.RenderUtilities;
using System.IO;
using Veldrid;

namespace BoneSpray.Net.Visuals.Scenes
{
    /// <summary>
    /// Provides a base for each Renderer, setting up things such as the scene
    /// camera and acting as a contract for implementations.
    /// </summary>
    public abstract class BaseRenderer
    {
        /// <summary>
        /// Is the Renderer ready? Set once all resources are ready.
        /// </summary>
        protected bool Initialised = false;

        /// <summary>
        /// The scene's CommandList object, for recording graphics commands.
        /// </summary>
        protected CommandList CommandList { get; set; }

        /// <summary>
        /// The renderers main swapchain.
        /// </summary>
        protected Swapchain MainSwapchain => VisualsControlService.GraphicsDevice.MainSwapchain;

        /// <summary>
        /// Proxy through to the ResourceFactory on our GraphicsDevice.
        /// </summary>
        protected ResourceFactory Factory => VisualsControlService.GraphicsDevice.ResourceFactory;

        /// <summary>
        /// The directory for the Renderer's assets, such as Shaders.
        /// </summary>
        protected abstract string ResourceDirectory { get; set; }

        /// <summary>
        /// Get a shader resource extension based on the current backend type.
        /// </summary>
        protected string GetExtension(GraphicsBackend backendType)
        {
            return backendType == GraphicsBackend.Direct3D11 ? "hlsl"
                : backendType == GraphicsBackend.Vulkan ? "spv"
                    : backendType == GraphicsBackend.Metal ?  "metallib" 
                        : backendType == GraphicsBackend.OpenGL ? "430.glsl"
                            : "300.glsles";
        }

        /// <summary>
        /// Read an embedded asset, by filename, returns a byte[]
        /// </summary>
        protected byte[] ReadAssetBytes(AssetType type, string name)
        {
            var dir = type == AssetType.Shader ? "Shaders" : string.Empty;
            var subdir = ResourceDirectory;
            name += $".{GetExtension(VisualsControlService.GraphicsDevice.BackendType)}";

            var path = Path.Combine(dir, subdir, name);

            using (Stream stream = File.OpenRead(path))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }

        /// <summary>
        /// Required for every scene. This scene will process the DeviceBuffers and pump them into the
        /// GraphicsDevice attached to the VisualsControlService, which renders to our window.
        /// </summary>
        public abstract void Draw(float deltaSeconds);

        /// <summary>
        /// Build any resources such as vertex buffers, pipelines, etc. required for initialising our
        /// scene.
        /// </summary>
        public abstract void CreateResources();

        /// <summary>
        /// Dispose of the resources this scene created on startup.
        /// </summary>
        public void DisposeResources()
        {
            CommandList.Dispose();
            MainSwapchain.Dispose();
        }
    }
}