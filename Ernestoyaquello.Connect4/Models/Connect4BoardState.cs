using System.Linq;
using System.Text;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.Connect4.Models
{
    public class Connect4BoardState : BaseBoardState<Connect4MoveInfo>
    {
        public Player[][] Columns { get; }

        public Connect4BoardState(Player[][] columns)
        {
            Columns = columns;
        }

        protected override BaseBoardState<Connect4MoveInfo> Clone()
        {
            var clonedData = Columns.Select(column => column.ToArray()).ToArray();
            return new Connect4BoardState(clonedData);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            var numberOfRows = Columns[0].Length;
            for (int row = 0; row < numberOfRows; row++)
            {
                stringBuilder.Append(" |");

                for (var column = 0; column < Columns.Length; column++)
                {
                    var player = Columns[column][numberOfRows - row - 1];
                    var playerAsString = Connect4Board.FromPlayerToString(player);
                    stringBuilder.Append($"{playerAsString}|");
                }

                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }
    }
}
