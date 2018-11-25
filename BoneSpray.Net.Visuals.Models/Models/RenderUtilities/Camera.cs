using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace BoneSpray.Net.Visuals.Models.Models.RenderUtilities
{
    public class Camera
    {
        private float _near = 1f;

        private float _far = 1000f;

        private Vector3 _position = new Vector3(0, 3, 0);

        private Vector3 _lookDirection = new Vector3(0, -.3f, -1f);

        private float _yaw;

        private float _pitch;

        private float _windowWidth;

        private float _windowHeight;

        public event Action<Matrix4x4> ProjectionChanged;

        public event Action<Matrix4x4> ViewChanged;

        public Matrix4x4 ViewMatrix { get; private set; }

        public Matrix4x4 ProjectionMatrix { get; private set; }

        public Vector3 Position { get => _position; set { _position = value; UpdateViewMatrix(); } }

        public float FarDistance { get => _far; set { _far = value; UpdatePerspectiveMatrix(); } }

        public float FieldOfView { get; } = 1f;

        public float NearDistance { get => _near; set { _near = value; UpdatePerspectiveMatrix(); } }

        public float AspectRatio => _windowWidth / _windowHeight;

        public float Yaw { get => _yaw; set { _yaw = value; UpdateViewMatrix(); } }

        public float Pitch { get => _pitch; set { _pitch = value; UpdateViewMatrix(); } }

        public Vector3 Forward => GetLookDir();

        public CameraInfo GetCameraInfo() => new CameraInfo
        {
            CameraPosition_WorldSpace = _position,
            CameraLookDirection = _lookDirection
        };

        public Camera(float width, float height)
        {
            _windowWidth = width;
            _windowHeight = height;
            UpdatePerspectiveMatrix();
            UpdateViewMatrix();
        }

        public void Update(float deltaSeconds)
        {
            UpdateViewMatrix();
        }

        private float Clamp(float value, float min, float max)
        {
            return value > max
                ? max
                : value < min
                    ? min
                    : value;
        }

        public void WindowResized(float width, float height)
        {
            _windowWidth = width;
            _windowHeight = height;
            UpdatePerspectiveMatrix();
        }

        private void UpdatePerspectiveMatrix()
        {
            ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, _windowWidth / _windowHeight, _near, _far);
            ProjectionChanged?.Invoke(ProjectionMatrix);
        }

        private void UpdateViewMatrix()
        {
            Vector3 lookDir = GetLookDir();
            _lookDirection = lookDir;
            ViewMatrix = Matrix4x4.CreateLookAt(_position, _position + _lookDirection, Vector3.UnitY);
            ViewChanged?.Invoke(ViewMatrix);
        }

        private Vector3 GetLookDir()
        {
            Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            Vector3 lookDir = Vector3.Transform(-Vector3.UnitZ, lookRotation);
            return lookDir;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraInfo
    {
        public Vector3 CameraPosition_WorldSpace;
        private float _padding1;
        public Vector3 CameraLookDirection;
        private float _padding2;
}
}
