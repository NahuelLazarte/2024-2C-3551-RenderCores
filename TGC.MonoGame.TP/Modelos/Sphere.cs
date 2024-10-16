using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using TGC.MonoGame.TP.Collisions;
namespace TGC.MonoGame.TP.Modelos
{
    class Sphere : Modelo
    {
        float LinearSpeed = 40f;
        private float RotationSpeed = 20f;
        Vector3 _velocity;

        float escala;

        Vector3 direction;

        BoundingSphere boundingSphere;

        public List<BoundingBox> Colliders { get; set; }


        private bool OnGround { get; set; }
        private KeyboardState previousKeyboardState;


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
            boundingSphere.Radius *= 0.0059f;
            Console.WriteLine($"boundingSphere.Radius={boundingSphere.Radius}");
        }
        public Sphere(Vector3 position, Matrix rotation, Color color)
            : base(position, rotation, color)
        {
            escala = 0.01f;
            SetScale(Matrix.CreateScale(escala));
            World = Scale * rotation * Matrix.CreateTranslation(position);
            Colliders = new List<BoundingBox>();
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
                ApplyRotation(elapsedTime, direction);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                acceleration -= LinearSpeed * direction;
                accelerating = true;
                ApplyRotation(elapsedTime, -direction);
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                Vector3 rightDirection = Vector3.Cross(direction, Vector3.Up);
                acceleration += LinearSpeed * rightDirection;
                accelerating = true;
                ApplyRotation(elapsedTime, rightDirection);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                Vector3 leftDirection = Vector3.Cross(Vector3.Up, direction);
                acceleration += LinearSpeed * leftDirection;
                accelerating = true;
                ApplyRotation(elapsedTime, leftDirection);
            }


            if (!accelerating && (_velocity.X != 0f || _velocity.Z != 0f))
            {
                acceleration.X -= _velocity.X * 0.95f;
                acceleration.Z -= _velocity.Z * 0.95f;
            }

            if (keyboardState.IsKeyDown(Keys.Up) && !previousKeyboardState.IsKeyDown(Keys.Up))
            {
                // Mover la pelota arriba
                Vector3 irArriba = new Vector3(0f, 1f, 0f);

                Position += irArriba;
                Console.WriteLine($"Posicion pelota Y ={Position.Y}");
                SolveVerticalMovement();
            }
            if (keyboardState.IsKeyDown(Keys.Down) && !previousKeyboardState.IsKeyDown(Keys.Down))
            {
                // Mover la pelota abajo
                Vector3 irAbajo = new Vector3(0f, -1f, 0f);

                Position += irAbajo;
                Console.WriteLine($"Posicion pelota Y ={Position.Y}");
                SolveVerticalMovement();
            }

            // Mejorar la condición de salto
            /*
            if (keyboardState.IsKeyDown(Keys.Space) && Math.Abs(Position.Y - 4f) < 0.1f)
            {
                _velocity.Y += 30f;

                Console.WriteLine($"Saltando");
                Console.WriteLine($"_velocity.Y = {_velocity.Y}");
            }*/

            //acceleration.Y = -50f;

            _velocity += acceleration * elapsedTime;

            Position += _velocity * elapsedTime;

            previousKeyboardState = keyboardState;

            /*
            if (Position.Y < 4f)
            {
                Vector3 posicionNueva = new Vector3(Position.X, 4f, Position.Z);
                SetPosition(posicionNueva);

                _velocity.Y = 0f;
            }*/

            // // Aplicar la detección de colisiones con el suelo
            //SolveVerticalMovement();
            //SolveVerticalMovement();

            boundingSphere.Center = Position;

