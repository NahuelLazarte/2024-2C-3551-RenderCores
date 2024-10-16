﻿using System;
using System.Collections.Generic;
using BepuPhysics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Objects;

using TGC.MonoGame.TP.PistaCurvaDerecha;
using TGC.MonoGame.TP.PistaCurvaIzquierda;
using TGC.MonoGame.TP.PistaRecta;

using TGC.MonoGame.TP.PowerUpPez;

using TGC.MonoGame.TP.ObstaculoPiedras;

using TGC.MonoGame.TP.CheckPoint;

using TGC.MonoGame.TP.Modelos;

using TGC.MonoGame.TP.Fondo;


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
        private PistasCurvasIzquierdas _pistasCurvasIzquierdas { get; set; }
        private PistasCurvasDerechas _pistasCurvasDerechas { get; set; }
        private PistasRectas _pistasRectas { get; set; }
        private PowerUpPeces _peces { get; set; }
        private CheckPoints _checkPoints { get; set; }
        private ObstaculosPiedras _piedras { get; set; }
        private Vector3 posicionActual { get; set; }
        private SkyBox SkyBox { get; set; }
        float rotacionActual = 0f;

        Vector3 posicionCheckPoint;
        Modelos.Sphere esfera;
        LineDrawer lineDrawer;

        Pista pistaPrueba;

        public TGCGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _pistasCurvasDerechas = new PistasCurvasDerechas();
            _pistasCurvasIzquierdas = new PistasCurvasIzquierdas();
            _pistasRectas = new PistasRectas();

            _peces = new PowerUpPeces();

            _piedras = new ObstaculosPiedras();

            _checkPoints = new CheckPoints();
        }

        protected override void Initialize()
        {
            Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);

            sphere = new TGC.MonoGame.TP.Objects.Sphere(new Vector3(0f, 30f, 0f));
            sphere.SphereCamera = Camera;
            Gizmos = new Gizmos.Gizmos();

            _pistasCurvasIzquierdas.LoadContent(Content);
            _pistasCurvasDerechas.LoadContent(Content);
            _pistasRectas.LoadContent(Content);

            _peces.LoadContent(Content);

            _piedras.LoadContent(Content);

            _checkPoints.LoadContent(Content);

            posicionActual = new Vector3(0f, 0f, 0f);
            posicionCheckPoint = new Vector3(0f, 0f, 0f);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPowerUpPez(_peces);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);
            AgregarPowerUpPez(_peces);
            AgregarPistaCurvaDerecha(_pistasCurvasDerechas);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPowerUpPez(_peces);
            AgregarPistaCurvaDerecha(_pistasCurvasDerechas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaCurvaIzquierda(_pistasCurvasIzquierdas);
            AgregarObstaculoPiedra(_piedras);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarObstaculoPiedra(_piedras);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPowerUpPez(_peces);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarObstaculoPiedra(_piedras);
            AgregarCheckPoint(_checkPoints);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaCurvaDerecha(_pistasCurvasDerechas);
            AgregarPistaRecta(_pistasRectas);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPowerUpPez(_peces);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarObstaculoPiedra(_piedras);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPowerUpPez(_peces);
            AgregarPistaRecta(_pistasRectas);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaCurvaDerecha(_pistasCurvasDerechas);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPowerUpPez(_peces);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);
            _pistasCurvasIzquierdas.IniciarColliders();
            _pistasCurvasDerechas.IniciarColliders();
            _pistasRectas.IniciarColliders();

            _peces.IniciarColliders();

            _piedras.IniciarColliders();

            _checkPoints.IniciarColliders();

            //sphere.Colliders = CombineColliders(_pistasRectas.Colliders, _pistasCurvasDerechas.Colliders, _pistasCurvasIzquierdas.Colliders);

            // Crear una matriz de rotación con rotación 0
            Matrix rotation = Matrix.Identity;


            esfera = new Modelos.Sphere(new Vector3(0.0f, 4.0f, 0.0f), rotation, Color.Yellow);
            pistaPrueba = new Pista(new Vector3(50.0f, 4.0f, 0.0f), rotation, Color.White);

            lineDrawer = new LineDrawer(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            sphere.LoadContent(Content);

            var skyBox = Content.Load<Model>("Models/skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 500);

            Gizmos.LoadContent(GraphicsDevice, Content);//Gizmos.LoadContent(GraphicsDevice, new ContentManager(Content.ServiceProvider, "Content"));


            esfera.LoadContent(Content);
            pistaPrueba.LoadContent(Content);


            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //sphere.Update(gameTime);

            _pistasCurvasDerechas.Update(gameTime);
            _pistasCurvasIzquierdas.Update(gameTime);
            _pistasRectas.Update(gameTime);

            _peces.Update(gameTime, this);

            _piedras.Update(gameTime, this);

            _checkPoints.Update(gameTime, this);


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
            //sphere.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);

            //esfera.Draw(View,Projection);
            esfera.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix);
            pistaPrueba.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix);

            _pistasCurvasDerechas.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);
            _pistasCurvasIzquierdas.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);
            _pistasRectas.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);

            _peces.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);

            _piedras.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);

            _checkPoints.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);

            GraphicsDevice.RasterizerState = originalRasterizerState;

            foreach (var posicion in Esferas)
            {
                Gizmos.DrawSphere(posicion, Vector3.One * 100, Color.Yellow);
            }


            //Con esto se dibuja la caja alrededor de la pista
            BoundingBox boundingBox = pistaPrueba.GetBoundingBox();
            Gizmos.DrawCube((boundingBox.Max + boundingBox.Min) / 2f, boundingBox.Max - boundingBox.Min, Color.Green);


            List<BoundingBox> CollidersPistaRecta = _pistasRectas.Colliders;
            // Iterar sobre cada BoundingBox en la lista y dibujarla
            
            foreach (var boundingBoxPista in CollidersPistaRecta)
            {

                Gizmos.DrawCube((boundingBoxPista.Max + boundingBoxPista.Min) / 2f, boundingBoxPista.Max - boundingBoxPista.Min, Color.Green);
            }



            BoundingSphere boundingSphere = esfera.GetBoundingSphere();
            _peces._envolturaEsfera = boundingSphere;

            _piedras._envolturaEsfera = boundingSphere;

            _checkPoints._envolturaEsfera = boundingSphere;
            //BoundingSphere boundingSphere = new BoundingSphere(new Vector3(0, 0, 0), 5.0f);
            Gizmos.DrawSphere(boundingSphere.Center, boundingSphere.Radius * Vector3.One, Color.Green);

            Gizmos.Draw();




            Vector3 start = new Vector3(0, 0, 0);
            Vector3 endGreen = new Vector3(50, 0, 0);
            Vector3 endRed = new Vector3(0, 0, 50);

            lineDrawer.DrawLine(start, endGreen, Color.Green, Camera.ViewMatrix, Camera.ProjectionMatrix);
            lineDrawer.DrawLine(start, endRed, Color.Red, Camera.ViewMatrix, Camera.ProjectionMatrix);

        }

        protected override void UnloadContent()
        {
            Content.Unload();
            base.UnloadContent();
        }

        /*
        void AgregarPista<T>(T pista) where T : class
        {
            Vector3 desplazamiento = pista.Desplazamiento();
            float rotacion = pista.Rotacion();
            pista.agregarNuevaPista(rotacionActual, posicionActual);

            rotacionActual += rotacion;
            posicionActual += desplazamiento;
        }
        */
        private List<Vector3> Esferas = new();

        void AgregarPistaRecta(PistasRectas unaPista)
        {
            Vector3 desplazamiento = unaPista.Desplazamiento();
            float rotacion = unaPista.Rotacion();
            unaPista.agregarNuevaPista(rotacionActual, posicionActual);
            Console.WriteLine($"Pista Recta dibujada: Posicion en ejes: X = {posicionActual.X}, Y = {posicionActual.Y}, Z = {posicionActual.Z}");

            rotacionActual += rotacion;
            posicionActual += Vector3.Transform(desplazamiento, Matrix.CreateRotationY(rotacionActual));
            Esferas.Add(posicionActual);
        }

        void AgregarPistaCurvaDerecha(PistasCurvasDerechas unaPista)
        {
            Vector3 desplazamiento = unaPista.Desplazamiento();

            float rotacion = unaPista.Rotacion();

            //posicionActual = new Vector3(posicionActual.X +300f, posicionActual.Y, posicionActual.Z + 500f);

            //posicionActual = new Vector3(posicionActual.X, posicionActual.Y, posicionActual.Z);

            unaPista.agregarNuevaPista(rotacionActual, posicionActual);
            Console.WriteLine($"Pista Curva dibujada: Posicion en ejes: X = {posicionActual.X}, Y = {posicionActual.Y}, Z = {posicionActual.Z}");

            rotacionActual += rotacion;
            posicionActual += Vector3.Transform(desplazamiento, Matrix.CreateRotationY(rotacionActual));
            Esferas.Add(posicionActual);
        }

        void AgregarPistaCurvaIzquierda(PistasCurvasIzquierdas unaPista)
        {
            Vector3 desplazamiento = unaPista.Desplazamiento();

            float rotacion = unaPista.Rotacion();

            //posicionActual = new Vector3(posicionActual.X +300f, posicionActual.Y, posicionActual.Z + 500f);

            //posicionActual = new Vector3(posicionActual.X, posicionActual.Y, posicionActual.Z);

            unaPista.agregarNuevaPista(rotacionActual, posicionActual);
            Console.WriteLine($"Pista Curva dibujada: Posicion en ejes: X = {posicionActual.X}, Y = {posicionActual.Y}, Z = {posicionActual.Z}");

            rotacionActual += rotacion;
            posicionActual += Vector3.Transform(desplazamiento, Matrix.CreateRotationY(rotacionActual));
            Esferas.Add(posicionActual);
        }

        void AgregarPowerUpPez(PowerUpPeces unPowerUp)
        {
            Vector3 posicionObstaculo = new(posicionActual.X / 167f, posicionActual.Y / 180f + 0.5f, posicionActual.Z / 167f);
            unPowerUp.AgregarNuevoPowerUp(rotacionActual, posicionObstaculo);
            Console.WriteLine($"Obstaculo Pez dibujado: Posicion en ejes: X = {posicionObstaculo.X}, Y = {posicionObstaculo.Y}, Z = {posicionObstaculo.Z}");
            //Esferas.Add(posicionObstaculo);
        }

        void AgregarObstaculoPiedra(ObstaculosPiedras unObstaculo)
        {
            Vector3 posicionObstaculo = new(posicionActual.X / 38f, posicionActual.Y / 38f, posicionActual.Z / 33.5f);
            unObstaculo.AgregarNuevoObstaculo(rotacionActual, posicionObstaculo);
            Console.WriteLine($"Obstaculo Pez dibujado: Posicion en ejes: X = {posicionObstaculo.X}, Y = {posicionObstaculo.Y}, Z = {posicionObstaculo.Z}");
            //Esferas.Add(posicionObstaculo);
        }

        void AgregarCheckPoint(CheckPoints unCheckPoint)
        {
            Vector3 posicionObstaculo = new(posicionActual.X / 40f, posicionActual.Y / 40f, posicionActual.Z / 40f);
            unCheckPoint.AgregarNuevoCheckPoint(rotacionActual, posicionObstaculo);
            Console.WriteLine($"Obstaculo Pez dibujado: Posicion en ejes: X = {posicionObstaculo.X}, Y = {posicionObstaculo.Y}, Z = {posicionObstaculo.Z}");
            //Esferas.Add(posicionObstaculo);
        }

        public void Respawn()
        {
            esfera.RespawnAt(posicionCheckPoint);
            // Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);No funciona
        }

        public void nuevoCheckPoint(Vector3 posicion)
        {
            posicionCheckPoint = posicion;
            // Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);No funciona
        }


        public void recibirPowerUpPez()
        {
            esfera.aumentarVelocidad(1.5f);
        }

        private BoundingBox[] CombineColliders(BoundingBox[] rectas, BoundingBox[] curvasDerechas, BoundingBox[] curvasIzquierdas)
        {
            var combined = new BoundingBox[curvasDerechas.Length + curvasIzquierdas.Length + rectas.Length];

            rectas.CopyTo(combined, 0);
            curvasDerechas.CopyTo(combined, rectas.Length);
            curvasIzquierdas.CopyTo(combined, rectas.Length + curvasDerechas.Length);

            return combined;
        }

    }


}