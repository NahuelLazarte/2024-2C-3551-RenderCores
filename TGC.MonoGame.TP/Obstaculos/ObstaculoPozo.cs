using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.MaterialesJuego;
using System;
using TGC.MonoGame.TP.Levels;
namespace TGC.MonoGame.TP.ObstaculoPozo
{
    public class ObstaculosPozos
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(2f);
        public Model ModeloPozo { get; set; }
        private BoundingBox PozoBox { get; set; }
        private Vector3 desplazamientoEnEjes { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private List<Matrix> _pozos { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        float escala = 2f;
        BoundingBox size;
        private Texture2D Textura { get; set; }
        private Texture2D NormalTextura { get; set; }

        private BoundingFrustum _frustum;

        public ObstaculosPozos(Matrix view, Matrix projection)
        {
            Initialize(view, projection);
        }

        private void Initialize(Matrix view, Matrix projection)
        {
            _pozos = new List<Matrix>();
            Colliders = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);

        }

        public void IniciarColliders()
        {
            
        }


        public void LoadContent(ContentManager Content)
        {
            ModeloPozo = Content.Load<Model>("Models/" + "obstaculos/rockLargeWithTexture");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");
            Textura = Content.Load<Texture2D>("Textures/texturaRoca");
            NormalTextura = Content.Load<Texture2D>("Textures/NormalMapRoca");

            foreach (var mesh in ModeloPozo.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }
            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloPozo);
        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection)
        {
            for (int i = 0; i < _pozos.Count; i++) {
                if (_envolturaEsfera.Intersects(Colliders[i])){
                    Game.Respawn();
                }
            }
            _frustum = new BoundingFrustum(view * projection * scale);
        }

        public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            var viewProjection = view * projection;

            foreach (var worldMatrix in _pozos)
            {
                foreach (var mesh in ModeloPozo.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;

                    Vector3 transformedMin = Vector3.Transform(size.Min, worldMatrix);
                    Vector3 transformedMax = Vector3.Transform(size.Max, worldMatrix);

                    BoundingBox boundingBox = new BoundingBox(transformedMin, transformedMax);

                    //if (_frustum.Intersects(boundingBox))
                    ShadowMapEffect.Parameters["ambientColor"].SetValue(new Vector3(0.3f, 0.3f, 0.3f));
                    ShadowMapEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));
                    ShadowMapEffect.Parameters["specularColor"].SetValue(new Vector3(0.6f, 0.6f, 0.6f));
                    ShadowMapEffect.Parameters["shininess"].SetValue(16f);
                    ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                    ShadowMapEffect.Parameters["baseTexture"].SetValue(Textura);
                    ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                    ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));
                    ShadowMapEffect.Parameters["normalMap"].SetValue(NormalTextura);

                    mesh.Draw();
                    //}
                }
            }
        }


        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {

            foreach (var worldMatrix in _pozos)
            {
                foreach (var modelMesh in ModeloPozo.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloPozo.Bones.Count];
                    ModeloPozo.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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
            PozoBox = BoundingVolumesExtensions.CreateAABBFrom(ModeloPozo);
            desplazamientoEnEjes = PozoBox.Max - PozoBox.Min; // aca consigo el tamaño el largo de la pista para que coincida son 3/4, el ancho es el mismo.
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X, 0, 0);

            Console.WriteLine($"Pozo: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");
            return desplazamientoEnEjes;
        }

        
        public void agregarNuevoPozo(float Rotacion, Vector3 Posicion, Materiales _materiales)
        {
            _materiales._muros.AgregarMurosPozo(Rotacion, Posicion);

            Posicion = new Vector3(Posicion.X / 67, Posicion.Y / 100, Posicion.Z / 67);

            Posicion += Vector3.Transform(new Vector3(1,-14,0), Matrix.CreateRotationY(Rotacion));
            Matrix transform = Matrix.CreateRotationY(Rotacion) *  Matrix.CreateTranslation(Posicion) *Matrix.CreateScale(escala);


            _pozos.Add(transform);

            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            BoundingBox box = new BoundingBox(transformedMin, transformedMax);
             box = new BoundingBox(size.Min * escala + Posicion * escala, size.Max * escala + Posicion * escala);

            Colliders.Add(box);
        }

    }
}