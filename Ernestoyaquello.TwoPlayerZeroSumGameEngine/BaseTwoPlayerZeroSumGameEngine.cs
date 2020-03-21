using System;
using System.Linq;
using System.Threading.Tasks;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine
{
    /// <summary>
    /// Base class that implements a basic AI algorithm to play two-player, zero-sum games against the machine.
    /// </summary>
    /// <typeparam name="TBoard">The type of the board.</typeparam>
    /// <typeparam name="TMoveInfo">The type of the moves used by the board.</typeparam>
    /// <typeparam name="TBoardState">The type of the state holder used by the board.</typeparam>
    public abstract class BaseTwoPlayerZeroSumGameEngine<TBoard, TMoveInfo, TBoardState>
        where TBoard : BaseBoard<TMoveInfo, TBoardState>
        where TMoveInfo : BaseMoveInfo
        where TBoardState : BaseBoardState<TMoveInfo>
    {
        /// <summary>
        /// The maximum depth that the algorithm will use by default to find the best moves.
        /// </summary>
        protected abstract int DefaultMaxTreeDepth { get; }

        /// <summary>
        /// The game board managed by this game engine.
        /// </summary>
        protected TBoard Board { get; private set; }

        /// <summary>
        /// Initialises the game engine.
        /// </summary>
        /// <param name="board">The board to manage.</param>
        public void Initialise(TBoard board)
        {
            Board = board;
        }

        /// <summary>
        /// First, it makes the manual move specified in the parameters.
        /// Then, it automatically calculates and makes the opponent's move.
        /// Finally, it returns the current game result for the player who made the manual move.
        /// </summary>
        /// <param name="moveInfo">The manual move to be made.</param>
        /// <param name="maxTreeDepth">The maximum depth that the algorithm will use to find the best moves.
        /// Null to use the default value.</param>
        /// <returns>The game result given to the player who made the manual move after all moves have been made.</returns>
        public async Task<GameResult> MakeMoveAndGetResult(TMoveInfo moveInfo, int? maxTreeDepth = null)
        {
            var moved = Board.TryMakeMove(moveInfo);
            var result = Board.GetGameResult(moveInfo.Player);

            if (moved && result == GameResult.StillGame)
            {
                var oppositePlayer = moveInfo.Player.ToOppositePlayer();
                var bestMove = await GetBestMove(oppositePlayer, maxTreeDepth).ConfigureAwait(false);
                Board.TryMakeMove(bestMove);
                result = Board.GetGameResult(moveInfo.Player);
            }

            return result;
        }

        /// <summary>
        /// Calculates and gets the best possible move for the specified player.
        /// </summary>
        /// <param name="player">The player for whom the best possible move will be found.</param>
        /// <param name="maxTreeDepth">The maximum depth that the algorithm will use to find the best moves.
        /// Null to use the default value.</param>
        /// <returns>The best possible move for the specified player.</returns>
        public async Task<TMoveInfo> GetBestMove(Player player, int? maxTreeDepth = null)
        {
            var maxAllowedTreeDepth = maxTreeDepth ?? DefaultMaxTreeDepth;
            var (bestMove, score) = await CalculateBestMoveRecursively(player, 1, maxAllowedTreeDepth, double.MinValue)
                .ConfigureAwait(false);

            return bestMove;
        }

        private async Task<(TMoveInfo move, double score)> CalculateBestMoveRecursively(
            Player player,
            int currentTreeLevel,
            int maximumTreeDepth,
            double previousTreeLevelScore)
        {
            var currentBoardState = Board.GetStateCopy();
            var availableMoves = Board.GetValidMoves(player);
            var randomisedAvailableMoves = availableMoves.OrderBy(move => Guid.NewGuid());
            var bestScore = double.MinValue;
            var bestMove = default(TMoveInfo);
            foreach (var move in randomisedAvailableMoves)
            {
                Board.TryMakeMove(move);

                var score = 0d;
                var gameResult = Board.GetGameResult(player);
                if (gameResult != GameResult.StillGame || currentTreeLevel == maximumTreeDepth)
                {
                    score = Board.GetGameScore(player);
                }
                else
                {
                    var nextPlayer = player.ToOppositePlayer();
                    var nextTreeDepth = currentTreeLevel + 1;
                    var (nextTreeLevelBestMove, nextTreeLevelBestScore) = await CalculateBestMoveRecursively(
                        nextPlayer,
                        nextTreeDepth,
                        maximumTreeDepth,
                        bestScore)
                        .ConfigureAwait(false);

                    // We need to invert the score calculated at the next tree level because
                    // what's good for the opponent is actually bad for us (and vice versa)
                    score = -nextTreeLevelBestScore;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }

                Board.RestoreStateFromCopy(currentBoardState);

                var currentScoreAsSeenInPreviousTreeBranch = -bestScore;
                if (currentScoreAsSeenInPreviousTreeBranch <= previousTreeLevelScore)
                {
                    // No need to continue with this tree branch because the parent one has already
                    // found a score bigger than the inverse of the current best score will ever be
                    break;
                }
            }

            return (bestMove, bestScore);
        }
    }
}
