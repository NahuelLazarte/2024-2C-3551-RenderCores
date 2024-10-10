using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP.Modelos
{
    class Sphere : Modelo
    {
        float Angle = 0f;
        float LinearSpeed = 100f;
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

            World = Scale * rotation * Matrix.CreateTranslation(position);


        }
        public override void Update(GameTime gameTime)
        {

            var keyboardState = Keyboard.GetState();

        
            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            Vector3 acceleration = Vector3.Zero;

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

            var Rotation = Matrix.CreateRotationY(Angle);
            //var direction = Rotation.Forward;

            bool accelerating = false;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                acceleration += LinearSpeed * direction;
                accelerating = true;
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                acceleration -= LinearSpeed * direction;
                accelerating = true;
            }

            if (!accelerating && (_velocity.X != 0f || _velocity.Z != 0f))
            {
                acceleration.X -= _velocity.X * 0.95f;
                acceleration.Z -= _velocity.Z * 0.95f;
            }

            _velocity += acceleration * elapsedTime;
            Position += _velocity * elapsedTime;


            World = Scale * Rotation * Matrix.CreateTranslation(Position);
        }
    }


}