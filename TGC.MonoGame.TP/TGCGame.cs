﻿﻿using System;
using System.Collections.Generic;
using BepuPhysics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Objects;

using TGC.MonoGame.TP.Modelos;

using TGC.MonoGame.TP.Fondo;
using TGC.MonoGame.TP.MaterialesJuego;
using TGC.MonoGame.TP.Constructor;

using Vector3 = Microsoft.Xna.Framework.Vector3;
using System.Net.Http.Headers;
using TGC.MonoGame.TP.Collisions;


namespace TGC.MonoGame.TP
{
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
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

            esfera = new Modelos.Sphere(new Vector3(0.0f, 10.0f, 0.0f), rotation, Color.Yellow);
            esfera.Game = this;

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
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 500);

            Gizmos.LoadContent(GraphicsDevice, Content);

            esfera.LoadContent(Content);        

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }


            _materiales.Update(gameTime, this, Camera.ViewMatrix, Camera.ProjectionMatrix);

            BoundingSphere boundingSphere = esfera.GetBoundingSphere();

            _materiales.ColliderEsfera(boundingSphere);

            Gizmos.UpdateViewProjection(Camera.ViewMatrix, Camera.ProjectionMatrix);

            Camera.Update(esfera.GetPosition());

            esfera.Update(gameTime);

            esfera.setDirection(Camera.GetDirection());
            
            


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None
            };
            GraphicsDevice.RasterizerState = rasterizerState;

            SkyBox.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix, Camera.position);

            _materiales.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);

            BoundingSphere boundingSphere = esfera.GetBoundingSphere();

            Gizmos.DrawSphere(boundingSphere.Center, boundingSphere.Radius * Vector3.One, Color.White);

            Gizmos.Draw();

            esfera.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix);

            Vector3 start = new Vector3(0, 0, 0);
            Vector3 endGreen = new Vector3(50, 0, 0);
            Vector3 endRed = new Vector3(0, 0, 50);

            lineDrawer.DrawLine(start, endGreen, Color.Green, Camera.ViewMatrix, Camera.ProjectionMatrix);
            lineDrawer.DrawLine(start, endRed, Color.Red, Camera.ViewMatrix, Camera.ProjectionMatrix);

            GraphicsDevice.RasterizerState = originalRasterizerState;

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
            _constructorMateriales.posicionCheckPoint = new Vector3(posicion.X, posicion.Y +2f, posicion.Z) ;
        }



        public void recibirPowerUpPez()
        {
            esfera.aumentarVelocidad(1.5f);
        }

    }


}