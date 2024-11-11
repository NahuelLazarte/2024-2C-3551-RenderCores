/*using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector3 = Microsoft.Xna.Framework.Vector3;

using Microsoft.Xna.Framework.Media;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Fondo;
using TGC.MonoGame.TP.MaterialesJuego;
using TGC.MonoGame.TP.Constructor;
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

        //Esfera
        private Matrix rotation = Matrix.Identity;
        Sphere esfera;

        //Mundo
        private Materiales _materiales { get; set; }
        private ConstructorMateriales _constructorMateriales { get; set; }
        LineDrawer lineDrawer;
        private Gizmos.Gizmos Gizmos;
        private SkyBox SkyBox { get; set; }

        //Camaras
        private FollowCamera FrustrumCamera { get; set; }
        private FreeCamera TestCamera;
        BoundingFrustum _frustum { get; set; }

        //Luz
        private CubePrimitive LightBox { get; set; }
        private Matrix LightBoxWorld { get; set; } = Matrix.Identity;

        //Extras
        private Menu menu;
        private SpriteFont menuFont; // Asegúrate de cargar una fuente para el menú
        public bool isMenuActive = true;
        private SpriteBatch SpriteBatch { get; set; }
        private Song backgroundMusic;
        private bool isMusicPlaying = false;
        public bool isGodModeActive = false;

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
            //Inicializar Menu
            menu = new Menu();

            //Inicializar Gizmos
            Gizmos = new Gizmos.Gizmos();
            lineDrawer = new LineDrawer(GraphicsDevice);

            //Camara de Prueba
            TestCamera = new FreeCamera(new Vector3(0.0f, 10.0f, 0.0f), GraphicsDevice);

            //Camara con Frustrum
            FrustrumCamera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);
            _frustum = new BoundingFrustum(FrustrumCamera.ViewMatrix * FrustrumCamera.ProjectionMatrix);

            //Inicializar Esfera
            esfera = new Sphere(new Vector3(0.0f, 10.0f, 0.0f), rotation, new Vector3(0.5f, 0.5f, 0.5f));
            esfera.Game = this;

            //Inicializar Materiales
            _materiales = new Materiales(Content, GraphicsDevice, _frustum);
            _constructorMateriales = new ConstructorMateriales();
            _constructorMateriales.CargarElementos(_materiales);
            _materiales.DarCollidersEsfera(esfera);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Skybox
            var skyBox = Content.Load<Model>("Models/skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 500);

            //Menu
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            menuFont = Content.Load<SpriteFont>(ContentFolderSpriteFonts + "CascadiaCodePl");

            //Sonidos de Fondo
            backgroundMusic = Content.Load<Song>(ContentFolderMusic + "Sad Town");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;

            Gizmos.LoadContent(GraphicsDevice, Content);
            esfera.LoadContent(Content, GraphicsDevice);

            //Luz
            LightBox = new CubePrimitive(GraphicsDevice, 1f, Color.White);
            SetLightPosition(Vector3.Up * 45f);

            base.LoadContent();
        }
        private void SetLightPosition(Vector3 position)
        {
            LightBoxWorld = Matrix.CreateScale(3f) * Matrix.CreateTranslation(position);
            esfera.Effect.Parameters["lightPosition"].SetValue(position);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (isMusicPlaying)
            {
                MediaPlayer.Play(backgroundMusic);
                isMusicPlaying = false;
            }

            if (isMenuActive)
            {
                if (!(MediaPlayer.Volume == 0.3f)) MediaPlayer.Volume = 0.3f;

                _materiales.Update(gameTime, this, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, _frustum);
                menu.Update(this, gameTime);

            }
            else
            {

                if (!(MediaPlayer.Volume == 0.1f)) MediaPlayer.Volume = 0.2f;

                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    isMenuActive = true;
                }
                esfera.isGodModeActive = isGodModeActive;

                FrustrumCamera.Update(esfera.GetPosition());

                _frustum.Matrix = FrustrumCamera.ViewMatrix * FrustrumCamera.ProjectionMatrix;
                _materiales.Update(gameTime, this, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, _frustum);
                esfera.Update(gameTime, Content);
                esfera.setDirection(FrustrumCamera.GetDirection());

                //Gizmos.UpdateViewProjection(Camera.ViewMatrix, Camera.ProjectionMatrix);
                Gizmos.UpdateViewProjection(TestCamera.ViewMatrix, TestCamera.ProjectionMatrix);


                BoundingSphere boundingSphere = esfera.GetBoundingSphere();
                _materiales.ColliderEsfera(boundingSphere);

            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None
            };

            GraphicsDevice.RasterizerState = rasterizerState;

            // Guarda los estados actuales
            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var originalBlendState = GraphicsDevice.BlendState;
            var originalDepthStencilState = GraphicsDevice.DepthStencilState;

            // Calcula el tiempo para girar la cámara
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float rotationSpeed = 0.3f; // Velocidad de rotación
            float radius = 100f; // Distancia de la cámara a la pelota

            if (isMenuActive)
            {
                float angle = rotationSpeed * (float)gameTime.TotalGameTime.TotalSeconds;

                // Calcula la posición de la cámara en un círculo inclinado a 45 grados
                float height = radius * (float)Math.Sin(MathHelper.PiOver4); // Altura a 45 grados
                float distance = radius * (float)Math.Cos(MathHelper.PiOver4 - 10); // Distancia horizontal a 45 grados

                var position = new Vector3((float)Math.Cos(angle) * distance, height, (float)Math.Sin(angle) * distance);

                FrustrumCamera = new FollowCamera(GraphicsDevice, position, Vector3.One, Vector3.Up);

                //Skybox
                SkyBox.Draw(FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, FrustrumCamera.position);

                //Objetos
                _materiales.Draw(gameTime, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, GraphicsDevice);
                esfera.Draw(FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, FrustrumCamera.position);

                //Menu
                menu.Draw(SpriteBatch, menuFont);
            }
            else
            {

                //Skybox
                SkyBox.Draw(FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, FrustrumCamera.position);

                //Objetos
                _materiales.Draw(gameTime, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, GraphicsDevice);
                esfera.Draw(FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, FrustrumCamera.position);



                Vector3 start = new Vector3(0, 0, 0);
                Vector3 endGreen = new Vector3(50, 0, 0);
                Vector3 endRed = new Vector3(0, 0, 50);

                lineDrawer.DrawLine(start, endGreen, Color.Green, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix);
                lineDrawer.DrawLine(start, endRed, Color.Red, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix);

                LightBox.Draw(LightBoxWorld, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix);

                //Gizmos
                Gizmos.DrawSphere(esfera.GetBoundingSphere().Center, esfera.GetBoundingSphere().Radius * Vector3.One, Color.White);
                Gizmos.DrawFrustum(FrustrumCamera.ViewMatrix * FrustrumCamera.ProjectionMatrix, Color.Aqua);
                Gizmos.Draw();
            }

            // Restaura los estados originales
            GraphicsDevice.RasterizerState = originalRasterizerState;
            GraphicsDevice.BlendState = originalBlendState;
            GraphicsDevice.DepthStencilState = originalDepthStencilState;
        }

        protected override void UnloadContent()
        {
            Content.Unload();
            base.UnloadContent();
        }
        public void Respawn()
        {
            esfera.RespawnAt(_constructorMateriales.posicionCheckPoint);
            // Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);No funciona
        }
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

    }


}*/