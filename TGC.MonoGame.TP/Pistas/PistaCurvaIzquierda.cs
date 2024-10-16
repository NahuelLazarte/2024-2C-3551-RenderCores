using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo

namespace TGC.MonoGame.TP.PistaCurvaIzquierda{
    public class PistasCurvasIzquierdas{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.03f);

        public Model ModeloPistaCurva { get; set; }

        private BoundingBox PistaCurvaBox { get; set; }

        private Vector3 desplazamientoEnEjes { get; set; }

        public List<BoundingBox> Colliders { get; set; }

        private List<Matrix> _pistasCurvas { get; set; }
        BoundingBox size;

        public PistasCurvasIzquierdas() {
            Initialize();
        }

        private void Initialize() {
            _pistasCurvas = new List<Matrix>();
            Colliders = new List<BoundingBox>();
        }

        public void IniciarColliders() {
            /*
            Colliders = new BoundingBox[_pistasCurvas.Count];

            for (int i = 0; i < _pistasCurvas.Count; i++) {
                Colliders[i] = BoundingVolumesExtensions.FromMatrix(_pistasCurvas[i]);
            }
            */
        }



        public void LoadContent(ContentManager Content){
            ModeloPistaCurva = Content.Load<Model>("Models/" + "pistas/road_curve_fix");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloPistaCurva.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloPistaCurva);
        }

        public void Update(GameTime gameTime){

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {


            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
            
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
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X, 0, desplazamientoEnEjes.Z -1000);
            Console.WriteLine($"Pista Curva: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");

            return desplazamientoEnEjes;  
        }

        public float Rotacion() {
            return MathHelper.ToRadians(90);
        }

        public void agregarNuevaPista(float Rotacion, Vector3 Posicion) {
            Posicion += Vector3.Transform(new Vector3(500,0,-500), Matrix.CreateRotationY(Rotacion));
        
            Matrix transform = Matrix.CreateRotationY(Rotacion) *  Matrix.CreateRotationX(MathHelper.Pi) * Matrix.CreateTranslation(Posicion) * scale;

            // Agregar la matriz de transformación a la lista de pistas
            _pistasCurvas.Add(transform);

            // Transformar los puntos mínimos y máximos del BoundingBox original
            // Aquí asumo que tienes un 'size' definido, que representa el tamaño original de la pista curva
            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            // Crear y agregar el nuevo BoundingBox transformado a la lista de colliders
            BoundingBox box = new BoundingBox(transformedMin, transformedMax);
            Colliders.Add(box);

            // Imprimir los valores del BoundingBox para depuración
            Console.WriteLine($"Box min= {box.Min}  Box max= {box.Max}");
        }

    }
}