            World = Scale * Rotation * Matrix.CreateTranslation(Position);
        }
        private void ApplyRotation(float elapsedTime, Vector3 direction)
        {
            float rotationAngle = RotationSpeed * elapsedTime;
            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, direction), rotationAngle);
            Rotation *= rotationMatrix;
        }

        public void RespawnAt(Vector3 newPosition)
        {
            Position = newPosition;
            _velocity = new Vector3(0, 0, 0);
        }

        public void aumentarVelocidad(float aumento)
        {
            _velocity *= aumento;
        }

        public bool Intersects(BoundingSphere sphere, BoundingBox box)
        {
            // Encuentra el punto más cercano en el BoundingBox al centro del BoundingSphere
            Vector3 closestPoint = Vector3.Clamp(sphere.Center, box.Min, box.Max);

            // Calcula la distancia desde el punto más cercano al centro del BoundingSphere
            float distanceSquared = Vector3.DistanceSquared(closestPoint, sphere.Center);

            // Verifica si la distancia es menor o igual al radio del BoundingSphere al cuadrado
            bool intersects = distanceSquared <= sphere.Radius * sphere.Radius;

            Console.WriteLine($"BoundingSphere Center: {sphere.Center}, Radius: {sphere.Radius}");
            Console.WriteLine($"BoundingBox Min: {box.Min}, Max: {box.Max}");
            Console.WriteLine($"Closest Point: {closestPoint}");
            Console.WriteLine($"Distance Squared: {distanceSquared}, Radius Squared: {sphere.Radius * sphere.Radius}");
            Console.WriteLine($"Intersects: {intersects}");
            return intersects;
        }

        private void SolveVerticalMovement()
        {
            int contador = 0;

            //boundingSphere.Radius *= 1.2f;
            //Console.WriteLine($"El tamaño de la lista Colliders es: {Colliders.Count}");
            Console.WriteLine($"BoundingSphere Center: {boundingSphere.Center}, Radius: {boundingSphere.Radius}");
            foreach (BoundingBox collider in Colliders)
            {
                // Imprimir información del BoundingBox


                // Verificar intersección
                //bool hayIntersecion = boundingSphere.Intersects(collider);
                bool hayIntersecion = Intersects(boundingSphere, collider);

                if (hayIntersecion)
                {

                }
                else
                {

                }
                Console.WriteLine($"BoundingBox {contador} Min: {collider.Min}, Max: {collider.Max} " + (hayIntersecion ? "Hay interseccion" : "No hay interseccion"));
                contador++;
            }
            Console.WriteLine("");

            /*
            // Si la esfera no tiene velocidad vertical, no hay nada que hacer
            if (_velocity.Y == 0f)
                return;

            // Mover la esfera según la velocidad vertical
            boundingSphere.Center += Vector3.Up * _velocity.Y;
            // Inicialmente, asumimos que la esfera no está en el suelo
            OnGround = false;

            // Detectar colisiones
            var collided = false;
            var foundIndex = -1;
            for (var index = 0; index < Colliders.Length; index++)
            {
                if (!boundingSphere.Intersects(Colliders[index]))
                    continue;

                // Si colisionamos, detener la velocidad vertical
                _velocity.Y = 0f;

                // Marcar la colisión y guardar el índice del collider
                collided = true;
                foundIndex = index;
                break;
            }

            // Corregir la penetración hasta que no haya colisiones
            while (collided)
            {
                var collider = Colliders[foundIndex];
                var colliderY = BoundingVolumesExtensions.GetCenter(collider).Y;
                var sphereY = boundingSphere.Center.Y;
                var extents = BoundingVolumesExtensions.GetExtents(collider);

                // Calcular la penetración y ajustar la posición de la esfera
                float penetration = (sphereY > colliderY)
                    ? colliderY + extents.Y - sphereY + boundingSphere.Radius
                    : -sphereY - boundingSphere.Radius + colliderY - extents.Y;

                // Si estamos encima del collider, estamos en el suelo
                if (sphereY > colliderY)
                    OnGround = true;

                // Mover la esfera para resolver la penetración
                boundingSphere.Center += Vector3.Up * penetration;
                collided = false;

                // Verificar colisiones nuevamente
                for (var index = 0; index < Colliders.Length; index++)
                {
                    if (!boundingSphere.Intersects(Colliders[index]))

                        continue;

                    // Si aún colisionamos, repetir el proceso
                    collided = true;
                    foundIndex = index;
                    break;
                }
            }*/
        }


    }
}