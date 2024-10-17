using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo

namespace TGC.MonoGame.TP.ObstaculoPozo
{
    public class ObstaculosPozos
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(2f);
        public Model ModeloPozo { get; set; }
        public Model ModeloMuro { get; set; }
        private BoundingBox PozoBox { get; set; }
        private Vector3 desplazamientoEnEjes { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private List<Matrix> _pozos { get; set; }
        private List<Matrix> _muros { get; set; }

        public BoundingSphere _envolturaEsfera{ get; set; }
        float escala = 2f;
        float escalaMuros = 3f;

        BoundingBox size;


        public ObstaculosPozos()
        {
            Initialize();
            _muros = new List<Matrix>();
        }

        private void Initialize()
        {
            _pozos = new List<Matrix>();
            Colliders = new List<BoundingBox>();
        }

        public void IniciarColliders()
        {
            
        }


        public void LoadContent(ContentManager Content)
        {
            ModeloPozo = Content.Load<Model>(ContentFolder3D + "obstaculos/rockLarge");
            ModeloMuro = Content.Load<Model>(ContentFolder3D + "pistas/wallHalf");

            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            foreach (var mesh in ModeloPozo.Meshes)
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

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloPozo);
        }

        public void Update(GameTime gameTime, TGCGame Game)
        {
            for (int i = 0; i < _pozos.Count; i++) {
                if (_envolturaEsfera.Intersects(Colliders[i])){
                    Game.Respawn();
                }
                       

            }
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            

            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));

            foreach (var mesh in ModeloPozo.Meshes)
            {
                for (int i = 0; i < _pozos.Count; i++)
                {
                    Matrix _pisoWorld = _pozos[i];
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
            PozoBox = BoundingVolumesExtensions.CreateAABBFrom(ModeloPozo);
            desplazamientoEnEjes = PozoBox.Max - PozoBox.Min; // aca consigo el tamaño el largo de la pista para que coincida son 3/4, el ancho es el mismo.
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X, 0, 0);

            Console.WriteLine($"Pozo: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");
            return desplazamientoEnEjes;
        }

        
        public void agregarNuevoPozo(float Rotacion, Vector3 Posicion)
        {
            Posicion += Vector3.Transform(new Vector3(0,-15,0), Matrix.CreateRotationY(Rotacion));

            Matrix transform = Matrix.CreateRotationY(Rotacion) *  Matrix.CreateTranslation(Posicion) *Matrix.CreateScale(escala);

            var posicionMuros = new Vector3(Posicion.X / 1.47f , (Posicion.Y + 15f)/  1.47f  , Posicion.Z/  1.47f );
            var posicionIzquierda = posicionMuros;
            var posicionDerecha =  posicionMuros;
            var desplazamientoDerecha = new Vector3(25.22f , -12f , 9f);
            var desplazamientoIzquierda = new Vector3(-25.22f , -12f, -9f);
            
            posicionIzquierda += Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));
            posicionDerecha += Vector3.Transform(desplazamientoDerecha, Matrix.CreateRotationY(Rotacion));

            Matrix muroDerecha = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) *  Matrix.CreateTranslation(posicionDerecha) *Matrix.CreateScale(escalaMuros);
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) *  Matrix.CreateTranslation(posicionIzquierda) *Matrix.CreateScale(escalaMuros);

            _pozos.Add(transform);

            _muros.Add(muroDerecha); 
            _muros.Add(muroIzquierda); 

            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            BoundingBox box = new BoundingBox(transformedMin, transformedMax);
            Colliders.Add(box);
        }

    }
}