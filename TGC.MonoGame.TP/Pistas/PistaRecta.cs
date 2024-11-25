using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using System; 
using TGC.MonoGame.TP.MaterialesJuego;

namespace TGC.MonoGame.TP.PistaRecta
{
    public class PistasRectas
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.03f);
        public Model ModeloPistaRecta { get; set; }
        public Model ModeloMuro { get; set; }
        private BoundingBox PistaRectaBox { get; set; }
        private Vector3 desplazamientoEnEjes { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        public List<BoundingBox> CollidersMuro { get; set; }
        private List<Matrix> _pistasRectas { get; set; }
        private List<Matrix> _muros { get; set; }
        float escala = 0.03f;        
        BoundingBox Pistasize;
        private BoundingFrustum _frustum;
        private Texture2D NormalTexture { get; set; }
        private Texture2D Texture { get; set; }
        public PistasRectas(Matrix view, Matrix projection)
        {
            Initialize(view,projection);
        }

        private void Initialize(Matrix view, Matrix projection)
        {
            _pistasRectas = new List<Matrix>();
            _muros = new List<Matrix>();
            Colliders = new List<BoundingBox>();
            CollidersMuro = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);
        }

        public void LoadContent(ContentManager Content)
        {
            ModeloPistaRecta = Content.Load<Model>(ContentFolder3D + "pistas/straightRoad");            

            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");

            Texture = Content.Load<Texture2D>("Textures/texturaMadera");
            NormalTexture = Content.Load<Texture2D>("Textures/NormalMapMadera");

            //Effect.CurrentTechnique = Effect.Techniques["LightingTechnique"];

            foreach (var mesh in ModeloPistaRecta.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            Pistasize = BoundingVolumesExtensions.CreateAABBFrom(ModeloPistaRecta);
            

        }

        public void Update(GameTime gameTime, Matrix view, Matrix projection)
        {
            _frustum = new BoundingFrustum(view * projection * scale); // * scale
        }

        /*
        public void Draw(GameTime gameTime, Matrix view, Matrix projection, RenderTarget2D ShadowMapRenderTarget, Vector3 LightPosition, Matrix ligtViewProj)
        {

            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["Texture"]?.SetValue(Texture);
            Effect.Parameters["ShadowMap"]?.SetValue(ShadowMapRenderTarget);
            
            //Effect.Parameters["lightPosition"].SetValue(ligtViewProj);
            //Effect.Parameters["LightViewProjection"].SetValue(ligtViewProj);
            
            // Dibujar las pistas
            foreach (var mesh in ModeloPistaRecta.Meshes)
            {
                for (int i = 0; i < _pistasRectas.Count; i++)
                {
                    Matrix _pisoWorld = _pistasRectas[i];
                    BoundingBox boundingBox = BoundingVolumesExtensions.FromMatrix(_pisoWorld);
                    
                    if(_frustum.Intersects(boundingBox)){
                        Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                        //Effect.Parameters["WorldViewProjection"].SetValue(_pisoWorld *mesh.ParentBone.Transform* view * projection);
                        //Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(_pisoWorld * mesh.ParentBone.Transform)));
                        mesh.Draw();
                    }
                }
            }

            //BasicColorDrawing

            
        }*/

        public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            var viewProjection = view * projection;

            foreach (var worldMatrix in _pistasRectas)
            {
                foreach (var mesh in ModeloPistaRecta.Meshes)
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

            foreach (var worldMatrix in _pistasRectas)
            {
                foreach (var modelMesh in ModeloPistaRecta.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloPistaRecta.Bones.Count];
                    ModeloPistaRecta.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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
            
            desplazamientoEnEjes = Pistasize.Max - Pistasize.Min; // aca consigo el tamaño el largo de la pista para que coincida son 3/4, el ancho es el mismo.
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X, 0, 0);

            //Console.WriteLine($"Pista Recta: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");
            return desplazamientoEnEjes;
        }

        public float Rotacion()
        {
            return 0; // No hay rotacion
        }
        
        public void agregarNuevaPista(float Rotacion, Vector3 Posicion, Materiales _materiales)
        {
            // Crear la matriz de transformación completa
            Matrix worldPista = Matrix.CreateRotationY(Rotacion) *  Matrix.CreateTranslation(Posicion) *Matrix.CreateScale(escala);

            _materiales._muros.AgregarMurosPistaRecta(Rotacion, Posicion);

            _pistasRectas.Add(worldPista);   
            
            BoundingBox box = new BoundingBox(Pistasize.Min * escala + Posicion * escala , Pistasize.Max * escala + Posicion * escala);

            Colliders.Add(box);


        }

    }
}