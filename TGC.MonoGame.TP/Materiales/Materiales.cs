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
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.MurosExtra;
using TGC.MonoGame.TP.ObstaculoCarretilla;

namespace TGC.MonoGame.TP.MaterialesJuego
{
    public class Materiales
    {
        private Gizmos.Gizmos Gizmos;
        LineDrawer lineDrawer;

        public PistasCurvasIzquierdas _pistasCurvasIzquierdas { get; set; }
        public PistasCurvasDerechas _pistasCurvasDerechas { get; set; }
        public PistasRectas _pistasRectas { get; set; }
        public PowerUpHamburguesas _hamburguesas { get; set; }
        public CheckPoints _checkPoints { get; set; }
        public ObstaculosPiedras _piedras { get; set; }
        public ObstaculosPozos _pozos { get; set; }
        public ObstaculosCarretillas _carretillas { get; set; }
        public PowerUpEspadas _espadas { get; set; }

        public Muros _muros { get; set; }

        private List<BoundingBox> CollidersDibujo { get; set; }



        public Materiales(ContentManager Content, GraphicsDevice graphicsDevice)
        {

            _pistasCurvasDerechas = new PistasCurvasDerechas();
            _pistasCurvasIzquierdas = new PistasCurvasIzquierdas();
            _pistasRectas = new PistasRectas();
            _hamburguesas = new PowerUpHamburguesas();
            _espadas = new PowerUpEspadas();
            _piedras = new ObstaculosPiedras();
            _pozos = new ObstaculosPozos();
            _checkPoints = new CheckPoints();
            _muros = new Muros();
            _carretillas = new ObstaculosCarretillas();

            CollidersDibujo = new List<BoundingBox>();

            Initialize(Content, graphicsDevice);
        }

        private void Initialize(ContentManager Content, GraphicsDevice graphicsDevice)
        {
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
            _muros.LoadContent(Content, graphicsDevice);
            _carretillas.LoadContent(Content);
        }


        public void Update(GameTime gameTime, TGCGame Game, Matrix view, Matrix projection)
        {
            
            _pistasCurvasDerechas.Update(gameTime);
            _pistasCurvasIzquierdas.Update(gameTime);
            _pistasRectas.Update(gameTime);
            _espadas.Update(gameTime, Game);
            _hamburguesas.Update(gameTime, Game);
            _checkPoints.Update(gameTime, Game);
            _piedras.Update(gameTime, Game);
            _pozos.Update(gameTime, Game);
            _muros.Update(gameTime, Game);
            _carretillas.Update(gameTime, Game);

            Gizmos.UpdateViewProjection(view, projection);
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection, GraphicsDevice graphicsDevice )
        {
            _pistasCurvasDerechas.Draw(gameTime, view, projection);
            _pistasCurvasIzquierdas.Draw(gameTime, view, projection);
            _pistasRectas.Draw(gameTime, view, projection);
            
            _piedras.Draw(gameTime, view, projection);
            _checkPoints.Draw(gameTime, view, projection);
            _pozos.Draw(gameTime, view, projection);
            _muros.Draw(gameTime, view, projection);
            _carretillas.Draw(gameTime, view, projection);
            _hamburguesas.Draw(gameTime, view, projection, graphicsDevice);
            _espadas.Draw(gameTime, view, projection, graphicsDevice);
            /*
            foreach (var boundingBoxPista in CollidersDibujo)
            {
                Gizmos.DrawCube((boundingBoxPista.Max + boundingBoxPista.Min) / 2f, boundingBoxPista.Max - boundingBoxPista.Min, Color.Green);
            }

             Gizmos.Draw(); // PARA DIBUJAR LOS CUBOS DE GIZMOS
            */
        }

        public void ColliderEsfera(BoundingSphere boundingSphere){
            _hamburguesas._envolturaEsfera = boundingSphere;
            _espadas._envolturaEsfera = boundingSphere;
            _piedras._envolturaEsfera = boundingSphere;
            _pozos._envolturaEsfera = boundingSphere;
            _muros._envolturaEsfera = boundingSphere;
            _checkPoints._envolturaEsfera = boundingSphere;
            _carretillas._envolturaEsfera = boundingSphere;
        }

        internal void DarCollidersEsfera(Modelos.Sphere esfera){
            List<BoundingBox> CollidersPistaRecta = _pistasRectas.Colliders;
            List<BoundingBox> CollidersPistaCurvaDerecha = _pistasCurvasDerechas.Colliders;
            List<BoundingBox> CollidersPistaCurvaIzquierda = _pistasCurvasIzquierdas.Colliders;
            List<BoundingBox> CollidersPiedras = _piedras.Colliders;
            List<BoundingBox> CollidersCheckpoints = _checkPoints.Colliders;
            CollidersDibujo = _muros.Colliders;

            esfera.Colliders.AddRange(CollidersPistaRecta);
            esfera.Colliders.AddRange(CollidersPistaCurvaDerecha);
            esfera.Colliders.AddRange(CollidersPistaCurvaIzquierda);

        }

    }
}
