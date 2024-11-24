using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Constructor;

namespace TGC.MonoGame.TP.Levels
{
    public abstract class Level
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Audio/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";
        protected GraphicsDevice GraphicsDevice { get; }
        protected ContentManager Content { get; }

        public FollowCamera FrustrumCamera { get; set; }

        public Sphere esfera; 

        private Matrix rotation = Matrix.Identity;

        protected Level(GraphicsDevice graphicsDevice, ContentManager content)
        {
            // Inicializar Esfera
            esfera = new Sphere(new Vector3(0.0f, 10.0f, 0.0f), rotation, new Vector3(0.5f, 0.5f, 0.5f));
            GraphicsDevice = graphicsDevice;
            Content = content;
        }

        public abstract void Initialize();
        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
        public abstract void UnloadContent();
        public bool leveIsActive = false;

        protected ConstructorMateriales _constructorMateriales { get; set; }

        /*
        public void nuevoCheckPoint(Vector3 posicion){
            _constructorMateriales.posicionCheckPoint = new Vector3(posicion.X, posicion.Y + 5f, posicion.Z);
        }*/

        public void nuevoCheckPoint(Vector3 posicion)
        {
            _constructorMateriales.posicionCheckPoint = new Vector3(posicion.X, posicion.Y + 5f, posicion.Z);
        }

        public void recibirPowerUpPez()
        {
            esfera.aumentarVelocidad(1.5f);
        }
        public void recibirPowerUpEspada()
        {
            // por ahora nada, luego rompera obstaculos
            //esfera.aumentarVelocidad(1.5f);
        }

        public void Respawn() {
            esfera.RespawnAt(_constructorMateriales.posicionCheckPoint);
            // Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);No funciona
        }
    }
}