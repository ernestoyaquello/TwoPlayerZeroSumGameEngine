using System;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.Connect4.Models
{
    public class Connect4MoveInfo : BaseMoveInfo, IEquatable<Connect4MoveInfo>
    {
        public int Column { get; }

        public Connect4MoveInfo(Player player, int column)
            : base(player)
        {
            Column = column;
        }

        public override bool Equals(object other)
        {
            return other is Connect4MoveInfo moveInfo && Equals(moveInfo);
        }

        public bool Equals(Connect4MoveInfo other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Player, other.Player) && Equals(Column, other.Column);
        }

        public override int GetHashCode()
        {
            return (Player, Column).GetHashCode();
        }

        public override string ToString()
        {
            var playerAsString = Connect4Board.FromPlayerToString(Player);
            return $"Add chip {playerAsString} to column {Column + 1}";
        }
    }
}
