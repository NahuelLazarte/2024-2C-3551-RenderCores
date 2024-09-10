using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP.Objects{
    class Sphere {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Model SphereModel{get; set;}
        public Matrix SphereWorld{get; set;}
        public Effect Effect { get; set; }
        public Vector3 SpherePosition { get; set; }
        public Matrix SphereRotation { get; set; }
        public int index { get; set; }
        public Vector3 SphereVelocity { get; set; }
        public Vector3 SphereAcceleration { get; set; }
        public Vector3 DireccionBola { get; set; }
        public FollowCamera SphereCamera { get; set; }

        private Vector3 SphereFrontDirection { get; set; }

        // Valores que afectan el movimiento de la esfera
        private const float SphereRotatingVelocity = 0.06f;
        private const float SphereSideSpeed = 30f;
        private const float SphereJumpSpeed = 30f;
        private const float EPSILON = 0.00001f;
        private const float Gravity = 40f;
        private Matrix SphereScale { get; set; }

        // Booleano que indica si esta tocando el piso
        private bool OnGround { get; set; }
        
        private static bool Compare(float a, float b){
            return MathF.Abs(a - b) < float.Epsilon;
        }

        public Sphere(Vector3 initialPosition){
            OnGround = false;
            
            //Inicializacion de la esfera
            SpherePosition = initialPosition;
            SphereScale = Matrix.CreateScale(0.024f);

            //Atributos para el movimiento de la esfera
            SphereAcceleration = Vector3.Down * Gravity;
            SphereVelocity = Vector3.Zero;
        }

        public void LoadContent(ContentManager Content){
            SphereModel = Content.Load<Model>(ContentFolder3D + "Spheres/sphere");
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");
            
            foreach (var mesh in SphereModel.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            SpherePosition += 0.5f * Vector3.Up * SphereScale.M22;
            SphereWorld = SphereScale * Matrix.CreateTranslation(SpherePosition);
        }

        public void Update(GameTime gameTime){
            var deltaTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            var keyboardState = Keyboard.GetState();
            SphereAcceleration = Vector3.Zero;
            Vector3 friccion = -SphereVelocity * 0.03f;


            if (keyboardState.IsKeyDown(Keys.A))
                SphereAcceleration += Vector3.Left;
            if (keyboardState.IsKeyDown(Keys.D))
                SphereAcceleration += Vector3.Right;
            if (keyboardState.IsKeyDown(Keys.W))
                SphereAcceleration += Vector3.Forward;
            if (keyboardState.IsKeyDown(Keys.S))
                SphereAcceleration += Vector3.Backward;
            if (keyboardState.IsKeyDown(Keys.Space) && (OnGround == true)) {
                SphereVelocity += Vector3.Up * 300f;
                OnGround = false;
            }
            
            SphereAcceleration += friccion;
            SphereVelocity += SphereAcceleration * 180f * deltaTime;
            SpherePosition += SphereVelocity * deltaTime;

            var minimumFloor = MathHelper.Max(0f, SpherePosition.Y);
            SpherePosition = new Vector3(SpherePosition.X, minimumFloor, SpherePosition.Z);

            if (Compare(SpherePosition.Y, 0.0f) && (OnGround == false)) {
                SphereVelocity = new Vector3(SphereVelocity.X, 0f, SphereVelocity.Z);
                OnGround = true;
            }
            
            SphereWorld = SphereScale * Matrix.CreateTranslation(SpherePosition);

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection){
            Effect.Parameters["View"].SetValue(view); //Cambio View por Eso
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Yellow.ToVector3());
            foreach (var mesh in SphereModel.Meshes)
            {
                Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * SphereWorld);
                mesh.Draw();
            }

        }
    }
    
}