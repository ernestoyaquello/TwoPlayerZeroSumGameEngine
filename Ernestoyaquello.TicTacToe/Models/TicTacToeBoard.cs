using System.Collections.Generic;
using System.Linq;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.TicTacToe.Models
{
    public class TicTacToeBoard : BaseBoard<TicTacToeMoveInfo, TicTacToeBoardState>
    {
        public TicTacToeBoard(ITwoPlayerZeroSumGameMovesEngine movesEngine)
            : this(new TicTacToeBoardState(new Player[3][]
            {
                new Player[3] { Player.None, Player.None, Player.None },
                new Player[3] { Player.None, Player.None, Player.None },
                new Player[3] { Player.None, Player.None, Player.None },
            }), movesEngine)
        {
        }

        private TicTacToeBoard(TicTacToeBoardState boardState, ITwoPlayerZeroSumGameMovesEngine movesEngine)
            : base(boardState, movesEngine)
        {
        }

        public override bool IsValidMove(TicTacToeMoveInfo moveInfo)
        {
            return State.BoardData[moveInfo.Row][moveInfo.Column] == Player.None &&
                GetWinner() == Player.None;
        }

        public override void MakeMove(TicTacToeMoveInfo moveInfo)
        {
            State.BoardData[moveInfo.Row][moveInfo.Column] = moveInfo.Player;
        }

        public override bool AreThereValidMoves(Player player)
        {
            var allValues = State.BoardData.SelectMany(row => row);
            return allValues.Any(player => player == Player.None) &&
                GetWinner() == Player.None;
        }

        protected override List<TicTacToeMoveInfo> CalculateValidMoves(Player player)
        {
            var validMoves = new List<TicTacToeMoveInfo>();

            for (var row = 0; row < 3; row++)
            {
                for (var column = 0; column < 3; column++)
                {
                    var move = new TicTacToeMoveInfo(player, row, column);
                    if (IsValidMove(move))
                    {
                        validMoves.Add(move);
                    }
                }
            }

            return validMoves;
        }

        protected override Player CalculateWinner()
        {
            var winnerInRows = CalculateWinnerInRows();
            if (winnerInRows != Player.None)
            {
                return winnerInRows;
            }

            var winnerInColumns = CalculateWinnerInColumns();
            if (winnerInColumns != Player.None)
            {
                return winnerInColumns;
            }

            var winnerInDiagonals = CalculateWinnerInDiagonals();
            if (winnerInDiagonals != Player.None)
            {
                return winnerInDiagonals;
            }

            return Player.None;
        }

        private Player CalculateWinnerInRows()
        {
            for (var row = 0; row < 3; row++)
            {
                var rowValues = State.BoardData[row];
                if (rowValues.Distinct().Count() == 1 && rowValues.First() != Player.None)
                {
                    return rowValues.First();
                }
            }

            return Player.None;
        }

        private Player CalculateWinnerInColumns()
        {
            for (var column = 0; column < 3; column++)
            {
                var columnValues = new Player[3]
                {
                    State.BoardData[0][column],
                    State.BoardData[1][column],
                    State.BoardData[2][column],
                };
                if (columnValues.Distinct().Count() == 1 && columnValues.First() != Player.None)
                {
                    return columnValues.First();
                }
            }

            return Player.None;
        }

        private Player CalculateWinnerInDiagonals()
        {
            var firstDiagonalValues = new Player[3] { State.BoardData[0][0], State.BoardData[1][1], State.BoardData[2][2] };
            if (firstDiagonalValues.Distinct().Count() == 1 && firstDiagonalValues.First() != Player.None)
            {
                return firstDiagonalValues.First();
            }

            var secondDiagonalValues = new Player[3] { State.BoardData[0][2], State.BoardData[1][1], State.BoardData[2][0] };
            if (secondDiagonalValues.Distinct().Count() == 1 && secondDiagonalValues.First() != Player.None)
            {
                return secondDiagonalValues.First();
            }

            return Player.None;
        }

        protected override double CalculateHeuristicGameScore(Player player)
        {
            // This game is simple enough for the game engine to be able to check all the possible available moves,
            // which means that this function will never be called (the tree must have a maximum depth of 9 or more)
            return 0d;
        }

        public static string FromPlayerToString(Player player)
        {
            switch (player)
            {
                case Player.First:
                    return "O";

                case Player.Second:
                    return "X";

                case Player.None:
                default:
                    return " ";
            }
        }

        public static string FromRowTostring(int row)
        {
            switch (row)
            {
                case 2:
                    return "C";

                case 1:
                    return "B";

                case 0:
                default:
                    return "A";
            }
        }

        protected override BaseBoard<TicTacToeMoveInfo, TicTacToeBoardState> CreateNew(TicTacToeBoardState boardState, ITwoPlayerZeroSumGameMovesEngine movesEngine)
        {
            return new TicTacToeBoard(boardState, movesEngine);
        }
    }
}
