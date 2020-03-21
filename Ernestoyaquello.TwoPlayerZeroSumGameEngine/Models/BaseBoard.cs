using System.Collections.Generic;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models
{
    /// <summary>
    /// A model implementing this base class will represent the game board.
    /// Each game board will have a reference to the board state and it will manage its changes,
    /// which will be caused by player moves.
    /// Thus, this is where the rules and heuristics of the game will be implemented.
    /// </summary>
    /// <typeparam name="TMoveInfo">The type of moves that this board will make use of.</typeparam>
    /// <typeparam name="TBoardState">The type of the board state.</typeparam>
    public abstract class BaseBoard<TMoveInfo, TBoardState> : IBoard<TMoveInfo, TBoardState>
        where TMoveInfo : BaseMoveInfo
        where TBoardState : BaseBoardState<TMoveInfo>
    {
        /// <summary>
        /// The state of the board. E.g. the positions of all the chips.
        /// </summary>
        protected TBoardState State { get; private set; }

        protected BaseBoard(TBoardState state)
        {
            State = state;
        }

        ///<inheritdoc/>
        public bool TryMakeMove(TMoveInfo moveInfo)
        {
            var isValidMove = IsValidMove(moveInfo);
            if (isValidMove)
            {
                ClearCachedData();
                MakeMove(moveInfo);
            }

            return isValidMove;
        }

        ///<inheritdoc/>
        public IList<TMoveInfo> GetValidMoves(Player player)
        {
            // Since calculating the valid moves of a player can be expensive, we only do it if necessary
            var cachedData = GetCachedData();
            if (!cachedData.ValidMovesPerPlayer.ContainsKey(player))
            {
                cachedData.ValidMovesPerPlayer[player] = CalculateValidMoves(player);
            }

            return cachedData.ValidMovesPerPlayer[player];
        }

        ///<inheritdoc/>
        public GameResult GetGameResult(Player player)
        {
            // Since calculating the game result can be expensive, we only do it if necessary
            var cachedData = GetCachedData();
            cachedData.Result ??= CalculateGameResult(player);

            return (GameResult)cachedData.Result;
        }

        ///<inheritdoc/>
        public double GetGameScore(Player player)
        {
            // Since calculating the game score can be expensive, we only do it if necessary
            var cachedData = GetCachedData();
            cachedData.Score ??= CalculateGameScore(player);

            return (double)cachedData.Score;
        }

        ///<inheritdoc/>
        public Player GetWinner()
        {
            // Since calculating a winner can be expensive, we only do it if necessary
            var cachedData = GetCachedData();
            cachedData.Winner ??= CalculateWinner();

            return (Player)cachedData.Winner;
        }

        private GameResult CalculateGameResult(Player player)
        {
            var winner = GetWinner();
            if (winner == Player.None)
            {
                if (AreThereValidMoves())
                {
                    return GameResult.StillGame;
                }

                return GameResult.Tie;
            }

            return winner == player ? GameResult.Victory : GameResult.Defeat;
        }

        private double CalculateGameScore(Player player)
        {
            return GetGameResult(player) switch
            {
                GameResult.Victory => 1d,
                GameResult.Defeat => -1d,
                GameResult.Tie => 0d,
                _ => CalculateHeuristicGameScore(player),
            };
        }

        private void ClearCachedData()
        {
            State.CachedData = null;
        }

        private StateCache<TMoveInfo> GetCachedData()
        {
            State.CachedData = State.CachedData ?? new StateCache<TMoveInfo>();
            return State.CachedData;
        }

        ///<inheritdoc/>
        public TBoardState GetStateCopy()
        {
            return State.CloneBoardState() as TBoardState;
        }

        ///<inheritdoc/>
        public void RestoreStateFromCopy(TBoardState state)
        {
            State = state.CloneBoardState() as TBoardState;
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            return State.ToString();
        }

        /// <summary>
        /// Makes the specified move.
        /// </summary>
        /// <param name="moveInfo">The move to make.</param>
        protected abstract void MakeMove(TMoveInfo moveInfo);

        /// <summary>
        /// Determines whether the specified move is valid or not.
        /// </summary>
        /// <param name="moveInfo">The move to check.</param>
        /// <returns>True if the move is valid and coud be made; false otherwise.</returns>
        protected abstract bool IsValidMove(TMoveInfo moveInfo);

        /// <summary>
        /// Determines whether or not there are still some valid moves that could be made.
        /// </summary>
        /// <returns>True if some moves could still be made; false otherwise.</returns>
        protected abstract bool AreThereValidMoves();

        /// <summary>
        /// Calculates the valid moves for the specified player.
        /// </summary>
        /// <param name="player">The player for whom the moves will be calculated.</param>
        /// <returns>A list with all the valid moves for the specified player.</returns>
        protected abstract IList<TMoveInfo> CalculateValidMoves(Player player);

        /// <summary>
        /// Calculates the winning player on this board.
        /// </summary>
        /// <returns>The winning player, or null if no player can be considered a winner.</returns>
        protected abstract Player CalculateWinner();

        /// <summary>
        /// Calculates a heuristic score that indicates how likely the specified player is to win or lose.
        /// Positive values indicate likelihood of victory, while negative values indicate likelihood of defeat.
        /// Since it will only be called when the game still doesn't have a winner, the returned values must be
        /// between -0.999 and +0.999, as +1 and -1 are reserved for actual victories and defeats.
        /// </summary>
        /// <param name="player">The player for whom the score will be calculated.</param>
        /// <returns>The score that estimates how likely the player is to win or lose.</returns>
        protected abstract double CalculateHeuristicGameScore(Player player);
    }
}
