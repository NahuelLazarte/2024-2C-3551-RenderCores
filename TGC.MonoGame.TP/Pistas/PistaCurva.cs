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

        public Model ModeloPistaCurva { get; set; }

        public Matrix PistaCurvaWorlds { get; set; }

        private BoundingBox PistaCurvaBox { get; set; }

        private Vector3 desplazamientoEnEjes { get; set; }

        public BoundingBox Collider { get; set; }

        public PistaCurva() {
            Initialize();
        }

        private void Initialize() {
            Collider = BoundingVolumesExtensions.FromMatrix(PistaCurvaWorlds);
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

        public void Draw(GameTime gameTime, Matrix view, Matrix projection, Vector3 position, Matrix rotation)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkRed.ToVector3());
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());

            PistaCurvaWorlds =  scale * Matrix.CreateTranslation(position) * rotation;

            foreach (var mesh in ModeloPistaCurva.Meshes) 
            {
                Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * PistaCurvaWorlds);
                mesh.Draw();
            }
        }

        public Vector3 Desplazamiento() 
        {
            PistaCurvaBox = BoundingVolumesExtensions.CreateAABBFrom(ModeloPistaCurva);
            desplazamientoEnEjes = PistaCurvaBox.Max - PistaCurvaBox.Min;
            return new Vector3(desplazamientoEnEjes.X, 0, desplazamientoEnEjes.Z * 0.75f);
        }

        public Matrix Rotacion() {
            return Matrix.CreateRotationY(MathHelper.ToRadians(90));
        }

    }
}