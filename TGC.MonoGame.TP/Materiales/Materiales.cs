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
            _pistasCurvasDerechas.Draw(gameTime, view, projection);
            _pistasCurvasIzquierdas.Draw(gameTime, view, projection);
            _pistasRectas.Draw(gameTime, view, projection);
            
            _piedras.Draw(gameTime, view, projection);
            _checkPoints.Draw(gameTime, view, projection);
            _marcadoresCheckPoints.Draw(gameTime, view, projection);

            _pozos.Draw(gameTime, view, projection);
            _muros.Draw(gameTime, view, projection);
            _carretillas.Draw(gameTime, view, projection);
            _hamburguesas.Draw(gameTime, view, projection, graphicsDevice);
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
