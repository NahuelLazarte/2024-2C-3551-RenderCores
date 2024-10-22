﻿using System;
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
        private SpriteBatch SpriteBatch { get; set; }
        private Song backgroundMusic;
        private Model Model { get; set; }
        private Effect Effect { get; set; }
        private Matrix World { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private VertexBuffer VertexBuffer { get; set; }
        private IndexBuffer IndexBuffer { get; set; }
        private FollowCamera Camera { get; set; }
        private TGC.MonoGame.TP.Objects.Sphere sphere { get; set; }
        private Gizmos.Gizmos Gizmos;
        Modelos.Sphere esfera;
        LineDrawer lineDrawer;
        private SkyBox SkyBox { get; set; }
        private Materiales _materiales { get; set; }
        private ConstructorMateriales _constructorMateriales { get; set; }
        //   
        private Menu menu;
        public bool isMenuActive = true;
        public bool isGodModeActive = false;
        private SpriteFont menuFont; // Asegúrate de cargar una fuente para el menú

        private bool isMusicPlaying = false;

        private CubePrimitive LightBox { get; set; }
        private Matrix LightBoxWorld { get; set; } = Matrix.Identity;
        //
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
            Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);
            Gizmos = new Gizmos.Gizmos();
            Matrix rotation = Matrix.Identity;

            esfera = new Modelos.Sphere(new Vector3(0.0f, 10.0f, 0.0f), rotation, new Vector3(0.5f, 0.5f, 0.5f));
            esfera.Game = this;

            menu = new Menu();

            lineDrawer = new LineDrawer(GraphicsDevice);

            BoundingSphere boundingSphere = esfera.GetBoundingSphere();

            _materiales = new Materiales(Content, GraphicsDevice);
            _constructorMateriales = new ConstructorMateriales();
            _constructorMateriales.CargarElementos(_materiales);
            _materiales.DarCollidersEsfera(esfera);

            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var skyBox = Content.Load<Model>("Models/skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            menuFont = Content.Load<SpriteFont>(ContentFolderSpriteFonts + "CascadiaCodePl");

            backgroundMusic = Content.Load<Song>(ContentFolderMusic + "Sad Town");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;


            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 500);
            Gizmos.LoadContent(GraphicsDevice, Content);
            esfera.LoadContent(Content, GraphicsDevice);

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


            if (!isMusicPlaying)
            {
                MediaPlayer.Play(backgroundMusic);
                isMusicPlaying = true;
            }

            if (isMenuActive)
            {
                if (!(MediaPlayer.Volume == 0.3f)) MediaPlayer.Volume = 0.3f;

                _materiales.Update(gameTime, this, Camera.ViewMatrix, Camera.ProjectionMatrix);
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

                _materiales.Update(gameTime, this, Camera.ViewMatrix, Camera.ProjectionMatrix);
                BoundingSphere boundingSphere = esfera.GetBoundingSphere();
                _materiales.ColliderEsfera(boundingSphere);
                Gizmos.UpdateViewProjection(Camera.ViewMatrix, Camera.ProjectionMatrix);
                Camera.Update(esfera.GetPosition());
                esfera.Update(gameTime, Content);
                esfera.setDirection(Camera.GetDirection());

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

                Camera = new FollowCamera(GraphicsDevice, position, Vector3.One, Vector3.Up);

                SkyBox.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix, Camera.position);

                _materiales.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix, GraphicsDevice);


                esfera.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix,Camera.position);


                menu.Draw(SpriteBatch, menuFont);
            }
            else
            {


                SkyBox.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix, Camera.position);

                _materiales.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix, GraphicsDevice);

                BoundingSphere boundingSphere = esfera.GetBoundingSphere();

                Gizmos.DrawSphere(boundingSphere.Center, boundingSphere.Radius * Vector3.One, Color.White);

                Gizmos.Draw();

                esfera.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix,Camera.position);

                Vector3 start = new Vector3(0, 0, 0);
                Vector3 endGreen = new Vector3(50, 0, 0);
                Vector3 endRed = new Vector3(0, 0, 50);

                lineDrawer.DrawLine(start, endGreen, Color.Green, Camera.ViewMatrix, Camera.ProjectionMatrix);
                lineDrawer.DrawLine(start, endRed, Color.Red, Camera.ViewMatrix, Camera.ProjectionMatrix);

                LightBox.Draw(LightBoxWorld, Camera.ViewMatrix, Camera.ProjectionMatrix);

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

        private List<Vector3> Esferas = new();

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


}