using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;
using Microsoft.Xna.Framework.Audio;

using System;
using TGC.MonoGame.TP.Levels;

namespace TGC.MonoGame.TP.ObstaculoCarretilla {
    public class ObstaculosCarretillas {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(1f);
        public Model ModeloCarretilla { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private Texture2D TexturaMadera { get; set; }
        private Texture2D TexturaMetal { get; set; }

        // Clase para representar el estado de cada carretilla
        private class Carretilla {
            public Matrix Transform;
            public Vector3 Position;
            public float MovementDirection = 1f; // 1: adelante, -1: atrás
            public bool IsTurning = false;
            public float InitialAngle;
            public float TurnAngle = 0f; // Ángulo actual de giro
            public const float TurnSpeed = MathHelper.Pi / 2; // Velocidad de giro (180 grados en 1 segundo)
            public Vector3 InitialPosition;
        }

        private List<Carretilla> _obstaculosCarretilla;
        public BoundingSphere _envolturaEsfera { get; set; }
        public SoundEffect CollisionSound { get; set; }
        private float moveSpeed = 50f; // Velocidad de movimiento
        private float movementRange = 30f; // Rango de movimiento en el eje Z
        BoundingBox size;
        private BoundingFrustum _frustum;

        public ObstaculosCarretillas(Matrix view, Matrix projection) {
            Initialize(view,projection);
        }

        private void Initialize(Matrix view, Matrix projection) {
            _obstaculosCarretilla = new List<Carretilla>();
            Colliders = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);
        }
//P: "DocumentUrl", "KString", "Url", "", "G:\TP-TGC\2024-1C-3551-RenderCores\TGC.MonoGame.TP\Content\Models\obstaculos\cartWithTextures2.fbx"
//			P: "SrcDocumentUrl", "KString", "Url", "", "G:\TP-TGC\2024-1C-3551-RenderCores\TGC.MonoGame.TP\Content\Models\obstaculos\cartWithTextures2.fbx"


        public void LoadContent(ContentManager Content) {
            ModeloCarretilla = Content.Load<Model>("Models/" + "obstaculos/cartWithTextures2");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");
            TexturaMadera = Content.Load<Texture2D>("Textures/texturaMadera");
            TexturaMetal = Content.Load<Texture2D>("Textures/texturaMetal");

            foreach (var mesh in ModeloCarretilla.Meshes) {
                Console.WriteLine($"Meshname carreta: {mesh.Name}");
                foreach (var meshPart in mesh.MeshParts) {
                    meshPart.Effect = Effect;
                }
            }
            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloCarretilla);
            CollisionSound = Content.Load<SoundEffect>("Audio/ColisionPez"); 
        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection) {
            for (int i = 0; i < _obstaculosCarretilla.Count; i++) {
                var carretilla = _obstaculosCarretilla[i];


                if (carretilla.IsTurning) {
                    // Girar la carretilla
                    carretilla.TurnAngle += Carretilla.TurnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (carretilla.TurnAngle >= MathHelper.Pi) {
                        // Finaliza el giro
                        carretilla.TurnAngle = MathHelper.Pi * -1;
                        carretilla.IsTurning = false;
                        carretilla.MovementDirection *= -1; // Cambia dirección al completar el giro
                    }
                } else {
                    // Mover en el eje Z según la dirección
                    Vector3 movimiento = new Vector3(0, 0, carretilla.MovementDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    movimiento = Vector3.Transform(movimiento, Matrix.CreateRotationY(carretilla.InitialAngle)); // Aplicar rotación inicial
                    carretilla.Position += movimiento;
                    
                    
                    var aux1 = Vector3.Transform(carretilla.Position, Matrix.CreateRotationY(carretilla.InitialAngle));
                    var aux2 = Vector3.Transform(carretilla.InitialPosition, Matrix.CreateRotationY(carretilla.InitialAngle));
                    // Cambiar dirección al llegar al límite
                    if (aux1.Z - aux2.Z >  movementRange || aux1.Z - aux2.Z < -movementRange) {
                        //carretilla.Position.Z = MathHelper.Clamp(carretilla.Position.Z, -movementRange, movementRange);
                        carretilla.IsTurning = true; // Iniciar giro
                    }
                }

                // Crear la matriz de transformación
                carretilla.Transform = Matrix.CreateRotationY(carretilla.TurnAngle + carretilla.InitialAngle) * Matrix.CreateTranslation(carretilla.Position) * scale;
                _obstaculosCarretilla[i] = carretilla;

                UpdateCollider(i, carretilla.Position);

                // Colisión
                if (_envolturaEsfera.Intersects(Colliders[i])) {
                    CollisionSound.Play();
                    Game.Respawn();
                }
            }
            _frustum = new BoundingFrustum(view * projection);
        }
        
        private void UpdateCollider(int index, Vector3 position) {
            BoundingBox box = new BoundingBox(size.Min + position, size.Max + position);
            Colliders[index] = box;
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection) {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
//            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));

            foreach (var mesh in ModeloCarretilla.Meshes) {
                for (int i = 0; i < _obstaculosCarretilla.Count; i++) {
                    var carretilla = _obstaculosCarretilla[i];
                    BoundingBox boundingBox = BoundingVolumesExtensions.FromMatrix(carretilla.Transform);
                    if(_frustum.Intersects(boundingBox)){
                        Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * carretilla.Transform);
                        string meshName = mesh.Name.ToLower();
                        switch (meshName) {
                            case "wheel":
                                //Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0f, 0f, 0f)); 
                                Effect.Parameters["Texture"]?.SetValue(TexturaMetal);
                                break;
                            case "cart":
                                //Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.545f, 0.271f, 0.075f));
                                Effect.Parameters["Texture"]?.SetValue(TexturaMadera);
                                break;
                        }
                        mesh.Draw();
                    }
                }
            }
        }

        public void AgregarNuevoObstaculo(float rotationY, Vector3 Posicion) {
            // Crear transformación inicial
            Posicion = new Vector3(Posicion.X /34f, 0, Posicion.Z/34f );
            //Posicion += Vector3.Transform(new Vector3(0f,0,-80f), Matrix.CreateRotationY(rotationY));

            var carretilla = new Carretilla {
                Transform = Matrix.CreateRotationY(rotationY) * Matrix.CreateTranslation(Posicion) * scale,
                Position = Posicion,
                InitialPosition = Posicion,
                InitialAngle = rotationY
            };
            _obstaculosCarretilla.Add(carretilla);

            BoundingBox box = new BoundingBox(size.Min + Posicion, size.Max + Posicion);
            Colliders.Add(box);
        }
    }
}
