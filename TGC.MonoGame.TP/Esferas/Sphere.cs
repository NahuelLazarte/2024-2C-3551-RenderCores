using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Pelotas;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using TGC.MonoGame.TP.Levels;

namespace TGC.MonoGame.TP.Modelos
{
    public class Sphere : Modelo
    {
        public float escala;
        Vector3 _velocity;
        public Pelota pelota;
        Vector3 direction;
        Vector3 direccionActual = new Vector3(0f,0f,0f);
        float rotationAngle;
        float rotationSpeed;
        float velocidadLineal;
        BoundingSphere boundingSphere;

        public List<BoundingBox> Colliders { get; set; }

        Vector3 posicionNueva;
        public bool isGodModeActive;

        private bool OnGround = false;
        private KeyboardState previousKeyboardState;

        private SoundEffect soundEffect1, soundEffect2;

        private SoundEffectInstance soundEffectInstance1;

        bool isMoving = false;
        bool isMoving2 = false;

        public Level Game;

        public void setDirection(Vector3 newDirection)
        {
            direction = newDirection;
        }
        public void setGodMode(bool mode)
        {
            this.isGodModeActive = mode;
            
        }

        public BoundingSphere GetBoundingSphere()
        {
            return boundingSphere;
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            pelota = new Pelota();
            Model3D = content.Load<Model>("Models/" + "Spheres/sphere");
            Effect = content.Load<Effect>("Effects/" + "SphereShader");

            pelota.Texture1 = content.Load<Texture2D>("Textures/texturaGolf");
            pelota.Texture2 = content.Load<Texture2D>("Textures/texturaOro");
            pelota.Texture3 = content.Load<Texture2D>("Textures/texturaMaderaPelota");
            pelota.Texture4 = content.Load<Texture2D>("Textures/texturaPlastico");

            pelota.NormalTexture1 = content.Load<Texture2D>("Textures/NormalMapGolf");
            pelota.NormalTexture2 = content.Load<Texture2D>("Textures/NormalMapOro");
            pelota.NormalTexture3 = content.Load<Texture2D>("Textures/NormalMapMaderaPelota");
            pelota.NormalTexture4 = content.Load<Texture2D>("Textures/NormalMapPlastico");

            Texture = pelota.Texture1;

            base.LoadContent(content, graphicsDevice);

            boundingSphere = BoundingVolumesExtensions.CreateSphereFrom(Model3D);

            boundingSphere.Center = Position;
            //boundingSphere.Radius *= 0.0059f;
            //boundingSphere.Radius *= 0.026f;
            boundingSphere.Radius *= 0.023f;

            /*
            Effect.Parameters["ambientColor"].SetValue(Microsoft.Xna.Framework.Color.White.ToVector3());
            Effect.Parameters["diffuseColor"].SetValue(Microsoft.Xna.Framework.Color.Yellow.ToVector3());
            Effect.Parameters["specularColor"].SetValue(Microsoft.Xna.Framework.Color.White.ToVector3());

            Effect.Parameters["KAmbient"]?.SetValue(0.860f);
            Effect.Parameters["KDiffuse"]?.SetValue(1f);
            Effect.Parameters["KSpecular"]?.SetValue(0f);
            Effect.Parameters["shininess"]?.SetValue(1f);

            Effect.CurrentTechnique = Effect.Techniques["LightingTechnique"];*/

            // FALTA CARGAR AUDIOS
            pelota.soundEffectMovimientoMadera = content.Load<SoundEffect>("Audio/movimientoMadera");
            pelota.soundEffectCaidaMadera = content.Load<SoundEffect>("Audio/caidaMadera");
            pelota.soundEffectMovimientoMetal = content.Load<SoundEffect>("Audio/movimientoPiedra");
            pelota.soundEffectCaidaMetal = content.Load<SoundEffect>("Audio/caidaMetal");
            pelota.soundEffectMovimientoPlastico = content.Load<SoundEffect>("Audio/movimientoPiedra");
            pelota.soundEffectCaidaPlastico = content.Load<SoundEffect>("Audio/caidaPlastico");
            pelota.soundEffectMovimientoGolf = content.Load<SoundEffect>("Audio/movimientoPiedra");
            pelota.soundEffectCaidaGolf = content.Load<SoundEffect>("Audio/caidaGolf");
            // FALTA CARGAR AUDIOS

            pelota.soundEffectCaida = pelota.soundEffectCaidaGolf.CreateInstance();
            pelota.soundEffectMovimiento = pelota.soundEffectMovimientoGolf.CreateInstance();


        }

        public Sphere(Vector3 position, Matrix rotation, Vector3 color)
            : base(position, rotation, color)
        {
            //escala = 0.045f;

            escala = 4.2f;

            SetScale(Matrix.CreateScale(escala));
            World = Scale * rotation * Matrix.CreateTranslation(position);
            Colliders = new List<BoundingBox>();
        }

