using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Asegúrate de tener esta directiva

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Levels;

using Microsoft.Xna.Framework.Audio;

using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo


namespace TGC.MonoGame.TP.MarcadorCheckPoint{
    public class MarcadoresCheckPoints{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Texture2D Textura { get; set; }
        public Texture2D normalTexture { get; set; }

        public Matrix scale = Matrix.CreateScale(1f);
        public Model ModeloMarcadorCheckPoint { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _marcadoresCheckPoints { get; set; }
        private BoundingFrustum _frustum;
        private BoundingBox size;
        public int checkpointActualAux { get; set; }
        public int checkpointAux = 0;
        public MarcadoresCheckPoints(Matrix view, Matrix projection) {
            Initialize(view,projection);
        }

        private void Initialize(Matrix view, Matrix projection) {
            _marcadoresCheckPoints = new List<Matrix>();
            Colliders = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);
        }

        public void LoadContent(ContentManager Content){
            ModeloMarcadorCheckPoint = Content.Load<Model>("Models/" + "CheckPoint/checkpointArrowWithTextures"); 
            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");
            Textura = Content.Load<Texture2D>("Textures/" + "texturaOro");
            normalTexture = Content.Load<Texture2D>("Textures/" + "NormalMapOro");


            foreach (var mesh in ModeloMarcadorCheckPoint.Meshes){
                Console.WriteLine($"Meshname marcadorCheck {mesh.Name}");
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloMarcadorCheckPoint);

        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection, int checkpointActual) {
            
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            float sinOffset = (float)Math.Sin(Rotation) * 0.8f; // multiplicador para la amplitud
            for (int i = 0; i < _marcadoresCheckPoints.Count; i++)
            {
                var originalPosition = _marcadoresCheckPoints[i].Translation; // Obtener la posición original
                _marcadoresCheckPoints[i] = Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(originalPosition.X, originalPosition.Y + (sinOffset) * 0.05f, originalPosition.Z);

                checkpointActualAux = checkpointActual - checkpointAux;
                if (i < checkpointActualAux){
                    _marcadoresCheckPoints.RemoveAt(i);
                    Colliders.RemoveAt(i);
                    checkpointAux++;
                }

                _frustum = new BoundingFrustum(view * projection);

            }
        }

        public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            var viewProjection = view * projection;
            
            for (int i = 0; i < _marcadoresCheckPoints.Count; i++){
                var worldMatrix = _marcadoresCheckPoints[i];
                foreach (var mesh in ModeloMarcadorCheckPoint.Meshes)
                    {
                        var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                        var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                        if (_frustum.Intersects(boundingBox) && checkpointActualAux == i){
                            ShadowMapEffect.Parameters["ambientColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));
                            ShadowMapEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.7f, 0.7f, 0.7f));
                            ShadowMapEffect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
                            ShadowMapEffect.Parameters["shininess"].SetValue(64f);
                            ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                            ShadowMapEffect.Parameters["baseTexture"]?.SetValue(Textura);
                            ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                            ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));
                            ShadowMapEffect.Parameters["normalMap"].SetValue(normalTexture);

                        mesh.Draw();
                        }
                    }
            }
        }

        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {
            
            foreach (var worldMatrix in _marcadoresCheckPoints)
            {
                foreach (var modelMesh in ModeloMarcadorCheckPoint.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloMarcadorCheckPoint.Bones.Count];
                    ModeloMarcadorCheckPoint.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

                    // Combina las transformaciones locales y globales.
                    var meshWorld = modelMeshesBaseTransforms[modelMesh.ParentBone.Index] * worldMatrix;
                    ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * LightView * Projection);

                    foreach (var part in modelMesh.MeshParts)
                    {
                        part.Effect = ShadowMapEffect; // Aplica el shader de sombras
                    }

                    modelMesh.Draw(); // Dibuja el mesh en el mapa de sombras
                }
            }
        }


        public void AgregarNuevoMarcadorCheckPoint(float Rotacion, Vector3 Posicion) {
            Posicion += Vector3.Transform(new Vector3(0, 0, -0.8f), Matrix.CreateRotationY(Rotacion));

            var transform = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(Posicion.X, Posicion.Y + 5f, Posicion.Z) * Matrix.CreateScale(5f); 
            _marcadoresCheckPoints.Add(transform);
            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            BoundingBox box = new BoundingBox(size.Min* 5f  + Posicion* 5f , size.Max* 5f + Posicion * 5f);

            Colliders.Add(box);
        }

    }
}
