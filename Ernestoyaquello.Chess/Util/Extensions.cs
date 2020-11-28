using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.Chess.Util
{
    public static class Extensions
    {
        public static bool IsNone(this Piece piece)
        {
            return piece == null || piece.Type == PieceType.None || piece.Player == Player.None;
        }
    }
}
