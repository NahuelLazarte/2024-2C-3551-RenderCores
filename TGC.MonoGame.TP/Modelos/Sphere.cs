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

        float escala;

        Vector3 direction;

        BoundingSphere boundingSphere;


        public void setDirection(Vector3 newDirection)
        {
            direction = newDirection;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return boundingSphere;
        }

        public override void LoadContent(ContentManager content)
        {
            Model3D = content.Load<Model>("Models/" + "Spheres/sphere");
            Effect = content.Load<Effect>("Effects/" + "BasicShader");

            base.LoadContent(content);

            boundingSphere = BoundingVolumesExtensions.CreateSphereFrom(Model3D);

            boundingSphere.Center = Position;
            boundingSphere.Radius *= escala;
            Console.WriteLine($"boundingSphere.Radius={boundingSphere.Radius}");

            Console.WriteLine($"boundingSphere.Radius={boundingSphere.Radius}");

        }
        public Sphere(Vector3 position, Matrix rotation, Color color)
            : base(position, rotation, color)
        {
            escala = 0.01f;
            SetScale(Matrix.CreateScale(escala));
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
                Vector3 rightDirection = Vector3.Cross(direction, Vector3.Up);
                acceleration += LinearSpeed * rightDirection;
                accelerating = true;

                float rotationAngle = RotationSpeed * elapsedTime;
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, rightDirection), rotationAngle);
                Rotation *= rotationMatrix;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                Vector3 leftDirection = Vector3.Cross(Vector3.Up, direction);
                acceleration += LinearSpeed * leftDirection;
                accelerating = true;

                float rotationAngle = RotationSpeed * elapsedTime;
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, leftDirection), rotationAngle);
                Rotation *= rotationMatrix;
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
            
            boundingSphere.Center = Position;

            World = Scale * Rotation * Matrix.CreateTranslation(Position);
        }

        public void RespawnAt(Vector3 newPosition){
            Position = newPosition;
            _velocity = new Vector3(0,0,0);
        }

    }


}