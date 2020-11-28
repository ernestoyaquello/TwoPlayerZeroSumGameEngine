using System;
using System.Threading.Tasks;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine
{
    // TODO Implement cancellation in the asynchronous methods
    public interface ITwoPlayerZeroSumGameMovesEngine
    {
        /// <summary>
        /// The maximum depth that the algorithm will use to calculate the best moves.
        /// </summary>
        int MaxTreeDepth { get; set; }

        /// <summary>
        /// Initializes the moves engine.
        /// </summary>
        /// <param name="MaxTreeDepth">The maximum depth that the algorithm will use to calculate the best moves.</param>
        void Initialize(int MaxTreeDepth);

        /// <summary>
        /// Checks if the move is valid and makes it in case it is.
        /// </summary>
        /// <param name="board">The board where the move will be performed.</param>
        /// <param name="moveInfo">The move to make.</param>
        /// <param name="shouldBeValid">True if the move must be verified as valid in order to be made; false if not.</param>
        /// <returns>True if the move was made; false otherwise.</returns>
        bool TryMakeMove<TBoard, TMoveInfo, TBoardState>(TBoard board, TMoveInfo moveInfo, bool shouldBeValid = true)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>;

        /// <summary>
        /// Checks if the move is valid, makes it in case it is, then executes the provided action, and then restores
        /// the board state to how it was before making the move in order to pretend that the move was never made.
        /// Useful to check what would happen if a certain move were to be made.
        /// </summary>
        /// <param name="board">The board where the move will be performed.</param>
        /// <param name="moveInfo">The move to make.</param>
        /// <param name="actionAfterMove">The action that will be invoked once the move is made.</param>
        /// <param name="shouldBeValid">True if the move must be verified as valid in order to be made; false if not.</param>
        /// <returns>True if the move was made successfully; false otherwise.</returns>
        bool TryMakeDummyMove<TBoard, TMoveInfo, TBoardState>(TBoard board, TMoveInfo moveInfo, Action<TBoard> actionAfterMove, bool shouldBeValid = true)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>;

        /// <summary>
        /// Checks if the move is valid, makes it in case it is, then executes the provided action, and then restores
        /// the board state to how it was before making the move in order to pretend that the move was never made.
        /// Useful to check what would happen if a certain move were to be made.
        /// </summary>
        /// <param name="board">The board where the move will be performed.</param>
        /// <param name="moveInfo">The move to make.</param>
        /// <param name="actionAfterMove">The action that will be invoked once the move is made.</param>
        /// <param name="shouldBeValid">True if the move must be verified as valid in order to be made; false if not.</param>
        /// <returns>True if the move was made successfully; false otherwise.</returns>
        Task<bool> TryMakeDummyMove<TBoard, TMoveInfo, TBoardState>(TBoard board, TMoveInfo moveInfo, Func<TBoard, Task> actionAfterMove, bool shouldBeValid = true)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>;

        /// <summary>
        /// First, it makes the manual move (i.e., human move) specified in the parameters.
        /// Then, it automatically calculates and makes the opponent's move (i.e., the machine's move), if any.
        /// It will return the information about the move chosen and made by the machine.
        /// </summary>
        /// <param name="board">The board where the move(s) will be performed.</param>
        /// <param name="moveInfo">The manual move to be made.</param>
        /// <param name="shouldBeValid">True if the move(s) must be verified as valid in order to be made; false if not.</param>
        /// <param name="onProgressMade">The callback that will be invoked when the algorithm makes some progress.</param>
        /// <returns>A result holder with information about the move made by the machine.</returns>
        Task<GetBestMoveResult<TMoveInfo>> MakeMoveAgainstMachine<TBoard, TMoveInfo, TBoardState>(TBoard board, TMoveInfo moveInfo, bool shouldBeValid = true, Action<double> onProgressMade = null)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>;

        /// <summary>
        /// Calculates and gets the information related to the best possible move for the specified player.
        /// </summary>
        /// <param name="board">The board where the move will be performed.</param>
        /// <param name="player">The player for whom the best possible move will be found.</param>
        /// <param name="onProgressMade">The callback that will be invoked when the algorithm makes some progress.</param>
        /// <returns>A result holder with information about the best possible move.</returns>
        Task<GetBestMoveResult<TMoveInfo>> CalculateBestMove<TBoard, TMoveInfo, TBoardState>(TBoard board, Player player, Action<double> onProgressMade = null)
            where TBoard : BaseBoard<TMoveInfo, TBoardState>
            where TMoveInfo : BaseMoveInfo
            where TBoardState : BaseBoardState<TMoveInfo>;
    }
}
