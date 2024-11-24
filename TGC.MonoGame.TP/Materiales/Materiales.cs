using System;
using System.Collections.Generic;
using BepuPhysics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.PistaCurvaDerecha;
using TGC.MonoGame.TP.PistaCurvaIzquierda;
using TGC.MonoGame.TP.PistaRecta;
using TGC.MonoGame.TP.PowerUpHamburguesa;
using TGC.MonoGame.TP.PowerUpEspada;

using TGC.MonoGame.TP.ObstaculoPiedras;
using TGC.MonoGame.TP.ObstaculoPozo;
using TGC.MonoGame.TP.CheckPoint;
using TGC.MonoGame.TP.MarcadorCheckPoint;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.MurosExtra;
using TGC.MonoGame.TP.ObstaculoCarretilla;
using TGC.MonoGame.TP.Levels;
using TGC.MonoGame.TP.Camera;
using TGC.MonoGame.TP.Geometries;

namespace TGC.MonoGame.TP.MaterialesJuego {
    public class Materiales {
        private Gizmos.Gizmos Gizmos;
        LineDrawer lineDrawer;

        public PistasCurvasIzquierdas _pistasCurvasIzquierdas { get; set; }
        public PistasCurvasDerechas _pistasCurvasDerechas { get; set; }
        public PistasRectas _pistasRectas { get; set; }
        public PowerUpHamburguesas _hamburguesas { get; set; }
        public CheckPoints _checkPoints { get; set; }
        public MarcadoresCheckPoints _marcadoresCheckPoints { get; set; }
        public ObstaculosPiedras _piedras { get; set; }
        public ObstaculosPozos _pozos { get; set; }
        public ObstaculosCarretillas _carretillas { get; set; }
        public PowerUpEspadas _espadas { get; set; }
        public Muros _muros { get; set; }
        private List<BoundingBox> CollidersDibujo { get; set; }
        private const int ShadowmapSize = 2048;
        private readonly float LightCameraFarPlaneDistance = 3000f;
        private readonly float LightCameraNearPlaneDistance = 5f;
        private CubePrimitive LightBox;
        private Vector3 LightPosition = Vector3.One * 500f;
        private RenderTarget2D ShadowMapRenderTarget;
        private TargetCamera TargetLightCamera { get; set; }
        private Effect ShadowMapEffect { get; set; }

        public Materiales(ContentManager Content, GraphicsDevice graphicsDevice, BoundingFrustum frustrum, Matrix view, Matrix projection) {
            _pistasCurvasDerechas = new PistasCurvasDerechas(view,projection);
            _pistasCurvasIzquierdas = new PistasCurvasIzquierdas(view,projection);
            _pistasRectas = new PistasRectas(view,projection);
            _hamburguesas = new PowerUpHamburguesas(view,projection);
            _espadas = new PowerUpEspadas(view,projection);
            _piedras = new ObstaculosPiedras(view,projection);
            _pozos = new ObstaculosPozos();
            _checkPoints = new CheckPoints(view,projection);
            _marcadoresCheckPoints = new MarcadoresCheckPoints(view,projection);
            _muros = new Muros(frustrum);
            _carretillas = new ObstaculosCarretillas(view,projection);

            CollidersDibujo = new List<BoundingBox>();

            Initialize(Content, graphicsDevice);
        }

        private void Initialize(ContentManager Content, GraphicsDevice graphicsDevice) {
            
            TargetLightCamera = new TargetCamera(1f, LightPosition, Vector3.Zero, graphicsDevice.Viewport);
            TargetLightCamera.BuildProjection(1f, LightCameraNearPlaneDistance, LightCameraFarPlaneDistance,
            MathHelper.PiOver2);        

            ShadowMapEffect = Content.Load<Effect>("Effects/ShadowMap");
            
            // Create a shadow map. It stores depth from the light position
            ShadowMapRenderTarget = new RenderTarget2D(graphicsDevice, ShadowmapSize, ShadowmapSize, false,
            SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            LightBox = new CubePrimitive(graphicsDevice, 5, Color.White);

            graphicsDevice.BlendState = BlendState.Opaque;


            Gizmos = new Gizmos.Gizmos();
            lineDrawer = new LineDrawer(graphicsDevice);
            Gizmos.LoadContent(graphicsDevice, Content);

            _pistasCurvasIzquierdas.LoadContent(Content);
            _pistasCurvasDerechas.LoadContent(Content);
            _pistasRectas.LoadContent(Content);
            _hamburguesas.LoadContent(Content, graphicsDevice);
            _espadas.LoadContent(Content, graphicsDevice);
            _piedras.LoadContent(Content);
            _pozos.LoadContent(Content);
            _checkPoints.LoadContent(Content);
            _marcadoresCheckPoints.LoadContent(Content);
            _muros.LoadContent(Content, graphicsDevice);
            _carretillas.LoadContent(Content);
        }


        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection, BoundingFrustum frustum)
        {
            
            _pistasCurvasDerechas.Update(gameTime, view, projection);
            _pistasCurvasIzquierdas.Update(gameTime, view, projection);
            _pistasRectas.Update(gameTime, view, projection);
            _espadas.Update(gameTime, Game, view, projection);
            _hamburguesas.Update(gameTime, Game, view, projection);
            _checkPoints.Update(gameTime, Game, view, projection);
            _marcadoresCheckPoints.Update(gameTime, Game, view, projection);
            _piedras.Update(gameTime, Game, view, projection);
            _pozos.Update(gameTime, Game, view, projection);
            _muros.Update(gameTime, Game, view, projection, frustum);
            _carretillas.Update(gameTime, Game, view, projection);

            Gizmos.UpdateViewProjection(view, projection);
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection, GraphicsDevice graphicsDevice )
        {

            #region Pass 1: Renderizar el Shadow Map

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SetRenderTarget(ShadowMapRenderTarget);
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            ShadowMapEffect.CurrentTechnique = ShadowMapEffect.Techniques["DepthPass"];

            
            _piedras.ShadowMapRender(ShadowMapEffect, TargetLightCamera.View, TargetLightCamera.Projection);
            _pistasRectas.ShadowMapRender(ShadowMapEffect, TargetLightCamera.View, TargetLightCamera.Projection);
            _pistasCurvasIzquierdas.ShadowMapRender(ShadowMapEffect, TargetLightCamera.View, TargetLightCamera.Projection);
            _pistasCurvasDerechas.ShadowMapRender(ShadowMapEffect, TargetLightCamera.View, TargetLightCamera.Projection);
            _muros.ShadowMapRender(ShadowMapEffect, TargetLightCamera.View, TargetLightCamera.Projection);
            _pozos.ShadowMapRender(ShadowMapEffect, TargetLightCamera.View, TargetLightCamera.Projection);
            _carretillas.ShadowMapRender(ShadowMapEffect, TargetLightCamera.View, TargetLightCamera.Projection);

            #endregion

            #region Pass 2: Renderizar la escena con sombras
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);
            
