using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo

namespace TGC.MonoGame.TP.PistaCurva{
    public class PistasCurvas{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.03f);

        public Model ModeloPistaCurva { get; set; }

        private BoundingBox PistaCurvaBox { get; set; }

        private Vector3 desplazamientoEnEjes { get; set; }

        public BoundingBox[] Colliders { get; set; }

        private List<Matrix> _pistasCurvas { get; set; }


        public PistasCurvas() {
            Initialize();
        }

        private void Initialize() {
            _pistasCurvas = new List<Matrix>();
        }

        public void IniciarColliders() {
            Colliders = new BoundingBox[_pistasCurvas.Count];

            int indice = 0;
            int Aux = 0;

            for(; Aux < _pistasCurvas.Count; Aux++){
                Colliders[indice] = BoundingVolumesExtensions.FromMatrix(_pistasCurvas[Aux]);
                indice++;
            }
            
        }



        public void LoadContent(ContentManager Content){
            ModeloPistaCurva = Content.Load<Model>("Models/" + "pistas/road_curve_fix");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloPistaCurva.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }
        }

        public void Update(GameTime gameTime){

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {


            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());
            
            foreach (var mesh in ModeloPistaCurva.Meshes){
                for(int i=0; i < _pistasCurvas.Count; i++){
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
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X - 500, 0, desplazamientoEnEjes.Z - 2500);
            Console.WriteLine($"Pista Curva: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");

            return desplazamientoEnEjes;  
        }

        public float Rotacion() {
            return MathHelper.ToRadians(-90);
        }

        public void agregarNuevaPista(float Rotacion, Vector3 Posicion) {
            Posicion += Vector3.Transform(new Vector3(0,0,500), Matrix.CreateRotationY(Rotacion));
            _pistasCurvas.Add(Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale); // METER MATRIZ DENTRO DE CADA PISTA
        }

    }
}
