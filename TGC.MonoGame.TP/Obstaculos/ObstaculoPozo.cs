using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using System; // Asegúrate de que esto esté presente en la parte superior de tu archivo

namespace TGC.MonoGame.TP.ObstaculoPozo
{
    public class ObstaculosPozos
    {
        public Gizmos.Gizmos Gizmos { get; }
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }
        public Matrix scale = Matrix.CreateScale(2f);
        public Model ModeloPozo { get; set; }
        private BoundingBox PozoBox { get; set; }
        private Vector3 desplazamientoEnEjes { get; set; }
        public List<BoundingBox> Colliders { get; set; }
        private List<Matrix> _pozos { get; set; }
        public BoundingSphere _envolturaEsfera{ get; set; }
        float escala = 2f;
        BoundingBox size;


        public ObstaculosPozos()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pozos = new List<Matrix>();
            Colliders = new List<BoundingBox>();
        }

        public void IniciarColliders()
        {
            //Colliders = new BoundingBox[_pozos.Count];

            /*
            // Define factores de escala para cada eje
            float scaleX = 1500f; // Ajusta este valor para el eje X
            float scaleY = 1.0f; // Ajusta este valor para el eje Y
            float scaleZ = 600f; // Ajusta este valor para el eje Z

            for (int i = 0; i < _pozos.Count; i++) {
                // Crear el collider original
                Colliders[i] = BoundingVolumesExtensions.FromMatrix(_pozos[i]);

                // Aplicar la escala al BoundingBox
                Vector3 center = (Colliders[i].Min + Colliders[i].Max) / 2;
                Vector3 size = Colliders[i].Max - Colliders[i].Min;

                // Escalar el tamaño en cada eje
                size.X *= scaleX;
                size.Y *= scaleY;
                size.Z *= scaleZ;

                // Crear un nuevo BoundingBox escalado
                Colliders[i] = new BoundingBox(center - size / 2, center + size / 2);
            }
            */
        }


        public void LoadContent(ContentManager Content)
        {
            ModeloPozo = Content.Load<Model>(ContentFolder3D + "obstaculos/rockLarge");
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            foreach (var mesh in ModeloPozo.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            size = BoundingVolumesExtensions.CreateAABBFrom(ModeloPozo);
        }

        public void Update(GameTime gameTime, TGCGame Game)
        {
            for (int i = 0; i < _pozos.Count; i++) {
                var originalPosition = _pozos[i].Translation; // Obtener la posición original            
           // Comprobar colisión
            var piedrasBoundingSphere = new BoundingSphere(originalPosition, scale.Translation.X); // Ajustar el tamaño de la esfera de colisión según sea necesario
            if (_envolturaEsfera.Intersects(piedrasBoundingSphere)) { //Colliders[i]
                // Acción al tocar el modelo
                Console.WriteLine($"¡Colisión con piedras en la posición {originalPosition}!");
                // Aquí puedes realizar la acción que desees, como eliminar el pez, reducir vida, etc.
                //MediaPlayer.Play(CollisionSound);
                //_obstaculosPiedras.RemoveAt(i);

                Game.Respawn();
                
            }
            }
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            /*
            Colliders = new BoundingBox[_pozos.Count];

            for (int i = 0; i < _pozos.Count; i++) {
                Colliders[i] = BoundingVolumesExtensions.FromMatrix(_pozos[i]);
            }

            */

            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0.5f, 0.5f));

            foreach (var mesh in ModeloPozo.Meshes)
            {
                for (int i = 0; i < _pozos.Count; i++)
                {
                    Matrix _pisoWorld = _pozos[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }

        }

        public Vector3 Desplazamiento()
        {
            PozoBox = BoundingVolumesExtensions.CreateAABBFrom(ModeloPozo);
            desplazamientoEnEjes = PozoBox.Max - PozoBox.Min; // aca consigo el tamaño el largo de la pista para que coincida son 3/4, el ancho es el mismo.
            desplazamientoEnEjes = new Vector3(desplazamientoEnEjes.X, 0, 0);

            Console.WriteLine($"Pozo: Desplazamiento en ejes: X = {desplazamientoEnEjes.X}, Y = {desplazamientoEnEjes.Y}, Z = {desplazamientoEnEjes.Z}");
            return desplazamientoEnEjes;
        }

        /*
        public void agregarNuevaPista(float Rotacion, Vector3 Posicion) {
            _pozos.Add(Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion) * scale); // METER MATRIZ DENTRO DE CADA PISTA
            
            BoundingBox box = new BoundingBox(size.Min * escala + Posicion, size.Max * escala + Posicion);
            Colliders.Add(box);
            Console.WriteLine($"Box min= {box.Min}  Box max= {box.Max} ");
        }*/
        public void agregarNuevoPozo(float Rotacion, Vector3 Posicion)
        {
            Posicion += Vector3.Transform(new Vector3(0,-15,0), Matrix.CreateRotationY(Rotacion));
            // Crear la matriz de transformación completa
            Matrix transform = Matrix.CreateRotationY(Rotacion) *  Matrix.CreateTranslation(Posicion) *Matrix.CreateScale(escala);

            // Agregar la matriz de transformación a la lista de pistas
            _pozos.Add(transform);

            // Transformar los puntos mínimos y máximos del BoundingBox original
            Vector3 transformedMin = Vector3.Transform(size.Min, transform);
            Vector3 transformedMax = Vector3.Transform(size.Max, transform);

            // Crear y agregar el nuevo BoundingBox transformado a la lista de colliders
            BoundingBox box = new BoundingBox(transformedMin, transformedMax);
            Colliders.Add(box);

            // Imprimir los valores del BoundingBox para depuración
            Console.WriteLine($"Box min= {box.Min}  Box max= {box.Max} ");
        }

    }
}