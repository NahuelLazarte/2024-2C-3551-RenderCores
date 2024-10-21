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
        private string[] options = { "Play", "GodMode", "Exit" };
        private int selectedIndex = 0;
        public bool isGodModeActive = false; // Variable para el modo GodMode

        private float keyHoldDuration = 0.2f; // Duración de la presión de la tecla
        private float timer = 0f; // Temporizador para la duración de la tecla
        private KeyboardState previousKeyboardState;

        public void Update(TGCGame Game, GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Reseteo del temporizador si la tecla se ha soltado
            if (previousKeyboardState.IsKeyUp(Keys.Up) && keyboardState.IsKeyDown(Keys.Up) ||
                previousKeyboardState.IsKeyUp(Keys.Down) && keyboardState.IsKeyDown(Keys.Down))
            {
                timer = 0f; // Reinicia el temporizador si se presiona una tecla
            }

            // Actualizar el temporizador
            timer += elapsedTime;

            // Manejo de las teclas de dirección
            if (keyboardState.IsKeyDown(Keys.Up) && timer >= keyHoldDuration)
            {
                selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                timer = 0f; // Reinicia el temporizador tras realizar el movimiento
            }
            else if (keyboardState.IsKeyDown(Keys.Down) && timer >= keyHoldDuration)
            {
                selectedIndex = (selectedIndex + 1) % options.Length;
                timer = 0f; // Reinicia el temporizador tras realizar el movimiento
            }

            // Manejo de la tecla Enter
            if (keyboardState.IsKeyDown(Keys.Enter) && timer >= keyHoldDuration)
            {
                if (selectedIndex == 0)
                {
                    Game.isMenuActive = false;
                }
                else if (selectedIndex == 1)
                {
                    isGodModeActive = !isGodModeActive; // Alternar el estado de GodMode
                    Game.isGodModeActive = isGodModeActive; // Asegurarse de que el estado sea el mismo
                }
                else if (selectedIndex == 2)
                {
                    Environment.Exit(0); // Salir del juego
                }
                timer = 0f;
            }

            // Actualizar el estado anterior del teclado
            previousKeyboardState = keyboardState;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin();
            for (int i = 0; i < options.Length; i++)
            {
                Color color = (i == selectedIndex) ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, options[i], new Vector2(100, 100 + i * 40), color);
            }

            // Mostrar el estado de GodMode en el menú
            spriteBatch.DrawString(font, $"GodMode: {(isGodModeActive ? "ON" : "OFF")}", 
                new Vector2(100, 100 + options.Length * 40), 
                isGodModeActive ? Color.Red : Color.Gray);

            spriteBatch.End();
        }
    }
}