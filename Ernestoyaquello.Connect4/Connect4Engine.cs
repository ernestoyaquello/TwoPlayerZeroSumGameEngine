using Ernestoyaquello.Connect4.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine;

namespace Ernestoyaquello.Connect4
{
    public class Connect4Engine
        : BaseTwoPlayerZeroSumGameEngine<Connect4Board, Connect4MoveInfo, Connect4BoardState>
    {
        protected override int DefaultMaxTreeDepth => 8;
    }
}
