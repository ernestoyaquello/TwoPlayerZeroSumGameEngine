using System;
using Ernestoyaquello.Chess.Util;

namespace Ernestoyaquello.Chess.Models
{
    public class ChessMoveStepInfo : IEquatable<ChessMoveStepInfo>
    {
        public PiecePosition OldPosition { get; }
        public PiecePosition NewPosition { get; }
        public Piece PieceToMove { get; }
        public Piece PieceToCapture { get; }

        internal ChessMoveStepInfo(PiecePosition oldPosition, PiecePosition newPosition, Piece pieceToMove, Piece pieceToCapture = null)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
            PieceToMove = pieceToMove;
            PieceToCapture = pieceToCapture;
        }

        public override bool Equals(object other)
        {
            return other is ChessMoveStepInfo moveInfo && Equals(moveInfo);
        }

        public bool Equals(ChessMoveStepInfo other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(OldPosition, other.OldPosition) &&
                Equals(NewPosition, other.NewPosition) &&
                Equals(PieceToMove, other.PieceToMove) &&
                Equals(PieceToCapture, other.PieceToCapture);
        }

        public override int GetHashCode()
        {
            return (OldPosition, NewPosition, PieceToMove, PieceToCapture).GetHashCode();
        }

        public override string ToString()
        {
            var capturesString = !PieceToCapture.IsNone() ? $", capturing {PieceToCapture.ToString().ToLower()} on {PieceToCapture.Position}" : "";
            return $"{PieceToMove} from {OldPosition} to {NewPosition}{capturesString}";
        }
    }
}
