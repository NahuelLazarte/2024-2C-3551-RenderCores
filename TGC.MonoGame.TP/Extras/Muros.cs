using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Aseg�rate de tener esta directiva

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;
using Microsoft.Xna.Framework.Audio;

using System;
using System.Linq;
using TGC.MonoGame.TP.Levels; // Aseg�rate de que esto est� presente en la parte superior de tu archivo


namespace TGC.MonoGame.TP.MurosExtra{
    public class Muros{
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        private BasicEffect Efecto { get; set; }
        private Texture2D Texture { get; set; }
        private Texture2D TexturePasto { get; set; }

        private Texture2D NormalTexture { get; set; }
        private Texture2D NormalTexturePasto { get; set; }

        private Texture2D metallicTexture { get; set; }
        private Texture2D roughnessTexture { get; set; }
        private Texture2D aoTexture { get; set; }
        public Model ModeloMuro { get; set; }
        public Model ModeloMuroPared { get; set; }
        public Model ModeloMuroEsquina { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _muros { get; set; }
        private List<Matrix> _murosEsquina { get; set; }
        private List<Matrix> _murosPared { get; set; }
        private List<Matrix> _pasto { get; set; }

        public BoundingSphere _envolturaEsfera { get; set; }
        public SoundEffect CollisionSound { get; set; }
        BoundingBox MuroSize;
        BoundingBox MuroEsquinaSize;
        BoundingBox MuroParedSize;
        //3f
        float escalaMuros = 0.03f;
        //10f

        //0.09
        float escalaMurosEsquina = 0.09f;
        float escalaMurosPared = 0.03f;

        private BoundingFrustum _frustumMuros;
        private BoundingFrustum _frustumEsquinas;
        private BoundingFrustum _frustumParedes;
        private float time;

        public Muros(BoundingFrustum frustrum){
            Initialize();
            time = 0;
            _frustumMuros = frustrum;
            _frustumEsquinas = frustrum;
            _frustumParedes = frustrum;
        }

        private void Initialize(){
            _muros = new List<Matrix>();
            _murosEsquina = new List<Matrix>();
            _murosPared = new List<Matrix>();
            _pasto = new List<Matrix>();

            Colliders = new List<BoundingBox>();
        }

        public void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice){
            Effect = Content.Load<Effect>("Effects/" + "BasicShader2");
            Texture = Content.Load<Texture2D>("Textures/texturaPiedra");
            TexturePasto = Content.Load<Texture2D>("Textures/texturaPasto");

            NormalTexture = Content.Load<Texture2D>("Textures/NormalMapPiedra");
            NormalTexturePasto = Content.Load<Texture2D>("Textures/NormalMapPasto");

            /*metallicTexture = Content.Load<Texture2D>("Textures/SpecularMapPiedra");
            roughnessTexture = Content.Load<Texture2D>("Textures/DisplacementMapPiedra");
            aoTexture = Content.Load<Texture2D>("Textures/AmbientOcclusionMapPiedra");*/
            CollisionSound = Content.Load<SoundEffect>("Audio/ColisionPez");

            //Ponerle el efecto/shader a las paredes rectas
            //ModeloMuro = Content.Load<Model>("Models/" + "Muros/wallHalf");
            ModeloMuro = Content.Load<Model>("Models/" + "Muros/wallHalfWithoutTextureCentered");
            foreach (var mesh in ModeloMuro.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            //Ponerle el efecto/shader a las paredes curvas
            ModeloMuroEsquina = Content.Load<Model>("Models/" + "Muros/wallCornerCentered");
            foreach (var mesh in ModeloMuroEsquina.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            ModeloMuroPared = Content.Load<Model>("Models/" + "Muros/towerSquareBaseWithTexture");
            foreach (var mesh in ModeloMuroPared.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            MuroSize = BoundingVolumesExtensions.CreateAABBFrom(ModeloMuro);
            MuroEsquinaSize = BoundingVolumesExtensions.CreateAABBFrom(ModeloMuroEsquina);
            MuroParedSize = BoundingVolumesExtensions.CreateAABBFrom(ModeloMuroPared);

        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection, BoundingFrustum frustum)
        {

            for (int i = 0; i < _muros.Count + _murosEsquina.Count; i++)
            {
                if (_envolturaEsfera.Intersects(Colliders[i]))
                {
                    Game.Respawn(); //TODAVIA NO FUNCIONA BIEN EL POSICIONAMIENTO DE LOS COLLIDERS
                    //CollisionSound.Play();
                    Console.WriteLine("Colisión detectada con el muro");
                    break;
                }
            }
            _frustumEsquinas = new BoundingFrustum(view * projection * Matrix.CreateScale(escalaMurosEsquina));
            _frustumMuros = new BoundingFrustum(view * projection * Matrix.CreateScale(escalaMuros));
            _frustumParedes = new BoundingFrustum(view * projection * Matrix.CreateScale(escalaMurosPared));

        }



        public void Draw(GameTime gameTime, Effect ShadowMapEffect, Matrix view, Matrix projection)
        {
            var viewProjection = view * projection;

            foreach (var worldMatrix in _muros)
            {
                foreach (var mesh in ModeloMuro.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustumMuros.Intersects(boundingBox))
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
            foreach (var worldMatrix in _murosEsquina)
            {
                foreach (var mesh in ModeloMuroEsquina.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustumEsquinas.Intersects(boundingBox))
                    {
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        ShadowMapEffect.Parameters["baseTexture"].SetValue(Texture);
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));
                        ShadowMapEffect.Parameters["normalMap"].SetValue(NormalTexture);

                        mesh.Draw();
                    }
                }
            }

            foreach (var worldMatrix in _murosPared)
            {
                foreach (var mesh in ModeloMuroPared.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustumParedes.Intersects(boundingBox))
                    {
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        ShadowMapEffect.Parameters["baseTexture"].SetValue(Texture);
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));
                        ShadowMapEffect.Parameters["normalMap"].SetValue(NormalTexture);

                        mesh.Draw();
                    }
                }
            }

