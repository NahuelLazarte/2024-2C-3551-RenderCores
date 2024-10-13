using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TGC.MonoGame.TP.Collisions;
namespace TGC.MonoGame.TP.Modelos
{
    class Sphere : Modelo
    {
        float LinearSpeed = 100f;
        private float RotationSpeed = 10f;
        Vector3 _velocity;

        Vector3 direction;

        BoundingBox esferaBox;

        BoundingBox size;

        public void setDirection(Vector3 newDirection)
        {
            direction = newDirection;
        }

        public BoundingBox GetBoundingBox()
        {
            return esferaBox;
        }

        public override void LoadContent(ContentManager content)
        {
            Model3D = content.Load<Model>("Models/" + "pistas/road_straight_fix");
            Model3D = content.Load<Model>("Models/" + "Spheres/sphere");
            Effect = content.Load<Effect>("Effects/" + "BasicShader");

            base.LoadContent(content);

            // This gets an AABB with the bounds of the robot model
            size = BoundingVolumesExtensions.CreateAABBFrom(Model3D);
            // This moves the min and max points to the world position of each robot (one and two)
            esferaBox = new BoundingBox(size.Min * 0.01f + Position, size.Max * 0.01f + Position);


        }
        public Sphere(Vector3 position, Matrix rotation, Color color)
            : base(position, rotation, color)
        {
            SetScale(Matrix.CreateScale(0.01f));
            World = Scale * rotation * Matrix.CreateTranslation(position);
        }

        public override void Update(GameTime gameTime)
        {

            var keyboardState = Keyboard.GetState();


            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);


            Vector3 acceleration = Vector3.Zero;

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

            }

            if (keyboardState.IsKeyDown(Keys.A))
            {

            }


            if (!accelerating && (_velocity.X != 0f || _velocity.Z != 0f))
            {
                acceleration.X -= _velocity.X * 0.95f;
                acceleration.Z -= _velocity.Z * 0.95f;
            }

            // Mejorar la condición de salto
            if (keyboardState.IsKeyDown(Keys.Space) && Math.Abs(Position.Y - 4f) < 0.1f)
            {
                _velocity.Y += 30f;

                Console.WriteLine($"Saltando");
                Console.WriteLine($"_velocity.Y = {_velocity.Y}");
            }

            acceleration.Y = -50f;

            _velocity += acceleration * elapsedTime;

            Position += _velocity * elapsedTime;

            if (Position.Y < 4f)
            {
                Vector3 posicionNueva = new Vector3(Position.X, 4f, Position.Z);
                SetPosition(posicionNueva);

                _velocity.Y = 0f;
            }
            esferaBox = new BoundingBox(size.Min * 0.01f + Position, size.Max * 0.01f + Position);

            World = Scale * Rotation * Matrix.CreateTranslation(Position);
        }
    }


}