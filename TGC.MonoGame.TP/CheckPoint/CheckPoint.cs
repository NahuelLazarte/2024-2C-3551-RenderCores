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


namespace TGC.MonoGame.TP.CheckPoint{
    public class CheckPoints{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(1f);
        public Model ModeloCheckPoint { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _checkPoints { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public SoundEffect CollisionSound { get; set; }
        BoundingBox size;
        private BoundingFrustum _frustum;
        public int checkpointActual;
        public bool touchedLastCheckpoint = false;

        private Texture2D NormalMapTexturaMetal { get; set; }
        private Texture2D NormalMapTexturaMadera { get; set; }
        private Texture2D NormalMapTexturaPiedra { get; set; }
        private Texture2D NormalMapTexturaFuego { get; set; }
        private Texture2D TexturaMadera { get; set; }
        private Texture2D TexturaMetal { get; set; }
        private Texture2D TexturaPiedra { get; set; }
        private Texture2D TexturaFuego { get; set; }

        public CheckPoints(Matrix view, Matrix projection) {
            Initialize(view,projection);
        }

        private void Initialize(Matrix view, Matrix projection) {
            _checkPoints = new List<Matrix>();
            Colliders = new List<BoundingBox>();
            _frustum = new BoundingFrustum(view * projection);
            checkpointActual = 0;
        }

        public void IniciarColliders() {
          
            
        }

        public void LoadContent(ContentManager Content){
            ModeloCheckPoint = Content.Load<Model>("Models/" + "CheckPoint/campfireWithTextures"); // HAY QUE MOVERLO DE CARPETA
            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");

            TexturaMadera = Content.Load<Texture2D>("Textures/texturaMadera");
            TexturaMetal = Content.Load<Texture2D>("Textures/texturaMetal");
            TexturaPiedra = Content.Load<Texture2D>("Textures/texturaPiedra");
            TexturaFuego = Content.Load<Texture2D>("Textures/texturaFuego");

            NormalMapTexturaMetal = Content.Load<Texture2D>("Textures/NormalMapMetal");
            NormalMapTexturaMadera = Content.Load<Texture2D>("Textures/NormalMapMadera");
            NormalMapTexturaPiedra = Content.Load<Texture2D>("Textures/NormalMapPiedra");
            NormalMapTexturaFuego = Content.Load<Texture2D>("Textures/NormalMapFuego");

            foreach (var mesh in ModeloCheckPoint.Meshes){
                Console.WriteLine($"Meshname pistacurva {mesh.Name}");
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            CollisionSound = Content.Load<SoundEffect>("Audio/CheckPoint"); // Ajusta la ruta según sea necesario

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloCheckPoint);


        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection) {
            
            //Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            
            //float sinOffset = (float)Math.Sin(Rotation) * 0.8f; // Ajusta el multiplicador para la amplitud

            for (int i = 0; i < _checkPoints.Count; i++) {
                var originalPosition = _checkPoints[i].Translation;
                
                if (_envolturaEsfera.Intersects(Colliders[i])) {
                    // Acción al tocar el modelo
                    // Aquí puedes realizar la acción que desees, como eliminar el pez, reducir vida, etc.
                    CollisionSound.Play();
                    _checkPoints.RemoveAt(i);
                    Colliders.RemoveAt(i);
                    checkpointActual++;
                    Console.WriteLine($"Intercciono con CheckPoint {checkpointActual}!");
                    if (_checkPoints.Count == 0){
                        Game.nuevoCheckPoint(new Vector3(0f,0f,0f));
                        touchedLastCheckpoint = true;
                    } else {
                        Game.nuevoCheckPoint(originalPosition);
                    }
                    
                }
            }
            _frustum = new BoundingFrustum(view * projection);

        }
        /*
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Chocolate.ToVector3());

            
            foreach (var mesh in ModeloCheckPoint.Meshes){
                string meshName = mesh.Name.ToLower();
                for (int i=0; i < _checkPoints.Count; i++){
                    Matrix _pisoWorld = _checkPoints[i];
                    BoundingBox boundingBox = BoundingVolumesExtensions.FromMatrix(_pisoWorld);
                    
                    if(_frustum.Intersects(boundingBox)){
                        Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                        switch (meshName)
                        {
                            case "campfire":
                                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(1.0f, 0.5f, 0.0f)); // Color para el pan de abajo
                                break;
                            case "bucket":
                                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.7f, 0.7f, 0.7f)); // Color para el pan de arriba
                                break;
                            case "rocks":
                                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f)); // Color para el queso
                                break;
                            case "wood":
                                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.6f, 0.3f, 0.1f)); // Color para la carne
                                break;
                        }
                        mesh.Draw();
                    }
                }
            }
        }
        */
        public int getCheckPointActual(){
            return checkpointActual;
        }

        public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            var viewProjection = view * projection;

            foreach (var worldMatrix in _checkPoints)
            {
                foreach (var mesh in ModeloCheckPoint.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustum.Intersects(boundingBox))
                    {
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        string meshName = mesh.Name.ToLower();
                        switch (meshName)
                        {
                            
                            case "campfire":
                                ShadowMapEffect.Parameters["ambientColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));
                                ShadowMapEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.6f, 0.6f, 0.6f));
                                ShadowMapEffect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
                                ShadowMapEffect.Parameters["shininess"].SetValue(32f);
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaMadera);
                                ShadowMapEffect.Parameters["normalMap"].SetValue(NormalMapTexturaMadera);
                                
                                break;
                            case "bucket":
                                ShadowMapEffect.Parameters["ambientColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));
                                ShadowMapEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.6f, 0.6f, 0.6f));
                                ShadowMapEffect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
                                ShadowMapEffect.Parameters["shininess"].SetValue(32f);
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaMetal);
                                ShadowMapEffect.Parameters["normalMap"].SetValue(NormalMapTexturaMetal);
                                break;
                            case "rocks":
                                ShadowMapEffect.Parameters["ambientColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));
                                ShadowMapEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.6f, 0.6f, 0.6f));
                                ShadowMapEffect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
                                ShadowMapEffect.Parameters["shininess"].SetValue(32f);
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaPiedra);
                                ShadowMapEffect.Parameters["normalMap"].SetValue(NormalMapTexturaPiedra);
                                break;
                            case "wood":
                                ShadowMapEffect.Parameters["ambientColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));
                                ShadowMapEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.6f, 0.6f, 0.6f));
                                ShadowMapEffect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
                                ShadowMapEffect.Parameters["shininess"].SetValue(32f);
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaFuego);
                                ShadowMapEffect.Parameters["normalMap"].SetValue(NormalMapTexturaFuego);
                                break;
                        }
                        //ShadowMapEffect.Parameters["baseTexture"].SetValue(TexturaMadera);
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));

                        mesh.Draw();
                    }
                }
            }
        }


        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {

            foreach (var worldMatrix in _checkPoints)
            {
                foreach (var modelMesh in ModeloCheckPoint.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloCheckPoint.Bones.Count];
                    ModeloCheckPoint.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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


        public void AgregarNuevoCheckPoint(float Rotacion, Vector3 Posicion) {

            Posicion += Vector3.Transform(new Vector3(0, 0, -0.8f), Matrix.CreateRotationY(Rotacion));


            var transform = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(Posicion) * Matrix.CreateScale(5f); 
            _checkPoints.Add(transform);
            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            BoundingBox box = new BoundingBox(size.Min* 5f  + Posicion* 5f , size.Max* 5f + Posicion * 5f);

            Colliders.Add(box);
        }

    }
}
