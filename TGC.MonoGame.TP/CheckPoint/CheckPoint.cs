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


namespace TGC.MonoGame.TP.CheckPoint{
    public class CheckPoints{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(1f);
        public Model ModeloCheckPoint { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _checkPoints { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public SoundEffect CollisionSound { get; set; }
        BoundingBox size;
        private BoundingFrustum _frustum;
        public CheckPoints(Matrix view, Matrix projection) {
            Initialize(view,projection);
        }

        private void Initialize(Matrix view, Matrix projection) {
            _checkPoints = new List<Matrix>();
            Colliders = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);
        }

        public void IniciarColliders() {
          
            
        }

        public void LoadContent(ContentManager Content){
            ModeloCheckPoint = Content.Load<Model>("Models/" + "CheckPoint/campfire"); // HAY QUE MOVERLO DE CARPETA
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloCheckPoint.Meshes){
                Console.WriteLine($"Meshname pistacurva {mesh.Name}");
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            CollisionSound = Content.Load<SoundEffect>("Audio/CheckPoint"); // Ajusta la ruta según sea necesario

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloCheckPoint);


        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection) {
            
            //Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            
            //float sinOffset = (float)Math.Sin(Rotation) * 0.8f; // Ajusta el multiplicador para la amplitud

            for (int i = 0; i < _checkPoints.Count; i++) {
                var originalPosition = _checkPoints[i].Translation;
                if (_envolturaEsfera.Intersects(Colliders[i])) {
                    // Acción al tocar el modelo
                    // Aquí puedes realizar la acción que desees, como eliminar el pez, reducir vida, etc.
                    CollisionSound.Play();
                    _checkPoints.RemoveAt(i);
                    Game.nuevoCheckPoint(originalPosition);
                }
            }
            _frustum = new BoundingFrustum(view * projection);

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Chocolate.ToVector3());

            
            foreach (var mesh in ModeloCheckPoint.Meshes){
                string meshName = mesh.Name.ToLower();
                for (int i=0; i < _checkPoints.Count; i++){
                    Matrix _pisoWorld = _checkPoints[i];
                    BoundingBox boundingBox = BoundingVolumesExtensions.FromMatrix(_pisoWorld);
                    
                    if(_frustum.Intersects(boundingBox)){
                        Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                        switch (meshName)
                        {
                            case "campfire":
                                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(1.0f, 0.5f, 0.0f)); // Color para el pan de abajo
                                break;
                            case "bucket":
                                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.7f, 0.7f, 0.7f)); // Color para el pan de arriba
                                break;
                            case "rocks":
                                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f)); // Color para el queso
                                break;
                            case "wood":
                                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.6f, 0.3f, 0.1f)); // Color para la carne
                                break;
                        }
                        mesh.Draw();
                    }
                }
            }
        }


        public void AgregarNuevoCheckPoint(float Rotacion, Vector3 Posicion) {
            var transform = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(Posicion) * Matrix.CreateScale(5f); 
            _checkPoints.Add(transform);
            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            BoundingBox box = new BoundingBox(size.Min* 5f  + Posicion* 5f , size.Max* 5f + Posicion * 5f);

            Colliders.Add(box);
        }

    }
}
