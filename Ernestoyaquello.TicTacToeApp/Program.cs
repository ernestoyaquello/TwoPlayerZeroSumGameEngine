using System;
using System.Threading.Tasks;
using Ernestoyaquello.TicTacToe;
using Ernestoyaquello.TicTacToe.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.TicTacToeApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ticTacToeEngine = new TicTacToeEngine();
            await PlayTicTacToe(ticTacToeEngine);
        }

        private static async Task PlayTicTacToe(TicTacToeEngine gameEngine, bool firstPlayerStarts = true)
        {
            var board = new TicTacToeBoard();
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

            var selectedRow = 0;
            var selectedColumn = 0;

            var gameResult = GameResult.StillGame;
            while (gameResult == GameResult.StillGame)
            {
                var cursorLeft = 4 + (4 * selectedColumn);
                var cursorTop = Console.WindowHeight - 7 + (selectedRow * 2);
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
                        selectedColumn = selectedColumn < 2 ? selectedColumn + 1 : selectedColumn;
                        break;

                    case ConsoleKey.UpArrow:
                        selectedRow = selectedRow > 0 ? selectedRow - 1 : selectedRow;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedRow = selectedRow < 2 ? selectedRow + 1 : selectedRow;
                        break;

                    case ConsoleKey.Enter:
                        DrawMove();
                        Console.SetCursorPosition(0, Console.WindowHeight);
                        Console.Write("Calculating move...");
                        var move = new TicTacToeMoveInfo(Player.First, selectedRow, selectedColumn);
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
            await PlayTicTacToe(gameEngine, !firstPlayerStarts);
        }

        private static void DrawMove()
        {
            Console.CursorVisible = false;
            Console.Write(TicTacToeBoard.FromPlayerToString(Player.First));
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

        private static void PrintBoard(TicTacToeBoard board)
        {
            Console.Clear();
            var cursorTop = Console.WindowHeight - 9;
            Console.SetCursorPosition(0, cursorTop);

            Console.Write("TIC TAC TOE\n");
            Console.Write("\n");
            Console.Write("Players:\n");
            Console.Write("\n");
            Console.Write($"* You: {TicTacToeBoard.FromPlayerToString(Player.First)}\n");
            Console.Write($"* Computer: {TicTacToeBoard.FromPlayerToString(Player.Second)}\n");
            Console.Write("\n");
            Console.Write("How to play:\n");
            Console.Write("\n");
            Console.Write("* Select an empty cell and click enter to make your move.\n");
            Console.Write("* Type Q to exit.\n");
            Console.Write("\n\n");
            Console.Write(board);
            Console.Write("\n");
        }
    }
}
