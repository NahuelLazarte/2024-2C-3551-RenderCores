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
    public float LinearSpeed;
    public float RotationSpeed;
    public float escala;
    private Texture2D Texture { get; set; }

    internal void Update(GameTime gameTime, Modelos.Sphere esfera, ContentManager content)
    {
        var keyboardState = Keyboard.GetState();
        
        if (keyboardState.IsKeyDown(Keys.D1)) // Metal
        {
            Texture = content.Load<Texture2D>("Textures/texturaMetal");
                esfera.SetTexture(Texture);
                esfera.SetColor(new Vector3(0.75f, 0.75f, 0.75f));
            LinearSpeed  = 15f;
            RotationSpeed = 50f;
        }
        if (keyboardState.IsKeyDown(Keys.D2)) // Madera
        {
            Texture = content.Load<Texture2D>("Textures/texturaMadera");
                esfera.SetTexture(Texture);
                esfera.SetColor(new Vector3(0.54f, 0.27f, 0.07f));
            LinearSpeed  = 30f;
            RotationSpeed = 20f;
        }

        if (keyboardState.IsKeyDown(Keys.D3)) // Plastico
        {
            Texture = content.Load<Texture2D>("Textures/texturaPlastico");
            esfera.SetTexture(Texture);
            esfera.SetColor(new Vector3(0.9f, 0.9f, 0.9f));
            LinearSpeed  = 40f;
            RotationSpeed = 15f;
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