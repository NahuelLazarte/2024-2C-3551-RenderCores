using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Aseg�rate de tener esta directiva

using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;

using System; // Aseg�rate de que esto est� presente en la parte superior de tu archivo


namespace TGC.MonoGame.TP.MurosExtra
{
    public class Muros
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        
        private BasicEffect Efecto { get; set; }
        private Texture2D Texture { get; set; }
        

        public Model ModeloMuro { get; set; }
        public Model ModeloMuroEsquina { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _muros { get; set; }
        private List<Matrix> _murosEsquina { get; set; }
        public BoundingSphere _envolturaEsfera { get; set; }
        public Song CollisionSound { get; set; }

        BoundingBox MuroSize;
        BoundingBox MuroEsquinaSize;

        float escalaMuros = 3f;
        float escalaMurosEsquina = 10f;

        public Muros()
        {
            Initialize();
        }

        private void Initialize()
        {
            _muros = new List<Matrix>();
            _murosEsquina = new List<Matrix>();
            Colliders = new List<BoundingBox>();
        }

        public void IniciarColliders()
        {

        }

        public void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            ModeloMuro = Content.Load<Model>("Models/" + "Muros/wallHalf");
            ModeloMuroEsquina = Content.Load<Model>("Models/" + "Muros/wallCornerSlant_exclusive");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");


            Texture = Content.Load<Texture2D>("Textures/texturaPiedra");

            Efecto = new BasicEffect(graphicsDevice);
            Efecto.TextureEnabled = true;
            Efecto.Texture = Texture;

            foreach (var mesh in ModeloMuro.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Efecto;
                }
            }
            // aca se puede cambiarla textura del efecto para darle otra onda
            foreach (var mesh in ModeloMuroEsquina.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Efecto;
                }
            }

            CollisionSound = Content.Load<Song>("Audio/ColisionPez"); // Ajusta la ruta seg�n sea necesario
            
            MuroSize = BoundingVolumesExtensions.CreateAABBFrom(ModeloMuro);
            MuroEsquinaSize = BoundingVolumesExtensions.CreateAABBFrom(ModeloMuroEsquina);

        }

        public void Update(GameTime gameTime, TGCGame Game)
        {

            for (int i = 0; i < _muros.Count + _murosEsquina.Count; i++)
            {
                if (_envolturaEsfera.Intersects(Colliders[i]))
                {
                    Game.Respawn(); //TODAVIA NO FUNCIONA BIEN EL POSICIONAMIENTO DE LOS COLLIDERS
                    MediaPlayer.Play(CollisionSound);
                    Console.WriteLine("Colisión detectada con el muro");
                    break;
                }
            }

        }




        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Efecto.Projection = projection;
            Efecto.View = view;


            // Dibujar los muros
            Effect.Parameters["DiffuseColor"].SetValue(Color.Gray.ToVector3()); // Color para los muros
            foreach (var mesh in ModeloMuro.Meshes)
            {
                for (int i = 0; i < _muros.Count; i++)
                {
                    Matrix _muroWorld = _muros[i];
                    //Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _muroWorld);
                    Efecto.World = mesh.ParentBone.Transform * _muroWorld;
                    mesh.Draw();  
                }
            }
            foreach (var mesh in ModeloMuroEsquina.Meshes)
            {
                for (int i = 0; i < _murosEsquina.Count; i++)
                {
                    Matrix _muroWorld = _murosEsquina[i];
                    //Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _muroWorld);
                    Efecto.World = mesh.ParentBone.Transform * _muroWorld;
                    mesh.Draw();  
                }
            }
        }

        public void AgregarMurosPistaRecta(float Rotacion, Vector3 Posicion) {
            var posicionMuros = new Vector3(Posicion.X / 100f, Posicion.Y / 100f, Posicion.Z / 100f);
            var desplazamientoDerecha = new Vector3(25.22f, -12f, 9f);
            var desplazamientoIzquierda = new Vector3(-25.22f, -12f, -9f);

            // Calcular las posiciones de los muros aplicando la rotación
            var posicionDerecha = posicionMuros + Vector3.Transform(desplazamientoDerecha, Matrix.CreateRotationY(Rotacion));
            var posicionIzquierda = posicionMuros + Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));

            // Crear las matrices de transformación para los muros
            Matrix muroDerecha = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(posicionDerecha) * Matrix.CreateScale(escalaMuros);
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) * Matrix.CreateTranslation(posicionIzquierda) * Matrix.CreateScale(escalaMuros);

            _muros.Add(muroDerecha);
            _muros.Add(muroIzquierda);

            // Crear y agregar los BoundingBox
            BoundingBox boxDerecha = CreateTransformedBoundingBox(muroDerecha, MuroSize, 5.0f);
            Colliders.Add(boxDerecha);

            BoundingBox boxIzquierda = CreateTransformedBoundingBox(muroIzquierda, MuroSize, 5.0f);
            Colliders.Add(boxIzquierda);
        }
        public void AgregarMurosPistaCurvaDerecha(float Rotacion, Vector3 Posicion) {
            var posicionMuros = new Vector3(Posicion.X / 334f, Posicion.Y / 250f, Posicion.Z / 334f);
            var desplazamientoIzquierda = new Vector3(-29.9f, -12f, +37.5f);

            // Calcular las posiciones de los muros aplicando la rotación
            var posicionIzquierda = posicionMuros + Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));

            // Crear las matrices de transformación para los muros
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-180)) * Matrix.CreateTranslation(posicionIzquierda) * Matrix.CreateScale(escalaMurosEsquina);

            _murosEsquina.Add(muroIzquierda);

            BoundingBox boxIzquierda = CreateTransformedBoundingBox(muroIzquierda, MuroEsquinaSize, 14.0f);
            Colliders.Add(boxIzquierda);
        }

        public void AgregarMurosPistaCurvaIzquierda(float Rotacion, Vector3 Posicion) {
            /*
            var posicionMuros = new Vector3(Posicion.X / 1.47f , (Posicion.Y + 15f)/  1.47f  , Posicion.Z/  1.47f );
            
            var desplazamientoDerecha = new Vector3(25.22f , -12f , 9f);
            var desplazamientoIzquierda = new Vector3(-25.22f , -12f, -9f);
            
            var posicionDerecha = posicionMuros + Vector3.Transform(desplazamientoDerecha, Matrix.CreateRotationY(Rotacion));
            var posicionIzquierda = posicionMuros + Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));

            Matrix muroDerecha = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(posicionDerecha) * Matrix.CreateScale(escalaMuros);
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) * Matrix.CreateTranslation(posicionIzquierda) * Matrix.CreateScale(escalaMuros);

            _muros.Add(muroDerecha);
            _muros.Add(muroIzquierda);

            // Crear y agregar los BoundingBox
            BoundingBox boxDerecha = CreateTransformedBoundingBox(muroDerecha);
            Colliders.Add(boxDerecha);

            BoundingBox boxIzquierda = CreateTransformedBoundingBox(muroIzquierda);
            Colliders.Add(boxIzquierda);
            */
        }

        public void AgregarMurosPozo(float Rotacion, Vector3 Posicion) {
            
            var posicionMuros = new Vector3(Posicion.X / 1.47f , (Posicion.Y + 15f)/  1.47f  , Posicion.Z/  1.47f );
            
            var desplazamientoDerecha = new Vector3(25.22f , -12f , 9f);
            var desplazamientoIzquierda = new Vector3(-25.22f , -12f, -9f);
            
            var posicionDerecha = posicionMuros + Vector3.Transform(desplazamientoDerecha, Matrix.CreateRotationY(Rotacion));
            var posicionIzquierda = posicionMuros + Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));

            Matrix muroDerecha = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(posicionDerecha) * Matrix.CreateScale(escalaMuros);
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) * Matrix.CreateTranslation(posicionIzquierda) * Matrix.CreateScale(escalaMuros);

            _muros.Add(muroDerecha);
            _muros.Add(muroIzquierda);

            // Crear y agregar los BoundingBox
            BoundingBox boxDerecha = CreateTransformedBoundingBox(muroDerecha, MuroSize, 5.0f);
            Colliders.Add(boxDerecha);

            BoundingBox boxIzquierda = CreateTransformedBoundingBox(muroIzquierda, MuroSize, 5.0f);
            Colliders.Add(boxIzquierda);
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
