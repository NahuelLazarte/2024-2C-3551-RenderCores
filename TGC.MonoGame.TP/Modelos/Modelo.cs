using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace TGC.MonoGame.TP.Modelos
{
    class Modelo
    {
        protected Model Model3D { get; set; } // variable del modelo
        private Vector3 position { get; set; } // posicion del modelo 
        private Matrix rotation { get; set; } // rotacion del modelo
        protected Matrix scale { get; set; } // escala del modelo 
        private Vector3 color { get; set; } // color del modelo
        public Matrix World { get; set; }
        private Effect Effect { get; set; }
        private string path="";

        public void SetPosition(Vector3 newPosition){position = newPosition;}
        public void SetRotation(Matrix newRotation){rotation = newRotation;}
        public void SetScale(Matrix newScale){scale = newScale;}
        public void SetColor(Vector3 newColor){color = newColor;}
        public void SetWorld(Matrix newWorld){}

        public Vector3 GetPosition(){return position;}
        public Matrix GetRotation(){return rotation;}
        public Matrix GetScale(){return scale;}
        public Vector3 GetColor(){return color;}
        public Matrix GetWorld(){return World;}

        public void LoadContent(Effect effect)
        {
            Effect = effect;

            foreach (var mesh in Model3D.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }
        }

        public Modelo(ContentManager Content, Vector3 Position, Matrix Rotation, Vector3 Color)
        {
            color = Color;
            position = Position;
            rotation = Rotation;            
            //Model = Content.Load<Model>("Models/" + path);// "Models/"  es lo mismo que poner ContentFolder3D
        }

        public void Update(GameTime gameTime)
        {
            World = scale * rotation * Matrix.CreateTranslation(position);
        }

        public void Draw()
        {
            var modelMeshesBaseTransforms = new Matrix[Model3D.Bones.Count];
            Model3D.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach (var mesh in Model3D.Meshes)
            {
                var relativeTransform = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                Effect.Parameters["World"].SetValue(relativeTransform * World);
                Effect.Parameters["DiffuseColor"].SetValue(color);
                mesh.Draw();
            }
        }
    }
}