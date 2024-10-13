using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TGC.MonoGame.TP.Collisions;
namespace TGC.MonoGame.TP.Modelos
{
    class Pista : Modelo
    {
        BoundingBox pistaBox;
        BoundingBox size;
        float escala;

        public BoundingBox GetBoundingBox()
        {
            return pistaBox;
        }

        public override void LoadContent(ContentManager content)
        {
            Model3D = content.Load<Model>("Models/" + "pistas/road_straight_fix");
            Effect = content.Load<Effect>("Effects/" + "BasicShader");

            base.LoadContent(content); //Llamo al método LoadContent de la clase que Pista hereda (Modelo)

            // This gets an AABB with the bounds of the robot model
            size = BoundingVolumesExtensions.CreateAABBFrom(Model3D);
            // This moves the min and max points to the world position of each robot (one and two)
            pistaBox = new BoundingBox(size.Min * escala + Position, size.Max * escala + Position);
        }
        public Pista(Vector3 position, Matrix rotation, Color color)
            : base(position, rotation, color)
        {
            escala = 0.01f;
            SetScale(Matrix.CreateScale(escala));
            World = Scale * rotation * Matrix.CreateTranslation(position);
        }
    }


}