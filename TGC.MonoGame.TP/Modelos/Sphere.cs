using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Modelos
{
    class Sphere : Modelo
    {    

        public Sphere(ContentManager content, Vector3 position, Matrix rotation, Color color)
            : base(content, position, rotation, color)
        {
            
            Model3D = content.Load<Model>("Models/" + "Spheres/sphere");
            Effect = content.Load<Effect>("Effects/" + "BasicShader");

            SetScale(Matrix.CreateScale(0.01f));
            
            World = Scale * rotation * Matrix.CreateTranslation(position); 

            
        }
    }
}