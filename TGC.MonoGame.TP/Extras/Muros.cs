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
        public List<BoundingBox> Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _muros { get; set; }
        public BoundingSphere _envolturaEsfera { get; set; }
        public Song CollisionSound { get; set; }

        BoundingBox Rocksize;

        float escalaMuros = 3f;

        public Muros()
        {
            Initialize();
        }

        private void Initialize()
        {
            _muros = new List<Matrix>();
            Colliders = new List<BoundingBox>();
        }

        public void IniciarColliders()
        {

        }

        public void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            ModeloMuro = Content.Load<Model>("Models/" + "pistas/wallHalf");
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

            CollisionSound = Content.Load<Song>("Audio/ColisionPez"); // Ajusta la ruta seg�n sea necesario
            
            Rocksize = BoundingVolumesExtensions.CreateAABBFrom(ModeloMuro);

        }

        public void Update(GameTime gameTime, TGCGame Game)
        {

            for (int i = 0; i < _muros.Count; i++)
            {
                if (_envolturaEsfera.Intersects(Colliders[i]))
                {
                    Game.Respawn();
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
        }

        public void AgregarMurosPistaRecta(float Rotacion, Vector3 Posicion)
        {
            var posicionMuros = new Vector3(Posicion.X / 100f, Posicion.Y / 100f, Posicion.Z / 100f);
            var posicionIzquierda = posicionMuros;
            var posicionDerecha = posicionMuros;
            var desplazamientoDerecha = new Vector3(25.22f, -12f, 9f);
            var desplazamientoIzquierda = new Vector3(-25.22f, -12f, -9f);

            posicionIzquierda += Vector3.Transform(desplazamientoIzquierda, Matrix.CreateRotationY(Rotacion));
            posicionDerecha += Vector3.Transform(desplazamientoDerecha, Matrix.CreateRotationY(Rotacion));

            Matrix muroDerecha = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(posicionDerecha) * Matrix.CreateScale(escalaMuros);
            Matrix muroIzquierda = Matrix.CreateRotationY(Rotacion + MathHelper.ToRadians(90)) * Matrix.CreateTranslation(posicionIzquierda) * Matrix.CreateScale(escalaMuros);

            _muros.Add(muroDerecha);
            _muros.Add(muroIzquierda);
            /*
            BoundingBox boxIzquierda = new BoundingBox(Rocksize.Min * escalaMuros + posicionIzquierda * escalaMuros, Rocksize.Max * escalaMuros + posicionIzquierda * escalaMuros);
            BoundingBox boxDerecha = new BoundingBox(Rocksize.Min * escalaMuros + posicionDerecha * escalaMuros, Rocksize.Max * escalaMuros + posicionDerecha * escalaMuros);

            Colliders.Add(boxIzquierda);
            Colliders.Add(boxDerecha);
            */
            
            Vector3 minIzquierda = Vector3.Transform(Rocksize.Min, muroIzquierda);
            Vector3 maxIzquierda = Vector3.Transform(Rocksize.Max, muroIzquierda);
            BoundingBox boxIzquierda = new BoundingBox(minIzquierda, maxIzquierda );
            Colliders.Add(boxIzquierda);

            Vector3 minDerecha = Vector3.Transform(Rocksize.Min , muroDerecha);
            Vector3 maxDerecha = Vector3.Transform(Rocksize.Max , muroDerecha);
            BoundingBox boxDerecha = new BoundingBox(minDerecha , maxDerecha);            
            Colliders.Add(boxDerecha);
            
        }

    }
}
