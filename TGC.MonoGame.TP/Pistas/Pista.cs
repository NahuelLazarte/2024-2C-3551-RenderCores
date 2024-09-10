using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP;

namespace TGC.MonoGame.TP.Pistas{
    public class Pista{

        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public Effect Effect { get; set; }

        public const float DistanceBetweenFloor = 12.33f;
        public const float DistanceBetweenWall = 18f;
        public Matrix scale = Matrix.CreateScale(0.03f);
        public const float distanciaEscaleras = 3f;
        public int index { get; set; }

        public Model PistaRecta { get; set; }
        public Model PistaCurva { get; set; }

        public Matrix[] PistaRectaWorlds { get; set; }
        public Matrix[] PistaCurvaWorlds { get; set; }

        public Pista() {
            Initialize();
        }

        private void Initialize() {
          
            PistaRectaWorlds = new Matrix[]{
                scale * Matrix.Identity,
            };

            PistaCurvaWorlds = new Matrix[]{
            
            };

        }


        public void LoadContent(ContentManager Content){
            PistaRecta = Content.Load<Model>(ContentFolder3D + "pistas/road_straight_fix");
            PistaCurva = Content.Load<Model>(ContentFolder3D + "pistas/road_curve");
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");
            foreach (var mesh in PistaRecta.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }

            foreach (var mesh in PistaCurva.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }
        }

        public void Update(GameTime gameTime){

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection){
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
            foreach (var mesh in PistaRecta.Meshes){
                for(int i=0; i < PistaRectaWorlds.Length; i++){
                    Matrix _pistaRectaWorld = PistaRectaWorlds[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pistaRectaWorld);
                    mesh.Draw();
                }
            }
            foreach (var mesh in PistaCurva.Meshes){
                for(int i=0; i < PistaCurvaWorlds.Length; i++){
                    Matrix _pisoWorld = PistaCurvaWorlds[i];
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * _pisoWorld);
                    mesh.Draw();
                }
            }
        }
    }
}