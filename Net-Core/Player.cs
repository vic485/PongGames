using System;

namespace Pong
{
    public class Player
    {
        public string Name { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsRight { get; private set; }
        public int Score { get; set; }

        public Player(string name, bool right)
        {
            Name = name;
            IsRight = right;

            Reset();
        }

        public void Reset()
        {
            X = IsRight ? Console.WindowWidth - 3 : 2;
            Y = Console.WindowHeight / 2 + 1;
        }

        public void Draw(bool clear = false)
        {
            lock (Program.DrawLock)
            {
                Console.BackgroundColor = clear ? ConsoleColor.Black : ConsoleColor.White;
                Console.ForegroundColor = clear ? ConsoleColor.Black : ConsoleColor.White;

                var drawPos = Y - 1;

                for (int i = 0; i < 3; i++)
                {
                    Console.CursorLeft = X;
                    Console.CursorTop = drawPos + i;
                    Console.Write("X");
                }
            }
        }

        public void Move(Move move)
        {
            var newPos = Y + (int)move;

            if (!CanMoveTo(newPos))
                return;

            Draw(true);
            Y = newPos;
            Draw();
        }

        private bool CanMoveTo(int newPos) => newPos + 1 < Console.WindowHeight && newPos - 1 >= 2;
    }
}
