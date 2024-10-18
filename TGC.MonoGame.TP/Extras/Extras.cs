using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP;
using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo

namespace TGC.MonoGame.TP.Extra
{
    public class Extras
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        float escalaMuro = 3;
        float escalaPuerta = 3;
        float escalaTecho = 2.4f;
        public List<BoundingBox> Colliders { get; set; }
        Matrix MuroWorld { get; set; }
        Matrix PuertaWorld { get; set; }
        Matrix TechoWorld { get; set; }

        BoundingBox Puertasize;
        BoundingBox Torresize;

        public Model ModeloMuro { get; set; }
        public Model ModeloPuerta { get; set; }
        public Model ModeloTecho { get; set; }



        private List<Matrix> _extras { get; set; }

        public Extras()
        {
            Initialize();
        }

        private void Initialize()
        {
            _extras = new List<Matrix>();
            Colliders = new List<BoundingBox>();
        }

        public void LoadContent(ContentManager Content)
        {
            ModeloMuro = Content.Load<Model>("Models/" + "Extras/wall");
            ModeloPuerta = Content.Load<Model>("Models/" + "Extras/door");
            ModeloTecho = Content.Load<Model>("Models/" + "Extras/towerSquareTopRoofHighWindows_exclusive");

            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloMuro.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            foreach (var mesh in ModeloPuerta.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            foreach (var mesh in ModeloTecho.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            Puertasize = BoundingVolumesExtensions.CreateAABBFrom(ModeloPuerta);
            Torresize = BoundingVolumesExtensions.CreateAABBFrom(ModeloMuro);



        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {


            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkGray.ToVector3());

            foreach (var mesh in ModeloMuro.Meshes)
            {
                Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * MuroWorld);
                mesh.Draw();
            }
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(123f / 255f, 75f / 255f, 58f / 255f));
            foreach (var mesh in ModeloPuerta.Meshes)
            {
                Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * PuertaWorld);
                mesh.Draw();
            }
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(123f / 255f, 75f / 255f, 58f / 255f));
            foreach (var mesh in ModeloTecho.Meshes)
            {
                Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * TechoWorld);
                mesh.Draw();
            }
        }

        

        public void AgregarExtras(Vector3 Posicion)
        {
            
            var posicionMuro = new Vector3(Posicion.X + 9F , Posicion.Y , Posicion.Z-11.22f );

            var posicionPuerta = new Vector3(Posicion.X +0.7F , Posicion.Y , Posicion.Z +58.5f );
            
            BoundingBox boxPuerta = new BoundingBox(Puertasize.Min * escalaPuerta + posicionPuerta * escalaPuerta , Puertasize.Max * escalaPuerta + posicionPuerta * escalaPuerta);

            Colliders.Add(boxPuerta);

            var posicionTecho = new Vector3(Posicion.X -45F , Posicion.Y +15f, Posicion.Z-24F);
            
            MuroWorld = Matrix.CreateTranslation(posicionMuro) * Matrix.CreateScale(escalaMuro);
            
            BoundingBox boxMuro = new BoundingBox(Torresize.Min * escalaMuro + posicionMuro * escalaMuro , Torresize.Max * escalaMuro + posicionMuro * escalaMuro);

            Colliders.Add(boxMuro);
            
            PuertaWorld = Matrix.CreateTranslation(posicionPuerta)  * Matrix.CreateScale(escalaPuerta);
            
            TechoWorld = Matrix.CreateTranslation(posicionTecho)  * Matrix.CreateScale(escalaTecho);
        }

    }
}
