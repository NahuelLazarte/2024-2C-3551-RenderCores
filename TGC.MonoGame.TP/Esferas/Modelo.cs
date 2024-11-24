//using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Camera;


namespace TGC.MonoGame.TP.Modelos
{
    public abstract class Modelo
    {
        protected Model Model3D { get; set; } // variable del modelo
        protected Vector3 Position { get; set; } // posicion del modelo 
        protected Matrix Rotation { get; set; } // rotacion del modelo
        protected Matrix Scale { get; set; } // escala del modelo 
        protected Vector3 Color { get; set; } // color del modelo
        public Matrix World { get; set; }
        public Effect Effect { get; set; } // mejorar
        protected Texture Texture { get; set; }
        protected Texture NormalTexture { get; set; }
        private RenderTargetCube EnvironmentMapRenderTarget { get; set; }
        private StaticCamera CubeMapCamera { get; set; }


        public void SetPosition(Vector3 newPosition) { Position = newPosition; }
        public void SetRotation(Matrix newRotation) { Rotation = newRotation; }
        public void SetScale(Matrix newScale) { Scale = newScale; }
        public void SetColor(Vector3 newColor) { Color = newColor; }
        public void SetTexture(Texture newTexture) { Texture = newTexture; }
        public void SetNormalTexture(Texture newTexture) { NormalTexture = newTexture; }


        public Vector3 GetPosition() { return Position; }
        public Matrix GetRotation() { return Rotation; }
        public Matrix GetScale() { return Scale; }
        public Vector3 GetColor() { return Color; }

        public virtual void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            foreach (var mesh in Model3D.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
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

        public virtual void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            foreach (var mesh in Model3D.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = ShadowMapEffect;
                }
            }

            var viewProjection = view * projection;

            ShadowMapEffect.Parameters["World"].SetValue(World);
            ShadowMapEffect.Parameters["baseTexture"].SetValue(Texture);
            ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(World * viewProjection);
            ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(World)));
            ShadowMapEffect.Parameters["normalMap"].SetValue(NormalTexture);

            foreach (var mesh in Model3D.Meshes)
            {
                mesh.Draw();
            }
        }

        


        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {

            
            foreach (var modelMesh in Model3D.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[Model3D.Bones.Count];
                    Model3D.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

                    // Combina las transformaciones locales y globales.
                    var meshWorld = modelMeshesBaseTransforms[modelMesh.ParentBone.Index] * World ;
                    ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(World * LightView * Projection);

                    foreach (var part in modelMesh.MeshParts)
                    {
                        part.Effect = ShadowMapEffect; // Aplica el shader de sombras
                    }

                    modelMesh.Draw(); // Dibuja el mesh en el mapa de sombras
                }
        }

        private void SetCubemapCameraForOrientation(CubeMapFace face)
        {
            switch (face)
            {
                default:
                case CubeMapFace.PositiveX:
                    CubeMapCamera.FrontDirection = -Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeX:
                    CubeMapCamera.FrontDirection = Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.PositiveY:
                    CubeMapCamera.FrontDirection = Vector3.Down;
                    CubeMapCamera.UpDirection = Vector3.UnitZ;
                    break;

                case CubeMapFace.NegativeY:
                    CubeMapCamera.FrontDirection = Vector3.Up;
                    CubeMapCamera.UpDirection = -Vector3.UnitZ;
                    break;

                case CubeMapFace.PositiveZ:
                    CubeMapCamera.FrontDirection = -Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeZ:
                    CubeMapCamera.FrontDirection = Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;
            }
        }

    }
}

