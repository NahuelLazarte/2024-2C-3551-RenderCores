using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.MaterialesJuego;
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
        private BoundingBox PozoBox { get; set; }
        private Vector3 desplazamientoEnEjes { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private List<Matrix> _pozos { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        float escala = 2f;
        BoundingBox size;


        public ObstaculosPozos()
        {
            Initialize();
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

            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            foreach (var mesh in ModeloPozo.Meshes)
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

        }

        public Vector3 Desplazamiento()
        {
            PozoBox = BoundingVolumesExtensions.CreateAABBFrom(ModeloPozo);
            desplazamientoEnEjes = PozoBox.Max - PozoBox.Min; // aca consigo el tamaño el largo de la pista para que coincida son 3/4, el ancho es el mismo.
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X, 0, 0);

            Console.WriteLine($"Pozo: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");
            return desplazamientoEnEjes;
        }

        
        public void agregarNuevoPozo(float Rotacion, Vector3 Posicion, Materiales _materiales)
        {
            Posicion += Vector3.Transform(new Vector3(0,-15,0), Matrix.CreateRotationY(Rotacion));

            Matrix transform = Matrix.CreateRotationY(Rotacion) *  Matrix.CreateTranslation(Posicion) *Matrix.CreateScale(escala);

            _materiales._muros.AgregarMurosPozo(Rotacion, Posicion);

            _pozos.Add(transform);

            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            BoundingBox box = new BoundingBox(transformedMin, transformedMax);
            Colliders.Add(box);
        }

    }
}