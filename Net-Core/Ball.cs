using System;

namespace Pong
{
    public class Ball
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Speed { get; private set; }

        private bool _downDir = false;
        private bool _rightDir = false;
        private Random _generator;

        public Ball()
        {
            _generator = new Random();
            Reset();
        }

        public void Reset()
        {
            X = Console.WindowWidth / 2;
            Y = Console.WindowHeight / 2 - 1;

            _downDir = _generator.Next(0, 2) == 1;
            _rightDir = _generator.Next(0, 2) == 1;
        }

        public void Draw(bool clear = false)
        {
            lock (Program.DrawLock)
            {
                Console.BackgroundColor = clear ? ConsoleColor.Black : ConsoleColor.White;
                Console.ForegroundColor = clear ? ConsoleColor.Black : ConsoleColor.White;

                Console.CursorLeft = X;
                Console.CursorTop = Y;

                Console.Write("X");
            }
        }

        public bool Move(Player p1, Player p2)
        {
            if (CollisionX())
                return false;

            Draw(true);
            if (PlayerCollision(p1) || PlayerCollision(p2))
            {
                _rightDir = !_rightDir;
                ChangeSpeed();
            }
            else if (CollisionY())
            {
                _downDir = !_downDir;
                ChangeSpeed();
            }

            X += _rightDir ? 1 : -1;
            Y += _downDir ? 1 : -1;

            Draw();
            return true;
        }

        private void ChangeSpeed()
        {
            var speedChange = _generator.Next(0, 2) == 1;

            if (!speedChange)
                return;

            var speedToAdd = _generator.Next(0, 2) == 1 ? 10 : -10;
            if (Speed + speedToAdd > 10 || Speed + speedToAdd < -10)
                return;

            Speed += speedToAdd;
        }

        private bool CollisionX() => !(X + 1 < Console.WindowWidth && X - 1 >= 0);

        private bool CollisionY() => !(Y + 1 < Console.WindowHeight && Y - 1 >= 2);

        private bool PlayerCollision(Player player)
        {
            var moveY = _downDir ? 1 : -1;
            var yCol = Y + moveY == player.Y - 1 || Y + moveY == player.Y + 1 || Y + moveY == player.Y;

            if (!yCol)
                return false;

            if (_rightDir && player.IsRight)
                return X + 1 >= player.X;
            else if (!_rightDir && !player.IsRight)
                return X - 1 <= player.X;

            return false;
        }
    }
}
