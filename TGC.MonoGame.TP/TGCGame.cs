using System;
using System.Collections.Generic;
using BepuPhysics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Objects;
using TGC.MonoGame.TP.Pistas;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Fondo;


using Vector3 = Microsoft.Xna.Framework.Vector3;
using System.Net.Http.Headers;
using TGC.MonoGame.TP.Collisions;


namespace TGC.MonoGame.TP{
    public class TGCGame : Game{
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
        private Pista pista { get; set; }

        private Vector3 posicionActual { get; set; }
        private Vector3 _dimensionesRectaEjeX { get; set; }

        private SkyBox SkyBox { get; set; }
        float rotacionActual = 0f;

        public TGCGame(){
            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Gizmos = new Gizmos.Gizmos();
        }

        protected override void Initialize(){
            Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);

            pista = new Pista();
            sphere = new TGC.MonoGame.TP.Objects.Sphere(new Vector3(0f,30f,0f));
            sphere.SphereCamera = Camera;
            sphere.Colliders = pista.Colliders;
            posicionActual = new Vector3(0f, 0f, 0f);
            
            _dimensionesRectaEjeX = new Vector3(30f, 0f, 30f);

            base.Initialize();
        }

        protected override void LoadContent(){
            pista.LoadContent(Content);
            sphere.LoadContent(Content);

            var skyBox = Content.Load<Model>("Models/skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime){
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape)){
                Exit();
            }

            sphere.Update(gameTime);
            pista.Update(gameTime);

            Camera.Update(sphere.SpherePosition);
            //Gizmos.UpdateViewProjection(Camera.ViewMatrix, Camera.ProjectionMatrix);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime){
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sphere.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);
            pista.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);

            //SkyBox.Draw(View, Projection, GraphicsDevice);

            /*var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            Graphics.GraphicsDevice.RasterizerState = rasterizerState;

            GraphicsDevice.RasterizerState = originalRasterizerState;*/

            //Gizmos.DrawSphere(sphere.SphereCollider.Center, sphere.SphereCollider.Radius * Vector3.One, Color.Red);
        }

        protected override void UnloadContent(){
            Content.Unload();
            base.UnloadContent();
        }

        void AgregarPista(Pista tipoPista)
        {
        /* 
            tipoPista.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix, posicionActual, rotacionActual);
            Vector3 desplazamiento = tipoPista.Desplazamiento();
            Matrix rotacion = tipoPista.Rotacion();
            rotacionActual *=rotacion
            posicionActual += dimensionEnEje; 
        */
        
        }
    }
}