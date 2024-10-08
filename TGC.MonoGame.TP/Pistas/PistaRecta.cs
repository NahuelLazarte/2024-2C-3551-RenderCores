using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo

namespace TGC.MonoGame.TP.PistaRecta{
    public class PistasRectas{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.03f);

        public Model ModeloPistaRecta { get; set; }

        private BoundingBox PistaRectaBox { get; set; }

        private Vector3 desplazamientoEnEjes { get; set; }
        public BoundingBox[] Colliders { get; set; }

        private List<Matrix> _pistasRectas  { get; set; }
        
    
        public PistasRectas() {
            Initialize();
        }

        private void Initialize() {
            _pistasRectas = new List<Matrix>();
        }

        public void IniciarColliders() {
            Colliders = new BoundingBox[_pistasRectas.Count];

            int indice = 0;
            int Aux = 0;

            for(; Aux < _pistasRectas.Count; Aux++){
                Colliders[indice] = BoundingVolumesExtensions.FromMatrix(_pistasRectas[Aux]);
                indice++;
            }
        }


        public void LoadContent(ContentManager Content){
            ModeloPistaRecta = Content.Load<Model>(ContentFolder3D + "pistas/road_straight_fix");
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            foreach (var mesh in ModeloPistaRecta.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }
        }

        public void Update(GameTime gameTime){

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {

            Colliders = new BoundingBox[_pistasRectas.Count];

            int indice = 0;
            int Aux = 0;

            for(; Aux < _pistasRectas.Count; Aux++){
                Colliders[indice] = BoundingVolumesExtensions.FromMatrix(_pistasRectas[Aux] * scale);
                indice++;
            }

            

            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());
            
            foreach (var mesh in ModeloPistaRecta.Meshes){
                for(int i=0; i < _pistasRectas.Count; i++){
                    Matrix _pisoWorld = _pistasRectas[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }
            
        }

        public Vector3 Desplazamiento() {
            PistaRectaBox = BoundingVolumesExtensions.CreateAABBFrom(ModeloPistaRecta);
            desplazamientoEnEjes = PistaRectaBox.Max - PistaRectaBox.Min; // aca consigo el tamaño el largo de la pista para que coincida son 3/4, el ancho es el mismo.
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X, 0, 0);

            Console.WriteLine($"Pista Recta: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");
            return desplazamientoEnEjes;
        }

        public float Rotacion() {
            return 0; // No hay rotacion
        }

        public void agregarNuevaPista(float Rotacion, Vector3 Posicion) {
            _pistasRectas.Add(Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale); // METER MATRIZ DENTRO DE CADA PISTA
        }

    }
}