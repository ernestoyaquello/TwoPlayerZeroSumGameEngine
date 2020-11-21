using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Util;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine
{
    /// <summary>
    /// Class that implements a basic AI algorithm to make and calculate moves to play two-player, zero-sum games against the machine.
    /// </summary>
    public class TwoPlayerZeroSumGameMovesEngine : ITwoPlayerZeroSumGameMovesEngine
    {
        public int MaxTreeDepth { get; set; }

        public void Initialize(int maxTreeDepth)
        {
            MaxTreeDepth = maxTreeDepth;
        }

        public bool TryMakeMove<TBoard, TMoveInfo, TBoardState>(TBoard board, TMoveInfo moveInfo, bool shouldBeValid = true)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>
        {
            var isValidMove = !shouldBeValid || (moveInfo != null && board.IsValidMove(moveInfo));
            if (isValidMove)
            {
                MakeMove<TBoard, TMoveInfo, TBoardState>(board, moveInfo);
            }

            return isValidMove;
        }

        public bool TryMakeDummyMove<TBoard, TMoveInfo, TBoardState>(TBoard board, TMoveInfo moveInfo, Action<TBoard> actionAfterMove, bool shouldBeValid = true)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>
        {
            var isValidMove = !shouldBeValid || (moveInfo != null && board.IsValidMove(moveInfo));
            if (isValidMove)
            {
                var clonedBoard = (TBoard)board.Clone();
                MakeMove<TBoard, TMoveInfo, TBoardState>(clonedBoard, moveInfo);
                actionAfterMove.Invoke(clonedBoard);
            }

            return isValidMove;
        }

        public async Task<bool> TryMakeDummyMove<TBoard, TMoveInfo, TBoardState>(TBoard board, TMoveInfo moveInfo, Func<TBoard, Task> actionAfterMove, bool shouldBeValid = true)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>
        {
            var isValidMove = !shouldBeValid || (moveInfo != null && board.IsValidMove(moveInfo));
            if (isValidMove)
            {
                var clonedBoard = (TBoard)board.Clone();
                MakeMove<TBoard, TMoveInfo, TBoardState>(clonedBoard, moveInfo);
                await actionAfterMove.Invoke(clonedBoard).ConfigureAwait(false);
            }

            return isValidMove;
        }

        private void MakeMove<TBoard, TMoveInfo, TBoardState>(TBoard board, TMoveInfo moveInfo)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>
        {
            board.State.CachedData = new StateCache<TMoveInfo>();
            board.MakeMove(moveInfo);
            board.State.History.Add(moveInfo);
        }

        public async Task<GetBestMoveResult<TMoveInfo>> MakeMoveAgainstMachine<TBoard, TMoveInfo, TBoardState>(TBoard board, TMoveInfo moveInfo, bool shouldBeValid = true, Action<double> onProgressMade = null)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>
        {
            var moved = TryMakeMove<TBoard, TMoveInfo, TBoardState>(board, moveInfo, shouldBeValid);
            var result = board.GetGameResult(moveInfo.Player);

            if (moved && result == GameResult.StillGame)
            {
                var oppositePlayer = moveInfo.Player.ToOppositePlayer();
                var bestMoveResult = await CalculateBestMove<TBoard, TMoveInfo, TBoardState>(board, oppositePlayer, onProgressMade).ConfigureAwait(false);
                var bestMove = bestMoveResult.BestMove;
                TryMakeMove<TBoard, TMoveInfo, TBoardState>(board, bestMove, shouldBeValid);

                return bestMoveResult;
            }

            return GetBestMoveResult<TMoveInfo>.Default;
        }

        public async Task<GetBestMoveResult<TMoveInfo>> CalculateBestMove<TBoard, TMoveInfo, TBoardState>(TBoard board, Player player, Action<double> onProgressMade = null)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>
        {
            onProgressMade?.Invoke(0d);

            var bestResult = await Task.Run(() => CalculateBestMoveRecursivelyInParallel<TBoard, TMoveInfo, TBoardState>(board, player, 1, () => double.MinValue, onProgressMade: onProgressMade));

            onProgressMade?.Invoke(1d);

            return bestResult;
        }

        private GetBestMoveResult<TMoveInfo> CalculateBestMoveRecursivelyInParallel<TBoard, TMoveInfo, TBoardState>(TBoard board, Player player, int currentTreeLevel, Func<double> previousTreeLevelScore, Action<double> onProgressMade = null)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>
        {
            var bestMoveResult = GetBestMoveResult<TMoveInfo>.Default;
            var nextTreeDepth = currentTreeLevel + 1;
            var availableMoves = board.GetValidMoves(player).OrderByDescending(move => move.Score);

            var progressPerIteration = 1d / availableMoves.Count();
            var iteration = 0;
            var lockObject = new object();

            Parallel.ForEach(
                availableMoves,
                (move, loopState, _) =>
                {
                    var moveScore = 0d;
                    var moveReachedDepth = currentTreeLevel;

                    TryMakeDummyMove<TBoard, TMoveInfo, TBoardState>(board, move, (clonedBoard) =>
                    {
                        var gameResult = clonedBoard.GetGameResult(player);
                        if (gameResult != GameResult.StillGame || currentTreeLevel == MaxTreeDepth)
                        {
                            moveScore = gameResult == GameResult.StillGame || gameResult == GameResult.Tie
                                ? clonedBoard.GetGameScore(player)
                                : (gameResult == GameResult.Victory ? 1d : -1d);
                        }
                        else
                        {
                            // Here we could continue calling the parallel method, but it is actually more efficient to avoid further parallelisation
                            var nextPlayer = player.ToOppositePlayer();
                            var nextTreeLevelResult = CalculateBestMoveRecursively<TBoard, TMoveInfo, TBoardState>(
                                clonedBoard,
                                nextPlayer,
                                nextTreeDepth,
                                () => bestMoveResult.BestMoveScore);

                            // We need to invert the score calculated at the tree level below because
                            // what's good for the opponent is actually bad for us (and vice versa)
                            moveScore = -nextTreeLevelResult.BestMoveScore;
                            moveReachedDepth = nextTreeLevelResult.BestMoveReachedDepth;
                        }

                        lock (lockObject)
                        {
                            Debug.WriteLine($"{move}: {moveScore}");

                            if (SetMoveAsTheBestIfNecessary(ref bestMoveResult, move, moveScore, moveReachedDepth, previousTreeLevelScore()))
                            {
                                loopState.Stop();
                                return;
                            }
                        }

                        if (onProgressMade != null)
                        {
                            iteration++;
                            onProgressMade.Invoke(iteration * progressPerIteration);
                        }

                    }, shouldBeValid: false);
                });

            return bestMoveResult;
        }

        private GetBestMoveResult<TMoveInfo> CalculateBestMoveRecursively<TBoard, TMoveInfo, TBoardState>(TBoard board, Player player, int currentTreeLevel, Func<double> previousTreeLevelScore)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>
        {
            var bestMoveResult = GetBestMoveResult<TMoveInfo>.Default;
            var nextTreeDepth = currentTreeLevel + 1;
            var availableMoves = board.GetValidMoves(player).OrderByDescending(move => move.Score);

            var shouldBreakLoop = false;

            foreach (var move in availableMoves)
            {
                var moveScore = 0d;
                var moveReachedDepth = currentTreeLevel;

                TryMakeDummyMove<TBoard, TMoveInfo, TBoardState>(board, move, (clonedBoard) =>
                {
                    var gameResult = clonedBoard.GetGameResult(player);
                    if (gameResult != GameResult.StillGame || currentTreeLevel == MaxTreeDepth)
                    {
                        moveScore = gameResult == GameResult.StillGame || gameResult == GameResult.Tie
                            ? clonedBoard.GetGameScore(player)
                            : (gameResult == GameResult.Victory ? 1d : -1d);
                    }
                    else
                    {
                        var nextPlayer = player.ToOppositePlayer();
                        var nextTreeLevelResult = CalculateBestMoveRecursively<TBoard, TMoveInfo, TBoardState>(
                            clonedBoard,
                            nextPlayer,
                            nextTreeDepth,
                            () => bestMoveResult.BestMoveScore);

                        // We need to invert the score calculated at the tree level below because
                        // what's good for the opponent is actually bad for us (and vice versa)
                        moveScore = -nextTreeLevelResult.BestMoveScore;
                        moveReachedDepth = nextTreeLevelResult.BestMoveReachedDepth;
                    }

                    if (SetMoveAsTheBestIfNecessary(ref bestMoveResult, move, moveScore, moveReachedDepth, previousTreeLevelScore()))
                    {
                        shouldBreakLoop = true;
                    }
                }, shouldBeValid: false);

                if (shouldBreakLoop)
                {
                    return bestMoveResult;
                }
            }

            return bestMoveResult;
        }

        private static bool SetMoveAsTheBestIfNecessary<TMoveInfo>(
            ref GetBestMoveResult<TMoveInfo> bestMoveResult,
            TMoveInfo move,
            double moveScore,
            int moveReachedTreeDepth,
            double previousTreeLevelScore)
            where TMoveInfo : BaseMoveInfo
        {
            if (moveScore > bestMoveResult.BestMoveScore ||
                (moveScore == bestMoveResult.BestMoveScore && // This check and the ones below are not needed, but make the machine play more "humanly"
                    ((moveScore > 0d && moveReachedTreeDepth < bestMoveResult.BestMoveReachedDepth) || // For equally good results, the faster the better
                    (moveScore < 0d && moveReachedTreeDepth > bestMoveResult.BestMoveReachedDepth)))) // For equally bad results, the slower the better
            {
                // The move was indeed better than the best we had so far, so we set it as the new result
                bestMoveResult.BestMoveScore = moveScore;
                bestMoveResult.BestMoveReachedDepth = moveReachedTreeDepth;
                bestMoveResult.BestMove = move;

                if (bestMoveResult.BestMoveScore == 1d || -bestMoveResult.BestMoveScore <= previousTreeLevelScore)
                {
                    // No need to continue with this tree branch because one of these things has happened:
                    // * The maximum possible score has already been found (1)
                    // * The parent branch has already found a score bigger than the inverse of the current best score will ever be,
                    //   rendering this branch effectively useless
                    return true;
                }
            }

            return false;
        }
    }
}
