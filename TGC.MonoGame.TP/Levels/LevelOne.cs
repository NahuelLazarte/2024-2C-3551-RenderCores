using System;
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
using TGC.MonoGame.TP.Camera;
using TGC.MonoGame.TP.Levels;


using Microsoft.Xna.Framework.Content;
namespace TGC.MonoGame.TP
{
    public class LevelOne : Level
    {

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


        //Mundo
        private Materiales _materiales { get; set; }

        LineDrawer lineDrawer;
        private Gizmos.Gizmos Gizmos;
        private SkyBox SkyBox { get; set; }

        //Camaras
        
        private FreeCamera TestCamera;
        BoundingFrustum _frustum { get; set; }

        //Luz
        private CubePrimitive LightBox { get; set; }
        private Matrix LightBoxWorld { get; set; } = Matrix.Identity;

        //Extras

        public bool isMenuActive = true;
        private SpriteBatch SpriteBatch { get; set; }
        private Song backgroundMusic;
        private bool isMusicPlaying = true;
        public bool isGodModeActive = false;

        //Enviroment Map
        private StaticCamera CubeMapCamera { get; set; }
        private RenderTargetCube EnvironmentMapRenderTarget { get; set; }
        private const int EnvironmentmapSize = 2048;

        public LevelOne(GraphicsDevice graphicsDevice, ContentManager content)
            : base(graphicsDevice, content)
        {
        }

        public override void Initialize()
        {

            //Inicializar Gizmos
            Gizmos = new Gizmos.Gizmos();
            Console.WriteLine("Gizmos initialized");
            lineDrawer = new LineDrawer(GraphicsDevice);
            Console.WriteLine("LineDrawer initialized");

            //Camara de Prueba
            TestCamera = new FreeCamera(new Vector3(0.0f, 10.0f, 0.0f), GraphicsDevice);
            Console.WriteLine("TestCamera initialized");

            //Camara con Frustrum
            FrustrumCamera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);
            Console.WriteLine("FrustrumCamera initialized");
            _frustum = new BoundingFrustum(FrustrumCamera.ViewMatrix * FrustrumCamera.ProjectionMatrix);
            Console.WriteLine("BoundingFrustum initialized");

            //Inicializar Esfera
            esfera = new Sphere(new Vector3(0.0f, 10.0f, 0.0f), rotation, new Vector3(0.5f, 0.5f, 0.5f));
            esfera.Game = this;

            //Inicializar Materiales
            _materiales = new Materiales(Content, GraphicsDevice, _frustum,FrustrumCamera.ViewMatrix,FrustrumCamera.ProjectionMatrix);
            Console.WriteLine("Materiales inicializados");
            _constructorMateriales = new ConstructorMateriales();
            _constructorMateriales.CargarElementos(_materiales);
            _materiales.DarCollidersEsfera(esfera);

            //base.Initialize();

            //Enviroment Map
            CubeMapCamera = new StaticCamera(1f, new Vector3(0, 0, 0), Vector3.UnitX, Vector3.Up);
            CubeMapCamera.BuildProjection(1f, 1f, 3000f, MathHelper.PiOver2);
        }

        public override void LoadContent()
        {
            //Skybox
            var skyBox = Content.Load<Model>("Models/skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 500);


            //Sonidos de Fondo
            backgroundMusic = Content.Load<Song>(ContentFolderMusic + "Sad Town");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;

            Gizmos.LoadContent(GraphicsDevice, Content);
            esfera.LoadContent(Content, GraphicsDevice);

            //Luz
            LightBox = new CubePrimitive(GraphicsDevice, 1f, Color.White);
            SetLightPosition(Vector3.Up * 45f);

            //base.LoadContent();
        }
        private void SetLightPosition(Vector3 position)
        {
            LightBoxWorld = Matrix.CreateScale(3f) * Matrix.CreateTranslation(position);
            esfera.Effect.Parameters["lightPosition"]?.SetValue(position);
            //_materiales._muros.Effect.Parameters["lightPosition"].SetValue(position);
        }

        public override bool reachedLastCheckpoint(){
            if (_materiales._checkPoints.Colliders.Count == 0){
                esfera.RespawnAt(new Vector3(0f,5f,0f));
                esfera.cambiarPosicionModelo();
                return true;
            } else {
                return false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (isMusicPlaying)
            {
                MediaPlayer.Play(backgroundMusic);
                isMusicPlaying = false;
            }
            /*
                        if (isMenuActive)
                        {
                            if (!(MediaPlayer.Volume == 0.3f)) MediaPlayer.Volume = 0.3f;

                            _materiales.Update(gameTime, this, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, _frustum);
                            menu.Update(this, gameTime);

                        }
                        else
                        {*/

            if (!(MediaPlayer.Volume == 0.1f)) MediaPlayer.Volume = 0.2f;
            /*
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                isMenuActive = true;
            }*/
            //esfera.isGodModeActive = isGodModeActive;
            if(leveIsActive){
                FrustrumCamera.Update(esfera.GetPosition());
                esfera.Update(gameTime, Content);
                esfera.setDirection(FrustrumCamera.GetDirection());
            }
            

            _frustum.Matrix = FrustrumCamera.ViewMatrix * FrustrumCamera.ProjectionMatrix;
            _materiales.Update(gameTime, this, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, _frustum, esfera);
            //Gizmos.UpdateViewProjection(Camera.ViewMatrix, Camera.ProjectionMatrix);
            Gizmos.UpdateViewProjection(TestCamera.ViewMatrix, TestCamera.ProjectionMatrix);


            BoundingSphere boundingSphere = esfera.GetBoundingSphere();
            _materiales.ColliderEsfera(boundingSphere);

            //}
            //base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            /*
            // Calcula el tiempo para girar la cámara
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float rotationSpeed = 0.3f; // Velocidad de rotación
            float radius = 100f; // Distancia de la cámara a la pelota*/

            /*
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

            }*/
            /*
            else
            {*/

            //ddd}  
            ObjectsToDraw(gameTime);
            
            //esfera.Draw(FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, FrustrumCamera.position);
            Gizmos.DrawSphere(esfera.GetBoundingSphere().Center, esfera.GetBoundingSphere().Radius * Vector3.One, Color.White);
            //DrawEnvironmentMap();
        }
        public void ObjectsToDraw(GameTime gameTime)
        {
            //Skybox
            //SkyBox.Draw(FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, FrustrumCamera.position);

            //Objetos
            _materiales.Draw(gameTime, esfera, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, GraphicsDevice, FrustrumCamera.position, SkyBox);
            

            Vector3 start = new Vector3(0, 0, 0);
            Vector3 endGreen = new Vector3(50, 0, 0);
            Vector3 endRed = new Vector3(0, 0, 50);

            //lineDrawer.DrawLine(start, endGreen, Color.Green, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix);
            //lineDrawer.DrawLine(start, endRed, Color.Red, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix);

            LightBox.Draw(LightBoxWorld, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix);

            //Gizmos
            Gizmos.DrawFrustum(FrustrumCamera.ViewMatrix * FrustrumCamera.ProjectionMatrix, Color.Aqua);
            Gizmos.Draw();
        }
        public override void UnloadContent()
        {
            Content.Unload();
            //base.UnloadContent();

            EnvironmentMapRenderTarget.Dispose();
        }




    }
}