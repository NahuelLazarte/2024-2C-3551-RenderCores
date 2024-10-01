using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP.PistaCurva{
    public class PistaCurva{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.03f);

        public Model PistaRecta { get; set; }

        public Matrix[] PistaRectaWorlds { get; set; }

        private BoundingBox[] PistaRectaBox { get; set; }

        private Vector3 desplazamientoEnEjes { get; set; }

        public PistaRecta() {
            Initialize();
        }

        private void Initialize() {

            Colliders = new BoundingBox[PistaRectaWorlds.Length];

            int index = 0;
            int AuxIndex = 0;

            for(; AuxIndex < PistaRectaWorlds.Length; AuxIndex++){
                Colliders[index] = BoundingVolumesExtensions.FromMatrix(PistaRectaWorlds[AuxIndex]);
                index++;
            }

        }


        public void LoadContent(ContentManager Content){
            PistaRecta = Content.Load<Model>(ContentFolder3D + "pistas/road_straight_fix");
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            foreach (var mesh in PistaRecta.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }
        }

        public void Update(GameTime gameTime){

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection, Vector3 position, Matrix rotation)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkRed.ToVector3());
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());

            PistaRectaWorlds =  scale * Matrix.CreateTranslation(position) * rotation;

            foreach (var mesh in PistaRecta.Meshes){
                for(int i=0; i < PistaRectaWorlds.Length; i++){
                    Matrix _pisoWorld = PistaRectaWorlds[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }
        }

        public void Desplazamiento() {
            PistaRectaBox = BoundingVolumesExtensions.CreateAABBFrom(PistaRecta);
            desplazamientoEnEjes = PistaRectaBox.Max - PistaRectaBox.Min; // aca consigo el tamaño el largo de la pista para que coincida son 3/4, el ancho es el mismo.
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes, 0, desplazamientoEnEjes*0.75f); // no estoy seguro que sea la Z
            return desplazamientoEnEjes;
        }

        public void Rotacion() {
            // tiene que retornar el angulo de giro, en este caso 0 grados
        }

    }
}