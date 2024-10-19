using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Asegúrate de tener esta directiva

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;

using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo


namespace TGC.MonoGame.TP.PowerUpHamburguesa{
    public class PowerUpHamburguesas{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(5f);
        public Model ModeloHamburguesa { get; set; }
        public BoundingBox[] Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _hamburguesas { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public Song CollisionSound { get; set; }

        public PowerUpHamburguesas() {
            Initialize();
        }

        private void Initialize() {
            _hamburguesas = new List<Matrix>();
        }

        public void IniciarColliders() {
            Colliders = new BoundingBox[_hamburguesas.Count];

            for (int i = 0; i < _hamburguesas.Count; i++) {
                Colliders[i] = BoundingVolumesExtensions.FromMatrix(_hamburguesas[i]);
            }
            
        }

        public void LoadContent(ContentManager Content){
            ModeloHamburguesa = Content.Load<Model>("Models/" + "PowerUps/burger"); // HAY QUE MOVERLO DE CARPETA
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloHamburguesa.Meshes){
                
                foreach (var meshPart in mesh.MeshParts){

                    meshPart.Effect = Effect;
                }
            }

            CollisionSound = Content.Load<Song>("Audio/ColisionPez"); // Ajusta la ruta según sea necesario


            Console.WriteLine(ModeloHamburguesa != null ? "Modelo cargado exitosamente" : "Error al cargar el modelo");

        }

        public void Update(GameTime gameTime, TGCGame Game) {
            
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            
            float sinOffset = (float)Math.Sin(Rotation) * 0.8f; // Ajusta el multiplicador para la amplitud

            for (int i = 0; i < _hamburguesas.Count; i++) {
                var originalPosition = _hamburguesas[i].Translation; // Obtener la posición original
                _hamburguesas[i] =  Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(originalPosition.X, originalPosition.Y + (sinOffset) * 0.05f, originalPosition.Z) ;
            
            
           // Comprobar colisión
            var fishBoundingSphere = new BoundingSphere(originalPosition, scale.Translation.X); // Ajustar el tamaño de la esfera de colisión según sea necesario
            if (_envolturaEsfera.Intersects(fishBoundingSphere)) {
                // Acción al tocar el modelo
                Console.WriteLine($"¡Colisión con el pez en la posición {originalPosition}!");
                // Aquí puedes realizar la acción que desees, como eliminar el pez, reducir vida, etc.
                MediaPlayer.Play(CollisionSound);
                _hamburguesas.RemoveAt(i);
                
                Game.recibirPowerUpPez();

            }
            
            }

        }




        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Chocolate.ToVector3());

            
            foreach (var mesh in ModeloHamburguesa.Meshes){
                string meshName = mesh.Name.ToLower(); // Asegúrate de comparar en minúsculas

                for (int i=0; i < _hamburguesas.Count; i++){
                    Matrix _pisoWorld = _hamburguesas[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    switch (meshName)
                    {
                        case "bunbottom":
                            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.8f, 0.52f, 0.25f)); // Color para el pan de abajo
                            break;
                        case "buntop":
                            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.8f, 0.52f, 0.25f)); // Color para el pan de arriba
                            break;
                        case "cheese":
                            Effect.Parameters["DiffuseColor"].SetValue(Color.Yellow.ToVector3()); // Color para el queso
                            break;
                        case "patty":
                            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0.25f, 0.1f)); // Color para la carne
                            break;
                        case "salad":
                            Effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector3()); // Color para la lechuga
                            break;
                        case "tomato":
                            Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3()); // Color para el tomate
                            break;
                        default:
                            Effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3()); // Color por defecto
                            break;
                    }
                    mesh.Draw();
                }
            }
        }


        public void AgregarNuevoPowerUp(float Rotacion, Vector3 Posicion) {
            var transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale ; 
            _hamburguesas.Add(transform); 
            Console.WriteLine($"Drawing fish at position {Posicion}");
        }

    }
}
