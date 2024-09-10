using System;
using System.Numerics;
using BepuPhysics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Objects;
using TGC.MonoGame.TP.Pistas;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP{
    public class TGCGame : Game{
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        public TGCGame(){
            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

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
        private Sphere sphere { get; set; }
        private Pista pista { get; set; }

        protected override void Initialize(){
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            World = Matrix.Identity;
            View = Matrix.CreateLookAt(Vector3.UnitZ * 150, Vector3.Zero, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 250);

            Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);

            pista = new Pista();
            sphere = new Sphere(new (0f,30f,0f));
            sphere.SphereCamera = Camera;

            base.Initialize();
        }

        protected override void LoadContent(){
            sphere.LoadContent(Content);
            pista.LoadContent(Content);

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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime){
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sphere.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);
            pista.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);

            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            Graphics.GraphicsDevice.RasterizerState = rasterizerState;

            GraphicsDevice.RasterizerState = originalRasterizerState;
        }

        protected override void UnloadContent(){
            Content.Unload();
            base.UnloadContent();
        }
    }
}