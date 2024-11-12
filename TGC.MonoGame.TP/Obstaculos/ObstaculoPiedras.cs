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
        public Model ModeloPez { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private float Rotation { get; set; }
        private List<Matrix> _obstaculosPiedras { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        public SoundEffect CollisionSound { get; set; }
        BoundingBox size;
        private BoundingFrustum _frustum;
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
            ModeloPez = Content.Load<Model>("Models/" + "obstaculos/rockLarge");
            Effect = Content.Load<Effect>("Effects/" + "BasicShader");

            foreach (var mesh in ModeloPez.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            CollisionSound = Content.Load<SoundEffect>("Audio/ColisionPez"); // Ajusta la ruta según sea necesario
            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloPez);


        }

        public void Update(GameTime gameTime, Level Game, Matrix view, Matrix projection) {
          
            for (int i = 0; i < _obstaculosPiedras.Count; i++) {
                if (_envolturaEsfera.Intersects(Colliders[i])) {
                    // Acción al tocar el modelo
                    CollisionSound.Play();
                    //_obstaculosPiedras.RemoveAt(i);
                    Game.Respawn();
                }
            }
            _frustum = new BoundingFrustum(view * projection);
        }




        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));

            
            foreach (var mesh in ModeloPez.Meshes){
                for(int i=0; i < _obstaculosPiedras.Count; i++){
                    Matrix _pisoWorld = _obstaculosPiedras[i];
                    BoundingBox boundingBox = BoundingVolumesExtensions.FromMatrix(_pisoWorld);
                    
                    if(_frustum.Intersects(boundingBox)){
                        Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                        mesh.Draw();
                    }
                }
            }
        }

        public void AgregarNuevoObstaculo(float Rotacion, Vector3 Posicion) {
            var transform = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale ; 
            _obstaculosPiedras.Add(transform);

            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            BoundingBox box = new BoundingBox(size.Min  + Posicion , size.Max + Posicion );

            Colliders.Add(box);
            
        }

    }
}
