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
using TGC.MonoGame.TP.ObstaculoPiedras;
using TGC.MonoGame.TP.ObstaculoPozo;
using TGC.MonoGame.TP.CheckPoint;
using TGC.MonoGame.TP.Modelos;


namespace TGC.MonoGame.TP.MaterialesJuego
{
    public class Materiales
    {
        public PistasCurvasIzquierdas _pistasCurvasIzquierdas { get; set; }
        public PistasCurvasDerechas _pistasCurvasDerechas { get; set; }
        public PistasRectas _pistasRectas { get; set; }
        public PowerUpHamburguesas _hamburguesas { get; set; }
        public CheckPoints _checkPoints { get; set; }
        public ObstaculosPiedras _piedras { get; set; }
        public ObstaculosPozos _pozos { get; set; }

        public Materiales(ContentManager Content)
        {

            _pistasCurvasDerechas = new PistasCurvasDerechas();
            _pistasCurvasIzquierdas = new PistasCurvasIzquierdas();
            _pistasRectas = new PistasRectas();
            _hamburguesas = new PowerUpHamburguesas();
            _piedras = new ObstaculosPiedras();
            _pozos = new ObstaculosPozos();
            _checkPoints = new CheckPoints();

            Initialize(Content);
        }

        private void Initialize(ContentManager Content)
        {
            _pistasCurvasIzquierdas.LoadContent(Content);
            _pistasCurvasDerechas.LoadContent(Content);
            _pistasRectas.LoadContent(Content);
            _hamburguesas.LoadContent(Content);
            _piedras.LoadContent(Content);
            _pozos.LoadContent(Content);
            _checkPoints.LoadContent(Content);
        }


        public void Update(GameTime gameTime, TGCGame Game)
        {
            _pistasCurvasDerechas.Update(gameTime);
            _pistasCurvasIzquierdas.Update(gameTime);
            _pistasRectas.Update(gameTime);

            _hamburguesas.Update(gameTime, Game);
            _checkPoints.Update(gameTime, Game);

            _piedras.Update(gameTime, Game);
            _pozos.Update(gameTime, Game);
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            _pistasCurvasDerechas.Draw(gameTime, view, projection);
            _pistasCurvasIzquierdas.Draw(gameTime, view, projection);
            _pistasRectas.Draw(gameTime, view, projection);
            _hamburguesas.Draw(gameTime, view, projection);
            _piedras.Draw(gameTime, view, projection);
            _checkPoints.Draw(gameTime, view, projection);
            _pozos.Draw(gameTime, view, projection);

            /*
            foreach (var boundingBoxPista in CollidersPistaCurvaDerecha)
            {
                Gizmos.DrawCube((boundingBoxPista.Max + boundingBoxPista.Min) / 2f, boundingBoxPista.Max - boundingBoxPista.Min, Color.Green);
            }

            foreach (var boundingBoxPista in CollidersPistaCurvaIzquierda)
            {
                Gizmos.DrawCube((boundingBoxPista.Max + boundingBoxPista.Min) / 2f, boundingBoxPista.Max - boundingBoxPista.Min, Color.Green);
            }

            foreach (var boundingBoxPista in CollidersPistaRecta)
            {
                Gizmos.DrawCube((boundingBoxPista.Max + boundingBoxPista.Min) / 2f, boundingBoxPista.Max - boundingBoxPista.Min, Color.Green);
            }

            foreach (var boundingBoxPista in CollidersPiedras)
            {
                Gizmos.DrawCube((boundingBoxPista.Max + boundingBoxPista.Min) / 2f, boundingBoxPista.Max - boundingBoxPista.Min, Color.Green);
            }

            foreach (var boundingBoxPista in CollidersCheckpoints)
            {
                Gizmos.DrawCube((boundingBoxPista.Max + boundingBoxPista.Min) / 2f, boundingBoxPista.Max - boundingBoxPista.Min, Color.Green);
            }

            */           
        }

        public void ColliderEsfera(BoundingSphere boundingSphere){
            _hamburguesas._envolturaEsfera = boundingSphere;
            _piedras._envolturaEsfera = boundingSphere;
            _pozos._envolturaEsfera = boundingSphere;
            _checkPoints._envolturaEsfera = boundingSphere;
        }

        internal void DarCollidersEsfera(Modelos.Sphere esfera){
            List<BoundingBox> CollidersPistaRecta = _pistasRectas.Colliders;
            List<BoundingBox> CollidersPistaCurvaDerecha = _pistasCurvasDerechas.Colliders;
            List<BoundingBox> CollidersPistaCurvaIzquierda = _pistasCurvasIzquierdas.Colliders;
            List<BoundingBox> CollidersPiedras = _piedras.Colliders;
            List<BoundingBox> CollidersCheckpoints = _checkPoints.Colliders;

            esfera.Colliders.AddRange(CollidersPistaRecta);
            esfera.Colliders.AddRange(CollidersPistaCurvaDerecha);
            esfera.Colliders.AddRange(CollidersPistaCurvaIzquierda);

        }

    }
}
