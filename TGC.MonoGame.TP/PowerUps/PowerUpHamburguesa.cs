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


namespace TGC.MonoGame.TP.PowerUpHamburguesa
{
    public class PowerUpHamburguesas
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(5f);
        public Model ModeloHamburguesa { get; set; }
        public BoundingBox[] Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _hamburguesas { get; set; }
        public BoundingSphere _envolturaEsfera { get; set; }
        public SoundEffect CollisionSound { get; set; }
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private int hamburguesasCount;
        private BoundingFrustum _frustum;

        protected Texture TexturaCarne { get; set; }
        protected Texture TexturaLechuga { get; set; }
        protected Texture TexturaPan { get; set; }
        protected Texture TexturaQueso { get; set; }
        protected Texture TexturaTomate { get; set; }

        public PowerUpHamburguesas(Matrix view, Matrix projection)
        {
            Initialize(view, projection);
        }

        private void Initialize(Matrix view, Matrix projection)
        {
            _hamburguesas = new List<Matrix>();
            hamburguesasCount = 0;
            _frustum = new BoundingFrustum(view * projection);
        }

        public void IniciarColliders()
        {
            Colliders = new BoundingBox[_hamburguesas.Count];

            for (int i = 0; i < _hamburguesas.Count; i++)
            {
                Colliders[i] = BoundingVolumesExtensions.FromMatrix(_hamburguesas[i]);
            }

        }

        public void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            ModeloHamburguesa = Content.Load<Model>("Models/" + "PowerUps/burgerWithTexture");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");
            TexturaCarne = Content.Load<Texture2D>("Textures/carne");
            TexturaPan = Content.Load<Texture2D>("Textures/pan");
            TexturaLechuga = Content.Load<Texture2D>("Textures/lechuga");
            TexturaQueso = Content.Load<Texture2D>("Textures/queso");
            TexturaTomate = Content.Load<Texture2D>("Textures/tomate");
            
            spriteBatch = new SpriteBatch(graphicsDevice); // Inicializa SpriteBatch
            spriteFont = Content.Load<SpriteFont>("SpriteFonts/" + "CascadiaCodePl");

            foreach (var mesh in ModeloHamburguesa.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            CollisionSound = Content.Load<SoundEffect>("Audio/ColisionPez");
        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection)
        {

            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            float sinOffset = (float)Math.Sin(Rotation) * 0.8f; // multiplicador para la amplitud

            for (int i = 0; i < _hamburguesas.Count; i++)
            {
                var originalPosition = _hamburguesas[i].Translation; // Obtener la posición original
                _hamburguesas[i] = Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(originalPosition.X, originalPosition.Y + (sinOffset) * 0.05f, originalPosition.Z);


                // Comprobar colisión
                var fishBoundingSphere = new BoundingSphere(originalPosition, scale.Translation.X); // Ajustar el tamaño de la esfera de colisión según sea necesario
                if (_envolturaEsfera.Intersects(fishBoundingSphere))
                {
                    // Acción al tocar el modelo
                    Console.WriteLine($"¡Colisión con el pez en la posición {originalPosition}!");

                    CollisionSound.Play();
                    _hamburguesas.RemoveAt(i);
                    hamburguesasCount++;
                    Game.recibirPowerUpPez();
                }
                _frustum = new BoundingFrustum(view * projection);

            }

        }

