using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Asegúrate de tener esta directiva

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Levels;

using Microsoft.Xna.Framework.Audio;

using System;
using TGC.MonoGame.TP.MaterialesJuego; // Asegúrate de que esto esté presente en la parte superior de tu archivo


namespace TGC.MonoGame.TP.CheckPoint{
    public class CheckPointFinal    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.03f);
        
        public Model ModeloPuerta { get; set; }
        public Model ModeloEscaleras { get; set; }
        private BoundingBox PuertaBox { get; set; }
        private List<Matrix> _muro { get; set; }
        private List<Matrix> _escalera { get; set; }
        public BoundingSphere _envolturaEsfera { get; set; }

        public List<BoundingBox> Colliders { get; set; }
        public List<BoundingBox> CollidersPuerta { get; set; }
        
        float escala = 0.08f;
        float escalaEscalera = 0.2f;

        BoundingBox puertaSize;
        private BoundingFrustum _frustum;

        protected Texture TextureMadera { get; set; }
        protected Texture TexturePiedra { get; set; }

        public CheckPointFinal(Matrix view, Matrix projection)
        {
            Initialize(view,projection);
        }

        private void Initialize(Matrix view, Matrix projection)
        {
            Colliders = new List<BoundingBox>();
            CollidersPuerta = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);
            _muro = new List<Matrix>();
            _escalera = new List<Matrix>();

        }

        public void LoadContent(ContentManager Content)
        {
            ModeloPuerta = Content.Load<Model>(ContentFolder3D + "Muros/wallDoorWithTexture");
            ModeloEscaleras = Content.Load<Model>(ContentFolder3D + "Muros/wallNarrowStairsFenceWithTexture");

            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");
            TexturePiedra = Content.Load<Texture2D>("Textures/texturaPiedra");
            TextureMadera = Content.Load<Texture2D>("Textures/texturaMadera");
            //Effect.CurrentTechnique = Effect.Techniques["LightingTechnique"];
            puertaSize = BoundingVolumesExtensions.CreateAABBFrom(ModeloPuerta);

        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection)
        {

            for (int i = 0; i < _muro.Count; i++)
            {
                if (_envolturaEsfera.Intersects(Colliders[i]))
                {
                    // Acción al tocar el modelo
                    //CollisionSound.Play();
                    //_obstaculosPiedras.RemoveAt(i);
                    Game.Respawn();
                }
            }
            _frustum = new BoundingFrustum(view * projection * scale);
        }
    

    public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            var viewProjection = view * projection;

            foreach (var worldMatrix in _muro)
            {
                foreach (var mesh in ModeloPuerta.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustum.Intersects(boundingBox))
                    {
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        ShadowMapEffect.Parameters["baseTexture"].SetValue(TexturePiedra);
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));

                        mesh.Draw();
                    }
                }
            }

            foreach (var worldMatrix in _escalera) //ACA HAY MESH PARA PINTAR UNA COSA DE MADERA Y OTRA DE PIEDRA
            {
                foreach (var mesh in ModeloEscaleras.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustum.Intersects(boundingBox))
                    {
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        ShadowMapEffect.Parameters["baseTexture"].SetValue(TexturePiedra);
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));

                        mesh.Draw();
                    }
                }
            }
        }


        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {

            foreach (var worldMatrix in _escalera)
            {
                foreach (var modelMesh in ModeloEscaleras.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloEscaleras.Bones.Count];
                    ModeloEscaleras.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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

            foreach (var worldMatrix in _muro)
            {
                foreach (var modelMesh in ModeloPuerta.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloPuerta.Bones.Count];
                    ModeloPuerta.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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
            return new Vector3(0f,0f,0f);
        }

        public float Rotacion()
        {
            return 0; // No hay rotacion
        }
        
        public void Final(float Rotacion, Vector3 Posicion, Materiales _materiales)
        {
            // Crear la matriz de transformación completa
            var posicionMuro = new Vector3(Posicion.X/ 2.65f, Posicion.Y / 1000f, Posicion.Z / 2.65f);
            var posicionEscalera = new Vector3(Posicion.X / 7f, Posicion.Y / 1000f, Posicion.Z / 7f);

            var desplazamientoMuro = new Vector3(4f * 100, 5f * 100, 0f);
            var desplazamientoEscalera = new Vector3(6.9f * 100, 6f * 100, -3f * 100);
            
            

            var posicionMuroFinal = posicionMuro + Vector3.Transform(desplazamientoMuro, Matrix.CreateRotationY(Rotacion));
            var posicionEscaleraFinal = posicionEscalera + Vector3.Transform(desplazamientoEscalera, Matrix.CreateRotationY(Rotacion));


            Matrix worldMuro = Matrix.CreateRotationY(Rotacion) *  Matrix.CreateTranslation(posicionMuroFinal) *Matrix.CreateScale(escala);
            Matrix worldEscalera = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(270))  * Matrix.CreateTranslation(posicionEscaleraFinal) * Matrix.CreateScale(escalaEscalera);


            _muro.Add(worldMuro);
            _escalera.Add(worldEscalera);

            BoundingBox box = new BoundingBox(puertaSize.Min * escala + posicionMuroFinal * escala , puertaSize.Max * escala + posicionMuroFinal * escala);

            Colliders.Add(box);

            _materiales._pistasRectas.agregarNuevaPista(Rotacion, Posicion, _materiales);

        }

    }
}