        public override void Update(GameTime gameTime, ContentManager content)
        {
            //Console.WriteLine($"Modo seteado a: {isGodModeActive}");

            var keyboardState = Keyboard.GetState();

            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            Vector3 acceleration = Vector3.Zero;

            bool accelerating = false;

            if (isGodModeActive)
            {                
                acceleration = new Vector3(0, 0, 0);
                MovimientoGodMode(gameTime, content);
                return;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                if (OnGround)
                {
                    acceleration += pelota.LinearSpeed * direction;
                    accelerating = true;
                    ApplyRotation(elapsedTime, direction);
                    if (!isMoving)
                    {
                        pelota.soundEffectMovimiento.Play();
                        isMoving = true;
                    }
                }
                else
                {
                    ApplyRotation(elapsedTime, direction);
                }

            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                if (OnGround)
                {
                    acceleration -= pelota.LinearSpeed * direction;
                    accelerating = true;
                    ApplyRotation(elapsedTime, -direction);
                    if (!isMoving)
                    {
                        pelota.soundEffectMovimiento.Play();
                        isMoving = true;
                    }
                }
                else
                {
                    ApplyRotation(elapsedTime, -direction);
                }

            }
            else
            {
                isMoving = false;
            }


            if (keyboardState.IsKeyDown(Keys.D))
            {
                Vector3 rightDirection = Vector3.Cross(direction, Vector3.Up);
                if (OnGround)
                {
                    acceleration += pelota.LinearSpeed * rightDirection;
                    accelerating = true;
                    ApplyRotation(elapsedTime, rightDirection);
                    if (!isMoving2)
                    {
                        pelota.soundEffectMovimiento.Play();
                        isMoving2 = true;
                    }
                }
                else
                {
                    ApplyRotation(elapsedTime, rightDirection);
                }
            }
            else if (keyboardState.IsKeyDown(Keys.A))
            {
                Vector3 leftDirection = Vector3.Cross(Vector3.Up, direction);
                if (OnGround)
                {
                    acceleration += pelota.LinearSpeed * leftDirection;
                    accelerating = true;
                    ApplyRotation(elapsedTime, leftDirection);
                    if (!isMoving2)
                    {
                        pelota.soundEffectMovimiento.Play();
                        isMoving2 = true;
                    }
                }
                else
                {
                    ApplyRotation(elapsedTime, leftDirection);
                }
            }
            else
            {
                isMoving2 = false;
            }

            if (!accelerating && (_velocity.X != 0f || _velocity.Z != 0f))
            {
                acceleration.X -= _velocity.X * 0.95f;
                acceleration.Z -= _velocity.Z * 0.95f;
                direccionActual = new Vector3(acceleration.X, 0f, acceleration.Z);
                ApplyRotationDesacceleration(elapsedTime, -Vector3.Normalize(direccionActual));
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
                _velocity.Y += 40f;

                //Console.WriteLine($"Saltando");
                //Console.WriteLine($"_velocity.Y = {_velocity.Y}");
            }

            if (!OnGround)
            {
                acceleration.Y = -50f;
                
            }

            for (int i = 0; i < Colliders.Count; i++)
            {
                if (Position.Y <= -50f && !boundingSphere.Intersects(Colliders[i]))
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

        private void MovimientoGodMode(GameTime gameTime, ContentManager content)
        {
            Console.WriteLine("Ejecutando GodMode");
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState(); // Obtener el estado del mouse

            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Vector3 acceleration = Vector3.Zero;
            _velocity = Vector3.Zero;
            Vector3 movVertical = new Vector3(0, 40, 0);
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
            velocidadLineal = _velocity.Length() * 0.06f;
            rotationSpeed = velocidadLineal * boundingSphere.Radius * pelota.velocidadRotacion;
            //rotationSpeed = pelota.RotationSpeed; //para calcular la velocidad de rotacion se nenesitaria el radio y la velocidad angular
            rotationAngle = rotationSpeed * elapsedTime;
            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, direction), rotationAngle);
            Rotation *= rotationMatrix;
        }

        private void ApplyRotationDesacceleration(float elapsedTime, Vector3 direction){   
            float friccionPisoMadera = 0.0035f; //Luego si se cambia el piso estaria bueno hacer que le pasen el valor del piso para que desacelere al rotacion dependiendo del piso
            if(rotationSpeed > 0f)
            {
                //rotationSpeed -= pelota.RotationSpeed * 0.003f + 0.0015f;
                rotationSpeed -= velocidadLineal * boundingSphere.Radius * friccionPisoMadera;
            } else if(rotationSpeed <= 0f)
            {
                rotationSpeed = 0f;
            }
            rotationAngle = rotationSpeed * elapsedTime;
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

        private bool wasOnGround = false;

        private void SolveVerticalMovement()
        {
            bool isCurrentlyOnGround = false;

            foreach (BoundingBox collider in Colliders)
            {
                bool hayIntersecion = collider.Intersects(boundingSphere);

                if (hayIntersecion)
                {
                    isCurrentlyOnGround = true;

                    if (!wasOnGround)
                    {
                        float posicionMinY = collider.Min.Y;
                        float posicionMaxY = collider.Max.Y;

                        if (posicionMinY >= posicionMaxY)
                        {
                            posicionNueva = new Vector3(Position.X, posicionMinY + boundingSphere.Radius - 0.01f, Position.Z);
                        }
                        else
                        {
                            posicionNueva = new Vector3(Position.X, posicionMaxY + boundingSphere.Radius - 0.01f, Position.Z);
                        }

                        SetPosition(posicionNueva);
                        pelota.soundEffectCaida.Play();

                        if (pelota.rebota && Math.Abs(_velocity.Y) > pelota.umbralVelocidadRebote)
                        {
                            // Invierte la velocidad vertical con un coeficiente de pérdida
                            _velocity.Y = -_velocity.Y * pelota.coeficienteRebote;
                        }
                        else
                        {
                            // Detener el rebote si la velocidad es muy baja
                            _velocity.Y = 0f;
                            OnGround = true;
                        }
                    }
                    break;
                }
            }

            if (!isCurrentlyOnGround && OnGround)
            {
                OnGround = false;
            }

            wasOnGround = isCurrentlyOnGround;
        }
    }
}