using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pong
{
    internal class Program
    {
        public static object DrawLock = new object();
        private bool _roundEnd = false;
        private Player _player1;
        private Player _player2;
        private Ball _ball;

        private int _gameWidth = 40;
        private int _gameHeight = 21;

        private static void Main(string[] args)
        {
            new Program().Run();
        }

        internal Program()
        {
            Console.CursorVisible = false;
            Console.WindowWidth = _gameWidth;
            Console.WindowHeight = _gameHeight;
            Console.BufferWidth = _gameWidth;
            Console.BufferHeight = _gameHeight;

            _player1 = InitPlayer();
            _player2 = InitPlayer(true);
            _ball = new Ball();
        }

        private Player InitPlayer(bool player2 = false)
        {
            Clear();
            var playerName = string.Empty;

            while (true)
            {
                Console.SetCursorPosition(0, 0);
                if (!player2)
                    Console.WriteLine("Player 1 uses W/S to move up/down");
                else
                    Console.WriteLine("Player 2 uses arrow keys to move up/down");

                Console.Write("Name of player {0}: ", player2 ? "two" : "one");
                playerName = Console.ReadLine();

                if (playerName.Length < 18)
                    break;

                Clear();
                Console.WriteLine("Player name must be less than 18 characters long.");
            }

            return new Player(playerName, player2);
        }

        private void Clear()
        {
            Console.ResetColor();
            Console.Clear();
        }

        private void Run()
        {
            while (_player1.Score < 3 && _player2.Score < 3)
            {
                _roundEnd = false;
                var inputTask = new Task(HandleInput);
                Clear();
                InitialDraw();

                while (!Console.KeyAvailable) { }

                inputTask.Start();
                while (_ball.Move(_player1, _player2))
                    Thread.Sleep(115 - _ball.Speed);

                _roundEnd = true;
                inputTask.Wait();

                if (_ball.X <= _player1.X)
                    _player2.Score++;
                else
                    _player1.Score++;

                Reset();
            }

            Clear();
            var winner = _player1.Score == 3 ? _player1 : _player2;

            Console.ResetColor();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"{winner.Name} won the game!");
        }

        private void HandleInput()
        {
            while (!_roundEnd)
            {
                var key = Console.ReadKey(true).Key;
                Thread.Sleep(10);

                if (key == ConsoleKey.W)
                    _player1.Move(Move.Up);
                if (key == ConsoleKey.S)
                    _player1.Move(Move.Down);

                if (key == ConsoleKey.UpArrow)
                    _player2.Move(Move.Up);
                if (key == ConsoleKey.DownArrow)
                    _player2.Move(Move.Down);
            }
        }

        private void InitialDraw()
        {
            _player1.Draw();
            _player2.Draw();
            _ball.Draw();
            DrawScoreboard();
        }

        private void DrawScoreboard()
        {
            lock (DrawLock)
            {
                Console.SetCursorPosition(0, 0);
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;

                for (int i = 0; i < Console.WindowWidth; i++)
                    Console.Write(" ");
                for (int i = 0; i < Console.WindowWidth; i++)
                    Console.Write(" ");

                Console.SetCursorPosition(18 - _player1.Name.Length, 0);
                Console.Write($"{_player1.Name} vs {_player2.Name}");

                Console.SetCursorPosition(19 - _player1.Score.ToString().Length, 1);
                Console.Write($"{_player1.Score}  {_player2.Score}");
            }
        }

        private void Reset(){
            _player1.Reset();
            _player2.Reset();
            _ball.Reset();
        }
    }
}
