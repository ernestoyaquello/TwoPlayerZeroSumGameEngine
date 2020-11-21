using System;

namespace Ernestoyaquello.Chess.Models
{
    public struct PiecePosition : IEquatable<PiecePosition>
    {
        public PieceHorizontalPosition HorizontalPosition { get; }
        public PieceVerticalPosition VerticalPosition { get; }

        public PiecePosition(PieceHorizontalPosition horizontalPosition, PieceVerticalPosition verticalPosition)
        {
            HorizontalPosition = horizontalPosition;
            VerticalPosition = verticalPosition;
        }

        public bool IsValid()
        {
            return (int)HorizontalPosition >= 0 && (int)HorizontalPosition < 8 &&
                (int)VerticalPosition >= 0 && (int)VerticalPosition < 8;
        }

        public static bool operator ==(PiecePosition left, PiecePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PiecePosition left, PiecePosition right)
        {
            return !(left == right);
        }

        public override bool Equals(object other)
        {
            return other is PiecePosition moveInfo && Equals(moveInfo);
        }

        public bool Equals(PiecePosition other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(HorizontalPosition, other.HorizontalPosition) && Equals(VerticalPosition, other.VerticalPosition);
        }

        public override int GetHashCode()
        {
            return (HorizontalPosition, VerticalPosition).GetHashCode();
        }

        public override string ToString()
        {
            var horizontalPositionName = HorizontalPosition.ToString().Replace("P_", "");
            var verticalPositionName = VerticalPosition.ToString().Replace("P_", "");
            return $"{horizontalPositionName}{verticalPositionName}";
        }
    }
}
