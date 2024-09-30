using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP
{
    public class FollowCamera
    {
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        private Vector3 position;
        private Vector3 target;
        private Vector3 up;
        private Vector3 offset = new(0f, 5f, 30f);

        public FollowCamera(GraphicsDevice graphicsDevice, Vector3 initialPosition, Vector3 initialTarget, Vector3 initialUp)
        {
            position = initialPosition;
            target = initialTarget;
            up = initialUp;

            ViewMatrix = Matrix.CreateLookAt(position, target, up);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f);
        }

        public void Update(Vector3 objectPosition)
        {

            var mouseState = Mouse.GetState();
            float deltaX = mouseState.X - (GraphicsDeviceManager.DefaultBackBufferWidth / 2);
            float deltaY = mouseState.Y - (GraphicsDeviceManager.DefaultBackBufferHeight / 2);

            position = objectPosition + offset;

            position = Vector3.Transform(position - objectPosition, Matrix.CreateFromAxisAngle(up, -0.007f * deltaX)) + objectPosition;
            float angleY = MathHelper.Clamp(0.0004f * deltaY, -MathHelper.PiOver2, MathHelper.PiOver2);
            position = Vector3.Transform(position - objectPosition, Matrix.CreateFromAxisAngle(Vector3.Cross(up, position - objectPosition), angleY)) + objectPosition;

            // Actualizar la matriz de vista
            ViewMatrix = Matrix.CreateLookAt(position, objectPosition, up);
        }
    }
}
