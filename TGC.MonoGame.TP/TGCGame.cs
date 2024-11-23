using TGC.MonoGame.TP.Levels;
using System.Threading;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector3 = Microsoft.Xna.Framework.Vector3;

using Microsoft.Xna.Framework.Media;
/*using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Fondo;
using TGC.MonoGame.TP.MaterialesJuego;
using TGC.MonoGame.TP.Constructor;*/
using TGC.MonoGame.MenuPrincipal;
using MonoGame.Framework;

using TGC.MonoGame.TP.Geometries;

namespace TGC.MonoGame.TP
{
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Audio/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";
        private GraphicsDeviceManager Graphics { get; }
        private Model Model { get; set; }
        private Effect Effect { get; set; }
        private Matrix World { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private VertexBuffer VertexBuffer { get; set; }
        private IndexBuffer IndexBuffer { get; set; }

        // Esfera
        private Matrix rotation = Matrix.Identity;
        //Sphere esfera;

        // Mundo
        //private Materiales _materiales { get; set; }
        //private ConstructorMateriales _constructorMateriales { get; set; }
        //LineDrawer lineDrawer;
        private Gizmos.Gizmos Gizmos;
        //private SkyBox SkyBox { get; set; }

        // Camaras       
        // Extras
        private Menu menu;
        private SpriteFont menuFont; // Asegúrate de cargar una fuente para el menú
        public bool isMenuActive = true;
        private SpriteBatch SpriteBatch { get; set; }
        private Song backgroundMusic;
        public bool isMusicActive = false;
        public bool isMusicPlaying = false;
        public bool isGodModeActive = false;

        private Level nivelActual;
        private int nivelSeleccionado = 0;

        private ManualResetEvent nivelSeleccionadoEvent = new ManualResetEvent(true);

        public TGCGame()
        {

            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Cambiar el título de la ventana
            Window.Title = " Sphere";
            menu = new Menu();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            menuFont = Content.Load<SpriteFont>(ContentFolderSpriteFonts + "CascadiaCodePl");
            backgroundMusic = Content.Load<Song>(ContentFolderMusic + "Sad Town");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;

            // Cargar contenido del nivel actual
            nivelActual = new LevelOne(GraphicsDevice, Content);
            iniciarNivelActual();
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // Esperar hasta que SeleccionarNivel se complete
            nivelSeleccionadoEvent.WaitOne();

            var keyboardState = Keyboard.GetState();

            if (isMusicPlaying)
            {
                if (!isMusicActive)
                {
                    MediaPlayer.Play(backgroundMusic);
                }
                isMusicActive= true;

                //isMusicPlaying = false;
            }
            else
            {
                MediaPlayer.Stop();
                isMusicActive = false;
            }

            if (isMenuActive)
            {
                if (!(MediaPlayer.Volume == 0.3f)) MediaPlayer.Volume = 0.3f;
                menu.Update(this, gameTime);
            }
            else
            {



                if (!(MediaPlayer.Volume == 0.1f)) MediaPlayer.Volume = 0.2f;

                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    isMenuActive = true;
                }

                if (nivelActual != null)
                {
                    nivelActual.esfera.setGodMode(isGodModeActive);
                    nivelActual.Update(gameTime);

                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Esperar hasta que SeleccionarNivel se complete
            nivelSeleccionadoEvent.WaitOne();

            GraphicsDevice.Clear(Color.CornflowerBlue);
            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None
            };
            GraphicsDevice.RasterizerState = rasterizerState;

            // Guarda los estados actuales
            var originalBlendState = GraphicsDevice.BlendState;
            var originalDepthStencilState = GraphicsDevice.DepthStencilState;

            // Dibuja el nivel actual siempre
            if (nivelActual != null)
            {
                nivelActual.Draw(gameTime);
            }

            // Dibuja el menú si está activo
            if (isMenuActive)
            {
                // Calcula el tiempo para girar la cámara
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float rotationSpeed = 0.3f; // Velocidad de rotación
            float radius = 100f; // Distancia de la cámara a la pelota

                float angle = rotationSpeed * (float)gameTime.TotalGameTime.TotalSeconds;

                // Calcula la posición de la cámara en un círculo inclinado a 45 grados
                float height = radius * (float)Math.Sin(MathHelper.PiOver4); // Altura a 45 grados
                float distance = radius * (float)Math.Cos(MathHelper.PiOver4 - 10); // Distancia horizontal a 45 grados

                var position = new Vector3((float)Math.Cos(angle) * distance, height, (float)Math.Sin(angle) * distance);

                nivelActual.FrustrumCamera = new FollowCamera(GraphicsDevice, position, Vector3.One, Vector3.Up);
                
                SpriteBatch.Begin();
                menu.Draw(SpriteBatch, menuFont, this);
                SpriteBatch.End();
            }

            // Restaura los estados originales
            GraphicsDevice.RasterizerState = originalRasterizerState;
            GraphicsDevice.BlendState = originalBlendState;
            GraphicsDevice.DepthStencilState = originalDepthStencilState;
            base.Draw(gameTime);
        }

        public void SeleccionarNivel(int nivel)
        {
            // Bloquear la ejecución
            nivelSeleccionadoEvent.Reset();

            // Imprimir en la consola
            Console.WriteLine("Nivel seleccionado");
            if (nivel == 1)
            {
                nivelActual = new LevelOne(GraphicsDevice, Content);
            }
            else if (nivel == 2)
            {
                nivelActual = new LevelTwo(GraphicsDevice, Content);
            }
            nivelSeleccionado = nivel;

            iniciarNivelActual();
            isMenuActive = false;

            Console.WriteLine("Ejecución terminada");

            // Liberar la ejecución
            nivelSeleccionadoEvent.Set();
        }
        public void iniciarNivelActual()
        {
            nivelActual.Initialize();
            nivelActual.LoadContent();
        }
    }
}