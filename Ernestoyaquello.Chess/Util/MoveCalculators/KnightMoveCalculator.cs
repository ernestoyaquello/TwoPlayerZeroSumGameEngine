using System.Collections.Generic;
using System.Linq;
using Ernestoyaquello.Chess.Models;

namespace Ernestoyaquello.Chess.Util.MoveCalculators
{
    internal static class KnightMoveCalculator
    {
        public static List<ChessMoveInfo> CalculateValidMovesForKnight(this ChessBoard board, Piece pieceToMove, int forwardMotion)
        {
            return new List<ChessMoveInfo>()
            {
                GetMoveWithOffset(board, pieceToMove, 1, (forwardMotion * 2)),
                GetMoveWithOffset(board, pieceToMove, 2, forwardMotion),
                GetMoveWithOffset(board, pieceToMove, -1, (forwardMotion * 2)),
                GetMoveWithOffset(board, pieceToMove, -2, forwardMotion),
                GetMoveWithOffset(board, pieceToMove, -1, -(forwardMotion * 2)),
                GetMoveWithOffset(board, pieceToMove, -2, -forwardMotion),
                GetMoveWithOffset(board, pieceToMove, 1, -(forwardMotion * 2)),
                GetMoveWithOffset(board, pieceToMove, 2, -forwardMotion),
            }.Where(move => move != null).ToList();
        }

        private static ChessMoveInfo GetMoveWithOffset(ChessBoard board, Piece pieceToMove, int horizontalOffset, int verticalOffset)
        {
            var moves = new List<ChessMoveInfo>();

            var potentialNewPosition = new PiecePosition(
                    (PieceHorizontalPosition)((int)pieceToMove.Position.HorizontalPosition + horizontalOffset),
                    (PieceVerticalPosition)((int)pieceToMove.Position.VerticalPosition + verticalOffset));

            if (board.IsPositionValidToMoveTo(potentialNewPosition, pieceInPosition => pieceInPosition.IsNone() || pieceInPosition.Player != pieceToMove.Player))
            {
                return board.CreateSimpleMove(pieceToMove, potentialNewPosition);
            }

            return null;
        }
    }
}
