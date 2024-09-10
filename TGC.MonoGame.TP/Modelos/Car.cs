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
        private Model Model { get; set; } // variable del modelo
        private Vector3 position { get; set; } // posicion del modelo 
        private Matrix rotation{ get; set; } // rotacion del modelo
        private Matrix scale { get; set; } // escala del modelo     
        public Matrix World { get; set; }
        private Effect Effect { get; set; }

        public void setPosition(Vector3 newPosition){
            position = newPosition;
        }
        public void setRotation(Matrix newRotation){
            rotation = newRotation;
        }
        public void setScale(Matrix newScale){
            scale = newScale;
        }

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

        public Car(ContentManager Content, Vector3 Position,Matrix Rotation)
        {
            position = Position;
            rotation = Rotation;
            scale = Matrix.CreateScale(0.9f); //poner acá la escala que va aplicar para todos 

            World = scale* rotation * Matrix.CreateTranslation(position);

            string path = "objetos/truck";//poner acá la ruta del modelo 3D
            Model = Content.Load<Model>("Models/" + path);// "Models/"  es lo mismo que poner ContentFolder3D
        }

        public void Update(GameTime gameTime)
        {
             World = scale* rotation * Matrix.CreateTranslation(position);
        }

        public void Draw()
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