using System;

namespace TopDownShooter
{
#if WINDOWS || LINUX

    public static class EntryPoint
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TopDownShooter())
                game.Run();
        }
    }
#endif
}
