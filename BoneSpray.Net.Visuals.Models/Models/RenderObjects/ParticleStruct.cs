using System.Numerics;

namespace BoneSpray.Net.Visuals.Models.Models.RenderObjects
{
    public struct ParticleStruct
    {
        //public Vector2 Position;
        public float PositionX;
        public float PositionY;

        //public Vector2 Velocity;
        public float VelocityX;
        public float VelocityY;

        //public Vector4 Color;
        public float ColorX;
        public float ColorY;
        public float ColorZ;
        public float ColorW;

        public ParticleStruct(Vector2 position, Vector2 velocity, Vector4 color)
        {
            PositionX = position.X;
            PositionY = position.Y;

            VelocityX = velocity.X;
            VelocityY = velocity.Y;

            ColorX = color.X;
            ColorY = color.Y;
            ColorZ = color.Z;
            ColorW = color.W;
        }
    }
}