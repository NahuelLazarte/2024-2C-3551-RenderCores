using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace TGC.MonoGame.TP.Modelos
{
    public abstract class Modelo
    {
        protected Model Model3D { get; set; } // variable del modelo
        protected Vector3 Position { get; set; } // posicion del modelo 
        protected Matrix Rotation { get; set; } // rotacion del modelo
        protected Matrix Scale { get; set; } // escala del modelo 
        private Vector3 Color { get; set; } // color del modelo
        public Matrix World { get; set; }
        public Effect Effect { get; set; }

        private BasicEffect Efecto { get; set; }
        private Texture2D Texture { get; set; }

        public void SetPosition(Vector3 newPosition) { Position = newPosition; }
        public void SetRotation(Matrix newRotation) { Rotation = newRotation; }
        public void SetScale(Matrix newScale) { Scale = newScale; }
        public void SetColor(Vector3 newColor) { Color = newColor; }
        public void SetTexture(Texture2D newTexture) { Efecto.Texture = newTexture; }

        public Vector3 GetPosition() { return Position; }
        public Matrix GetRotation() { return Rotation; }
        public Matrix GetScale() { return Scale; }
        public Vector3 GetColor() { return Color; }

        public virtual void LoadContent(ContentManager content,GraphicsDevice graphicsDevice)
        {
            Texture = content.Load<Texture2D>("Textures/texturaGolf");
            Efecto = new BasicEffect(graphicsDevice);
            Efecto.TextureEnabled = true;
            Efecto.Texture = Texture;

            foreach (var mesh in Model3D.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Efecto;
                }
            }
        }

        public Modelo(Vector3 _position, Matrix _rotation, Vector3 _color)
        {
            Color = _color;
            Position = _position;
            Rotation = _rotation;            
        }

        public virtual void Update(GameTime gameTime, ContentManager content)
        {
            World = Scale * Rotation * Matrix.CreateTranslation(Position);
        }

        public virtual void Draw(Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color);
            
            
            Efecto.Projection = projection;
            Efecto.View = view;


            foreach (var mesh in Model3D.Meshes)
            {
                Efecto.World = mesh.ParentBone.Transform * World;
                mesh.Draw();
            }
        }
    }
}

