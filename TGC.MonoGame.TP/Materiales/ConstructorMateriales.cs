using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; 

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;
using TGC.MonoGame.TP.MaterialesJuego;
using TGC.MonoGame.TP.PistaCurvaDerecha;
using TGC.MonoGame.TP.PistaCurvaIzquierda;
using TGC.MonoGame.TP.PistaRecta;
using TGC.MonoGame.TP.PowerUpHamburguesa;
using TGC.MonoGame.TP.ObstaculoPiedras;
using TGC.MonoGame.TP.ObstaculoPozo;
using TGC.MonoGame.TP.CheckPoint;
using TGC.MonoGame.TP.MarcadorCheckPoint;
using TGC.MonoGame.TP.ObstaculoCarretilla;
using TGC.MonoGame.TP.PowerUpEspada;
using TGC.MonoGame.TP.MurosExtra;
using System; 

namespace TGC.MonoGame.TP.Constructor{
    public class ConstructorMateriales{

        private Vector3 posicionActual { get; set; }
        public Vector3 posicionCheckPoint;
        float rotacionActual = 0f;


        public ConstructorMateriales(){
            Initialize();
        }
        private void Initialize()
        {
            posicionActual = new Vector3(0f, 0f, 0f);
            posicionCheckPoint = new Vector3(0f, 4f, 0f);

        }

        public void CargarElementos(Materiales _materiales) {


            AgregarPistaRecta(_materiales._pistasRectas, _materiales);

            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);

            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaCurvaDerecha(_materiales._pistasCurvasDerechas, _materiales);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarObstaculoPiedra(_materiales._piedras);

            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarObstaculoPozo(_materiales._pozos, _materiales);
            //AgregarObstaculoPozo(_materiales._pozos, _materiales);

