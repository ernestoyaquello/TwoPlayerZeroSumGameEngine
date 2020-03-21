using System;
using System.Linq;
using System.Threading.Tasks;
using Ernestoyaquello.Connect4;
using Ernestoyaquello.Connect4.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.Connect4App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ticTacToeEngine = new Connect4Engine();
            await PlayConnect4(ticTacToeEngine);
        }

        private static async Task PlayConnect4(Connect4Engine gameEngine, bool firstPlayerStarts = true)
        {
            var board = new Connect4Board();
            gameEngine.Initialise(board);
            PrintBoard(board);

            if (!firstPlayerStarts)
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, Console.WindowHeight);
                Console.Write("Calculating first move...");

                var firstMove = await gameEngine.GetBestMove(Player.Second).ConfigureAwait(false);
                board.TryMakeMove(firstMove);

                PrintBoard(board);
            }

            var selectedColumn = 0;

            var gameResult = GameResult.StillGame;
            while (gameResult == GameResult.StillGame)
            {
                var cursorLeft = 2 + (2 * selectedColumn);
                var cursorTop = Console.WindowHeight - 9;
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.CursorVisible = true;

                var keyInfo = Console.ReadKey();
                Console.SetCursorPosition(cursorLeft, cursorTop);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        selectedColumn = selectedColumn > 0 ? selectedColumn - 1 : selectedColumn;
                        break;

                    case ConsoleKey.RightArrow:
                        selectedColumn = selectedColumn < (board.GetStateCopy().Columns.Length - 1)
                            ? selectedColumn + 1
                            : selectedColumn;
                        break;

                    case ConsoleKey.Enter:
                        DrawMove(board, selectedColumn, cursorLeft, cursorTop);
                        Console.SetCursorPosition(0, Console.WindowHeight);
                        Console.Write("Calculating move...");
                        var move = new Connect4MoveInfo(Player.First, selectedColumn);
                        gameResult = await gameEngine.MakeMoveAndGetResult(move).ConfigureAwait(false);
                        PrintBoard(board);
                        break;

                    case ConsoleKey.Q:
                        PrintBoard(board);
                        return;

                    default:
                        PrintBoard(board);
                        break;
                }

                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.CursorVisible = true;
            }

            PrintGameResult(gameResult);
            await PlayConnect4(gameEngine, !firstPlayerStarts);
        }

        private static void DrawMove(Connect4Board board, int selectedColumn, int cursorLeft, int cursorTop)
        {
            var boardState = board.GetStateCopy();
            Console.CursorVisible = false;
            var emptyPositions = boardState.Columns[selectedColumn].Count(player => player == Player.None);
            Console.SetCursorPosition(cursorLeft, cursorTop + emptyPositions);
            Console.Write(Connect4Board.FromPlayerToString(Player.First));
        }

        private static void PrintGameResult(GameResult gameResult)
        {
            Console.SetCursorPosition(0, Console.WindowHeight);
            Console.CursorVisible = false;

            switch (gameResult)
            {
                case GameResult.Victory:
                    Console.Write("\nYou won!\n");
                    break;

                case GameResult.Defeat:
                    Console.Write("\nYou lost!\n");
                    break;

                case GameResult.Tie:
                    Console.Write("\nTie!\n");
                    break;
            }

            Console.ReadKey();
        }

        private static void PrintBoard(Connect4Board board)
        {
            Console.Clear();
            var cursorTop = Console.WindowHeight - 9;
            Console.SetCursorPosition(0, cursorTop);

            Console.Write("CONNECT 4\n");
            Console.Write("\n");
            Console.Write("Players:\n");
            Console.Write("\n");
            Console.Write($"* You: {Connect4Board.FromPlayerToString(Player.First)}\n");
            Console.Write($"* Computer: {Connect4Board.FromPlayerToString(Player.Second)}\n");
            Console.Write("\n");
            Console.Write("How to play:\n");
            Console.Write("\n");
            Console.Write("* Select a column and click enter to drop your chip.\n");
            Console.Write("* Type Q to exit.\n");
            Console.Write("\n\n");
            Console.Write(board);
            Console.Write("\n");
        }
    }
}
