using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP
{
    public class FollowCamera
    {
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        public Vector3 position { get; set; }
        private Vector3 target;
        private Vector3 up;
        private Vector3 offset = new(0f, 5f, 30f);

        private Vector3 posicionObjeto;


        public Vector3 GetDirection()
        {
            Vector3 direccion = posicionObjeto - position;
            direccion = new Vector3(direccion.X, 0f, direccion.Z);

            return Vector3.Normalize(direccion);
        }
        public FollowCamera(GraphicsDevice graphicsDevice, Vector3 initialPosition, Vector3 initialTarget, Vector3 initialUp)
        {
            position = initialPosition;
            target = initialTarget;
            up = initialUp;

            ViewMatrix = Matrix.CreateLookAt(position, target, up);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f);

        }

        private float accumulatedDeltaX = 0f;
        private float accumulatedDeltaY = 0f;

        public void Update(Vector3 objectPosition)
        {
            posicionObjeto = objectPosition;
            var mouseState = Mouse.GetState();
            float deltaX = mouseState.X - (GraphicsDeviceManager.DefaultBackBufferWidth / 2);
            float deltaY = mouseState.Y - (GraphicsDeviceManager.DefaultBackBufferHeight / 2);

            // Acumular los deltas del ratón
            accumulatedDeltaX += deltaX;
            accumulatedDeltaY += deltaY;

            // Restablecer el ratón al centro de la pantalla
            Mouse.SetPosition(GraphicsDeviceManager.DefaultBackBufferWidth / 2, GraphicsDeviceManager.DefaultBackBufferHeight / 2);

            position = objectPosition + offset;

            position = Vector3.Transform(position - objectPosition, Matrix.CreateFromAxisAngle(up, -0.007f * accumulatedDeltaX)) + objectPosition;
            float angleY = MathHelper.Clamp(0.0004f * accumulatedDeltaY, -MathHelper.PiOver2, MathHelper.PiOver2);
            position = Vector3.Transform(position - objectPosition, Matrix.CreateFromAxisAngle(Vector3.Cross(up, position - objectPosition), angleY)) + objectPosition;

            // Actualizar la matriz de vista
            ViewMatrix = Matrix.CreateLookAt(position, objectPosition, up);

            //Console.WriteLine($"Camera Position: X={position.X}, Y={position.Y}, Z={position.Z}");
            //Console.WriteLine($"objectPosition: X={objectPosition.X}, Y={objectPosition.Y}, Z={objectPosition.Z}");
            //Console.WriteLine($"up: X={up.X}, Y={up.Y}, Z={up.Z}");
        }
    }
}
