using System;
using System.Collections.Generic;
using System.Linq;
using Ernestoyaquello.Chess.Util;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.Chess.Models
{
    public class ChessMoveInfo : BaseMoveInfo, IEquatable<ChessMoveInfo>
    {
        public IReadOnlyList<ChessMoveStepInfo> MoveSteps { get; }
        public bool WouldCaptureKing { get; }
        public bool IsCastling { get; }

        internal ChessMoveInfo(Player player, IReadOnlyList<ChessMoveStepInfo> steps, bool wouldCaptureKing, bool isCastling = false, double? score = null)
            : base(player, score ?? CalculateMoveScore(steps))
        {
            MoveSteps = steps.ToList();
            WouldCaptureKing = wouldCaptureKing;
            IsCastling = isCastling;
        }

        private static double CalculateMoveScore(IReadOnlyList<ChessMoveStepInfo> steps)
        {
            var firstMoveStep = steps.First();
            var pieceToMove = firstMoveStep.PieceToMove;
            var pieceAfterMoving = pieceToMove.WithPosition(firstMoveStep.NewPosition);
            var moveScore = (double)(pieceAfterMoving.HeuristicValue - pieceToMove.HeuristicValue);

            var pieceToCapture = firstMoveStep.PieceToCapture;
            if (!pieceToCapture.IsNone())
            {
                moveScore += pieceToCapture.HeuristicValue / 10d;
            }

            return moveScore / 150d;
        }

        public ChessMoveInfo WithExtraStep(ChessMoveStepInfo extraStep)
        {
            var newMoveSteps = MoveSteps.ToList();
            newMoveSteps.Add(extraStep);
            return new ChessMoveInfo(Player, newMoveSteps, WouldCaptureKing, IsCastling);
        }

        public override bool Equals(object other)
        {
            return other is ChessMoveInfo moveInfo && Equals(moveInfo);
        }

        public bool Equals(ChessMoveInfo other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Player, other.Player) &&
                MoveSteps.SequenceEqual(other.MoveSteps) &&
                Equals(WouldCaptureKing, other.WouldCaptureKing) &&
                Equals(IsCastling, other.IsCastling);
        }

        public override int GetHashCode()
        {
            return (Player, MoveSteps, WouldCaptureKing, IsCastling).GetHashCode();
        }

        public override string ToString()
        {
            var firstMoveStep = MoveSteps.First();
            var pieceColourName = firstMoveStep.PieceToMove.Player == Player.First ? "White" : "Black";

            if (IsCastling)
            {
                var firstPieceToMove = firstMoveStep.PieceToMove;
                var secondPieceToMove = MoveSteps[1].PieceToMove;
                var distanceBetweenPieces = Math.Abs((int)firstPieceToMove.Position.HorizontalPosition - (int)secondPieceToMove.Position.HorizontalPosition);
                var isShortString = distanceBetweenPieces == 3 ? "short" : "long";

                return $"{pieceColourName} makes {isShortString} castling";
            }
            else if (MoveSteps.Count == 2)
            {
                // Pawn promotion
                var secondPieceToMove = MoveSteps[1].PieceToMove;
                return $"{pieceColourName} promotes pawn to {secondPieceToMove.Type.ToString().ToLower()} on {secondPieceToMove.Position}";
            }

            // Everything else
            return firstMoveStep.ToString();
        }
    }
}
