using System.Diagnostics;
using System.Xml.Linq;

namespace Snake_Game
{
    internal class Program
    {
        private const int MapWidth = 20;
        private const int MapHeight = 10;
        private const int ScreenWidth = MapWidth * 3;
        private const int ScreenHeight = MapHeight * 3;
        private const int FrameMilliseconds = 200;
        private const ConsoleColor BorderColor = ConsoleColor.Gray;
        private const ConsoleColor FoodColor = ConsoleColor.Green;
        private const ConsoleColor BodyColor = ConsoleColor.Cyan;
        private const ConsoleColor HeadColor = ConsoleColor.DarkBlue;

        private static readonly Random Random = new Random();

        static void Main()
        {
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.SetBufferSize(ScreenWidth, ScreenHeight);
            Console.CursorVisible = false;

            while (true)
            {
                StartGame();
                Thread.Sleep(2000);
                Console.ReadKey();
            }
        }
        static void StartGame()
        {
            int score = 0;
            Console.Clear();
            DrawBoard();
            Snake snake = new Snake(10, 5, HeadColor, BodyColor);
            Pixel food = GenFood(snake);
            food.Draw();
            Direction currentMovement = Direction.Right;
            int lagMs = 0;
            var sw = new Stopwatch();
            while (true)
            {
                sw.Restart();
                Direction oldMovement = currentMovement;

                while (sw.ElapsedMilliseconds <= FrameMilliseconds - lagMs)
                {
                    if (currentMovement == oldMovement)
                        currentMovement = ReadMovement(currentMovement);
                }

                sw.Restart();

                if (snake.Head.X == food.X && snake.Head.Y == food.Y)
                {
                    snake.Move(currentMovement, true);
                    food = GenFood(snake);
                    food.Draw();

                    score++;

                    Task.Run(() => Console.Beep(1200, 200));
                }
                else
                {
                    snake.Move(currentMovement);
                }

                if (snake.Head.X == MapWidth - 1
                    || snake.Head.X == 0
                    || snake.Head.Y == MapHeight - 1
                    || snake.Head.Y == 0
                    || snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y))
                    break;

                lagMs = (int)sw.ElapsedMilliseconds;
            }
            snake.Clear();
            food.Clear();

            Console.SetCursorPosition(ScreenWidth / 3, ScreenHeight / 2);
            Console.WriteLine($"Game over, Score: {score}");

            Task.Run(() => Console.Beep(200, 600));
        }
        static void DrawBoard()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                new Pixel(i, 0, BorderColor).Draw();
                new Pixel(i, MapHeight - 1, BorderColor).Draw();
            }

            for (int i = 0; i < MapHeight; i++)
            {
                new Pixel(0, i, BorderColor).Draw();
                new Pixel(MapWidth - 1, i, BorderColor).Draw();
            }
        }
        static Pixel GenFood(Snake snake)
        {
            Pixel food;

            do
            {
                food = new Pixel(Random.Next(1, MapWidth - 2), Random.Next(1, MapHeight - 2), FoodColor);
            } while (snake.Head.X == food.X && snake.Head.Y == food.Y ||
                     snake.Body.Any(b => b.X == food.X && b.Y == food.Y));

            return food;
        }
        static Direction ReadMovement(Direction currentDirection)
        {
            if (!Console.KeyAvailable)
                return currentDirection;

            ConsoleKey key = Console.ReadKey(true).Key;

            currentDirection = key switch
            {
                ConsoleKey.UpArrow when currentDirection != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,
                ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,
                ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
                _ => currentDirection
            };

            return currentDirection;
        }
    }
}