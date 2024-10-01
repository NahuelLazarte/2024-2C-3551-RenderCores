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

        public Model PistaCurva { get; set; }

        public Matrix[] PistaCurvaWorlds { get; set; }

        private BoundingBox[] PistaCurvaBox { get; set; }

        private Vector3 desplazamientoEnEjes { get; set; }

        public PistaCurva() {
            Initialize();
        }

        private void Initialize() {

            Colliders = new BoundingBox[PistaCurvaWorlds.Length];

            int index = 0;
            int AuxIndex = 0;

            for(; AuxIndex < PistaCurvaWorlds.Length; AuxIndex++){
                Colliders[index] = BoundingVolumesExtensions.FromMatrix(PistaCurvaWorlds[AuxIndex]);
                index++;
            }

        }


        public void LoadContent(ContentManager Content){
            PistaCurva = Content.Load<Model>(ContentFolder3D + "pistas/road_curve_fix");
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            foreach (var mesh in PistaCurva.Meshes){
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

            PistaCurvaWorlds =  scale * Matrix.CreateTranslation(position) * rotation;

            foreach (var mesh in PistaCurva.Meshes){
                for(int i=0; i < PistaCurvaWorlds.Length; i++){
                    Matrix _pisoWorld = PistaCurvaWorlds[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }
        }

        public void Desplazamiento() {
            PistaCurvaBox = BoundingVolumesExtensions.CreateAABBFrom(PistaCurva);
            desplazamientoEnEjes = PistaCurvaBox.Max - PistaCurvaBox.Min; // aca consigo el tamaño el largo de la pista para que coincida son 3/4, el ancho es el mismo.
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes, 0, desplazamientoEnEjes*0.75f); // no estoy seguro que sea la Z
            return desplazamientoEnEjes;
        }

        public void Rotacion() {
            // tiene que retornar el angulo de giro, en este caso 90 grados antihorario
        }

    }
}