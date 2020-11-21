using System;
using System.Linq;
using System.Threading.Tasks;
using Ernestoyaquello.Connect4.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Util;

namespace Ernestoyaquello.Connect4App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var gameEngine = new TwoPlayerZeroSumGameMovesEngine();
            gameEngine.Initialize(8);
            await PlayConnect4(gameEngine);
        }

        private static async Task PlayConnect4(TwoPlayerZeroSumGameMovesEngine gameEngine, bool humanIsFirstPlayer = true)
        {
            var humanPlayer = humanIsFirstPlayer ? Player.First : Player.Second;
            var board = new Connect4Board(gameEngine);
            PrintBoard(board, humanPlayer);

            if (!humanIsFirstPlayer)
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, Console.WindowHeight);
                Console.Write("Calculating first move...");

                var firstMoveResult = await gameEngine.CalculateBestMove<Connect4Board, Connect4MoveInfo, Connect4BoardState>(board, Player.First).ConfigureAwait(false);
                var firstMove = firstMoveResult.BestMove;
                gameEngine.TryMakeMove<Connect4Board, Connect4MoveInfo, Connect4BoardState>(board, firstMove, false);

                PrintBoard(board, humanPlayer);
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
                        selectedColumn = selectedColumn < (board.Clone().State.Columns.Length - 1)
                            ? selectedColumn + 1
                            : selectedColumn;
                        break;

                    case ConsoleKey.Enter:
                        DrawMove(board, selectedColumn, cursorLeft, cursorTop, humanPlayer);
                        Console.SetCursorPosition(0, Console.WindowHeight);
                        Console.Write("Calculating move...");
                        var humanMove = new Connect4MoveInfo(humanPlayer, selectedColumn);
                        await gameEngine.MakeMoveAgainstMachine<Connect4Board, Connect4MoveInfo, Connect4BoardState>(board, humanMove).ConfigureAwait(false);
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
            await PlayConnect4(gameEngine, !humanIsFirstPlayer);
        }

        private static void DrawMove(Connect4Board board, int selectedColumn, int cursorLeft, int cursorTop, Player humanPlayer)
        {
            var boardState = board.Clone().State;
            Console.CursorVisible = false;
            var emptyPositions = boardState.Columns[selectedColumn].Count(player => player == Player.None);
            Console.SetCursorPosition(cursorLeft, cursorTop + emptyPositions);
            Console.Write(Connect4Board.FromPlayerToString(humanPlayer));
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

        private static void PrintBoard(Connect4Board board, Player humanPlayer)
        {
            Console.Clear();
            var cursorTop = Console.WindowHeight - 9;
            Console.SetCursorPosition(0, cursorTop);

            Console.Write("CONNECT 4\n");
            Console.Write("\n");
            Console.Write("Players:\n");
            Console.Write("\n");
            Console.Write($"* You: {Connect4Board.FromPlayerToString(humanPlayer)}\n");
            Console.Write($"* Computer: {Connect4Board.FromPlayerToString(humanPlayer.ToOppositePlayer())}\n");
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
