using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media; // Aseg�rate de tener esta directiva
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP;

using System; 

namespace TGC.MonoGame.MenuPrincipal
{
public class Menu
{
    private string[] options = { "Play", "Exit" };
    private int selectedIndex = 0;
    public void Update(TGCGame Game)
    {
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Up))
        {
            selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
        }
        else if (keyboardState.IsKeyDown(Keys.Down))
        {
            selectedIndex = (selectedIndex + 1) % options.Length;
        }

        if (keyboardState.IsKeyDown(Keys.Enter))
        {
            if (selectedIndex == 0)
            {
                Game.isMenuActive = false;
            }
            else if (selectedIndex == 1)
            {
                Environment.Exit(0); // Salir del juego
            }
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
