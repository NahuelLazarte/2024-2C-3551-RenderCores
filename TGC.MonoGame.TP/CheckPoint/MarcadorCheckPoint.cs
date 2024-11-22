using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Asegúrate de tener esta directiva

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Levels;

using Microsoft.Xna.Framework.Audio;

using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo


namespace TGC.MonoGame.TP.MarcadorCheckPoint{
    public class MarcadoresCheckPoints{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(1f);
        public Model ModeloMarcadorCheckPoint { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _marcadoresCheckPoints { get; set; }
        private BoundingFrustum _frustum;
        private BoundingBox size;
        public MarcadoresCheckPoints(Matrix view, Matrix projection) {
            Initialize(view,projection);
        }

        private void Initialize(Matrix view, Matrix projection) {
            _marcadoresCheckPoints = new List<Matrix>();
            Colliders = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);
        }

        public void LoadContent(ContentManager Content){
            ModeloMarcadorCheckPoint = Content.Load<Model>("Models/" + "CheckPoint/windmill"); 
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloMarcadorCheckPoint.Meshes){
                Console.WriteLine($"Meshname marcadorCheck {mesh.Name}");
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloMarcadorCheckPoint);

        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection) {
            
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            float sinOffset = (float)Math.Sin(Rotation) * 0.8f; // multiplicador para la amplitud

            for (int i = 0; i < _marcadoresCheckPoints.Count; i++)
            {
                var originalPosition = _marcadoresCheckPoints[i].Translation; // Obtener la posición original
                _marcadoresCheckPoints[i] = Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(originalPosition.X, originalPosition.Y + (sinOffset) * 0.05f, originalPosition.Z);
                // Comprobar colisión
                var marcadorBoundingSphere = new BoundingSphere(originalPosition, scale.Translation.X); // Ajustar el tamaño de la esfera de colisión según sea necesario
                if (true) // logica del checkpoint fue renovado
                {
                    // Acción al tocar el modelo
                    Console.WriteLine($"¡Colisión con el pez en la posición {originalPosition}!");
                    _marcadoresCheckPoints.RemoveAt(i);
                    
                }
                _frustum = new BoundingFrustum(view * projection);

                
            }
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Chocolate.ToVector3());

            
            foreach (var mesh in ModeloMarcadorCheckPoint.Meshes){
                string meshName = mesh.Name.ToLower();
                for (int i=0; i < _marcadoresCheckPoints.Count; i++){
                    Matrix _pisoWorld = _marcadoresCheckPoints[i];
                    BoundingBox boundingBox = BoundingVolumesExtensions.FromMatrix(_pisoWorld);
                    
                    if(_frustum.Intersects(boundingBox)){
                        Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                        Console.WriteLine($"Meshname marcador checkpoint {meshName}");
                        mesh.Draw();
                    }
                }
            }
        }


        public void AgregarNuevoMarcadorCheckPoint(float Rotacion, Vector3 Posicion) {
            var transform = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(Posicion) * Matrix.CreateScale(5f); 
            _marcadoresCheckPoints.Add(transform);
            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            BoundingBox box = new BoundingBox(size.Min* 5f  + Posicion* 5f , size.Max* 5f + Posicion * 5f);

            Colliders.Add(box);
        }

    }
}
