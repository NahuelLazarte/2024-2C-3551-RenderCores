using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

using TGC.MonoGame.TP;

using System;

namespace TGC.MonoGame.MenuPrincipal
{
    public class Menu
    {

        public bool isMusicActive = true;
        private float keyHoldDuration = 0.2f; // Duración de la presión de la tecla
        private float timer = 0f; // Temporizador para la duración de la tecla
        private KeyboardState previousKeyboardState;
        private int selectedIndex = 0;
        private Option[] options;

        public Menu()
        {
            options = new Option[] { new Play(), new SelectLevel(), new GodMode(), new Music() ,new Exit() };
        }
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
                /*
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
                    isMusicActive = !isMusicActive; // Alternar el estado de la música

                    if (isMusicActive)
                    {
                        // Activar la música
                        MediaPlayer.Resume();
                    }
                    else
                    {
                        // Desactivar la música
                        MediaPlayer.Pause();
                    }

                    // Aquí podrías añadir lógica para activar/desactivar la música

                }
                else if (selectedIndex == 3)
                {
                    Environment.Exit(0); // Salir del juego
                }*/
                options[selectedIndex].LogicExecute(Game);
                timer = 0f;
            }

            // Actualizar el estado anterior del teclado
            previousKeyboardState = keyboardState;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font,TGCGame Game)
        {
            //spriteBatch.Begin();

            // Obtener el tamaño de la ventana
            var screenWidth = spriteBatch.GraphicsDevice.Viewport.Width;
            var screenHeight = spriteBatch.GraphicsDevice.Viewport.Height;

            // Establecer el tamaño del texto (ajusta esto según sea necesario)
            float scale = 1.0f; // Aumenta el tamaño del texto

            // Calcular la posición inicial para centrar el menú
            Vector2 startPosition = new Vector2(screenWidth / 2, screenHeight / 2 - (options.Length * 40 * scale) / 2);

            for (int i = 0; i < options.Length; i++)
            {
                Color color = (i == selectedIndex) ? Color.Yellow : Color.White;

                // Medir el ancho del texto
                Vector2 textSize = font.MeasureString(options[i].GetName()) * scale;

                // Calcular la posición horizontal centrada
                Vector2 position = startPosition + new Vector2(-textSize.X / 2, i * 40 * scale);

                // Dibujar el texto con escalado
                spriteBatch.DrawString(font, options[i].GetName(), position, color, 0f, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0f);
            }

            //Por ahora está comentado, Ya lo voy a volver a poner
            
            // Mostrar el estado de GodMode y Musica en el menú
            string godModeText = $"GodMode: {(Game.isMusicPlaying ? "ON" : "OFF")}";
            spriteBatch.DrawString(font, godModeText, startPosition + new Vector2(0, options.Length * 40 * scale),
            Game.isGodModeActive ? Color.Red : Color.Gray, 0f,
            font.MeasureString(godModeText) / 2, new Vector2(scale), SpriteEffects.None, 0f);

            string musicText = $"Musica: {(Game.isMusicPlaying ? "ON" : "OFF")}";
            spriteBatch.DrawString(font, musicText, startPosition + new Vector2(0, (options.Length + 1) * 40 * scale),
            isMusicActive ? Color.Green : Color.Gray, 0f,
            font.MeasureString(musicText) / 2, new Vector2(scale), SpriteEffects.None, 0f);
            

            //spriteBatch.End();
        }
    }
}