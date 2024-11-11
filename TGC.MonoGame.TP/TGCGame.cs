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
using MonoGamers.Camera;

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
        private bool isMusicPlaying = false;
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
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // Esperar hasta que SeleccionarNivel se complete
            nivelSeleccionadoEvent.WaitOne();

            var keyboardState = Keyboard.GetState();

            if (isMusicPlaying)
            {
                MediaPlayer.Play(backgroundMusic);
                isMusicPlaying = false;
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

            if (isMenuActive)
            {
                menu.Draw(SpriteBatch, menuFont);
            }
            else
            {
                if (nivelActual != null)
                {
                    nivelActual.Draw(gameTime);                    
                }
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
            if(nivel ==1){
                nivelActual = new LevelOne(GraphicsDevice, Content);
            }
            else if(nivel ==2){
                nivelActual = new LevelTwo(GraphicsDevice, Content);
            }
            nivelSeleccionado = nivel;
            
            nivelActual.Initialize();
            nivelActual.LoadContent();
            isMenuActive = false;

            Console.WriteLine("Ejecución terminada");

            // Liberar la ejecución
            nivelSeleccionadoEvent.Set();
        }
    }
}