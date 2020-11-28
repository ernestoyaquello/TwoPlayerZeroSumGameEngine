using System.Collections.Generic;
using System.Linq;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models
{
    internal class StateCache<TMoveInfo> where TMoveInfo : BaseMoveInfo
    {
        public Player? Winner { get; set; }
        public double? Score { get; set; }
        public GameResult? ResultFirstPlayer { get; set; }
        public GameResult? ResultSecondPlayer { get; set; }
        public Dictionary<Player, List<TMoveInfo>> ValidMovesPerPlayer { get; private set; }

        public StateCache()
        {
            Winner = null;
            Score = null;
            ResultFirstPlayer = null;
            ResultSecondPlayer = null;
            ValidMovesPerPlayer = new Dictionary<Player, List<TMoveInfo>>();
        }

        public StateCache<TMoveInfo> Clone()
        {
            return new StateCache<TMoveInfo>
            {
                Winner = Winner,
                Score = Score,
                ResultFirstPlayer = ResultFirstPlayer,
                ResultSecondPlayer = ResultSecondPlayer,
                ValidMovesPerPlayer = ValidMovesPerPlayer.ToDictionary(x => x.Key, x => x.Value.ToList()),
            };
        }
    }
}
