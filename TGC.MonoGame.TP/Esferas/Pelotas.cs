using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP.Pelotas
{
    public class Pelota
    {
        private string[] options = { "Play", "Exit" };
        private int selectedIndex = 0;
        public float LinearSpeed = 30f;
        public float RotationSpeed = 3f;
        public float escala;

        public Texture Texture1, Texture2, Texture3, Texture4;
        public Texture NormalTexture1, NormalTexture2, NormalTexture3, NormalTexture4;
        bool primera = true;

        internal void Update(GameTime gameTime, Modelos.Sphere esfera, ContentManager content)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.D1)) // Metal
            {
                primera = false;
                esfera.SetTexture(Texture2);
                esfera.SetColor(new Vector3(0.75f, 0.75f, 0.75f));
                LinearSpeed = 15f;
                RotationSpeed = 50f;
            }
            if (keyboardState.IsKeyDown(Keys.D2)) // Madera
            {
                primera = false;
                esfera.SetTexture(Texture3);
                esfera.SetColor(new Vector3(0.54f, 0.27f, 0.07f));
                LinearSpeed = 30f;
                RotationSpeed = 20f;
            }

            if (keyboardState.IsKeyDown(Keys.D3)) // Plastico
            {
                primera = false;
                esfera.SetTexture(Texture4);
                esfera.SetColor(new Vector3(0.9f, 0.9f, 0.9f));
                LinearSpeed = 40f;
                RotationSpeed = 15f;
            }

            if (keyboardState.IsKeyDown(Keys.D4) && !primera)
            {
                esfera.SetTexture(Texture1);
                esfera.SetColor(new Vector3(0.9f, 0.9f, 0.9f));
                LinearSpeed = 30f;
                RotationSpeed = 30f;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin();
            for (int i = 0; i < options.Length; i++)
            {
                Color color = (i == selectedIndex) ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, options[i], new Vector2(100, 100 + i * 40), color);
            }
            spriteBatch.End();
        }
    }
}