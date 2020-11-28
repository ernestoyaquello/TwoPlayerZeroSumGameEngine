using Ernestoyaquello.Chess.Models;

namespace Ernestoyaquello.Chess.Data.Models
{
    public class PuzzleSolution
    {
        public ChessMoveInfo BestMove { get; }
        public int RequiredTreeDepth { get; }

        internal PuzzleSolution(ChessMoveInfo bestMove, int requiredTreeDepth)
        {
            BestMove = bestMove;
            RequiredTreeDepth = requiredTreeDepth;
        }
    }
}
