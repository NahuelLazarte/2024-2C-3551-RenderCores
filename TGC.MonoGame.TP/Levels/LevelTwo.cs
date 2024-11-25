

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Fondo;

using MonoGame.Framework;
using TGC.MonoGame.TP.Geometries;
using TGC.MonoGame.TP.Camera;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using TGC.MonoGame.TP.MaterialesJuego;

namespace TGC.MonoGame.TP.Levels
{
    public class LevelTwo : Level
    {   
        private Matrix rotation = Matrix.Identity;
        private Sphere esfera;
        private Materiales _materiales { get; set; }
        //private ConstructorMateriales _constructorMateriales;
        //private LineDrawer lineDrawer;
        //private Gizmos.Gizmos Gizmos;
        private SkyBox SkyBox;
        private FollowCamera Camera { get; set; }


        BoundingFrustum _frustum { get; set; }

        private FreeCamera TestCamera;
        //private BoundingFrustum _frustum;
        private CubePrimitive LightBox;
        private Matrix LightBoxWorld = Matrix.Identity;

        public LevelTwo(GraphicsDevice graphicsDevice, ContentManager content)
            : base(graphicsDevice, content)
        {
        }

        public override void Initialize()
        {

            // Inicializar Esfera
            esfera = new Sphere(new Vector3(0.0f, 10.0f, 0.0f), rotation, new Vector3(0.5f, 0.5f, 0.5f));

            Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);

        }

        public override void LoadContent()
        {
            //Skybox
            var skyBox = Content.Load<Model>("Models/skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(TGCGame.ContentFolderTextures + "/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(TGCGame.ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 500);

            esfera.LoadContent(Content, GraphicsDevice);
            /*
            Gizmos.LoadContent(GraphicsDevice, Content);
            esfera.LoadContent(Content, GraphicsDevice);*/

            //Luz
            /*
            LightBox = new CubePrimitive(GraphicsDevice, 1f, Color.White);
            SetLightPosition(Vector3.Up * 45f);*/
        }
        /*
        private void SetLightPosition(Vector3 position)
        {
            LightBoxWorld = Matrix.CreateScale(3f) * Matrix.CreateTranslation(position);
            esfera.Effect.Parameters["lightPosition"].SetValue(position);
        }*/

        public override bool reachedLastCheckpoint(){
            if (_materiales._checkPoints.Colliders.Count == 0){
                return true;
            } else {
                return false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            /*
            esfera.Update(gameTime, Content);
            esfera.setDirection(FrustrumCamera.GetDirection());
            Gizmos.UpdateViewProjection(TestCamera.ViewMatrix, TestCamera.ProjectionMatrix);
            BoundingSphere boundingSphere = esfera.GetBoundingSphere();
            _materiales.ColliderEsfera(boundingSphere);*/

            Camera.Update(esfera.GetPosition());

            esfera.Update(gameTime, Content);
            esfera.setDirection(Camera.GetDirection());
        }

        public override void Draw(GameTime gameTime)
        {
            SkyBox.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix, Camera.position);
            //_materiales.Draw(gameTime, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, GraphicsDevice);
            //esfera.Draw(FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix, FrustrumCamera.position);
            /*LightBox.Draw(LightBoxWorld, FrustrumCamera.ViewMatrix, FrustrumCamera.ProjectionMatrix);
            Gizmos.DrawSphere(esfera.GetBoundingSphere().Center, esfera.GetBoundingSphere().Radius * Vector3.One, Color.White);
            Gizmos.DrawFrustum(FrustrumCamera.ViewMatrix * FrustrumCamera.ProjectionMatrix, Color.Aqua);
            Gizmos.Draw();*/

            //esfera.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix,Camera.position);
        }

        public override void UnloadContent()
        {
            Content.Unload();
        }
    }
}