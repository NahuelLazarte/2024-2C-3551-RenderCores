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


namespace TGC.MonoGame.TP.PowerUpEspada{
    public class PowerUpEspadas{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(3f);
        public Model ModeloEspada { get; set; }
        public BoundingBox[] Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _espadas { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public SoundEffect CollisionSound { get; set; }
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private BoundingFrustum _frustum;

        public PowerUpEspadas() {
            Initialize();
        }

        private void Initialize() {
            _espadas = new List<Matrix>();
        }

        public void IniciarColliders() {
            Colliders = new BoundingBox[_espadas.Count];

            for (int i = 0; i < _espadas.Count; i++) {
                Colliders[i] = BoundingVolumesExtensions.FromMatrix(_espadas[i]);
            }
        }

        public void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice){
            ModeloEspada = Content.Load<Model>("Models/" + "PowerUps/sword"); 
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");
            spriteBatch = new SpriteBatch(graphicsDevice); // Inicializa SpriteBatch
            spriteFont = Content.Load<SpriteFont>("SpriteFonts/" + "CascadiaCodePl"); 

            foreach (var mesh in ModeloEspada.Meshes){
                
                foreach (var meshPart in mesh.MeshParts){

                    meshPart.Effect = Effect;
                }
            }

            CollisionSound = Content.Load<SoundEffect>("Audio/ColisionPez"); 


            Console.WriteLine(ModeloEspada != null ? "Modelo cargado exitosamente" : "Error al cargar el modelo");

        }

        public void Update(GameTime gameTime, TGCGame Game, Matrix view, Matrix projection) {
            
            for (int i = 0; i < _espadas.Count; i++) {
                var originalPosition = _espadas[i].Translation; // Obtener la posición original
                // Comprobar colisión
                var swordBoundingSphere = new BoundingSphere(originalPosition, scale.Translation.X); // Ajustar el tamaño de la esfera de colisión según sea necesario
                if (_envolturaEsfera.Intersects(swordBoundingSphere)) {
                    // Acción al tocar el modelo
                    CollisionSound.Play();
                    _espadas.RemoveAt(i);
                    Game.recibirPowerUpEspada();

                }
            
            }
            _frustum = new BoundingFrustum(view * projection);

        }


        public void Draw(GameTime gameTime, Matrix view, Matrix projection, GraphicsDevice graphicsDevice)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Chocolate.ToVector3());

            
            foreach (var mesh in ModeloEspada.Meshes){

                for (int i=0; i < _espadas.Count; i++){
                    Matrix _pisoWorld = _espadas[i];
                    BoundingBox boundingBox = BoundingVolumesExtensions.FromMatrix(_pisoWorld);
                    if(_frustum.Intersects(boundingBox)){
                        Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                        mesh.Draw();
                    }
                }
            }
            
        }


        public void AgregarNuevoPowerUp(float Rotacion, Vector3 Posicion) {
            var posicionEspada = new Vector3(Posicion.X / 200f ,Posicion.Y / 200f, Posicion.Z / 200f );
            
            var desplazamiento = new Vector3(-27.5f , 3f , 80.5f);
            
            var posicionFinal = posicionEspada + Vector3.Transform(desplazamiento, Matrix.CreateRotationY(Rotacion));

            var transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(posicionFinal) * scale ; 

            _espadas.Add(transform); 
            
        }

    }
}
