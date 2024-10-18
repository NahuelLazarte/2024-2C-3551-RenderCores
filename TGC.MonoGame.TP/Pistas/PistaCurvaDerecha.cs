using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo

namespace TGC.MonoGame.TP.PistaCurvaDerecha
{
    public class PistasCurvasDerechas
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.03f);
        float escala = 0.03f;
        public Model ModeloPistaCurva { get; set; }

        private BoundingBox PistaCurvaBox { get; set; }

        private Vector3 desplazamientoEnEjes { get; set; }

        public List<BoundingBox> Colliders { get; set; }

        private List<Matrix> _pistasCurvas { get; set; }

        BoundingBox size;
        public PistasCurvasDerechas()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pistasCurvas = new List<Matrix>();
            Colliders = new List<BoundingBox>();
        }

        public void LoadContent(ContentManager Content)
        {
            ModeloPistaCurva = Content.Load<Model>("Models/" + "pistas/road_curve_fix");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloPistaCurva.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloPistaCurva);

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {


            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));

            foreach (var mesh in ModeloPistaCurva.Meshes)
            {
                for (int i = 0; i < _pistasCurvas.Count; i++)
                {
                    Matrix _pisoWorld = _pistasCurvas[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }

        }

        public Vector3 Desplazamiento()
        {
            PistaCurvaBox = BoundingVolumesExtensions.CreateAABBFrom(ModeloPistaCurva); // HACER UNA SOLA VEZ
            desplazamientoEnEjes = PistaCurvaBox.Max - PistaCurvaBox.Min;
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X - 100, 0, desplazamientoEnEjes.Z - 2800);
            Console.WriteLine($"Pista Curva: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");

            return desplazamientoEnEjes;
        }

        public float Rotacion()
        {
            return MathHelper.ToRadians(-90);
        }

        public void agregarNuevaPista(float Rotacion, Vector3 Posicion)
        {

            Posicion += Vector3.Transform(new Vector3(300, 0, 500), Matrix.CreateRotationY(Rotacion));
            Matrix transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale;

            _pistasCurvas.Add(transform);

            BoundingBox box = new BoundingBox(size.Min * escala + Posicion * escala, size.Max * escala + Posicion * escala);


            Colliders.Add(box);

        }

    }
}
