using System.Collections.Generic;
using Ernestoyaquello.Chess.Models;

namespace Ernestoyaquello.Chess.Util.MoveCalculators
{
    internal static class KingMoveCalculator
    {
        // This doesn't check whether the move would cause a check position; the board later will
        public static List<ChessMoveInfo> CalculateMovesForKing(this ChessBoard board, Piece pieceToMove, int forwardMotion)
        {
            var kingMoves = GetStandardMoves(board, pieceToMove, forwardMotion);
            kingMoves.AddRange(GetCastlingMoves(board, pieceToMove));

            return kingMoves;
        }

        private static List<ChessMoveInfo> GetStandardMoves(ChessBoard board, Piece pieceToMove, int forwardMotion)
        {
            var moves = new List<ChessMoveInfo>();

            var potentialPositions = new PiecePosition[8]
            {
                new PiecePosition(pieceToMove.Position.HorizontalPosition - 1, pieceToMove.Position.VerticalPosition),
                new PiecePosition(pieceToMove.Position.HorizontalPosition - 1, pieceToMove.Position.VerticalPosition - forwardMotion),
                new PiecePosition(pieceToMove.Position.HorizontalPosition, pieceToMove.Position.VerticalPosition - forwardMotion),
                new PiecePosition(pieceToMove.Position.HorizontalPosition + 1, pieceToMove.Position.VerticalPosition - forwardMotion),
                new PiecePosition(pieceToMove.Position.HorizontalPosition + 1, pieceToMove.Position.VerticalPosition),
                new PiecePosition(pieceToMove.Position.HorizontalPosition + 1, pieceToMove.Position.VerticalPosition + forwardMotion),
                new PiecePosition(pieceToMove.Position.HorizontalPosition, pieceToMove.Position.VerticalPosition + forwardMotion),
                new PiecePosition(pieceToMove.Position.HorizontalPosition - 1, pieceToMove.Position.VerticalPosition + forwardMotion),
            };

            foreach (var potentialPosition in potentialPositions)
            {
                if (!board.IsPositionValidToMoveTo(potentialPosition, pieceInPosition => pieceInPosition.IsNone() || pieceInPosition.Player != pieceToMove.Player))
                {
                    continue;
                }

                var potentialKingMove = board.CreateSimpleMove(pieceToMove, potentialPosition);
                moves.Add(potentialKingMove);
            }

            return moves;
        }

        private static List<ChessMoveInfo> GetCastlingMoves(ChessBoard board, Piece kingPiece)
        {
            var moves = new List<ChessMoveInfo>();

            var verticalPosition = kingPiece.Position.VerticalPosition;

            var shortCastlingRookPosition = new PiecePosition(PieceHorizontalPosition.P_H, verticalPosition);
            var shortCastlingRookPiece = board.GetPiece(shortCastlingRookPosition);
            var canDoShortCastling = CanDoCastling(board, kingPiece, shortCastlingRookPiece);
            if (canDoShortCastling)
            {
                var newRookPosition = new PiecePosition(PieceHorizontalPosition.P_F, verticalPosition);
                var newKingPosition = new PiecePosition(PieceHorizontalPosition.P_G, verticalPosition);
                var firstMoveStep = new ChessMoveStepInfo(shortCastlingRookPosition, newRookPosition, shortCastlingRookPiece);
                var secondMoveStep = new ChessMoveStepInfo(kingPiece.Position, newKingPosition, kingPiece);
                var steps = new List<ChessMoveStepInfo> { firstMoveStep, secondMoveStep };
                var move = new ChessMoveInfo(kingPiece.Player, steps, false, isCastling: true);
                moves.Add(move);
            }

            var longCastlingRookPosition = new PiecePosition(PieceHorizontalPosition.P_A, verticalPosition);
            var longCastlingRookPiece = board.GetPiece(longCastlingRookPosition);
            var canDoLongCastling = CanDoCastling(board, kingPiece, longCastlingRookPiece);
            if (canDoLongCastling)
            {
                var newRookPosition = new PiecePosition(PieceHorizontalPosition.P_D, verticalPosition);
                var newKingPosition = new PiecePosition(PieceHorizontalPosition.P_C, verticalPosition);
                var firstMoveStep = new ChessMoveStepInfo(longCastlingRookPosition, newRookPosition, longCastlingRookPiece);
                var secondMoveStep = new ChessMoveStepInfo(kingPiece.Position, newKingPosition, kingPiece);
                var steps = new List<ChessMoveStepInfo> { firstMoveStep, secondMoveStep };
                var move = new ChessMoveInfo(kingPiece.Player, steps, false, isCastling: true);
                moves.Add(move);
            }

            return moves;
        }

        private static bool CanDoCastling(ChessBoard board, Piece kingPiece, Piece expectedRookPiece)
        {
            if (kingPiece.NumberOfPlayedMoves > 0)
            {
                return false;
            }

            if (expectedRookPiece.IsNone() ||
                expectedRookPiece.Type != PieceType.Rook ||
                expectedRookPiece.NumberOfPlayedMoves > 0 ||
                expectedRookPiece.Player != kingPiece.Player)
            {
                return false;
            }

            var rookPosition = expectedRookPiece.Position;
            var kingPosition = kingPiece.Position;
            var horizontalIncrement = (int)rookPosition.HorizontalPosition > (int)kingPosition.HorizontalPosition ? 1 : -1;
            var firstHorizontalIndexToCheck = ((int)kingPosition.HorizontalPosition) + horizontalIncrement;
            for (int horizontalIndex = firstHorizontalIndexToCheck; horizontalIndex != (int)rookPosition.HorizontalPosition; horizontalIndex += horizontalIncrement)
            {
                var positionToCheck = new PiecePosition((PieceHorizontalPosition)horizontalIndex, kingPosition.VerticalPosition);
                var pieceInPosition = board.GetPiece(positionToCheck);
                if (!pieceInPosition.IsNone())
                {
                    // All positions between the king and the queen must be empty!
                    return false;
                }
            }

            return true;
        }
    }
}