            AgregarObstaculoCarretilla(_materiales._carretillas);
            
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarObstaculoCarretilla(_materiales._carretillas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            //AgregarObstaculoPozo(_materiales._pozos, _materiales);

            //AgregarObstaculoPozo(_materiales._pozos, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPowerUpHamburguesa(_materiales._hamburguesas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales); 
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPowerUpHamburguesa(_materiales._hamburguesas);
            AgregarPistaCurvaDerecha(_materiales._pistasCurvasDerechas, _materiales);

            //AgregarObstaculoPozo(_materiales._pozos, _materiales);

            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarObstaculoCarretilla(_materiales._carretillas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPowerUpHamburguesa(_materiales._hamburguesas);
            AgregarPistaCurvaDerecha(_materiales._pistasCurvasDerechas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarObstaculoCarretilla(_materiales._carretillas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaCurvaIzquierda(_materiales._pistasCurvasIzquierdas, _materiales);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarObstaculoCarretilla(_materiales._carretillas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPowerUpHamburguesa(_materiales._hamburguesas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            
            AgregarCheckPoint(_materiales._checkPoints, _materiales._marcadoresCheckPoints);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            
            AgregarPistaCurvaDerecha(_materiales._pistasCurvasDerechas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPowerUpHamburguesa(_materiales._hamburguesas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarObstaculoPiedra(_materiales._piedras);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
             
            AgregarPowerUpHamburguesa(_materiales._hamburguesas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaCurvaDerecha(_materiales._pistasCurvasDerechas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPowerUpHamburguesa(_materiales._hamburguesas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            //AgregarObstaculoCarretilla(_materiales._carretillas);
            AgregarObstaculoCarretilla(_materiales._carretillas);
            AgregarPistaRecta(_materiales._pistasRectas, _materiales);
            AgregarPistaCurvaIzquierda(_materiales._pistasCurvasIzquierdas, _materiales);

        }

        void AgregarPistaRecta(PistasRectas unaPista, Materiales _materialesAux)
        {
            Vector3 desplazamiento = unaPista.Desplazamiento();
            float rotacion = unaPista.Rotacion();
            unaPista.agregarNuevaPista(rotacionActual, posicionActual, _materialesAux);
            //Console.WriteLine($"Pista Recta dibujada: Posicion en ejes: X = {posicionActual.X}, Y = {posicionActual.Y}, Z = {posicionActual.Z}");
            rotacionActual += rotacion;
            posicionActual += Vector3.Transform(desplazamiento, Matrix.CreateRotationY(rotacionActual));
            //Esferas.Add(posicionActual);
        }

        void AgregarPistaCurvaDerecha(PistasCurvasDerechas unaPista,  Materiales _materialesAux)
        {
            Vector3 desplazamiento = unaPista.Desplazamiento();

            float rotacion = unaPista.Rotacion();

            unaPista.agregarNuevaPista(rotacionActual, posicionActual, _materialesAux);
            //Console.WriteLine($"Pista Curva dibujada: Posicion en ejes: X = {posicionActual.X}, Y = {posicionActual.Y}, Z = {posicionActual.Z}");

            rotacionActual += rotacion;
            posicionActual += Vector3.Transform(desplazamiento, Matrix.CreateRotationY(rotacionActual));
            //Esferas.Add(posicionActual);
        }

        void AgregarPistaCurvaIzquierda(PistasCurvasIzquierdas unaPista,  Materiales _materialesAux){
            Vector3 desplazamiento = unaPista.Desplazamiento();

            float rotacion = unaPista.Rotacion();

            unaPista.agregarNuevaPista(rotacionActual, posicionActual, _materialesAux);
            //Console.WriteLine($"Pista Curva dibujada: Posicion en ejes: X = {posicionActual.X}, Y = {posicionActual.Y}, Z = {posicionActual.Z}");

            rotacionActual += rotacion;
            posicionActual += Vector3.Transform(desplazamiento, Matrix.CreateRotationY(rotacionActual));
            //Esferas.Add(posicionActual);
        }

        void AgregarObstaculoPozo(ObstaculosPozos unPozo,  Materiales _materialesAux)
        {
            Vector3 desplazamiento = unPozo.Desplazamiento() * 62.55f;

            //Vector3 posicionObstaculo = new(posicionActual.X, posicionActual.Y, posicionActual.Z);

            unPozo.agregarNuevoPozo(rotacionActual, posicionActual, _materialesAux);
            //Console.WriteLine($"Pista Curva dibujada: Posicion en ejes: X = {posicionActual.X}, Y = {posicionActual.Y}, Z = {posicionActual.Z}");

            posicionActual += Vector3.Transform(desplazamiento, Matrix.CreateRotationY(rotacionActual));
            //Esferas.Add(posicionActual);
        }
        void AgregarPowerUpHamburguesa(PowerUpHamburguesas unPowerUp)
        {
            Vector3 posicionObstaculo = new(posicionActual.X / 167f, posicionActual.Y / 180f + 0.5f, posicionActual.Z / 167f);
            unPowerUp.AgregarNuevoPowerUp(rotacionActual, posicionObstaculo);
            Console.WriteLine($"Obstaculo Hamburguesa dibujado: Posicion en ejes: X = {posicionObstaculo.X}, Y = {posicionObstaculo.Y}, Z = {posicionObstaculo.Z}");
            
        }

        void AgregarPowerUpEspada(PowerUpEspadas unPowerUp)
        {
            Vector3 posicionObstaculo = posicionActual;
            unPowerUp.AgregarNuevoPowerUp(rotacionActual, posicionObstaculo);
            //Console.WriteLine($"Obstaculo Pez dibujado: Posicion en ejes: X = {posicionObstaculo.X}, Y = {posicionObstaculo.Y}, Z = {posicionObstaculo.Z}"); 
        }
        
        void AgregarObstaculoPiedra(ObstaculosPiedras unObstaculo)
        {
            Vector3 posicionObstaculo = new(posicionActual.X / 33.5f, posicionActual.Y / 38f, posicionActual.Z / 33.5f);
            unObstaculo.AgregarNuevoObstaculo(rotacionActual, posicionObstaculo);
            //Console.WriteLine($"Obstaculo Pez dibujado: Posicion en ejes: X = {posicionObstaculo.X}, Y = {posicionObstaculo.Y}, Z = {posicionObstaculo.Z}");
            
        }

        void AgregarCheckPoint(CheckPoints unCheckPoint, MarcadoresCheckPoints unMarcador)
        {
            Vector3 posicionObstaculo = new(posicionActual.X / 170f, posicionActual.Y / 185f, posicionActual.Z / 170f);
            unCheckPoint.AgregarNuevoCheckPoint(rotacionActual, posicionObstaculo);
            //Console.WriteLine($"Obstaculo Pez dibujado: Posicion en ejes: X = {posicionObstaculo.X}, Y = {posicionObstaculo.Y}, Z = {posicionObstaculo.Z}");
            posicionObstaculo = new(posicionActual.X / 170f, posicionActual.Y / 185f, posicionActual.Z / 170f);
            unMarcador.AgregarNuevoMarcadorCheckPoint(rotacionActual, posicionObstaculo);
        }


        void AgregarObstaculoCarretilla(ObstaculosCarretillas unObstaculo)
        {
            unObstaculo.AgregarNuevoObstaculo(rotacionActual, posicionActual);
            
            //Console.WriteLine($"Obstaculo Pez dibujado: Posicion en ejes: X = {posicionObstaculo.X}, Y = {posicionObstaculo.Y}, Z = {posicionObstaculo.Z}");
            
        }
        

    }
}
