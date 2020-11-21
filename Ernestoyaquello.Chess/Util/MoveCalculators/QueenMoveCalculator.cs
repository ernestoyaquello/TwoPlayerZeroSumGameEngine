using System.Collections.Generic;
using Ernestoyaquello.Chess.Models;

namespace Ernestoyaquello.Chess.Util.MoveCalculators
{
    internal static class QueenMoveCalculator
    {
        public static List<ChessMoveInfo> CalculateValidMovesForQueen(this ChessBoard board, Piece pieceToMove, int forwardMotion)
        {
            var moves = new List<ChessMoveInfo>();

            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 1, 0));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 1, forwardMotion));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 0, forwardMotion));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, -1, forwardMotion));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, -1, 0));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, -1, -forwardMotion));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 0, -forwardMotion));
            moves.AddRange(MoveCalculatorHelper.GetMovesInSpecifiedDirection(board, pieceToMove, 1, -forwardMotion));

            return moves;
        }
    }
}
