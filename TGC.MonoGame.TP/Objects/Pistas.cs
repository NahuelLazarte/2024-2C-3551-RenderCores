/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Objects
{
    class Pista : Modelo
    {         
        public Pista(ContentManager content, Vector3 position, Matrix rotation, Color color,string pista_name)
            : base(content, position, rotation, color)
        {
            string path = "pistas/"+pista_name;//poner acá la ruta del modelo 3D
            Model3D = content.Load<Model>("Models/" + path);// "Models/"  es lo mismo que poner ContentFolder3D

            scale = Matrix.CreateScale(2f); //al poner la escala acá aplica para todo los modelos de este tipo
            World = scale * Matrix.CreateTranslation(position) * rotation;// esto tiene que ir siempre
        }
    }
}*/