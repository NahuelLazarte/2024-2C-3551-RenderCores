using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Collisions;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace TGC.MonoGame.TP.Pelotas
{
    public class Pelota
    {
        private string[] options = { "Play", "Exit" };
        private int selectedIndex = 0;
        public float LinearSpeed = 30f;
        public float RotationSpeed = 3f;
        public bool rebota = false;
        public float coeficienteRebote; // Cuánta velocidad se conserva tras un rebote
        public float umbralVelocidadRebote; // Velocidad mínima para detener el rebote
        public float escala;
        public Vector3 ambientColor;
        public Vector3 diffuseColor;
        public Vector3 specularColor;
        public float shininess;

        public Texture Texture1, Texture2, Texture3, Texture4;
        public Texture NormalTexture1, NormalTexture2, NormalTexture3, NormalTexture4;
        public SoundEffect soundEffectMovimientoMadera, soundEffectMovimientoMetal, soundEffectMovimientoPlastico, soundEffectMovimientoGolf;
        public SoundEffect soundEffectCaidaMadera, soundEffectCaidaMetal, soundEffectCaidaPlastico, soundEffectCaidaGolf;

        public SoundEffectInstance soundEffectMovimiento, soundEffectCaida;


        bool primera = true;

        internal void Update(GameTime gameTime, Modelos.Sphere esfera, ContentManager content)
        {
            var keyboardState = Keyboard.GetState();

            

            if (keyboardState.IsKeyDown(Keys.D1)) // Metal
            {
                primera = true;
                esfera.SetTexture(Texture2);
                esfera.SetNormalTexture(NormalTexture2);
                esfera.SetColor(new Vector3(0.75f, 0.75f, 0.75f));
                LinearSpeed = 15f;
                RotationSpeed = 50f;
                ambientColor = new Vector3(0.5f, 0.5f, 0.5f);
                diffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                specularColor = new Vector3(0.5f, 0.5f, 0.5f);
                shininess = 32;
                soundEffectMovimiento = soundEffectMovimientoMetal.CreateInstance();
                soundEffectCaida = soundEffectCaidaMetal.CreateInstance();
                esfera.isEnvironmentMapActive = false;
            }
            if (keyboardState.IsKeyDown(Keys.D2)) // vidrio
            {
                esfera.SetTexture(Texture1);
                primera = false;
                LinearSpeed = 15f;
                RotationSpeed = 50f;
                soundEffectMovimiento = soundEffectMovimientoMetal.CreateInstance();
                soundEffectCaida = soundEffectCaidaMetal.CreateInstance();
                esfera.isEnvironmentMapActive = true;
            }
            if (keyboardState.IsKeyDown(Keys.D3)) // Madera
            {
                primera = false;
                esfera.SetTexture(Texture3);
                esfera.SetNormalTexture(NormalTexture3);

                esfera.SetColor(new Vector3(0.54f, 0.27f, 0.07f));
                LinearSpeed = 50f;
                RotationSpeed = 50f;
                rebota = true;
                coeficienteRebote = 0.6f;
                umbralVelocidadRebote = 3f;
                ambientColor = new Vector3(0.5f, 0.5f, 0.5f);
                diffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                specularColor = new Vector3(0.5f, 0.5f, 0.5f);
                shininess = 32;
                soundEffectMovimiento = soundEffectMovimientoMadera.CreateInstance();
                soundEffectCaida = soundEffectCaidaMadera.CreateInstance();
                esfera.isEnvironmentMapActive = false;
            }

            if (keyboardState.IsKeyDown(Keys.D4)) // Plastico
            {
                primera = false;
                esfera.SetTexture(Texture4);
                esfera.SetNormalTexture(NormalTexture4);
                esfera.SetColor(new Vector3(0.9f, 0.9f, 0.9f));
                LinearSpeed = 40f;
                RotationSpeed = 15f;
                rebota = true;
                coeficienteRebote = 0.65f;
                umbralVelocidadRebote = 3f;
                ambientColor = new Vector3(0.5f, 0.5f, 0.5f);
                diffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                specularColor= new Vector3(0.5f, 0.5f, 0.5f);
                shininess =32;
                soundEffectMovimiento = soundEffectMovimientoPlastico.CreateInstance();
                soundEffectCaida = soundEffectCaidaPlastico.CreateInstance();
                esfera.isEnvironmentMapActive = false;
            }

            if (keyboardState.IsKeyDown(Keys.D5) && !primera)
            {
                esfera.SetTexture(Texture1);
                esfera.SetNormalTexture(NormalTexture1);

                esfera.SetColor(new Vector3(0.9f, 0.9f, 0.9f));
                LinearSpeed = 30f;
                RotationSpeed = 30f;
                rebota = true;
                coeficienteRebote = 0.7f;
                umbralVelocidadRebote = 3f;
                ambientColor = new Vector3(0.5f, 0.5f, 0.5f);
                diffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                specularColor = new Vector3(0.5f, 0.5f, 0.5f);
                shininess = 32;
                soundEffectMovimiento = soundEffectMovimientoGolf.CreateInstance();
                soundEffectCaida = soundEffectCaidaGolf.CreateInstance();
                soundEffectCaida.Volume = 0.1f;
                soundEffectMovimiento.Volume = 0.1f;
                esfera.isEnvironmentMapActive = false;
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