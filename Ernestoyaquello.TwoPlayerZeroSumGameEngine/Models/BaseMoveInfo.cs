namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models
{
    /// <summary>
    /// A model implementing this base class will represent a move that a certain player could make.
    /// For example, in a game like Connect 4, a move could be "player 1 puts chip in column 3".
    /// </summary>
    public abstract class BaseMoveInfo
    {
        /// <summary>
        /// The player who will make the move.
        /// </summary>
        public Player Player { get; }

        protected BaseMoveInfo(Player player)
        {
            Player = player;
        }

        /// <summary>
        /// Returns a string that represents the move as a human-readable text.
        /// </summary>
        /// <returns>The string that represents the move.</returns>
        public override abstract string ToString();
    }
}
