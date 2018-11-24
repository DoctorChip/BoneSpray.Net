using System.Numerics;
using Veldrid;

namespace BoneSpray.Net.Visuals.Models.RenderObjects
{
    struct VertexPositionColor
    {
        public const uint SizeInBytes = 24;

        // This is the position, in normalized device coordinates.
        public Vector2 Position;

        // This is the color of the vertex.
        public RgbaFloat Color; 

        public VertexPositionColor(Vector2 position, RgbaFloat color)
        {
            Position = position;
            Color = color;
        }
    }
}