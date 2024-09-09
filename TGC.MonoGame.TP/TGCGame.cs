using System;
using System.Numerics;
using Microsoft.Xna.Framework;
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
        private Model BallModel{ get; set; }
        private Matrix BallWorld{ get; set; }
        private Matrix World { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private VertexBuffer VertexBuffer{ get; set;}
        private IndexBuffer IndexBuffer{ get; set; }

        private Model TrukModel{ get; set; }
        private Matrix TrukWorld{ get; set; }
        private Car car{get;set;}
        

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

           // Configuramos nuestras matrices de la escena.
            BallWorld = Matrix.CreateScale(0.01f) * Matrix.CreateTranslation(0f, 3.9f, 0f);
            TrukWorld = Matrix.CreateScale(0.8f) * Matrix.CreateTranslation(20f, 3.9f, 0f);

            

            World = Matrix.Identity;
            View = Matrix.CreateLookAt(new Vector3(0f, 25f, 100f), Vector3.Zero, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);

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

            BallModel = Content.Load<Model>(ContentFolder3D + "objetos/ball");
            TrukModel = Content.Load<Model>(ContentFolder3D + "objetos/truck");

            car = new Car(Content);
            car.LoadContent(Effect);

            VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 4, BufferUsage.None);
            var vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(150f, 0f, 150f), Color.Blue),
                new VertexPositionColor(new Vector3(150f, 0f, -150f), Color.Red),
                new VertexPositionColor(new Vector3(-150f, 0f, 150f), Color.Green),
                new VertexPositionColor(new Vector3(-150f, 0f, -150f), Color.Yellow),
            };
            VertexBuffer.SetData(vertices);

            IndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.None);
            var indices = new ushort[]{ 0, 1, 2, 1, 3, 2};

            IndexBuffer.SetData(indices);

            foreach( var mesh in BallModel.Meshes )
            {
                foreach( var meshPart in mesh.MeshParts )
                {
                    meshPart.Effect = Effect;
                }
            }

            foreach( var mesh in TrukModel.Meshes )
            {
                foreach( var meshPart in mesh.MeshParts )
                {
                    meshPart.Effect = Effect;
                }
            }

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
            
            car.Update(gameTime);
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
            /*
            foreach( var passes in Effect.CurrentTechnique.Passes )
            {
                passes.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }
            */

            /* -- */
            
            //CargarModelo(BallModel, BallWorld, 2);
            //CargarModelo(TrukModel, TrukWorld, 3);

            car.Draw(gameTime, View,Projection);

        }


        private Color RandomColor(Random random)
        {
            return new Color((uint)random.Next());
        }

        private void CargarModelo(Model _modelo, Matrix _modelWorld, int _seed)
        {
            /*Dibujo el modelo del objeto */
            Random _random = new Random(_seed);
            var modelMeshesBaseTransforms = new Matrix[_modelo.Bones.Count];
            _modelo.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach( var mesh in _modelo.Meshes )
            {
                var relativeTransform = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                Effect.Parameters["World"].SetValue(relativeTransform * _modelWorld);
                Effect.Parameters["DiffuseColor"].SetValue(RandomColor(_random).ToVector3());

                mesh.Draw();
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