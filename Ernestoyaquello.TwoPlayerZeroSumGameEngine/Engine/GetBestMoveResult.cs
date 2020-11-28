using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine
{
    /// <summary>
    /// Represents the information about the best possible move found by the machine.
    /// </summary>
    /// <typeparam name="TMoveInfo">The type of the moves of the game being played.</typeparam>
    public struct GetBestMoveResult<TMoveInfo>
        where TMoveInfo : BaseMoveInfo
    {
        /// <summary>
        /// The score of the best move. Will be between -1 (defeat) and +1 (victory).
        /// </summary>
        public double BestMoveScore { get; internal set; }

        /// <summary>
        /// The tree depth the algorithm had to reach in order to assess the move as the best possible one.
        /// </summary>
        public int BestMoveReachedDepth { get; internal set; }

        /// <summary>
        /// The best move found by the algorithm.
        /// </summary>
        public TMoveInfo BestMove { get; internal set; }

        internal static GetBestMoveResult<TMoveInfo> Default =>  new GetBestMoveResult<TMoveInfo>(double.MinValue, 0);

        private GetBestMoveResult(double bestMoveScore, int bestMoveReachedDepth)
        {
            BestMoveScore = bestMoveScore;
            BestMoveReachedDepth = bestMoveReachedDepth;
            BestMove = null;
        }
    }
}
