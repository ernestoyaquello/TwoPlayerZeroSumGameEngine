using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.Connect4.Models
{
    public class Connect4MoveInfo : BaseMoveInfo
    {
        public int Column { get; }

        public Connect4MoveInfo(Player player, int column)
            : base(player)
        {
            Column = column;
        }

        public override string ToString()
        {
            var playerAsString = Connect4Board.FromPlayerToString(Player);
            return $"Add chip {playerAsString} to column {Column + 1}";
        }
    }
}
