using System.Linq;
using System.Text;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.TicTacToe.Models
{
    public class TicTacToeBoardState : BaseBoardState<TicTacToeMoveInfo>
    {
        public Player[][] BoardData { get; }

        public TicTacToeBoardState(Player[][] boardData)
        {
            BoardData = boardData;
        }

        protected override BaseBoardState<TicTacToeMoveInfo> Clone()
        {
            var clonedData = BoardData.Select(row => row.ToArray()).ToArray();
            return new TicTacToeBoardState(clonedData);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            for (var row = 0; row < 3; row++)
            {
                if (row == 0)
                {
                    stringBuilder.Append("    1   2   3 \n\n");
                }

                for (var column = 0; column < 3; column++)
                {
                    if (column == 0)
                    {
                        var rowAsString = TicTacToeBoard.FromRowTostring(row);
                        stringBuilder.Append($"{rowAsString}  ");
                    }

                    var player = BoardData[row][column];
                    var playerAsString = TicTacToeBoard.FromPlayerToString(player);
                    stringBuilder.Append($" {playerAsString} ");

                    if (column < 2)
                    {
                        stringBuilder.Append("|");
                    }
                }

                stringBuilder.Append("\n");
                if (row < 2)
                {
                    stringBuilder.Append("   ---+---+---");
                    stringBuilder.Append("\n");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
