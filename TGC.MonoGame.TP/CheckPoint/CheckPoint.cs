using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Asegúrate de tener esta directiva

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;

using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo


namespace TGC.MonoGame.TP.CheckPoint{
    public class CheckPoints{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(1f);
        public Model ModeloCheckPoint { get; set; }
        public BoundingBox[] Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _checkPoints { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public Song CollisionSound { get; set; }

        public CheckPoints() {
            Initialize();
        }

        private void Initialize() {
            _checkPoints = new List<Matrix>();
        }

        public void IniciarColliders() {
            Colliders = new BoundingBox[_checkPoints.Count];

            for (int i = 0; i < _checkPoints.Count; i++) {
                Colliders[i] = BoundingVolumesExtensions.FromMatrix(_checkPoints[i]);
            }
            
        }

        public void LoadContent(ContentManager Content){
            ModeloCheckPoint = Content.Load<Model>("Models/" + "CheckPoint/lantern"); // HAY QUE MOVERLO DE CARPETA
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloCheckPoint.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            CollisionSound = Content.Load<Song>("Audio/CheckPoint"); // Ajusta la ruta según sea necesario


            Console.WriteLine(ModeloCheckPoint != null ? "Modelo cargado exitosamente" : "Error al cargar el modelo");

        }

        public void Update(GameTime gameTime, TGCGame Game) {
            
            //Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            
            //float sinOffset = (float)Math.Sin(Rotation) * 0.8f; // Ajusta el multiplicador para la amplitud

            for (int i = 0; i < _checkPoints.Count; i++) {
                var originalPosition = _checkPoints[i].Translation; // Obtener la posición original
                //_checkPoints[i] =  Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(originalPosition.X, originalPosition.Y + (sinOffset) * 0.05f, originalPosition.Z) ;
            
           
           // Comprobar colisión
            var checkpointBoundingSphere = new BoundingSphere(originalPosition, scale.Translation.X); // Ajustar el tamaño de la esfera de colisión según sea necesario
            
            if (_envolturaEsfera.Intersects(checkpointBoundingSphere)) {
                // Acción al tocar el modelo
                Console.WriteLine($"¡Alcanzaste un nuevo checkpoint: {originalPosition}!");
                // Aquí puedes realizar la acción que desees, como eliminar el pez, reducir vida, etc.
                MediaPlayer.Play(CollisionSound);
                _checkPoints.RemoveAt(i);
                Game.nuevoCheckPoint(originalPosition);
            }
            
            }

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Chocolate.ToVector3());

            
            foreach (var mesh in ModeloCheckPoint.Meshes){
                for(int i=0; i < _checkPoints.Count; i++){
                    Matrix _pisoWorld = _checkPoints[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }
        }


        public void AgregarNuevoCheckPoint(float Rotacion, Vector3 Posicion) {
            var transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale ; 
            _checkPoints.Add(transform); 
            Console.WriteLine($"Drawing checkpoint at position {Posicion}");
        }

    }
}
