using System;
using System.Collections.Generic;
using System.Linq;
using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.Chess.Util.MoveCalculators
{
    internal static class PawnMoveCalculator
    {
        public static List<ChessMoveInfo> CalculateValidMovesForPawn(this ChessBoard board, Piece pieceToMove, int forwardMotion)
        {
            var pawnMoves = new List<ChessMoveInfo>();

            var positionAStepForward = new PiecePosition(pieceToMove.Position.HorizontalPosition, pieceToMove.Position.VerticalPosition + forwardMotion);
            if (board.IsPositionValidToMoveTo(positionAStepForward, pieceInPosition => pieceInPosition.IsNone()))
            {
                var moveAStepForward = board.CreateSimpleMove(pieceToMove, positionAStepForward);
                var movesAStepForward = AddReplacementMoveStepsIfNecessary(moveAStepForward);
                pawnMoves.AddRange(movesAStepForward);

                // If it is the first move of the pawn, it can also move two steps forward
                if (pieceToMove.NumberOfPlayedMoves == 0)
                {
                    var positionTwoStepsForward = new PiecePosition(positionAStepForward.HorizontalPosition, positionAStepForward.VerticalPosition + forwardMotion);
                    if (board.IsPositionValidToMoveTo(positionTwoStepsForward, pieceInPosition => pieceInPosition.IsNone()))
                    {
                        var moveTwoStepsForward = board.CreateSimpleMove(pieceToMove, positionTwoStepsForward);
                        pawnMoves.Add(moveTwoStepsForward);
                    }
                }
            }

            var nextPositionInLeftDiagonal = new PiecePosition(pieceToMove.Position.HorizontalPosition - 1, pieceToMove.Position.VerticalPosition + forwardMotion);
            if (board.IsPositionValidToMoveTo(nextPositionInLeftDiagonal, pieceInPosition => !pieceInPosition.IsNone() && pieceInPosition.Player != pieceToMove.Player))
            {
                var moveToNextPositionInLeftDiagonal = board.CreateSimpleMove(pieceToMove, nextPositionInLeftDiagonal);
                var movesToNextPositionInLeftDiagonal = AddReplacementMoveStepsIfNecessary(moveToNextPositionInLeftDiagonal);
                pawnMoves.AddRange(movesToNextPositionInLeftDiagonal);
            }

            var nextPositionInRightDiagonal = new PiecePosition(pieceToMove.Position.HorizontalPosition + 1, pieceToMove.Position.VerticalPosition + forwardMotion);
            if (board.IsPositionValidToMoveTo(nextPositionInRightDiagonal, pieceInPosition => !pieceInPosition.IsNone() && pieceInPosition.Player != pieceToMove.Player))
            {
                var moveToNextPositionInRightDiagonal = board.CreateSimpleMove(pieceToMove, nextPositionInRightDiagonal);
                var movesToNextPositionInRightDiagonal = AddReplacementMoveStepsIfNecessary(moveToNextPositionInRightDiagonal);
                pawnMoves.AddRange(movesToNextPositionInRightDiagonal);
            }

            var isInEnPassantPosition = (pieceToMove.Player == Player.First && pieceToMove.Position.VerticalPosition == PieceVerticalPosition.P_5) ||
                (pieceToMove.Player == Player.Second && pieceToMove.Position.VerticalPosition == PieceVerticalPosition.P_4);
            if (isInEnPassantPosition)
            {
                var moveHistory = board.GetMoveHistory();
                var lastMoveStep = moveHistory.LastOrDefault()?.MoveSteps?.First();
                if (lastMoveStep != null)
                {
                    var positionToTheLeft = new PiecePosition(pieceToMove.Position.HorizontalPosition - 1, pieceToMove.Position.VerticalPosition);
                    if (positionToTheLeft.IsValid())
                    {
                        var pieceToTheLeft = board.GetPiece(positionToTheLeft);
                        if (CanMakeEnPassantMove(pieceToMove, lastMoveStep, pieceToTheLeft))
                        {
                            var enPassantToTheLeft = board.CreateSimpleMove(pieceToMove, nextPositionInLeftDiagonal, pieceToTheLeft);
                            pawnMoves.Add(enPassantToTheLeft);
                        }
                    }

                    var positionToTheRight = new PiecePosition(pieceToMove.Position.HorizontalPosition + 1, pieceToMove.Position.VerticalPosition);
                    if (positionToTheRight.IsValid())
                    {
                        var pieceToTheRight = board.GetPiece(positionToTheRight);
                        if (CanMakeEnPassantMove(pieceToMove, lastMoveStep, pieceToTheRight))
                        {
                            var enPassantToTheRight = board.CreateSimpleMove(pieceToMove, nextPositionInRightDiagonal, pieceToTheRight);
                            pawnMoves.Add(enPassantToTheRight);
                        }
                    }
                }
            }

            return pawnMoves;
        }

        private static bool CanMakeEnPassantMove(Piece pieceToMove, ChessMoveStepInfo lastMoveStep, Piece pieceOnTheSide)
        {
            return !pieceOnTheSide.IsNone() &&
                pieceOnTheSide.Type == PieceType.Pawn &&
                pieceOnTheSide.Player != pieceToMove.Player &&
                pieceOnTheSide.NumberOfPlayedMoves == 1 &&
                lastMoveStep.NewPosition.Equals(pieceOnTheSide.Position) &&
                Math.Abs((int)lastMoveStep.OldPosition.VerticalPosition - (int)lastMoveStep.NewPosition.VerticalPosition) == 2;
        }

        private static List<ChessMoveInfo> AddReplacementMoveStepsIfNecessary(ChessMoveInfo pawnMove)
        {
            var result = new List<ChessMoveInfo>();

            var moveStep = pawnMove.MoveSteps.First();
            var pawnToMove = moveStep.PieceToMove;
            var moveDestination = moveStep.NewPosition;

            var potentialNewVerticalPosition = moveDestination.VerticalPosition;
            if (potentialNewVerticalPosition == PieceVerticalPosition.P_1 || potentialNewVerticalPosition == PieceVerticalPosition.P_8)
            {
                var player = pawnToMove.Player;
                var queenReplacementPiece = player == Player.First ? Piece.WhiteQueen : Piece.BlackQueen;
                var rookReplacementPiece = player == Player.First ? Piece.WhiteRook : Piece.BlackRook;
                var bishopReplacementPiece = player == Player.First ? Piece.WhiteBishop : Piece.BlackBishop;
                var knightReplacementPiece = player == Player.First ? Piece.WhiteKnight : Piece.BlackKnight;

                result.AddRange(new ChessMoveInfo[]
                {
                    pawnMove.WithExtraStep(new ChessMoveStepInfo(moveDestination, moveDestination, queenReplacementPiece.WithPosition(moveDestination))),
                    pawnMove.WithExtraStep(new ChessMoveStepInfo(moveDestination, moveDestination, rookReplacementPiece.WithPosition(moveDestination))),
                    pawnMove.WithExtraStep(new ChessMoveStepInfo(moveDestination, moveDestination, bishopReplacementPiece.WithPosition(moveDestination))),
                    pawnMove.WithExtraStep(new ChessMoveStepInfo(moveDestination, moveDestination, knightReplacementPiece.WithPosition(moveDestination))),
                });
            }
            else
            {
                result.Add(pawnMove);
            }

            return result;
        }
    }
}
