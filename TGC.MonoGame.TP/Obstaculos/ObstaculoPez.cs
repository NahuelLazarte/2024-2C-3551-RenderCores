using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Asegúrate de tener esta directiva

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;

using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo


namespace TGC.MonoGame.TP.ObstaculoPez{
    public class ObstaculosPeces{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(5f);
        public Model ModeloPez { get; set; }
        public BoundingBox[] Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _peces { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public Song CollisionSound { get; set; }

        public ObstaculosPeces() {
            Initialize();
        }

        private void Initialize() {
            _peces = new List<Matrix>();
        }

        public void IniciarColliders() {
            Colliders = new BoundingBox[_peces.Count];

            for (int i = 0; i < _peces.Count; i++) {
                Colliders[i] = BoundingVolumesExtensions.FromMatrix(_peces[i]);
            }
            
        }

        public void LoadContent(ContentManager Content){
            ModeloPez = Content.Load<Model>("Models/" + "obstaculos/fish");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloPez.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            CollisionSound = Content.Load<Song>("Audio/ColisionPez"); // Ajusta la ruta según sea necesario


            Console.WriteLine(ModeloPez != null ? "Modelo cargado exitosamente" : "Error al cargar el modelo");

        }

        public void Update(GameTime gameTime, TGCGame Game) {
            
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            
            float sinOffset = (float)Math.Sin(Rotation) * 0.8f; // Ajusta el multiplicador para la amplitud

            for (int i = 0; i < _peces.Count; i++) {
                var originalPosition = _peces[i].Translation; // Obtener la posición original
                _peces[i] =  Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(originalPosition.X, originalPosition.Y + (sinOffset) * 0.05f, originalPosition.Z) ;
            
           
           // Comprobar colisión
            var fishBoundingSphere = new BoundingSphere(originalPosition, scale.Translation.X); // Ajustar el tamaño de la esfera de colisión según sea necesario
            if (_envolturaEsfera.Intersects(fishBoundingSphere)) {
                // Acción al tocar el modelo
                Console.WriteLine($"¡Colisión con el pez en la posición {originalPosition}!");
                // Aquí puedes realizar la acción que desees, como eliminar el pez, reducir vida, etc.
                MediaPlayer.Play(CollisionSound);
                _peces.RemoveAt(i);
                
            }
            
            }

        }




        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Chocolate.ToVector3());

            
            foreach (var mesh in ModeloPez.Meshes){
                for(int i=0; i < _peces.Count; i++){
                    Matrix _pisoWorld = _peces[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }
        }


        public void AgregarNuevoObstaculo(float Rotacion, Vector3 Posicion) {
            var transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale ; 
            _peces.Add(transform); 
            Console.WriteLine($"Drawing fish at position {Posicion}");
        }

    }
}