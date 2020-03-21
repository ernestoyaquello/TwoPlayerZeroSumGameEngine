using System.Collections.Generic;
using System.Linq;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models
{
    internal class StateCache<TMoveInfo> where TMoveInfo : BaseMoveInfo
    {
        internal Player? Winner { get; set; }
        internal double? Score { get; set; }
        internal GameResult? Result { get; set; }
        internal Dictionary<Player, IList<TMoveInfo>> ValidMovesPerPlayer { get; private set; }

        internal StateCache()
        {
            Winner = null;
            Score = null;
            Result = null;
            ValidMovesPerPlayer = new Dictionary<Player, IList<TMoveInfo>>();
        }

        internal StateCache<TMoveInfo> Clone()
        {
            return new StateCache<TMoveInfo>
            {
                Winner = Winner,
                Score = Score,
                Result = Result,
                ValidMovesPerPlayer = ValidMovesPerPlayer.ToDictionary(x => x.Key, x => x.Value),
            };
        }
    }
}
