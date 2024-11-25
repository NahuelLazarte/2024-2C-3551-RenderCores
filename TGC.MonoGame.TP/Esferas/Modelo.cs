//using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Camera;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Framework;
using Microsoft.Xna.Framework.Input;
using Vector3 = Microsoft.Xna.Framework.Vector3;


namespace TGC.MonoGame.TP.Modelos
{
    public abstract class Modelo
    {
        protected Model Model3D { get; set; } // variable del modelo
        protected Vector3 Position { get; set; } // posicion del modelo 
        protected Matrix Rotation { get; set; } // rotacion del modelo
        protected Matrix Scale { get; set; } // escala del modelo 
        protected Vector3 ColorA { get; set; } // color del modelo
        public Matrix World { get; set; }
        public Effect Effect { get; set; } // mejorar
        protected Texture Texture { get; set; }
        protected Texture NormalTexture { get; set; }
        private RenderTargetCube EnvironmentMapRenderTarget { get; set; }
        private StaticCamera CubeMapCamera { get; set; }
        private const int EnvironmentmapSize = 2048;

        public void SetPosition(Vector3 newPosition) { Position = newPosition; }
        public void SetRotation(Matrix newRotation) { Rotation = newRotation; }
        public void SetScale(Matrix newScale) { Scale = newScale; }
        public void SetColor(Vector3 newColor) { ColorA = newColor; }
        public void SetTexture(Texture newTexture) { Texture = newTexture; }
        public void SetNormalTexture(Texture newTexture) { NormalTexture = newTexture; }


        public Vector3 GetPosition() { return Position; }
        public Matrix GetRotation() { return Rotation; }
        public Matrix GetScale() { return Scale; }
        public Vector3 GetColor() { return ColorA; }

        public virtual void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            CubeMapCamera = new StaticCamera(1f, Position, Vector3.UnitX, Vector3.Up);
            CubeMapCamera.BuildProjection(1f, 3f, 3000f, MathHelper.PiOver2);

            EnvironmentMapRenderTarget = new RenderTargetCube(graphicsDevice, EnvironmentmapSize, false,
                SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
            graphicsDevice.BlendState = BlendState.Opaque;

            

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
            ColorA = _color;
            Position = _position;
            Rotation = _rotation;
        }

        public virtual void Update(GameTime gameTime, ContentManager content)
        {
            CubeMapCamera.Position = Position;

            World = Scale * Rotation * Matrix.CreateTranslation(Position);
        }

        public virtual void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection, GraphicsDevice graphicsDevice)
        {
            foreach (var mesh in Model3D.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = ShadowMapEffect;
                }
            }

            graphicsDevice.DepthStencilState = DepthStencilState.Default;


            for (var face = CubeMapFace.PositiveX; face <= CubeMapFace.NegativeZ; face++)
            {
                // Set the render target as our cubemap face, we are drawing the scene in this texture
                graphicsDevice.SetRenderTarget(EnvironmentMapRenderTarget, face);
                graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

                SetCubemapCameraForOrientation(face);
                CubeMapCamera.BuildView();

            }

            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);



            var viewProjection = view * projection;

            ShadowMapEffect.Parameters["environmentMap"].SetValue(EnvironmentMapRenderTarget);
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

