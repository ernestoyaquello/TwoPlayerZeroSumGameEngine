using System.Collections.Generic;
using Ernestoyaquello.Chess.Models;

namespace Ernestoyaquello.Chess.Util.MoveCalculators
{
    internal static class RookMoveCalculator
    {
        public static List<ChessMoveInfo> CalculateValidMovesForRook(this ChessBoard board, Piece pieceToMove, int forwardMotion)
        {
            var moves = new List<ChessMoveInfo>();

            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, -1, 0));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 0, forwardMotion));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 1, 0));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 0, -forwardMotion));

            return moves;
        }
    }
}
