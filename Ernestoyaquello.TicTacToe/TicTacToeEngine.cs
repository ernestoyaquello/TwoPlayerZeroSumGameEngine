using Ernestoyaquello.TicTacToe.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine;

namespace Ernestoyaquello.TicTacToe
{
    public class TicTacToeEngine
        : BaseTwoPlayerZeroSumGameEngine<TicTacToeBoard, TicTacToeMoveInfo, TicTacToeBoardState>
    {
        protected override int DefaultMaxTreeDepth => 9;
    }
}
