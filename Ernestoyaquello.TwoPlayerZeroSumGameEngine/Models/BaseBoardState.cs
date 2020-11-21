using System.Collections.Generic;
using System.Linq;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models
{
    /// <summary>
    /// A model implementing this base class will represent the board state and hold its data.
    /// For example, in a game like Connect 4, it will contain the information about where each chip is.
    /// </summary>
    /// <typeparam name="TMoveInfo">The type of moves that the board represented by this state makes use of.</typeparam>
    public abstract class BaseBoardState<TMoveInfo> where TMoveInfo : BaseMoveInfo
    {
        internal List<TMoveInfo> History { get; private set; }
        internal StateCache<TMoveInfo> CachedData { get; set; }

        protected BaseBoardState()
        {
            History = new List<TMoveInfo>();
            CachedData = new StateCache<TMoveInfo>();
        }

        public BaseBoardState<TMoveInfo> CloneBoardState()
        {
            var clone = Clone();
            clone.History = History.ToList();
            clone.CachedData = CachedData.Clone();

            return clone;
        }

        /// <summary>
        /// Returns a copy of the object.
        /// </summary>
        /// <returns>A copy of the object.</returns>
        protected abstract BaseBoardState<TMoveInfo> Clone();

        /// <summary>
        /// Gets the string representation of the board based on its current state.
        /// </summary>
        /// <returns>The string representation of the board on its current state.</returns>
        public override abstract string ToString();
    }
}
