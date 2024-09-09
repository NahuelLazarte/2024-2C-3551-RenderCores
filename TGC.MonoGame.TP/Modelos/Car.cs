using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//private Logo Logo{get;set;}
//Logo = new Logo();
//Logo.LoadContent

namespace TGC.MonoGame.TP.Modelos
{
    class Car
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

        public Car(ContentManager Content)
        {
            World = Matrix.CreateScale(0.01f) * Matrix.CreateTranslation(0f, 3.9f, 0f);
            string path = "objetos/ball";
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
            /*
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());

            foreach(var mesh in Model.Meshes)
            {
                var world = mesh.ParentBone.Transform * World;
                Effect.Parameters["World"].SetValue(world);
                mesh.Draw();
            }
            */

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