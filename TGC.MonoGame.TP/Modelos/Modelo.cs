using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Modelos
{
    class Modelo
    {
        private Model Model { get; set; }
        private Vector3 Position { get; set; } = Vector3.Zero;        
        public Matrix World { get; set; }
        private Effect Effect { get; set; }
        public void LoadContent(Effect effect)
        {
            Effect = effect;    
            
            foreach( var mesh in Model.Meshes )
            {
                foreach( var meshPart in mesh.MeshParts )
                {
                    meshPart.Effect = Effect;
                }
            }        
        }

        public Modelo(ContentManager Content)
        {
            
            World = Matrix.CreateScale(0.8f) * Matrix.CreateTranslation(20f, 3.9f, 0f);
            string path = "objetos/truck";
            Model = Content.Load<Model>("Models/" + path);// "Models/"  es lo mismo que poner ContentFolder3D

        }

        public void Update(GameTime gameTime)
        {
            
            /*
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Position += Vector3.Up * elapsedTime * 5f;
            var quaternion = Quaternion.CreateFromAxisAngle(Vector3.Up,MathF.PI);
            World = Matrix.CreateScale(0.2f) * Matrix.CreateFromQuaternion(quaternion);
            */
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach( var mesh in Model.Meshes )
            {
                var relativeTransform = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                Effect.Parameters["World"].SetValue(relativeTransform * World);
                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0, 1, 0));
                mesh.Draw();
            }
        }
    }
}