namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models
{
    /// <summary>
    /// A model implementing this base class will represent a move that a certain player could make.
    /// For example, in a game like Connect 4, a move could be "player 1 puts chip in column 3".
    /// Once created, all its properties must be immutable.
    /// </summary>
    public abstract class BaseMoveInfo
    {
        /// <summary>
        /// The player who will make the move.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// A score that indicates how likely this move is to produce a good output.
        /// </summary>
        public double Score { get; protected set; }

        protected BaseMoveInfo(Player player, double score = 0d)
        {
            Player = player;
            Score = score;
        }

        /// <summary>
        /// Returns a string that represents the move as a human-readable text.
        /// </summary>
        /// <returns>The string that represents the move.</returns>
        public override abstract string ToString();

        public override abstract bool Equals(object other);

        public override abstract int GetHashCode();
    }
}