        /*public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection, GraphicsDevice graphicsDevice){
            var viewProjection = view * projection;

            foreach (var mesh in ModeloHamburguesa.Meshes){

                for (int i = 0; i < _hamburguesas.Count; i++)
                {
                    Matrix _pisoWorld = _hamburguesas[i];
                    var meshWorld = mesh.ParentBone.Transform * _pisoWorld;
                    BoundingBox boundingBox = BoundingVolumesExtensions.FromMatrix(_pisoWorld);

                    if (_frustum.Intersects(boundingBox)){
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        string meshName = mesh.Name.ToLower();
                        switch (meshName){
                            case "bunbottom":
                                ShadowMapEffect.Parameters["Texture"]?.SetValue(TexturaPan);
                                break;
                            case "buntop":
                                ShadowMapEffect.Parameters["Texture"]?.SetValue(TexturaPan);
                                break;
                            case "cheese":
                                ShadowMapEffect.Parameters["Texture"]?.SetValue(TexturaQueso);
                                break;
                            case "patty":
                                ShadowMapEffect.Parameters["Texture"]?.SetValue(TexturaCarne);
                                break;
                            case "salad":
                                ShadowMapEffect.Parameters["Texture"]?.SetValue(TexturaLechuga);
                                break;
                            case "tomato":
                                ShadowMapEffect.Parameters["Texture"]?.SetValue(TexturaTomate);
                                break;
                            default:
                                ShadowMapEffect.Parameters["Texture"]?.SetValue(TexturaPan);
                                break;
                        }
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));
                        mesh.Draw();
                    }
                }
            }

            var originalRasterizerState = graphicsDevice.RasterizerState;
            var originalBlendState = graphicsDevice.BlendState;
            var originalDepthStencilState = graphicsDevice.DepthStencilState;
            var originalSamplerState = graphicsDevice.SamplerStates[0]; // Guarda el primer sampler state

            // Modifica aquí el estado de renderizado según sea necesario
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, $"Hamburguesas: {hamburguesasCount}", new Vector2(10, 10), Color.White);
            spriteBatch.End();

            // Restaura los estados originales
            graphicsDevice.RasterizerState = originalRasterizerState;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.DepthStencilState = originalDepthStencilState;
            graphicsDevice.SamplerStates[0] = originalSamplerState; // Restaura el sampler state
        }*/

        public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection, GraphicsDevice graphicsDevice)
        {
            var viewProjection = view * projection;

            foreach (var worldMatrix in _hamburguesas)
            {
                foreach (var mesh in ModeloHamburguesa.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustum.Intersects(boundingBox))
                    {
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        string meshName = mesh.Name.ToLower();
                        ShadowMapEffect.Parameters["ambientColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));
                        ShadowMapEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.6f, 0.6f, 0.6f));
                        ShadowMapEffect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
                        ShadowMapEffect.Parameters["shininess"].SetValue(32f);
                        switch (meshName){
                            case "bunbottom":
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaPan);
                                break;
                            case "buntop":
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaPan);
                                break;
                            case "cheese":
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaQueso);
                                break;
                            case "patty":
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaCarne);
                                break;
                            case "salad":
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaLechuga);
                                break;
                            case "tomato":
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaTomate);
                                break;
                            default:
                                ShadowMapEffect.Parameters["baseTexture"]?.SetValue(TexturaPan);
                                break;
                        }
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));

                        mesh.Draw();
                    }
                }
            }

            var originalRasterizerState = graphicsDevice.RasterizerState;
            var originalBlendState = graphicsDevice.BlendState;
            var originalDepthStencilState = graphicsDevice.DepthStencilState;
            var originalSamplerState = graphicsDevice.SamplerStates[0]; // Guarda el primer sampler state

            // Modifica aquí el estado de renderizado según sea necesario
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, $"Hamburguesas: {hamburguesasCount}", new Vector2(10, 10), Color.White);
            spriteBatch.End();

            // Restaura los estados originales
            graphicsDevice.RasterizerState = originalRasterizerState;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.DepthStencilState = originalDepthStencilState;
            graphicsDevice.SamplerStates[0] = originalSamplerState; // Restaura el sampler state
        }
        

        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {
            
            foreach (var worldMatrix in _hamburguesas)
            {
                foreach (var modelMesh in ModeloHamburguesa.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloHamburguesa.Bones.Count];
                    ModeloHamburguesa.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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

        public void AgregarNuevoPowerUp(float Rotacion, Vector3 Posicion)
        {
            var transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale;
            _hamburguesas.Add(transform);
            Console.WriteLine($"Drawing fish at position {Posicion}");
        }

    }
}
