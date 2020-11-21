using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ernestoyaquello.Chess.Util;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.Chess.Models
{
    public class ChessBoardState : BaseBoardState<ChessMoveInfo>
    {
        public Piece[][] Layout { get; }
        public Dictionary<Player, List<Piece>> PositionedPiecesPerPlayer { get; }
        public Dictionary<Player, List<Piece>> CapturedPiecesPerPlayer { get; }

        public ChessBoardState(Piece[][] layout, Dictionary<Player, List<Piece>> positionedPiecesPerPlayer = null, Dictionary<Player, List<Piece>> capturedPiecesPerPlayer = null)
            : base()
        {
            Layout = layout;
            PositionedPiecesPerPlayer = positionedPiecesPerPlayer ?? new Dictionary<Player, List<Piece>>();
            CapturedPiecesPerPlayer = capturedPiecesPerPlayer ?? new Dictionary<Player, List<Piece>>();
        }

        protected override BaseBoardState<ChessMoveInfo> Clone()
        {
            var clonedBoardLayout = Layout.Select(column => column.ToArray()).ToArray();
            var clonedPositionedPiecesPerPlayer = PositionedPiecesPerPlayer.ToDictionary(x => x.Key, x => x.Value?.ToList());
            var clonedCapturedPiecesPerPlayer = CapturedPiecesPerPlayer.ToDictionary(x => x.Key, x => x.Value?.ToList());
            return new ChessBoardState(clonedBoardLayout, clonedPositionedPiecesPerPlayer, clonedCapturedPiecesPerPlayer);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            for (int verticalIndex = 0; verticalIndex < 8; verticalIndex++)
            {
                stringBuilder.Append("+----+----+----+----+----+----+----+----+\n");

                for (int horizontalIndex = 0; horizontalIndex < 8; horizontalIndex++)
                {
                    var piece = Layout[verticalIndex][horizontalIndex];
                    stringBuilder.Append(!piece.IsNone() ? $"| {piece.ToSimplifiedString()} " : "|    ");
                }

                stringBuilder.Append("|\n");
            }

            stringBuilder.Append("+----+----+----+----+----+----+----+----+\n");

            return stringBuilder.ToString();
        }
    }
}