            var shadowMapSizeA = Vector2.One * ShadowmapSize;
            var ligtViewProj = TargetLightCamera.View * TargetLightCamera.Projection;

            ShadowMapEffect.CurrentTechnique = ShadowMapEffect.Techniques["DrawShadowedPCF"];
            ShadowMapEffect.Parameters["shadowMap"].SetValue(ShadowMapRenderTarget);
            ShadowMapEffect.Parameters["lightPosition"].SetValue(LightPosition);
            
            ShadowMapEffect.Parameters["shadowMapSize"].SetValue(shadowMapSizeA);
            ShadowMapEffect.Parameters["LightViewProjection"].SetValue(ligtViewProj);

            _piedras.Draw(gameTime, ShadowMapEffect, view, projection);
            _pistasRectas.Draw(gameTime, ShadowMapEffect, view, projection);
            _pistasCurvasIzquierdas.Draw(gameTime, ShadowMapEffect, view, projection);
            _pistasCurvasDerechas.Draw(gameTime, ShadowMapEffect, view, projection);
            _muros.Draw(gameTime, ShadowMapEffect, view, projection);
            _pozos.Draw(gameTime, ShadowMapEffect, view, projection);
            _carretillas.Draw(gameTime, ShadowMapEffect, view, projection);


            // _pistasCurvasIzquierdas.Draw(gameTime, view, projection);
            //_pistasRectas.Draw(gameTime, view, projection);

            LightBox.Draw(Matrix.CreateTranslation(LightPosition), TargetLightCamera.View, TargetLightCamera.Projection);

            #endregion


            _hamburguesas.Draw(gameTime, view, projection, graphicsDevice);

            _checkPoints.Draw(gameTime, view, projection);
            _marcadoresCheckPoints.Draw(gameTime, view, projection);
            _espadas.Draw(gameTime, view, projection, graphicsDevice);

            foreach (var boundingBoxPista in CollidersDibujo)
            {
                Gizmos.DrawCube((boundingBoxPista.Max + boundingBoxPista.Min) / 2f, boundingBoxPista.Max - boundingBoxPista.Min, Color.Green);
            }
            Gizmos.Draw(); // PARA DIBUJAR LOS CUBOS DE GIZMOS
            
        }

        public void ColliderEsfera(BoundingSphere boundingSphere){
            //Se le pasa el BoungingSphere de la esfera
            _hamburguesas._envolturaEsfera = boundingSphere;
            _espadas._envolturaEsfera = boundingSphere;
            _piedras._envolturaEsfera = boundingSphere;
            _pozos._envolturaEsfera = boundingSphere;
            _muros._envolturaEsfera = boundingSphere;
            _checkPoints._envolturaEsfera = boundingSphere;
            _carretillas._envolturaEsfera = boundingSphere;
        }

        internal void DarCollidersEsfera(Modelos.Sphere esfera){
            //a la esfera se agrega a la lista los colliders de todos los objetos
            List<BoundingBox> CollidersPistaRecta = _pistasRectas.Colliders;
            List<BoundingBox> CollidersPistaCurvaDerecha = _pistasCurvasDerechas.Colliders;
            List<BoundingBox> CollidersPistaCurvaIzquierda = _pistasCurvasIzquierdas.Colliders;
            List<BoundingBox> CollidersPiedras = _piedras.Colliders;
            List<BoundingBox> CollidersCheckpoints = _checkPoints.Colliders;
            List<BoundingBox> CollidersMarcadoresCheckpoints = _marcadoresCheckPoints.Colliders;
            CollidersDibujo = CollidersMarcadoresCheckpoints;

            esfera.Colliders.AddRange(CollidersPistaRecta);
            esfera.Colliders.AddRange(CollidersPistaCurvaDerecha);
            esfera.Colliders.AddRange(CollidersPistaCurvaIzquierda);

        }

    }
}
