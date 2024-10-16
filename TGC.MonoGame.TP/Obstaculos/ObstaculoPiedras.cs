using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Asegúrate de tener esta directiva

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;

using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo


namespace TGC.MonoGame.TP.ObstaculoPiedras{
    public class ObstaculosPiedras{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(1f);
        public Model ModeloPez { get; set; }
        public BoundingBox[] Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _obstaculosPiedras { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public Song CollisionSound { get; set; }

        public ObstaculosPiedras() {
            Initialize();
        }

        private void Initialize() {
            _obstaculosPiedras = new List<Matrix>();
        }

        public void IniciarColliders() {
            Colliders = new BoundingBox[_obstaculosPiedras.Count];

            for (int i = 0; i < _obstaculosPiedras.Count; i++) {
                Colliders[i] = BoundingVolumesExtensions.FromMatrix(_obstaculosPiedras[i]);
            }
            
        }

        public void LoadContent(ContentManager Content){
            ModeloPez = Content.Load<Model>("Models/" + "obstaculos/rockLarge");
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
            
            for (int i = 0; i < _obstaculosPiedras.Count; i++) {
                var originalPosition = _obstaculosPiedras[i].Translation; // Obtener la posición original            
           // Comprobar colisión
            var piedrasBoundingSphere = new BoundingSphere(originalPosition, scale.Translation.X); // Ajustar el tamaño de la esfera de colisión según sea necesario
            if (_envolturaEsfera.Intersects(piedrasBoundingSphere)) {
                // Acción al tocar el modelo
                Console.WriteLine($"¡Colisión con piedras en la posición {originalPosition}!");
                // Aquí puedes realizar la acción que desees, como eliminar el pez, reducir vida, etc.
                MediaPlayer.Play(CollisionSound);
                //_obstaculosPiedras.RemoveAt(i);

                Game.Respawn();
                
            }
            }

        }




        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));

            
            foreach (var mesh in ModeloPez.Meshes){
                for(int i=0; i < _obstaculosPiedras.Count; i++){
                    Matrix _pisoWorld = _obstaculosPiedras[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }
        }


        public void AgregarNuevoObstaculo(float Rotacion, Vector3 Posicion) {
            var transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale ; 
            _obstaculosPiedras.Add(transform); 
            Console.WriteLine($"Drawing fish at position {Posicion}");
        }

    }
}
