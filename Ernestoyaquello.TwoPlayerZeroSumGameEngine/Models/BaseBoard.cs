using System.Collections.Generic;
using System.Linq;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Util;

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
        public TBoardState State { get; }
        public ITwoPlayerZeroSumGameMovesEngine MovesEngine { get; }

        protected BaseBoard(TBoardState state, ITwoPlayerZeroSumGameMovesEngine movesEngine)
        {
            State = state;
            MovesEngine = movesEngine;
        }

        public List<TMoveInfo> GetValidMoves(Player player)
        {
            // Since calculating the valid moves of a player can be expensive, we only do it if necessary
            if (!State.CachedData.ValidMovesPerPlayer.ContainsKey(player))
            {
                State.CachedData.ValidMovesPerPlayer[player] = CalculateValidMoves(player);
            }

            return State.CachedData.ValidMovesPerPlayer[player];
        }

        public bool IsPlayerTurn(Player player)
        {
            return (!State.History.Any() && player == Player.First) || (State.History.Any() && State.History.Last().Player != player);
        }

        public GameResult GetGameResult(Player player)
        {
            // Since calculating the game result can be expensive, we only do it if necessary
            if (State.CachedData.ResultFirstPlayer == null || State.CachedData.ResultSecondPlayer == null)
            {
                if (player == Player.First)
                {
                    State.CachedData.ResultFirstPlayer = CalculateGameResult(Player.First);
                    State.CachedData.ResultSecondPlayer = State.CachedData.ResultFirstPlayer.InvertResult();
                }
                else
                {
                    State.CachedData.ResultSecondPlayer = CalculateGameResult(Player.Second);
                    State.CachedData.ResultFirstPlayer = State.CachedData.ResultSecondPlayer.InvertResult();
                }

                // The game result can already tell us who the winner is, so we set it in the cache to avoid unnecessary calculations
                var result = player == Player.First ? State.CachedData.ResultFirstPlayer.Value : State.CachedData.ResultSecondPlayer.Value;
                if (result == GameResult.Victory || result == GameResult.Defeat)
                {
                    State.CachedData.Winner = result == GameResult.Victory ? player : player.ToOppositePlayer();
                }

                return result;
            }

            return player == Player.First ? State.CachedData.ResultFirstPlayer.Value : State.CachedData.ResultSecondPlayer.Value;
        }

        public double GetGameScore(Player player)
        {
            // Since calculating the game score can be expensive, we only do it if necessary
            State.CachedData.Score ??= CalculateGameScore(player);

            return (double)State.CachedData.Score;
        }

        public Player GetWinner()
        {
            // Since calculating a winner can be expensive, we only do it if necessary
            if (State.CachedData.Winner == null)
            {
                var winer = CalculateWinner();
                State.CachedData.Winner = winer;
                SetWinner(winer);
            }

            return (Player)State.CachedData.Winner;
        }

        protected void SetWinner(Player winner)
        {
            State.CachedData.Winner = winner;

            // The winner can already tell us what the game result is, so we set it in the cache to avoid unnecessary calculations
            if (State.CachedData.Winner.Value != Player.None)
            {
                State.CachedData.ResultFirstPlayer = State.CachedData.Winner.Value == Player.First ? GameResult.Victory : GameResult.Defeat;
                State.CachedData.ResultSecondPlayer = State.CachedData.Winner.Value == Player.Second ? GameResult.Victory : GameResult.Defeat;
            }
        }

        private GameResult CalculateGameResult(Player player)
        {
            var winner = GetWinner();
            if (winner == Player.None)
            {
                var currentPlayer = IsPlayerTurn(Player.First) ? Player.First : Player.Second;
                if (AreThereValidMoves(currentPlayer))
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

        public virtual bool AreThereValidMoves(Player player)
        {
            return GetValidMoves(player).Any();
        }

        public List<TMoveInfo> GetMoveHistory()
        {
            return State.History.ToList();
        }

        public IBoard<TMoveInfo, TBoardState> Clone()
        {
            var clonedBoardState = State.CloneBoardState() as TBoardState;
            return CreateNew(clonedBoardState, MovesEngine);
        }

        public override string ToString()
        {
            return State.ToString();
        }

        public abstract bool IsValidMove(TMoveInfo moveInfo);

        public abstract void MakeMove(TMoveInfo moveInfo);

        /// <summary>
        /// Calculates the valid moves for the specified player. Will be called automatically by the game engine.
        /// </summary>
        /// <param name="player">The player for whom the moves will be calculated.</param>
        /// <returns>A list with all the valid moves for the specified player.</returns>
        protected abstract List<TMoveInfo> CalculateValidMoves(Player player);

        /// <summary>
        /// Calculates the winning player on this board. Will be called automatically by the game engine.
        /// </summary>
        /// <returns>The winning player, or null if no player can be considered a winner.</returns>
        protected abstract Player CalculateWinner();

        /// <summary>
        /// Calculates a heuristic score that indicates how likely the specified player is to win or lose.
        /// Positive values indicate likelihood of victory, while negative values indicate likelihood of defeat.
        /// Since it will only be called when the game still doesn't have a winner, the returned values must be
        /// lower than +1 and higher than -1, which are reserved for actual, certain victories and defeats.
        /// Will be called automatically by the game engine.
        /// </summary>
        /// <param name="player">The player for whom the score will be calculated.</param>
        /// <returns>The score that estimates how likely the player is to win or lose.</returns>
        protected abstract double CalculateHeuristicGameScore(Player player);

        /// <summary>
        /// Gets a new instance of the board. Will be called automatically by the game engine.
        /// </summary>
        /// <param name="boardState">The state of the board to create.</param>
        /// <param name="movesEngine">The moves engine that the board will make use of.</param>
        /// <returns>A new instance of the board created with the specified parameters.</returns>
        protected abstract BaseBoard<TMoveInfo, TBoardState> CreateNew(TBoardState boardState, ITwoPlayerZeroSumGameMovesEngine movesEngine);
    }
}
