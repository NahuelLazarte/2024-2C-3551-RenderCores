using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP;

namespace TGC.MonoGame.TP
{
    
    public class FreeCamera
    {
        private Vector3 cameraPosition;
        private Vector3 cameraFront;
        private Vector3 cameraUp;
        private float yaw;
        private float pitch;
        private MouseState prevMouseState;
        private float cameraSpeed = 0.9f;
        private float mouseSensitivity = 0.2f; 
        private GraphicsDevice graphicsDevice;

        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        public FreeCamera(Vector3 position, GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            cameraPosition = position;
            cameraFront = Vector3.UnitZ;
            cameraUp = Vector3.UnitY;
            yaw = -MathHelper.PiOver2;
            pitch = 0f;

            float aspectRatio = graphicsDevice.Viewport.AspectRatio;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.01f, 1000f);
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
                cameraPosition += cameraFront * cameraSpeed;
            if (keyboardState.IsKeyDown(Keys.S))
                cameraPosition -= cameraFront * cameraSpeed;
            if (keyboardState.IsKeyDown(Keys.A))
                cameraPosition -= Vector3.Cross(cameraFront, cameraUp) * cameraSpeed;
            if (keyboardState.IsKeyDown(Keys.D))
                cameraPosition += Vector3.Cross(cameraFront, cameraUp) * cameraSpeed;
            if (keyboardState.IsKeyDown(Keys.Space))
                cameraPosition += cameraUp * cameraSpeed;
            if (keyboardState.IsKeyDown(Keys.LeftShift))
                cameraPosition -= cameraUp * cameraSpeed;

            float deltaX = -(mouseState.X - prevMouseState.X) * mouseSensitivity;
            float deltaY = (mouseState.Y - prevMouseState.Y) * mouseSensitivity;
            yaw += deltaX * 0.01f;
            pitch += deltaY * 0.01f;

            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            prevMouseState = Mouse.GetState();

            cameraFront = Vector3.Transform(Vector3.UnitZ, Matrix.CreateFromYawPitchRoll(yaw, pitch, 0));

            ViewMatrix = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraFront, cameraUp);
        }
    }

}