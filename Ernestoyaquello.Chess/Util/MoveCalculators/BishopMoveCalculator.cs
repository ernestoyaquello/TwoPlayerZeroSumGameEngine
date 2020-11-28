using System.Collections.Generic;
using Ernestoyaquello.Chess.Models;

namespace Ernestoyaquello.Chess.Util.MoveCalculators
{
    internal static class BishopMoveCalculator
    {
        public static List<ChessMoveInfo> CalculateValidMovesForBishop(this ChessBoard board, Piece pieceToMove, int forwardMotion)
        {
            var moves = new List<ChessMoveInfo>();

            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 1, forwardMotion));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, -1, forwardMotion));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, -1, -forwardMotion));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 1, -forwardMotion));

            return moves;
        }
    }
}
