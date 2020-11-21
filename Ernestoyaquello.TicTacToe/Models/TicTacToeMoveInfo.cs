using System;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.TicTacToe.Models
{
    public class TicTacToeMoveInfo : BaseMoveInfo, IEquatable<TicTacToeMoveInfo>
    {
        public int Row { get; }
        public int Column { get; }

        public TicTacToeMoveInfo(Player player, int row, int column)
            : base(player)
        {
            Row = row;
            Column = column;
        }

        public override bool Equals(object other)
        {
            return other is TicTacToeMoveInfo moveInfo && Equals(moveInfo);
        }

        public bool Equals(TicTacToeMoveInfo other)
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
                Equals(Row, other.Row) &&
                Equals(Column, other.Column);
        }

        public override int GetHashCode()
        {
            return (Player, Row, Column).GetHashCode();
        }

        public override string ToString()
        {
            var playerAsString = TicTacToeBoard.FromPlayerToString(Player);
            var rowAsString = TicTacToeBoard.FromRowTostring(Row);
            return $"Add {playerAsString} to position ({rowAsString}, {Column + 1})";
        }
    }
}
