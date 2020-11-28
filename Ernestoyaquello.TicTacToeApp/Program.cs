using System;
using System.Threading.Tasks;
using Ernestoyaquello.TicTacToe.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Util;

namespace Ernestoyaquello.TicTacToeApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var gameEngine = new TwoPlayerZeroSumGameMovesEngine();
            gameEngine.Initialize(9);
            await PlayTicTacToe(gameEngine);
        }

        private static async Task PlayTicTacToe(TwoPlayerZeroSumGameMovesEngine gameEngine, bool humanIsFirstPlayer = true)
        {
            var humanPlayer = humanIsFirstPlayer ? Player.First : Player.Second;
            var board = new TicTacToeBoard(gameEngine);
            PrintBoard(board, humanPlayer);

            if (!humanIsFirstPlayer)
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, Console.WindowHeight);
                Console.Write("Calculating first move...");

                var firstMoveResult = await gameEngine.CalculateBestMove<TicTacToeBoard, TicTacToeMoveInfo, TicTacToeBoardState>(board, Player.First).ConfigureAwait(false);
                var firstMove = firstMoveResult.BestMove;
                gameEngine.TryMakeMove<TicTacToeBoard, TicTacToeMoveInfo, TicTacToeBoardState>(board, firstMove, false);

                PrintBoard(board, humanPlayer);
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
                        DrawMove(humanPlayer);
                        Console.SetCursorPosition(0, Console.WindowHeight);
                        Console.Write("Calculating move...");
                        var humanMove = new TicTacToeMoveInfo(humanPlayer, selectedRow, selectedColumn);
                        await gameEngine.MakeMoveAgainstMachine<TicTacToeBoard, TicTacToeMoveInfo, TicTacToeBoardState>(board, humanMove).ConfigureAwait(false);
                        gameResult = board.GetGameResult(humanMove.Player);
                        PrintBoard(board, humanPlayer);
                        break;

                    case ConsoleKey.Q:
                        PrintBoard(board, humanPlayer);
                        return;

                    default:
                        PrintBoard(board, humanPlayer);
                        break;
                }

                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.CursorVisible = true;
            }

            PrintGameResult(gameResult);
            await PlayTicTacToe(gameEngine, !humanIsFirstPlayer);
        }

        private static void DrawMove(Player humanPlayer)
        {
            Console.CursorVisible = false;
            Console.Write(TicTacToeBoard.FromPlayerToString(humanPlayer));
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

        private static void PrintBoard(TicTacToeBoard board, Player humanPlayer)
        {
            Console.Clear();
            var cursorTop = Console.WindowHeight - 9;
            Console.SetCursorPosition(0, cursorTop);

            Console.Write("TIC TAC TOE\n");
            Console.Write("\n");
            Console.Write("Players:\n");
            Console.Write("\n");
            Console.Write($"* You: {TicTacToeBoard.FromPlayerToString(humanPlayer)}\n");
            Console.Write($"* Computer: {TicTacToeBoard.FromPlayerToString(humanPlayer.ToOppositePlayer())}\n");
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
