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
        public float coeficienteFriccion = 1.0f; // Cuánta velocidad se conserva tras un rebote

        public float umbralVelocidadRebote; // Velocidad mínima para detener el rebote
        public float escala;
        public Vector3 ambientColor;
        public Vector3 diffuseColor;
        public Vector3 specularColor;
        public float shininess;
        public float velocidadRotacion;

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
                setMetal(esfera);
            }
            if (keyboardState.IsKeyDown(Keys.D2)) // vidrio
            {
                rebota = false;
                velocidadRotacion = 1f;
                esfera.SetTexture(Texture1);
                esfera.SetNormalTexture(NormalTexture1);

                // Cambia el color base de la pelota a blanco
                esfera.SetColor(new Vector3(1.0f, 1.0f, 1.0f));

                ambientColor = new Vector3(0.5f, 0.5f, 0.5f);
                diffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                specularColor = new Vector3(0.5f, 0.5f, 0.5f);

                
                primera = false;
                LinearSpeed = 15f;
                RotationSpeed = 20f;
                soundEffectMovimiento = soundEffectMovimientoMetal.CreateInstance();
                soundEffectCaida = soundEffectCaidaMetal.CreateInstance();
                esfera.isEnvironmentMapActive = true;



                coeficienteRebote = 0.7f;
                umbralVelocidadRebote = 3f;
                shininess = 32;
                soundEffectMovimiento = soundEffectMovimientoGolf.CreateInstance();
                soundEffectCaida = soundEffectCaidaGolf.CreateInstance();
                soundEffectCaida.Volume = 0.1f;
                soundEffectMovimiento.Volume = 0.1f;

                /*
                
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
                esfera.isEnvironmentMapActive = false;*/
            }
            if (keyboardState.IsKeyDown(Keys.D3)) // Madera
            {
                primera = false;
                esfera.SetTexture(Texture3);
                esfera.SetNormalTexture(NormalTexture3);

                esfera.SetColor(new Vector3(0.54f, 0.27f, 0.07f));

                rebota = true;
                coeficienteRebote = 0.6f;
                umbralVelocidadRebote = 3f;
                ambientColor = new Vector3(0.5f, 0.5f, 0.5f);
                diffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                specularColor = new Vector3(0.5f, 0.5f, 0.5f);
                shininess = 32;


                LinearSpeed = 20f;
                RotationSpeed = 5f;
                coeficienteFriccion = 10f;
                velocidadRotacion = 0.7f;


                soundEffectMovimiento = soundEffectMovimientoMadera.CreateInstance();
                soundEffectCaida = soundEffectCaidaMadera.CreateInstance();
                esfera.isEnvironmentMapActive = false;
            }

            if (keyboardState.IsKeyDown(Keys.D4)) // Plastico
            {
                velocidadRotacion = 0.8f;
                primera = false;
                esfera.SetTexture(Texture4);
                esfera.SetNormalTexture(NormalTexture4);
                esfera.SetColor(new Vector3(0.9f, 0.9f, 0.9f));
                LinearSpeed = 30f;
                RotationSpeed = 20f;
                rebota = true;
                coeficienteRebote = 0.65f;
                umbralVelocidadRebote = 3f;
                ambientColor = new Vector3(0.5f, 0.5f, 0.5f);
                diffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                specularColor = new Vector3(0.5f, 0.5f, 0.5f);
                shininess = 32;
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
        public void setMetal(Modelos.Sphere esfera)
        {
            primera = true;
            rebota = false;
            velocidadRotacion = 0.9f;
            esfera.SetTexture(Texture2);
            esfera.SetNormalTexture(NormalTexture2);
            esfera.SetColor(new Vector3(0.75f, 0.75f, 0.75f));
            LinearSpeed = 16f;
            RotationSpeed = 20f;
            ambientColor = new Vector3(0.8f, 0.7f, 0.2f);
            diffuseColor = new Vector3(0.9f, 0.85f, 0.4f);
            specularColor = new Vector3(1.0f, 0.95f, 0.7f);
            shininess = 64;
            soundEffectMovimiento = soundEffectMovimientoMetal.CreateInstance();
            soundEffectCaida = soundEffectCaidaMetal.CreateInstance();
            esfera.isEnvironmentMapActive = true;
        }
    }
}