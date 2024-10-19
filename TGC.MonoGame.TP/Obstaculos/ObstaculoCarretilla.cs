using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;

using System;

namespace TGC.MonoGame.TP.ObstaculoCarretilla {
    public class ObstaculosCarretillas{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(1f);
        public Model ModeloCarretilla { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private List<Matrix> _obstaculosCarretilla { get; set; }
        public BoundingSphere _envolturaEsfera { get; set; }
        public Song CollisionSound { get; set; }
        private float moveSpeed = 50f; // Velocidad de movimiento
        private float movementDirection = 1f; // 1: adelante, -1: atrás
        private float movementRange = 30f; // Rango de movimiento en el eje Z
        private bool isTurning = false; // Estado de giro
        private float turnAngle = 0f; // Ángulo actual de giro
        private const float turnSpeed = MathHelper.Pi / 2; // Velocidad de giro (180 grados en 1 segundo)
        BoundingBox size;
        public ObstaculosCarretillas() {
            Initialize();
        }

        private void Initialize() {
            _obstaculosCarretilla = new List<Matrix>();
            Colliders = new List<BoundingBox>();
        }

        public void LoadContent(ContentManager Content) {
            ModeloCarretilla = Content.Load<Model>("Models/" + "obstaculos/cart");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloCarretilla.Meshes) {
                Console.WriteLine($"Meshname carreta: {mesh.Name}");
                foreach (var meshPart in mesh.MeshParts) {
                    meshPart.Effect = Effect;
                }
            }
            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloCarretilla);
            CollisionSound = Content.Load<Song>("Audio/ColisionPez"); // Ajusta la ruta según sea necesario
        }

        public void Update(GameTime gameTime, TGCGame Game) {
            for (int i = 0; i < _obstaculosCarretilla.Count; i++) {
                var position = _obstaculosCarretilla[i].Translation;

                if (isTurning) {
                    // Girar la carretilla
                    turnAngle += turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (turnAngle >= MathHelper.Pi) {
                        // Finaliza el giro
                        turnAngle = 0f;
                        isTurning = false;
                        movementDirection *= -1; // Cambia dirección al completar el giro
                    }
                } else {
                    // Mover en el eje Z según la dirección
                    position.Z += movementDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Cambiar dirección al llegar al límite
                    if (position.Z > movementRange || position.Z < -movementRange) {
                        position.Z = MathHelper.Clamp(position.Z, -movementRange, movementRange);
                        isTurning = true; // Iniciar giro
                    }
                }

                // Crear la matriz de transformación
                var transform = Matrix.CreateRotationY(turnAngle) * Matrix.CreateTranslation(position) * scale;
                _obstaculosCarretilla[i] = transform;

                UpdateCollider(i, position);

                // Colisión
                if (_envolturaEsfera.Intersects(Colliders[i])) {
                    MediaPlayer.Play(CollisionSound);
                    Game.Respawn();
                }
            }
        }
        
        private void UpdateCollider(int index, Vector3 position) {
            
            BoundingBox box = new BoundingBox(size.Min + position, size.Max + position);
            Colliders[index] = box;
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection) {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));

            foreach (var mesh in ModeloCarretilla.Meshes) {
                for (int i = 0; i < _obstaculosCarretilla.Count; i++) {
                    Matrix _carretillaWorld = _obstaculosCarretilla[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _carretillaWorld);
                    string meshName = mesh.Name.ToLower();
                    switch (meshName)
                    {
                        case "wheel":
                            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0f, 0f, 0f)); // Color para el pan de abajo
                            break;
                        case "cart":
                            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.545f, 0.271f, 0.075f)); // Color para el pan de arriba
                            break;
                        
                    }
                    mesh.Draw();
                }
            }
        }

        public void AgregarNuevoObstaculo(float rotationY, Vector3 Posicion) {
            // Crear transformación inicial
            var transform = Matrix.CreateRotationY(rotationY) * Matrix.CreateTranslation(Posicion) * scale;
            _obstaculosCarretilla.Add(transform);

            BoundingBox box = new BoundingBox(size.Min + Posicion, size.Max + Posicion);
            Colliders.Add(box);
        }
    }
}
