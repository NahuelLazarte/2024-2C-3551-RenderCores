using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using System; 
using TGC.MonoGame.TP.MaterialesJuego;

namespace TGC.MonoGame.TP.PistaCurvaDerecha
{
    public class PistasCurvasDerechas
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.03f);
        float escala = 0.03f;
        public Model ModeloPistaCurva { get; set; }
        private BoundingBox PistaCurvaBox { get; set; }
        private Vector3 desplazamientoEnEjes { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private List<Matrix> _pistasCurvas { get; set; }
        private Texture2D Texture { get; set; }
        private Texture2D NormalTexture { get; set; }

        BoundingBox size;
        private BoundingFrustum _frustum;
        public PistasCurvasDerechas(Matrix view, Matrix projection)
        {
            Initialize(view, projection);
        }

        private void Initialize(Matrix view, Matrix projection)
        {
            _pistasCurvas = new List<Matrix>();
            Colliders = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);
        }

        public void LoadContent(ContentManager Content)
        {
            ModeloPistaCurva = Content.Load<Model>("Models/" + "pistas/curvedRoad");
            Console.WriteLine($"Modelo de pista curva cargado");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");
            Texture = Content.Load<Texture2D>("Textures/texturaMadera");
            NormalTexture = Content.Load<Texture2D>("Textures/NormalMapMadera");

            foreach (var mesh in ModeloPistaCurva.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloPistaCurva);

        }

        public void Update(GameTime gameTime, Matrix view, Matrix projection)
        {
            _frustum = new BoundingFrustum(view * projection * scale);
        }
        /*
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {


            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            //Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            Effect.Parameters["Texture"]?.SetValue(Texture);

            foreach (var mesh in ModeloPistaCurva.Meshes)
            {
                for (int i = 0; i < _pistasCurvas.Count; i++)
                {
                    Matrix _pisoWorld = _pistasCurvas[i];
                    BoundingBox boundingBox = BoundingVolumesExtensions.FromMatrix(_pisoWorld);
                    
                    if(_frustum.Intersects(boundingBox)){
                        Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                        mesh.Draw();
                    }
                }
            }

        }*/

        public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            var viewProjection = view * projection;

            foreach (var worldMatrix in _pistasCurvas)
            {
                foreach (var mesh in ModeloPistaCurva.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustum.Intersects(boundingBox))
                    {
                        ShadowMapEffect.Parameters["ambientColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));
                        ShadowMapEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.6f, 0.6f, 0.6f));
                        ShadowMapEffect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
                        ShadowMapEffect.Parameters["shininess"].SetValue(32f);
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        ShadowMapEffect.Parameters["baseTexture"].SetValue(Texture);
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));
                        ShadowMapEffect.Parameters["normalMap"].SetValue(NormalTexture);

                        mesh.Draw();
                    }
                }
            }
        }


        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {

            foreach (var worldMatrix in _pistasCurvas)
            {
                foreach (var modelMesh in ModeloPistaCurva.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloPistaCurva.Bones.Count];
                    ModeloPistaCurva.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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

        public Vector3 Desplazamiento()
        {
            
            desplazamientoEnEjes = size.Max - size.Min;
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X-2f, 0, desplazamientoEnEjes.Z - 3000);
            //Console.WriteLine($"Pista Curva: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");

            return desplazamientoEnEjes;
        }

        public float Rotacion()
        {
            return MathHelper.ToRadians(-90);
        }

        public void agregarNuevaPista(float Rotacion, Vector3 Posicion, Materiales _materiales)
        {
            Posicion += Vector3.Transform(new Vector3(495, 0, 500), Matrix.CreateRotationY(Rotacion));
            Matrix transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale;

            _materiales._muros.AgregarMurosPistaCurvaDerecha(Rotacion, Posicion);

            _pistasCurvas.Add(transform);

            BoundingBox box = new BoundingBox(size.Min * escala + Posicion * escala, size.Max * escala + Posicion * escala);


            Colliders.Add(box);

        }

    }
}
