using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Modelos;

using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            
            // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private Model Model { get; set; }
        private Effect Effect { get; set; }


        private Matrix World { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private VertexBuffer VertexBuffer{ get; set;}
        private IndexBuffer IndexBuffer{ get; set; }

        private Sphere esfera{ get; set; }

        private List<Car> CarList{ get; set; }
        private const uint NumCars = 50;
        private List<Tree> TreeList{ get; set; }
        private const uint NumTree = 50;
        private List<Tunnel> TunnelList{ get; set; }
        private const uint NumTunnel = 10;
        private List<Wave_A> Wave_AList{ get; set; }
        private const uint NumWave_A = 20;
        private const float MapSize = 500f;       

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

           // Configuramos nuestras matrices de la escena.           

            World = Matrix.Identity;
            View = Matrix.CreateLookAt(new Vector3(0f, 25f, 100f), Vector3.Zero, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);

            CarList = new List<Car>();
            TreeList = new List<Tree>();
            TunnelList = new List<Tunnel>();
            Wave_AList = new List<Wave_A>();

            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            esfera = new Sphere( Content, new Vector3(0f, 3.4f, 0f), Matrix.Identity, Color.Purple );
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
            var indices = new ushort[]{ 0, 1, 2, 1, 3, 2};

            IndexBuffer.SetData(indices);

            

            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logica de actualizacion del juego.

            // Capturar Input teclado
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //Salgo del juego.
                Exit();
            }
            

            // Basado en el tiempo que paso se va generando una rotacion.
            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Effect.CurrentTechnique = Effect.Techniques["BasicColorDrawing"];

            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            Effect.Parameters["World"].SetValue(World);
            Effect.Parameters["View"].SetValue(View);
            Effect.Parameters["Projection"].SetValue(Projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Gray.ToVector3());
            
            
            GraphicsDevice.Indices = IndexBuffer;
            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            
            
            foreach( var passes in Effect.CurrentTechnique.Passes )
            {
                passes.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }
            
            
            foreach( Car _car in CarList)
            {
                _car.Draw(Effect);
            }
            
            foreach( Tree _tree in TreeList )
            {
                _tree.Draw(Effect);
            }

            foreach( Tunnel _tunnel in TunnelList )
            {
                _tunnel.Draw(Effect);
            }

            foreach( Wave_A _waveA in Wave_AList )
            {
                _waveA.Draw(Effect);
            }

            esfera.Draw(Effect);

        }

        protected void AgregarAutos(uint _cantidad)
        {   
            int seed = 1;
            Random _random = new Random(seed);

            for (uint i = 0; i < _cantidad; i++)
            {
                float pos_x = (float)(_random.NextDouble() * 2 * MapSize - MapSize);
                float pos_z = (float)(_random.NextDouble() * 2 * MapSize - MapSize);

                Car _newCar = new Car(Content, new Vector3(pos_x, 0f, pos_z), Matrix.CreateRotationY((float)(_random.NextDouble())), Color.Red);
                _newCar.LoadContent(Effect);
                CarList.Add(_newCar);
            }
        }

         protected void AgregarArbol(uint _cantidad)
        {   
            int seed = 3;
            Random _random = new Random(seed);

            for (uint i = 0; i < _cantidad; i++)
            {
                float pos_x = (float)(_random.NextDouble() * 2 * MapSize - MapSize);
                float pos_z = (float)(_random.NextDouble() * 2 * MapSize - MapSize);

                Tree _newTree = new Tree(Content, new Vector3(pos_x, 0f, pos_z), Matrix.CreateRotationY((float)(_random.NextDouble())), Color.Green);
                _newTree.LoadContent(Effect);
                TreeList.Add(_newTree);
            }
        }

        protected void AgregarTunnel(uint _cantidad)
        {   
            int seed = 4;
            Random _random = new Random(seed);

            for (uint i = 0; i < _cantidad; i++)
            {
                float pos_x = (float)(_random.NextDouble() * 2 * MapSize - MapSize);
                float pos_z = (float)(_random.NextDouble() * 2 * MapSize - MapSize);

                Tunnel _newTunnel = new Tunnel(Content, new Vector3(pos_x, 0f, pos_z), Matrix.CreateRotationY((float)(_random.NextDouble())), Color.LightYellow);
                _newTunnel.LoadContent(Effect);
                TunnelList.Add(_newTunnel);
            }
        }

        protected void AgregarWave_A(uint _cantidad)
        {   
            int seed = 6;
            Random _random = new Random(seed);

            for (uint i = 0; i < _cantidad; i++)
            {
                float pos_x = (float)(_random.NextDouble() * 2 * MapSize - MapSize);
                float pos_z = (float)(_random.NextDouble() * 2 * MapSize - MapSize);

                Wave_A _newWave = new Wave_A(Content, new Vector3(pos_x, -3f, pos_z), Matrix.CreateRotationY((float)(_random.NextDouble())), Color.SaddleBrown);
                _newWave.LoadContent(Effect);
                Wave_AList.Add(_newWave);
            }
        }
        

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }
    }
}