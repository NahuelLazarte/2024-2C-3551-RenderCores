using System;
using System.Runtime.InteropServices;

namespace TGC.MonoGame.TP
{
    public static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [STAThread]
        static void Main()
        {
            AllocConsole();
            using (var game = new TGCGame())
                game.Run();
        }
    }
}
