using System.Collections.Generic;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models
{
    /// <summary>
    /// Will be implemented to hold the state of a board game and manage its changes by applying the necessary rules.
    /// </summary>
    /// <typeparam name="TMoveInfo">The type of moves that this board will make use of.</typeparam>
    /// <typeparam name="TBoardState">The type of the board state.</typeparam>
    public interface IBoard<TMoveInfo, TBoardState>
        where TMoveInfo : BaseMoveInfo
        where TBoardState : BaseBoardState<TMoveInfo>
    {
        /// <summary>
        /// Checks if the move is valid and makes it in case it is.
        /// </summary>
        /// <param name="moveInfo">The move to make.</param>
        /// <returns>True if the move was made; false otherwise.</returns>
        bool TryMakeMove(TMoveInfo moveInfo);

        /// <summary>
        /// Gets a list with all the valid moves for the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The list of valid moves.</returns>
        IList<TMoveInfo> GetValidMoves(Player player);

        /// <summary>
        /// Gets the game result obtained by the specified player on the current board.
        /// </summary>
        /// <param name="player">The player for whom the game result will be retrieved.</param>
        /// <returns>The game result of the player.</returns>
        GameResult GetGameResult(Player player);

        /// <summary>
        /// Gets the game score obtained by the specified player on the current board.
        /// </summary>
        /// <param name="player">The player for whom the game score will be retrieved.</param>
        /// <returns>The game score of the player.</returns>
        double GetGameScore(Player player);

        /// <summary>
        /// Gets the winning player on this board.
        /// </summary>
        /// <returns>The winning player, or null if no player can be considered a winner.</returns>
        Player GetWinner();

        /// <summary>
        /// Gets the current state of the board.
        /// </summary>
        /// <returns>The current state of the board.</returns>
        TBoardState GetStateCopy();

        /// <summary>
        /// Restores the board to the provided state.
        /// </summary>
        /// <param name="state">The state to which the board will be restored.</param>
        void RestoreStateFromCopy(TBoardState state);

        /// <summary>
        /// Gets the string representation of the board on its current state.
        /// </summary>
        /// <returns>The string representation of the board on its current state.</returns>
        string ToString();
    }
}
