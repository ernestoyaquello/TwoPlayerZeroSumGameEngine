using System.Collections.Generic;
using System.Linq;
using Ernestoyaquello.Chess.Models;

namespace Ernestoyaquello.Chess.Util.MoveCalculators
{
    internal class MoveCalculatorHelper
    {
        public static List<ChessMoveInfo> GetMovesInSpecifiedDirection(ChessBoard board, Piece pieceToMove, int horizontalIncrement, int verticalIncrement)
        {
            var moves = new List<ChessMoveInfo>();

            var potentialNewPosition = (PiecePosition?)pieceToMove.Position;
            while (potentialNewPosition is PiecePosition)
            {
                potentialNewPosition = new PiecePosition(
                    (PieceHorizontalPosition)((int)potentialNewPosition.Value.HorizontalPosition + horizontalIncrement),
                    (PieceVerticalPosition)((int)potentialNewPosition.Value.VerticalPosition + verticalIncrement));

                if (board.IsPositionValidToMoveTo(potentialNewPosition.Value, pieceInPosition => pieceInPosition.IsNone() || pieceInPosition.Player != pieceToMove.Player))
                {
                    var move = board.CreateSimpleMove(pieceToMove, potentialNewPosition.Value);
                    moves.Add(move);

                    // If this position implies capturing a piece, then the capturing piece won't be able to move any further
                    if (!board.GetPiece(move.MoveSteps.First().NewPosition).IsNone())
                    {
                        potentialNewPosition = null;
                    }
                }
                else
                {
                    potentialNewPosition = null;
                }
            }

            return moves;
        }
    }
}
