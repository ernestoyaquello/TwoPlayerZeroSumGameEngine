using System.Collections.Generic;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models
{
    /// <summary>
    /// Will implement the necessary game-related logic (rules, heuristics, etc.) for the moves engine to be able to calculate and make moves.
    /// It shouldn't hold any information about the game, as said information is to be stored in the state instance.
    /// </summary>
    /// <typeparam name="TMoveInfo">The type of moves that this board will make use of.</typeparam>
    /// <typeparam name="TBoardState">The type of the board state.</typeparam>
    public interface IBoard<TMoveInfo, TBoardState>
        where TMoveInfo : BaseMoveInfo
        where TBoardState : BaseBoardState<TMoveInfo>
    {
        /// <summary>
        /// The state of the board. E.g. the positions of all the chips in Connect4.
        /// </summary>
        TBoardState State { get; }

        /// <summary>
        /// The engine that the board will use to make moves.
        /// </summary>
        ITwoPlayerZeroSumGameMovesEngine MovesEngine { get; }

        /// <summary>
        /// Gets a list with all the valid moves for the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The list of valid moves.</returns>
        List<TMoveInfo> GetValidMoves(Player player);
        
        /// <summary>
        /// Determines whether it is the turn of the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if it is the player's turn; false otherwise.</returns>
        bool IsPlayerTurn(Player player);

        /// <summary>
        /// Gets the game result obtained by the specified player on the current board.
        /// </summary>
        /// <param name="player">The player for whom the game result will be retrieved.</param>
        /// <returns>The game result of the player.</returns>
        GameResult GetGameResult(Player player);

        /// <summary>
        /// Gets the game score obtained by the specified player on the current board.
        /// Will be a value between -1 (certain defeat) and +1 (certain victory).
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
        /// Determines whether the specified move is valid or not.
        /// </summary>
        /// <param name="moveInfo">The move to check.</param>
        /// <returns>True if the move is valid and coud be made; false otherwise.</returns>
        bool IsValidMove(TMoveInfo moveInfo);

        /// <summary>
        /// Determines whether or not there are still some valid moves that could be made.
        /// </summary>
        /// <param name="player">The player for whom we will check whether there are valid moves.</param>
        /// <returns>True if some moves could still be made; false otherwise.</returns>
        bool AreThereValidMoves(Player player);

        /// <summary>
        /// Makes the specified move in the board.
        /// Please note that this method is generally not to be used to trigger a move, as it is just the implementation of how a move is made in a specific board.
        /// In order to trigger moves properly, the methods of the engine should be called (those methods will, in turn, call this one).
        /// </summary>
        /// <param name="moveInfo">The move to make.</param>
        void MakeMove(TMoveInfo moveInfo);

        /// <summary>
        /// The history of moves that have been made so far.
        /// </summary>
        /// <returns>The list of moves that have been made in this board so far.</returns>
        List<TMoveInfo> GetMoveHistory();

        /// <summary>
        /// Gets the string representation of the board on its current state.
        /// </summary>
        /// <returns>The string representation of the board on its current state.</returns>
        string ToString();

        /// <summary>
        /// Gets a clone of the board and its state.
        /// </summary>
        /// <returns>A clone of the board.</returns>
        IBoard<TMoveInfo, TBoardState> Clone();
    }
}
