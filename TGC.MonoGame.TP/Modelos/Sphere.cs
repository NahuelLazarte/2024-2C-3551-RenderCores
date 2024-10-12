using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP.Modelos
{
    class Sphere : Modelo
    {
        float AngleZ = 0f;
        float AngleX = 0f;

        float LinearSpeed = 100f;
        private float RotationSpeed = 10f;
        Vector3 _velocity;

        Vector3 direction;

        

        public void setDirection(Vector3 newDirection)
        {
            direction = newDirection;
            //Console.WriteLine($"Direction set to: {direction}");

        }  


        public Sphere(ContentManager content, Vector3 position, Matrix rotation, Color color)
            : base(content, position, rotation, color)
        {

            Model3D = content.Load<Model>("Models/" + "Spheres/sphere");
            Effect = content.Load<Effect>("Effects/" + "BasicShader");

            SetScale(Matrix.CreateScale(0.01f));

            //SetScale(Matrix.CreateScale(0.01f));

            World = Scale * rotation * Matrix.CreateTranslation(position);


        }
        public override void Update(GameTime gameTime)
        {

            var keyboardState = Keyboard.GetState();

        
            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            

            /*
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Angle += elapsedTime;
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                Angle -= elapsedTime;
            }
            */
            Vector3 acceleration = Vector3.Zero;

            //var direction = Rotation.Forward;

            bool accelerating = false;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                acceleration += LinearSpeed * direction;
                accelerating = true;

                // Aplicar rotación para simular el rodamiento
                float rotationAngle = RotationSpeed * elapsedTime;
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, direction), rotationAngle);
                Rotation *= rotationMatrix;

            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                acceleration -= LinearSpeed * direction;
                accelerating = true;

                float rotationAngle = (-RotationSpeed) * elapsedTime;
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, direction), rotationAngle);
                Rotation *= rotationMatrix;
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                AngleZ+=0.1f;

                Rotation = Matrix.CreateRotationY(AngleZ) * Matrix.CreateRotationX(AngleX);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                AngleX+=0.1f;

                Rotation = Matrix.CreateRotationY(AngleZ) * Matrix.CreateRotationX(AngleX);
            }


            if (!accelerating && (_velocity.X != 0f || _velocity.Z != 0f))
            {
                acceleration.X -= _velocity.X * 0.95f;
                acceleration.Z -= _velocity.Z * 0.95f;
            }

            _velocity += acceleration * elapsedTime;

            Position += _velocity * elapsedTime;

            //Console.WriteLine($"acceleration: X={acceleration.X}, Y={acceleration.Y}, Z={acceleration.Z}");

            World = Scale * Rotation * Matrix.CreateTranslation(Position);
        }
    }


}