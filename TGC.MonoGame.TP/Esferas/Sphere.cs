using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TGC.MonoGame.TP;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Pelotas;

namespace TGC.MonoGame.TP.Modelos
{
    class Sphere : Modelo
    {
        public float escala;
        Vector3 _velocity;
        Pelota pelota;
        Vector3 direction;

        BoundingSphere boundingSphere;

        public List<BoundingBox> Colliders { get; set; }

        Vector3 posicionNueva;
        public bool isGodModeActive { get; set; }

        private bool OnGround = false;
        private KeyboardState previousKeyboardState;
        public TGCGame Game;

        public void setDirection(Vector3 newDirection)
        {
            direction = newDirection;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return boundingSphere;
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            pelota = new Pelota();
            Model3D = content.Load<Model>("Models/" + "Spheres/sphere");
            Effect = content.Load<Effect>("Effects/" + "BasicShader");

            base.LoadContent(content, graphicsDevice);

            boundingSphere = BoundingVolumesExtensions.CreateSphereFrom(Model3D);

            boundingSphere.Center = Position;
            //boundingSphere.Radius *= 0.0059f;
            boundingSphere.Radius *= 0.026f;
        }
        public Sphere(Vector3 position, Matrix rotation, Vector3 color)
            : base(position, rotation, color)
        {
            escala = 0.045f;
            SetScale(Matrix.CreateScale(escala));
            World = Scale * rotation * Matrix.CreateTranslation(position);
            Colliders = new List<BoundingBox>();
        }

        public override void Update(GameTime gameTime, ContentManager content)
        {

            var keyboardState = Keyboard.GetState();

            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            Vector3 acceleration = Vector3.Zero;

            bool accelerating = false;
            

            if (isGodModeActive)
            {
                acceleration = new Vector3(0,0,0);
                MovimientoGodMode(gameTime, content);
                return;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                acceleration += pelota.LinearSpeed * direction;
                accelerating = true;
                ApplyRotation(elapsedTime, direction);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                acceleration -= pelota.LinearSpeed * direction;
                accelerating = true;
                ApplyRotation(elapsedTime, -direction);
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                Vector3 rightDirection = Vector3.Cross(direction, Vector3.Up);
                acceleration += pelota.LinearSpeed * rightDirection;
                accelerating = true;
                ApplyRotation(elapsedTime, rightDirection);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                Vector3 leftDirection = Vector3.Cross(Vector3.Up, direction);
                acceleration += pelota.LinearSpeed * leftDirection;
                accelerating = true;
                ApplyRotation(elapsedTime, leftDirection);
            }

            if (!accelerating && (_velocity.X != 0f || _velocity.Z != 0f))
            {
                acceleration.X -= _velocity.X * 0.95f;
                acceleration.Z -= _velocity.Z * 0.95f;
            }

            /* //Dejar esto para debuggear
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
            }*/

            // Mejorar la condición de salto
            
            //if (keyboardState.IsKeyDown(Keys.Space) && Math.Abs(Position.Y - 4f) < 0.1f)
            if (keyboardState.IsKeyDown(Keys.Space) && Position.Y <= posicionNueva.Y && OnGround)  
            {
                _velocity.Y += 30f;

                Console.WriteLine($"Saltando");
                Console.WriteLine($"_velocity.Y = {_velocity.Y}");                
            }

            if(!OnGround){
                acceleration.Y = -50f;
            }

            for(int i = 0; i < Colliders.Count; i++) {
                if(Position.Y <= -50f && !boundingSphere.Intersects(Colliders[i]))
                {
                    Game.Respawn();
                }
            }

            //acceleration.Y = -10f;

            _velocity += acceleration * elapsedTime;

            Position += _velocity * elapsedTime;

            previousKeyboardState = keyboardState;

            boundingSphere.Center = Position;

            World = Scale * Rotation * Matrix.CreateTranslation(Position);

            SolveVerticalMovement();

            pelota.Update(gameTime, this, content);
        }

        private void MovimientoGodMode(GameTime gameTime, ContentManager content){
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState(); // Obtener el estado del mouse
            
            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Vector3 acceleration = Vector3.Zero;
            _velocity = Vector3.Zero;
            Vector3 movVertical = new Vector3(0,40,0);
            bool accelerating = false;
            var velocidadLineal = 100;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                acceleration += velocidadLineal * direction;
                accelerating = true;
                //ApplyRotation(elapsedTime, direction);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                acceleration -= velocidadLineal * direction;
                accelerating = true;
                //ApplyRotation(elapsedTime, -direction);
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                Vector3 rightDirection = Vector3.Cross(direction, Vector3.Up);
                acceleration += velocidadLineal * rightDirection;
                accelerating = true;
                //ApplyRotation(elapsedTime, rightDirection);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                Vector3 leftDirection = Vector3.Cross(Vector3.Up, direction);
                acceleration += velocidadLineal * leftDirection;
                accelerating = true;
                //ApplyRotation(elapsedTime, leftDirection);
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                Vector3 leftDirection = Vector3.Cross(Vector3.Up, direction);
                acceleration += movVertical;
                accelerating = true;
                //ApplyRotation(elapsedTime, leftDirection);
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector3 leftDirection = Vector3.Cross(Vector3.Up, direction);
                acceleration -= movVertical;
                accelerating = true;
                //ApplyRotation(elapsedTime, leftDirection);
            }

            if (!accelerating && (_velocity.X != 0f || _velocity.Z != 0f))
            {
                acceleration.X -= _velocity.X * 0.95f;
                acceleration.Z -= _velocity.Z * 0.95f;
            }

            //acceleration.Y = -10f;

            Position += acceleration * elapsedTime;

            previousKeyboardState = keyboardState;

            //boundingSphere.Center = Position;

            World = Scale * Rotation * Matrix.CreateTranslation(Position);

            pelota.Update(gameTime, this, content);
        }

        private void ApplyRotation(float elapsedTime, Vector3 direction)
        {
            float rotationAngle = pelota.RotationSpeed * elapsedTime;
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

        private void SolveVerticalMovement()
        {
            //Console.WriteLine($"BoundingSphere Center: {boundingSphere.Center}, Radius: {boundingSphere.Radius}");
            foreach (BoundingBox collider in Colliders)
            {
                bool hayIntersecion = collider.Intersects(boundingSphere);

                if (hayIntersecion && !OnGround)
                {
                    //Console.WriteLine($"BoundingBox {contador} Min: {collider.Min}, Max: {collider.Max} " + (hayIntersecion ? "Hay interseccion" : "No hay interseccion"));
                    float posicionMinY = collider.Min.Y;
                    float posicionMaxY = collider.Max.Y;
                    
                    if(posicionMinY >= posicionMaxY){
                        posicionNueva = new Vector3(Position.X, posicionMinY + boundingSphere.Radius - 0.01f, Position.Z);
                    }
                    else{
                        posicionNueva = new Vector3(Position.X, posicionMaxY + boundingSphere.Radius - 0.01f, Position.Z);
                    }
                    //Console.WriteLine($"posicionMinY: {posicionMinY}, posicionMaxY: {posicionMaxY}");
                    SetPosition(posicionNueva);
                    _velocity.Y = 0f;
                    OnGround = true;
                    //Console.WriteLine($"posicionMinY: {posicionMinY}, posicionMaxY: {posicionMaxY}");
                    //Console.WriteLine($"Interseccion");
                    return;
                }
                else{
                    OnGround = false;
                }
                /*
                else if (OnGround){
                    OnGround = false;
                    Console.WriteLine($"NO Interseccion");
                }*/
            }
            //Console.WriteLine("");
        }
    }
}