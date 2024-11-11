using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

using TGC.MonoGame.TP;

using System;

namespace TGC.MonoGame.MenuPrincipal
{
    public abstract class Option
    {
        protected string name;
        public abstract void LogicExecute(TGCGame Game);
        public string GetName (){
            return name;
        }
    }

    public class Play : Option
    {
        public Play()
        {
            name = "Level 1";
        }

        public override void LogicExecute(TGCGame Game)
        {
            Game.isMenuActive = false;
            //Game.SeleccionarNivel(1);
        }
    }

    public class GodMode : Option
    {
        public GodMode()
        {
            name = "GodMode";
        }

        public override void LogicExecute(TGCGame Game)
        {
            Game.isGodModeActive = !Game.isGodModeActive;
        }
    }
    public class Exit : Option
    {
        public Exit()
        {
            name = "Exit";
        }

        public override void LogicExecute(TGCGame Game)
        {
            Environment.Exit(0);
        }
    }

    public class SelectLevel : Option
    {
        public override void LogicExecute(TGCGame Game)
        {
            //Game.SeleccionarNivel(1);
        }

        public SelectLevel()
        {
            name = "Level 2";
        }
    }
}