namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models
{
    /// <summary>
    /// A model implementing this base class will represent the board state and hold its data.
    /// For example, in a game like Connect 4, it will contain the information about where each chip is.
    /// </summary>
    /// <typeparam name="TMoveInfo">The type of moves that the board represented by this state makes use of.</typeparam>
    public abstract class BaseBoardState<TMoveInfo> where TMoveInfo : BaseMoveInfo
    {
        internal StateCache<TMoveInfo> CachedData { get; set; }

        public BaseBoardState<TMoveInfo> CloneBoardState()
        {
            var clone = Clone();
            clone.CachedData = CachedData?.Clone();

            return clone;
        }

        protected abstract BaseBoardState<TMoveInfo> Clone();

        /// <summary>
        /// Gets the string representation of the board based on its current state.
        /// </summary>
        /// <returns>The string representation of the board on its current state.</returns>
        public override abstract string ToString();
    }
}
