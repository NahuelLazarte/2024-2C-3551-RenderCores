using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace TGC.MonoGame.TP.Modelos
{
    abstract class Modelo
    {
        protected Model Model3D { get; set; } // variable del modelo
        private Vector3 Position { get; set; } // posicion del modelo 
        private Matrix Rotation { get; set; } // rotacion del modelo
        protected Matrix Scale { get; set; } // escala del modelo 
        private Color Color { get; set; } // color del modelo
        public Matrix World { get; set; }


        public void SetPosition( Vector3 newPosition ){ Position = newPosition; }
        public void SetRotation( Matrix newRotation ){ Rotation = newRotation; }
        public void SetScale( Matrix newScale ){ Scale = newScale; }
        public void SetColor( Color newColor ){ Color = newColor; }

        public Vector3 GetPosition(){ return Position; }
        public Matrix GetRotation(){ return Rotation; } 
        public Matrix GetScale(){ return Scale; }
        public Color GetColor(){ return Color; }

        public void LoadContent(Effect _effect)
        {
            foreach (var mesh in Model3D.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = _effect;
                }
            }
        }

        public Modelo(ContentManager _content, Vector3 _position, Matrix _rotation, Color _color)
        {
            Color = _color;
            Position = _position;
            Rotation = _rotation;
            //Model = Content.Load<Model>("Models/" + path);// "Models/"  es lo mismo que poner ContentFolder3D
        }

        public void Update(GameTime gameTime)
        {
            World = Scale * Rotation * Matrix.CreateTranslation(Position);
        }

        public void Draw(Effect _effect)
        {
            var modelMeshesBaseTransforms = new Matrix[Model3D.Bones.Count];
            Model3D.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach (var mesh in Model3D.Meshes)
            {
                var relativeTransform = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                _effect.Parameters["World"].SetValue(relativeTransform * World);
                _effect.Parameters["DiffuseColor"].SetValue(Color.ToVector3());
                mesh.Draw();
            }
        }
    }
}