            foreach (var worldMatrix in _pasto)
            {
                foreach (var mesh in ModeloMuro.Meshes)
                {
                    var meshWorld = mesh.ParentBone.Transform * worldMatrix;
                    var boundingBox = BoundingVolumesExtensions.FromMatrix(meshWorld);

                    if (_frustumParedes.Intersects(boundingBox))
                    {
                        ShadowMapEffect.Parameters["World"].SetValue(meshWorld);
                        ShadowMapEffect.Parameters["baseTexture"].SetValue(TexturePasto);
                        ShadowMapEffect.Parameters["WorldViewProjection"].SetValue(meshWorld * viewProjection);
                        ShadowMapEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));
                        ShadowMapEffect.Parameters["normalMap"].SetValue(NormalTexturePasto);

                        mesh.Draw();
                    }
                }
            }
        }


        public void ShadowMapRender(Effect ShadowMapEffect, Matrix LightView, Matrix Projection)
        {

            foreach (var worldMatrix in _muros)
            {
                foreach (var modelMesh in ModeloMuro.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloMuro.Bones.Count];
                    ModeloMuro.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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
            foreach (var worldMatrix in _murosEsquina)
            {
                foreach (var modelMesh in ModeloMuroEsquina.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloMuroEsquina.Bones.Count];
                    ModeloMuroEsquina.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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
            foreach (var worldMatrix in _murosPared)
            {
                foreach (var modelMesh in ModeloMuroPared.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloMuroPared.Bones.Count];
                    ModeloMuroPared.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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

            foreach (var worldMatrix in _pasto)
            {
                foreach (var modelMesh in ModeloMuro.Meshes)
                {
                    var modelMeshesBaseTransforms = new Matrix[ModeloMuro.Bones.Count];
                    ModeloMuro.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

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

        public void AgregarMurosPistaRecta(float Rotacion, Vector3 Posicion) {
            //var posicionMuros = new Vector3(Posicion.X / 100f, Posicion.Y / 100f, Posicion.Z / 100f);
            var posicionMuros = new Vector3(Posicion.X, Posicion.Y, Posicion.Z);
            var posicionPasto = new Vector3(Posicion.X / 33.5f, Posicion.Y / 1000f, Posicion.Z / 33.5f);

            //var desplazamientoDerecha = new Vector3(25.22f, -12f, 9f);
            var desplazamientoDerecha = new Vector3(0f, -11.63f * 100, -10f * 100);
            //var desplazamientoIzquierda = new Vector3(-25.22f, -12f, -9f);
            var desplazamientoIzquierda = new Vector3(-0f, -11.63f * 100, 10f * 100);
            var desplazamientoAbajo = new Vector3(-0f, -5.95f * 100, 0f);
            var desplazamientoAbajoPastoIzquierda = new Vector3(0f, -35f, -116f);
            var desplazamientoAbajoPastoDerecha = new Vector3(0f, -35f, 116f);


            // Calcular las posiciones de los muros aplicando la rotación
            var posicionDerecha = posicionMuros + Vector3.Transform(desplazamientoDerecha, Matrix.CreateRotationY(Rotacion));
            var posicionIzquierda = posicionMuros + Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));
            var posicionAbajo = posicionMuros + Vector3.Transform(desplazamientoAbajo, Matrix.CreateRotationY(Rotacion));
            var posicionAbajoPastoIzquierda = posicionPasto + Vector3.Transform(desplazamientoAbajoPastoIzquierda, Matrix.CreateRotationY(Rotacion));
            var posicionAbajoPastoDerecha = posicionPasto + Vector3.Transform(desplazamientoAbajoPastoDerecha, Matrix.CreateRotationY(Rotacion));

            // Crear las matrices de transformación para los muros
            Matrix muroDerecha = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(posicionDerecha) * Matrix.CreateScale(escalaMuros);
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) * Matrix.CreateTranslation(posicionIzquierda) * Matrix.CreateScale(escalaMuros);
            Matrix muroAbajo = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) * Matrix.CreateTranslation(posicionAbajo) * Matrix.CreateScale(escalaMurosPared);

            Matrix pastoAbajoIzquierda = Matrix.CreateScale(new Vector3(escalaMuros, escalaMuros / 20, escalaMuros * 5)) * Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(posicionAbajoPastoIzquierda);
            Matrix pastoAbajoDerecha = Matrix.CreateScale(new Vector3(escalaMuros, escalaMuros / 20, escalaMuros * 5)) * Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(posicionAbajoPastoDerecha);


            _muros.Add(muroDerecha);
            _muros.Add(muroIzquierda);
            _murosPared.Add(muroAbajo);

            _pasto.Add(pastoAbajoIzquierda);
            _pasto.Add(pastoAbajoDerecha);

            // Crear y agregar los BoundingBox
            BoundingBox boxDerecha = CreateTransformedBoundingBox(muroDerecha, MuroSize, 5.0f);
            Colliders.Add(boxDerecha);

            BoundingBox boxIzquierda = CreateTransformedBoundingBox(muroIzquierda, MuroSize, 5.0f);
            Colliders.Add(boxIzquierda);

            BoundingBox boxAbajo = CreateTransformedBoundingBox(muroAbajo, MuroParedSize, 1f);
            Colliders.Add(boxAbajo);
        }
        public void AgregarMurosPistaCurvaIzquierda(float Rotacion, Vector3 Posicion)
        {
            float a = 3f;
            var posicionMuros = new Vector3(Posicion.X / a, Posicion.Y, Posicion.Z / a);
            var posicionPasto = new Vector3(Posicion.X / 33.5f, Posicion.Y / 1000f, Posicion.Z / 33.5f);

            float b = 175f;
            var desplazamientoIzquierda = new Vector3(b, -12f * 100, b);
            var desplazamientoAbajoPasto = new Vector3(84.5f, -35f, 85.5f);

            // Calcular las posiciones de los muros aplicando la rotación
            var posicionIzquierda = posicionMuros + Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));
            var posicionAbajoPasto = posicionPasto + Vector3.Transform(desplazamientoAbajoPasto, Matrix.CreateRotationY(Rotacion));

            // Crear las matrices de transformación para los muros
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-270)) * Matrix.CreateTranslation(posicionIzquierda) * Matrix.CreateScale(escalaMurosEsquina);
            Matrix pastoAbajo = Matrix.CreateScale(new Vector3(escalaMuros * 8, escalaMuros / 20, escalaMuros * 8)) * Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(posicionAbajoPasto);


            _murosEsquina.Add(muroIzquierda);
            _pasto.Add(pastoAbajo);

            BoundingBox boxIzquierda = CreateTransformedBoundingBox(muroIzquierda, MuroEsquinaSize, 14.0f);
            Colliders.Add(boxIzquierda);
        }
        public void AgregarMurosPistaCurvaDerecha(float Rotacion, Vector3 Posicion) {
            float a = 3f;

            var posicionMuros = new Vector3(Posicion.X / a, Posicion.Y, Posicion.Z / a);
            var posicionPasto = new Vector3(Posicion.X / 33.5f, Posicion.Y / 1000f, Posicion.Z / 33.5f);


            float b = 175f;
            var desplazamientoDerecha = new Vector3(b, -12f * 100, -b);
            var desplazamientoAbajoPasto = new Vector3(84.5f, -35f, -85.5f);

            // Calcular las posiciones de los muros aplicando la rotación
            var posicionDerecha = posicionMuros + Vector3.Transform(desplazamientoDerecha, Matrix.CreateRotationY(Rotacion));
            var posicionAbajoPasto = posicionPasto + Vector3.Transform(desplazamientoAbajoPasto, Matrix.CreateRotationY(Rotacion));

            // Crear las matrices de transformación para los muros
            Matrix muroDerecha = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-180)) * Matrix.CreateTranslation(posicionDerecha) * Matrix.CreateScale(escalaMurosEsquina);
            Matrix pastoAbajo = Matrix.CreateScale(new Vector3(escalaMuros * 8, escalaMuros / 20, escalaMuros * 8)) * Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(posicionAbajoPasto);

            _murosEsquina.Add(muroDerecha);
            _pasto.Add(pastoAbajo);

            BoundingBox boxDerecha = CreateTransformedBoundingBox(muroDerecha, MuroEsquinaSize, 14.0f);
            Colliders.Add(boxDerecha);
        }

        
        public void AgregarMurosPozo(float Rotacion, Vector3 Posicion) {
            

            var posicionMuros = new Vector3(Posicion.X, Posicion.Y, Posicion.Z);
            var posicionPasto = new Vector3(Posicion.X / 33.5f, Posicion.Y / 1000f, Posicion.Z / 33.5f);


            var desplazamientoDerecha = new Vector3(0f, -11.48f * 100, -10f * 100) ;
            var desplazamientoIzquierda = new Vector3(0f, -11.48f * 100, 10f * 100);
            var desplazamientoAbajo = new Vector3(-0f, -20f * 1000, 0f);
            var desplazamientoAbajoPastoIzquierda = new Vector3(0f, -35f, -116f);
            var desplazamientoAbajoPastoDerecha = new Vector3(0f, -35f, 116f);

            var posicionDerecha = posicionMuros + Vector3.Transform(desplazamientoDerecha, Matrix.CreateRotationY(Rotacion));
            var posicionIzquierda = posicionMuros + Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));
            var posicionAbajo = posicionMuros + Vector3.Transform(desplazamientoAbajo, Matrix.CreateRotationY(Rotacion));
            var posicionAbajoPastoIzquierda = posicionPasto + Vector3.Transform(desplazamientoAbajoPastoIzquierda, Matrix.CreateRotationY(Rotacion));
            var posicionAbajoPastoDerecha = posicionPasto + Vector3.Transform(desplazamientoAbajoPastoDerecha, Matrix.CreateRotationY(Rotacion));


            Matrix muroDerecha = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(posicionDerecha) * Matrix.CreateScale(escalaMuros);
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) * Matrix.CreateTranslation(posicionIzquierda) * Matrix.CreateScale(escalaMuros);
            Matrix muroAbajo = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) * Matrix.CreateTranslation(posicionAbajo) * Matrix.CreateScale(new Vector3(escalaMurosPared, escalaMurosPared/20, escalaMurosPared));
            Matrix pastoAbajoIzquierda = Matrix.CreateScale(new Vector3(escalaMuros, escalaMuros / 20, escalaMuros * 5)) * Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(posicionAbajoPastoIzquierda);
            Matrix pastoAbajoDerecha = Matrix.CreateScale(new Vector3(escalaMuros, escalaMuros / 20, escalaMuros * 5)) * Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(posicionAbajoPastoDerecha);


            _muros.Add(muroDerecha);
            _muros.Add(muroIzquierda);
            _murosPared.Add(muroAbajo);
            _pasto.Add(pastoAbajoIzquierda);
            _pasto.Add(pastoAbajoDerecha);
            // Crear y agregar los BoundingBox
            BoundingBox boxDerecha = CreateTransformedBoundingBox(muroDerecha, MuroSize, 5.0f);
            Colliders.Add(boxDerecha);

            BoundingBox boxIzquierda = CreateTransformedBoundingBox(muroIzquierda, MuroSize, 5.0f);
            Colliders.Add(boxIzquierda);

            BoundingBox boxAbajo = CreateTransformedBoundingBox(muroAbajo, MuroParedSize, 1f);
            Colliders.Add(boxAbajo);
        }

        private BoundingBox CreateTransformedBoundingBox(Matrix transform, BoundingBox size, float yDecrement) {
            // funcion de google, basicamente separa en modelo en esquinas, asigna el tipo que es cada esquina (maximo o minimo en el eje)
            // una vez asigna las propiedades, aplica la transformacion sobre la matriz, es decir saca los valorez de la matriz
            // al sacarlos pone los maximos y minimos en cada indice en base a los maximos y minimos predefinidos
            // una vez seteadas las 8 esquinas, agarra todos los minimos y los posiciona en el vector newMin, todos lo maximos en newMax

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

            for (int i = 0; i < corners.Length; i++) {
                corners[i] = Vector3.Transform(corners[i], transform);
            }            

            Vector3 newMin = new Vector3(float.MaxValue);
            Vector3 newMax = new Vector3(float.MinValue);

            foreach (var corner in corners) {
                newMin = Vector3.Min(newMin, corner);
                newMax = Vector3.Max(newMax, corner);
            }

            //Agrego esto asi achico el boundingBox en el eje Y que queda muy alto
             
            newMax.Y -= yDecrement;

            return new BoundingBox(newMin, newMax);
        }

    }
}
