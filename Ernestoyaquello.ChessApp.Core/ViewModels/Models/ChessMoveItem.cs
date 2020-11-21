using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.ChessApp.ViewModels.Models
{
    public class ChessMoveItem
    {
        public string PlayerName { get; }
        public string ShortDescription { get; }
        public string LongDescription { get; }
        public ChessMoveInfo Move { get; }

        private ChessMoveItem(ChessMoveInfo move, Player humanPlayer)
        {
            PlayerName = move.Player == humanPlayer ? "You:" : "Machine:";
            ShortDescription = ""; // TODO Proper chess notation?
            LongDescription = move.ToString();
            Move = move;
        }

        public static ChessMoveItem FromMove(ChessMoveInfo move, Player humanPlayer)
        {
            return new ChessMoveItem(move, humanPlayer);
        }
    }
}
