﻿using System;
using System.Collections.Generic;
using BepuPhysics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Objects;
using TGC.MonoGame.TP.PistaCurva;
using TGC.MonoGame.TP.PistaRecta;
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
        private PistasCurvas _pistasCurvas { get; set; }
        private PistasRectas _pistasRectas { get; set; }

        private Vector3 posicionActual { get; set; }
        private SkyBox SkyBox { get; set; }
        float rotacionActual = 0f;

        public TGCGame(){
            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            

            _pistasCurvas = new PistasCurvas();
            _pistasRectas = new PistasRectas();
        }

        protected override void Initialize(){
            Camera = new FollowCamera(GraphicsDevice, new Vector3(0, 5, 15), Vector3.Zero, Vector3.Up);

            sphere = new TGC.MonoGame.TP.Objects.Sphere(new Vector3(0f,30f,0f));
            sphere.SphereCamera = Camera;
            Gizmos = new Gizmos.Gizmos();
            
            _pistasCurvas.LoadContent(Content);
            _pistasRectas.LoadContent(Content);
            posicionActual = new Vector3(0f, 0f, 0f);
            
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);
            AgregarPistaCurva(_pistasCurvas);
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaCurva(_pistasCurvas);//CAMBIAR POR UN METODO UNICO, PARCHE
            AgregarPistaRecta(_pistasRectas);//CAMBIAR POR UN METODO UNICO, PARCHE

            _pistasCurvas.IniciarColliders();
            _pistasRectas.IniciarColliders();
            
            sphere.Colliders = _pistasRectas.Colliders; //CombineColliders(_pistasCurvas.Colliders, _pistasRectas.Colliders);
            
            
            
            base.Initialize();
        }

        protected override void LoadContent(){
            sphere.LoadContent(Content);

            var skyBox = Content.Load<Model>("Models/skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect,500);
            Gizmos.LoadContent(GraphicsDevice, Content);
            

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime){
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape)){
                Exit();
            }

            sphere.Update(gameTime);
            _pistasCurvas.Update(gameTime);
            _pistasRectas.Update(gameTime);

            Gizmos.UpdateViewProjection(Camera.ViewMatrix, Camera.ProjectionMatrix);
            Camera.Update(sphere.SpherePosition);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime){

            
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None
            };
            GraphicsDevice.RasterizerState = rasterizerState;

            SkyBox.Draw(Camera.ViewMatrix, Camera.ProjectionMatrix, Camera.position);
            sphere.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);
            
            _pistasCurvas.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);
            _pistasRectas.Draw(gameTime, Camera.ViewMatrix, Camera.ProjectionMatrix);

            GraphicsDevice.RasterizerState = originalRasterizerState;

            foreach (var posicion in Esferas){
                Gizmos.DrawSphere(posicion, Vector3.One * 100, Color.Yellow);
            }

            
            Gizmos.Draw();
        }

        protected override void UnloadContent(){
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
        private List<Vector3>Esferas = new ();

        void AgregarPistaRecta(PistasRectas unaPista) {
            Vector3 desplazamiento = unaPista.Desplazamiento();
            float rotacion = unaPista.Rotacion();
            unaPista.agregarNuevaPista(rotacionActual, posicionActual);
            Console.WriteLine($"Pista Recta dibujada: Posicion en ejes: X = {posicionActual.X}, Y = {posicionActual.Y}, Z = {posicionActual.Z}");

            rotacionActual += rotacion; 
            posicionActual += Vector3.Transform(desplazamiento, Matrix.CreateRotationY(rotacionActual));
            Esferas.Add(posicionActual);
        }

        void AgregarPistaCurva(PistasCurvas unaPista) {
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

        /*
        private BoundingBox[] CombineColliders(BoundingBox[] curvas, BoundingBox[] rectas) {
            if (curvas == null) return rectas;
            if (rectas == null) return curvas;

            var combined = new BoundingBox[curvas.Length + rectas.Length];
            curvas.CopyTo(combined, 0);
            rectas.CopyTo(combined, curvas.Length);

            return combined;
        }
        */
    }

    
}