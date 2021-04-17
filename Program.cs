using System;

namespace WillowWoodRefuge
{
    public static class TestProgram1
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
