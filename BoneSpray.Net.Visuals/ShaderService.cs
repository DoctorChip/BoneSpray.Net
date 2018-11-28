using System.IO;
using Veldrid;

namespace BoneSpray.Net.Visuals
{
    public static class ShaderService
    {
        public static Shader LoadShader(ShaderStages stage)
        {
            string extension = null;
            switch (VisualsControlService.GraphicsDevice.BackendType)
            {
                case GraphicsBackend.Direct3D11:
                    extension = "hlsl";
                    break;
                case GraphicsBackend.Vulkan:
                    extension = "spv";
                    break;
                case GraphicsBackend.OpenGL:
                    extension = "glsl";
                    break;
                case GraphicsBackend.Metal:
                    extension = "metallib";
                    break;
                default:
                    throw new System.InvalidOperationException();
            }

            string entryPoint = stage == ShaderStages.Vertex ? "VS" : "FS";
            string path = Path.Combine(System.AppContext.BaseDirectory, "Shaders", $"{stage.ToString()}.{extension}");
            byte[] shaderBytes = File.ReadAllBytes(path);
            return VisualsControlService.GraphicsDevice.ResourceFactory.CreateShader(new ShaderDescription(stage, shaderBytes, entryPoint));
        }
    }
}