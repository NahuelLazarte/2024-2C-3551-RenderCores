using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; 

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;
using Microsoft.Xna.Framework.Audio;
using System;
using TGC.MonoGame.TP.Levels;


namespace TGC.MonoGame.TP.PowerUpEspada{
    public class PowerUpEspadas{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(0.05f);
        float escala = 0.05f;
        public Model ModeloEspada { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _espadas { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public SoundEffect CollisionSound { get; set; }
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private BoundingFrustum _frustum;
        private float timer = 0f;
        private bool isTimerActive = false;
        protected Texture TexturaMetal { get; set; }
        protected Texture TexturaMadera { get; set; }
        public List<BoundingBox> Colliders { get; set; }

        BoundingBox size;

        public PowerUpEspadas(Matrix view, Matrix projection) {
            Initialize(view,projection);
        }

        private void Initialize(Matrix view, Matrix projection) {
            _espadas = new List<Matrix>();
            Colliders = new List<BoundingBox>();

            _frustum = new BoundingFrustum(view * projection);
        }

       

        public void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice){
            ModeloEspada = Content.Load<Model>("Models/" + "PowerUps/swordWithTexture"); 
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");
            TexturaMetal = Content.Load<Texture2D>("Textures/texturaMetal");
            TexturaMadera = Content.Load<Texture2D>("Textures/texturaMadera");
            
            spriteBatch = new SpriteBatch(graphicsDevice); // Inicializa SpriteBatch
            spriteFont = Content.Load<SpriteFont>("SpriteFonts/" + "CascadiaCodePl");



            foreach (var mesh in ModeloEspada.Meshes){
                Console.WriteLine($"Meshname espada {mesh.Name}");

                foreach (var meshPart in mesh.MeshParts){

                    meshPart.Effect = Effect;
                }
            }

            CollisionSound = Content.Load<SoundEffect>("Audio/ColisionPez");

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloEspada);

            Console.WriteLine(ModeloEspada != null ? "Modelo cargado exitosamente" : "Error al cargar el modelo");

        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection, Sphere esfera) {
            
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            float sinOffset = (float)Math.Sin(Rotation) * 0.8f; // multiplicador para la amplitud

            for (int i = 0; i < _espadas.Count; i++) {
                var originalPosition = _espadas[i].Translation; // Obtener la posición original
                _espadas[i] = scale * Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(originalPosition.X, originalPosition.Y + (sinOffset) * 0.05f, originalPosition.Z);

                // Comprobar colisión
                BoundingBox swordBoundingBox = new BoundingBox(size.Min * escala + originalPosition, size.Max * escala + originalPosition);

                if (_envolturaEsfera.Intersects(swordBoundingBox)) {
                    // Acción al tocar el modelo

                    CollisionSound.Play();
                    _espadas.RemoveAt(i);
                    Game.recibirPowerUpEspada();
                    isTimerActive = true;
                    timer = 0f; // Reiniciar el contador de tiempo
                }
            
            }

            if (isTimerActive)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds; // Incrementar el tiempo transcurrido
                if (timer >= 5f)
                {
                    isTimerActive = false; // Detener el temporizador
                    Game.finalizarPowerUpEspada(); // Llamar a la función
                }

                
            }

            _frustum = new BoundingFrustum(view * projection * scale);

        }


        

        public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection, GraphicsDevice graphicsDevice)
        {
            var viewProjection = view * projection;

            foreach (var worldMatrix in _espadas)
            {
                foreach (var mesh in ModeloEspada.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;

                    Vector3 transformedMin = Vector3.Transform(size.Min, worldMatrix);
                    Vector3 transformedMax = Vector3.Transform(size.Max, worldMatrix);

                    BoundingBox boundingBox = new BoundingBox(transformedMin, transformedMax);

                    //if (_frustum.Intersects(boundingBox))
                    //{
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        ShadowMapEffect.Parameters["ambientColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));
                        ShadowMapEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.6f, 0.6f, 0.6f));
                        ShadowMapEffect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
                        ShadowMapEffect.Parameters["shininess"].SetValue(32f);
                        ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaMetal);

                        /*
                        switch (meshName)
                        {
                            case "bunbottom":
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaPan);
                                break;
                        }*/

                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));
                        
                        mesh.Draw();
                    //}
                }
            }
            if (isTimerActive)
            {
                var originalRasterizerState = graphicsDevice.RasterizerState;
                var originalBlendState = graphicsDevice.BlendState;
                var originalDepthStencilState = graphicsDevice.DepthStencilState;
                var originalSamplerState = graphicsDevice.SamplerStates[0]; // Guarda el primer sampler state
                
                int tiempoRestante = (int)Math.Ceiling(5 - timer); // Ceil para evitar que se muestre 0 antes de tiempo

                // Modifica aquí el estado de renderizado según sea necesario
                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFont, $"Tiempo PowerUp Espada: {tiempoRestante}", new Vector2(10, 40), Color.White);
                spriteBatch.End();

                // Restaura los estados originales
                graphicsDevice.RasterizerState = originalRasterizerState;
                graphicsDevice.BlendState = originalBlendState;
                graphicsDevice.DepthStencilState = originalDepthStencilState;
                graphicsDevice.SamplerStates[0] = originalSamplerState; // Restaura el sampler state
            }

            
        }

        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {

            foreach (var worldMatrix in _espadas)
            {
                foreach (var modelMesh in ModeloEspada.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloEspada.Bones.Count];
                    ModeloEspada.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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

        public void AgregarNuevoPowerUp(float Rotacion, Vector3 Posicion) {
            
            var posicionEspada = new Vector3(Posicion.X /1.67f ,Posicion.Y / 2000f + 1f*100f, Posicion.Z / 1.67f);
            
            //var desplazamiento = new Vector3(0, 0 , 0);
            
            //var posicionFinal = posicionEspada + Vector3.Transform(desplazamiento, Matrix.CreateRotationY(Rotacion));

            var transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(posicionEspada) * scale ; 

            _espadas.Add(transform);

            BoundingBox box = new BoundingBox(size.Min * escala + posicionEspada * escala, size.Max * escala + posicionEspada * escala);

            Colliders.Add(box);

        }

    }
}
