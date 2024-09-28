using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Fondo;

using Vector3 = Microsoft.Xna.Framework.Vector3;

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

        public TGCGame()
        {
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

        private Sphere esfera { get; set; }

        private List<Car> CarList { get; set; }
        private const uint NumCars = 50;
        private List<Tree> TreeList { get; set; }
        private const uint NumTree = 50;
        private List<Tunnel> TunnelList { get; set; }
        private const uint NumTunnel = 10;
        private List<Wave_A> Wave_AList { get; set; }
        private const uint NumWave_A = 20;
        private const float MapSize = 500f;

        private SkyBox SkyBox { get; set; }
        private Vector3 CameraPosition { get; set; }
        private float Distance { get; set; }
        private Vector3 CameraTarget { get; set; }
        private Vector3 ViewVector { get; set; }
        private float Angle { get; set; }

        protected override void Initialize()
        {
            World = Matrix.Identity;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000f);

            CarList = new List<Car>();
            TreeList = new List<Tree>();
            TunnelList = new List<Tunnel>();
            Wave_AList = new List<Wave_A>();

            CameraTarget = new Vector3(-10f, 0f, -10f);//Vector3.Zero;
            Distance = 400;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            esfera = new Sphere(Content, new Vector3(0f, 3.4f, 0f), Matrix.Identity, Color.Purple);
            esfera.LoadContent(Effect);

            AgregarTunnel(NumTunnel);
            AgregarArbol(NumTree);
            AgregarAutos(NumCars);
            AgregarWave_A(NumWave_A);

            VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 4, BufferUsage.None);
            var vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(MapSize, 0f, MapSize), Color.Blue),
                new VertexPositionColor(new Vector3(MapSize, 0f, -MapSize), Color.Red),
                new VertexPositionColor(new Vector3(-MapSize, 0f, MapSize), Color.Green),
                new VertexPositionColor(new Vector3(-MapSize, 0f, -MapSize), Color.Yellow),
            };
            VertexBuffer.SetData(vertices);

            IndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.None);
            var indices = new ushort[] { 0, 1, 2, 1, 3, 2 };
            IndexBuffer.SetData(indices);

            var skyBox = Content.Load<Model>("Models/skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            CameraPosition = Distance * new Vector3((float)Math.Sin(Angle), 0, (float)Math.Cos(Angle));
            ViewVector = Vector3.Transform(CameraTarget - CameraPosition, Matrix.CreateRotationY(0));
            ViewVector.Normalize();

            Angle += 0.002f;
            View = Matrix.CreateLookAt(CameraPosition, CameraTarget, Vector3.UnitY);

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

            SkyBox.Draw(View, Projection, CameraPosition);

            GraphicsDevice.RasterizerState = originalRasterizerState;

            Effect.CurrentTechnique = Effect.Techniques["BasicColorDrawing"];
            Effect.Parameters["World"].SetValue(World);
            Effect.Parameters["View"].SetValue(View);
            Effect.Parameters["Projection"].SetValue(Projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Gray.ToVector3());

            GraphicsDevice.Indices = IndexBuffer;
            GraphicsDevice.SetVertexBuffer(VertexBuffer);

            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }

            foreach (var car in CarList)
            {
                car.Draw(Effect);
            }

            foreach (var tree in TreeList)
            {
                tree.Draw(Effect);
            }

            foreach (var tunnel in TunnelList)
            {
                tunnel.Draw(Effect);
            }

            foreach (var waveA in Wave_AList)
            {
                waveA.Draw(Effect);
            }

            esfera.Draw(Effect);

            base.Draw(gameTime);
        }

        protected void AgregarAutos(uint cantidad)
        {
            var random = new Random(1);

            for (uint i = 0; i < cantidad; i++)
            {
                float posX = (float)(random.NextDouble() * 2 * MapSize - MapSize);
                float posZ = (float)(random.NextDouble() * 2 * MapSize - MapSize);

                var newCar = new Car(Content, new Vector3(posX, 0f, posZ), Matrix.CreateRotationY((float)(random.NextDouble())), Color.Red);
                newCar.LoadContent(Effect);
                CarList.Add(newCar);
            }
        }

        protected void AgregarArbol(uint cantidad)
        {
            var random = new Random(3);

            for (uint i = 0; i < cantidad; i++)
            {
                float posX = (float)(random.NextDouble() * 2 * MapSize - MapSize);
                float posZ = (float)(random.NextDouble() * 2 * MapSize - MapSize);

                var newTree = new Tree(Content, new Vector3(posX, 0f, posZ), Matrix.CreateRotationY((float)(random.NextDouble())), Color.Green);
                newTree.LoadContent(Effect);
                TreeList.Add(newTree);
            }
        }

        protected void AgregarTunnel(uint cantidad)
        {
            var random = new Random(4);

            for (uint i = 0; i < cantidad; i++)
            {
                float posX = (float)(random.NextDouble() * 2 * MapSize - MapSize);
                float posZ = (float)(random.NextDouble() * 2 * MapSize - MapSize);

                var newTunnel = new Tunnel(Content, new Vector3(posX, 0f, posZ), Matrix.CreateRotationY((float)(random.NextDouble())), Color.LightYellow);
                newTunnel.LoadContent(Effect);
                TunnelList.Add(newTunnel);
            }
        }

        protected void AgregarWave_A(uint cantidad)
        {
            var random = new Random(6);

            for (uint i = 0; i < cantidad; i++)
            {
                float posX = (float)(random.NextDouble() * 2 * MapSize - MapSize);
                float posZ = (float)(random.NextDouble() * 2 * MapSize - MapSize);

                var newWave = new Wave_A(Content, new Vector3(posX, -3f, posZ), Matrix.CreateRotationY((float)(random.NextDouble())), Color.SaddleBrown);
                newWave.LoadContent(Effect);
                Wave_AList.Add(newWave);
            }
        }

        protected override void UnloadContent()
        {
            Content.Unload();
            base.UnloadContent();
        }
    }
}