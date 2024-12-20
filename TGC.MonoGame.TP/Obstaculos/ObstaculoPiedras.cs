using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; 
using System; 
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;
using Microsoft.Xna.Framework.Audio;
using TGC.MonoGame.TP.Levels;


namespace TGC.MonoGame.TP.ObstaculoPiedras{
    public class ObstaculosPiedras{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(1f);
        public Model ModeloPiedra { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _obstaculosPiedras { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public SoundEffect CollisionSound { get; set; }
        BoundingBox size;
        private BoundingFrustum _frustum;
        private Texture2D Textura { get; set; }
        private Texture2D NormalTextura { get; set; }

        Random random = new Random();
        
        public ObstaculosPiedras(Matrix view, Matrix projection) {
            Initialize(view,projection);
        }

        private void Initialize(Matrix view, Matrix projection) {
            _obstaculosPiedras = new List<Matrix>();
            Colliders = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);
        }

        public void IniciarColliders() {
            
        }

        public void LoadContent(ContentManager Content){
            ModeloPiedra = Content.Load<Model>("Models/" + "obstaculos/rockLargeWithTexture");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");
            Textura = Content.Load<Texture2D>("Textures/texturaRoca");
            NormalTextura = Content.Load<Texture2D>("Textures/NormalMapRoca");
            foreach (var mesh in ModeloPiedra.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }
            
            CollisionSound = Content.Load<SoundEffect>("Audio/ColisionPez"); // Ajusta la ruta según sea necesario
            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloPiedra);
        }

        


        
        public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            var viewProjection = view * projection;

            foreach (var worldMatrix in _obstaculosPiedras)
            {
                foreach (var mesh in ModeloPiedra.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustum.Intersects(boundingBox))
                    {
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
                    }
                }
            }
        }
        

        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {
            
            foreach (var worldMatrix in _obstaculosPiedras)
            {
                foreach (var modelMesh in ModeloPiedra.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloPiedra.Bones.Count];
                    ModeloPiedra.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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



        /*public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            var viewProjection = view * projection;
            ShadowMapEffect.Parameters["Texture"].SetValue(Textura);

            foreach (var worldMatrix in _obstaculosPiedras)
            {
                foreach (var mesh in ModeloPiedra.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustum.Intersects(boundingBox))
                    {
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));

                        mesh.Draw();
                    }
                }
            }
        }*/


        
    

    public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection, Sphere esfera) {
          
            for (int i = 0; i < _obstaculosPiedras.Count; i++) {

                if (_envolturaEsfera.Intersects(Colliders[i])) {
                    
                    CollisionSound.Play();
                    if (esfera.rompeObjetos == true)
                    {
                        _obstaculosPiedras.RemoveAt(i);
                        Colliders.RemoveAt(i);
                    }
                    else
                    {
                        Game.Respawn();
                    }
                    // Acción al tocar el modelo
                    
                    //
                    
                }
            }
            _frustum = new BoundingFrustum(view * projection * scale);
        }

        public void AgregarNuevoObstaculo(float Rotacion, Vector3 Posicion)
        {
            int randomInt = random.Next(-10, 10);
            Posicion += Vector3.Transform(new Vector3(0, 0, randomInt), Matrix.CreateRotationY(Rotacion));
            var transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale;
            _obstaculosPiedras.Add(transform);

            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            //BoundingBox box = new BoundingBox(size.Min + Posicion, size.Max + Posicion);
            BoundingBox box = CreateTransformedBoundingBox(transform, size, 0, 7f);


            Colliders.Add(box);

        }


        private BoundingBox CreateTransformedBoundingBox(Matrix transform, BoundingBox size, float yDecrement, float xDecrement)
        {
            // Crear un arreglo de las esquinas del BoundingBox original
            Vector3[] corners = new Vector3[8];
            Vector3 min = size.Min;
            Vector3 max = size.Max;

            corners[0] = new Vector3(min.X, min.Y, min.Z);
            corners[1] = new Vector3(max.X, min.Y, min.Z);
            corners[2] = new Vector3(min.X, max.Y, min.Z);
            corners[3] = new Vector3(max.X, max.Y, min.Z);
            corners[4] = new Vector3(min.X, min.Y, max.Z);
            corners[5] = new Vector3(max.X, min.Y, max.Z);
            corners[6] = new Vector3(min.X, max.Y, max.Z);
            corners[7] = new Vector3(max.X, max.Y, max.Z);

            // Transformar las esquinas aplicando la matriz de transformación
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = Vector3.Transform(corners[i], transform);
            }

            // Inicializar los valores para calcular los nuevos mínimos y máximos
            Vector3 newMin = new Vector3(float.MaxValue);
            Vector3 newMax = new Vector3(float.MinValue);

            foreach (var corner in corners)
            {
                newMin = Vector3.Min(newMin, corner);
                newMax = Vector3.Max(newMax, corner);
            }

            // Reducir el tamaño del BoundingBox en los ejes especificados
            newMax.Y -= yDecrement; // Decrementar en el eje Y
            newMax.X -= xDecrement; // Decrementar en el eje X

            return new BoundingBox(newMin, newMax);
        }
    }
}
