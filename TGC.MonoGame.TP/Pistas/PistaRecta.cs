using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo

namespace TGC.MonoGame.TP.PistaRecta
{
    public class PistasRectas
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.03f);

        public Model ModeloPistaRecta { get; set; }
        public Model ModeloMuro { get; set; }
        private BoundingBox PistaRectaBox { get; set; }
        private Vector3 desplazamientoEnEjes { get; set; }
    
        public List<BoundingBox> Colliders { get; set; }
        public List<BoundingBox> CollidersMuro { get; set; }

        private List<Matrix> _pistasRectas { get; set; }
        private List<Matrix> _muros { get; set; }
        float escala = 0.03f;
        float escalaMuros = 3f;
        BoundingBox Pistasize;
        BoundingBox Rocksize;


        public PistasRectas()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pistasRectas = new List<Matrix>();
            _muros = new List<Matrix>();
            Colliders = new List<BoundingBox>();
            CollidersMuro = new List<BoundingBox>();
        }

        public void LoadContent(ContentManager Content)
        {
            ModeloPistaRecta = Content.Load<Model>(ContentFolder3D + "pistas/road_straight_fix");
            ModeloMuro = Content.Load<Model>(ContentFolder3D + "pistas/wallHalf");
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            foreach (var mesh in ModeloPistaRecta.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            foreach (var mesh in ModeloMuro.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            Pistasize = BoundingVolumesExtensions.CreateAABBFrom(ModeloPistaRecta);
            Rocksize = BoundingVolumesExtensions.CreateAABBFrom(ModeloMuro);

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {

            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());

            // Dibujar las pistas
            foreach (var mesh in ModeloPistaRecta.Meshes)
            {
                for (int i = 0; i < _pistasRectas.Count; i++)
                {
                    Matrix _pisoWorld = _pistasRectas[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }
            // Dibujar los muros
            Effect.Parameters["DiffuseColor"].SetValue(Color.Gray.ToVector3()); // Color para los muros
            foreach (var mesh in ModeloMuro.Meshes)
            {
                for (int i = 0; i < _muros.Count; i++)
                {
                    Matrix _muroWorld = _muros[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _muroWorld);
                    mesh.Draw();
                }
            }
        }

        public Vector3 Desplazamiento()
        {
            PistaRectaBox = BoundingVolumesExtensions.CreateAABBFrom(ModeloPistaRecta);
            desplazamientoEnEjes = PistaRectaBox.Max - PistaRectaBox.Min; // aca consigo el tamaño el largo de la pista para que coincida son 3/4, el ancho es el mismo.
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X, 0, 0);

            Console.WriteLine($"Pista Recta: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");
            return desplazamientoEnEjes;
        }

        public float Rotacion()
        {
            return 0; // No hay rotacion
        }
        
        public void agregarNuevaPista(float Rotacion, Vector3 Posicion)
        {
            // Crear la matriz de transformación completa
            Matrix worldPista = Matrix.CreateRotationY(Rotacion) *  Matrix.CreateTranslation(Posicion) *Matrix.CreateScale(escala);
            
            var posicionMuros = new Vector3(Posicion.X / 100f, Posicion.Y/ 100f, Posicion.Z/ 100f);
            var posicionIzquierda = posicionMuros;
            var posicionDerecha =  posicionMuros;
            var desplazamientoDerecha = new Vector3(25.22f, -12f, 9f);
            var desplazamientoIzquierda = new Vector3(-25.22f, -12f, -9f);
            
            posicionIzquierda += Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));
            posicionDerecha += Vector3.Transform(desplazamientoDerecha, Matrix.CreateRotationY(Rotacion));

            Matrix muroDerecha = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) *  Matrix.CreateTranslation(posicionDerecha) *Matrix.CreateScale(escalaMuros);
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) *  Matrix.CreateTranslation(posicionIzquierda) *Matrix.CreateScale(escalaMuros);

            _pistasRectas.Add(worldPista);  

            _muros.Add(muroDerecha); 
            _muros.Add(muroIzquierda);    
            
            BoundingBox box = new BoundingBox(Pistasize.Min * escala + Posicion * escala , Pistasize.Max * escala + Posicion * escala);

            Colliders.Add(box);

        }

    }
}