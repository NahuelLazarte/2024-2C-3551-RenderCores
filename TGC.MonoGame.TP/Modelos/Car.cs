using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*
namespace TGC.MonoGame.TP.Modelos
{
    class Car : Modelo
    {   
        private const string Path = "Models/objetos/truck"; //poner acá la ruta del modelo 3D

        public Car(ContentManager content, Vector3 position, Matrix rotation, Color color)
            : base(content, position, rotation, color)
        {
            
            Model3D = content.Load<Model>(Path);// "Models/"  es lo mismo que poner ContentFolder3D

            SetScale(Matrix.CreateScale(0.8f)); //al poner la escala acá aplica para todo los modelos de este tipo
            
            World = Scale * rotation * Matrix.CreateTranslation(position);// esto tiene que ir siempre
        }
    }
}
*/