using System.Collections.Generic;
using System.Linq;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Util;

namespace Ernestoyaquello.Connect4.Models
{
    public class Connect4Board : BaseBoard<Connect4MoveInfo, Connect4BoardState>
    {
        public Connect4Board(ITwoPlayerZeroSumGameMovesEngine movesEngine)
            : this(new Connect4BoardState(new Player[7][]
            {
                new Player[6] { Player.None, Player.None, Player.None, Player.None, Player.None, Player.None },
                new Player[6] { Player.None, Player.None, Player.None, Player.None, Player.None, Player.None },
                new Player[6] { Player.None, Player.None, Player.None, Player.None, Player.None, Player.None },
                new Player[6] { Player.None, Player.None, Player.None, Player.None, Player.None, Player.None },
                new Player[6] { Player.None, Player.None, Player.None, Player.None, Player.None, Player.None },
                new Player[6] { Player.None, Player.None, Player.None, Player.None, Player.None, Player.None },
                new Player[6] { Player.None, Player.None, Player.None, Player.None, Player.None, Player.None },
            }), movesEngine)
        {
        }

        private Connect4Board(Connect4BoardState boardState, ITwoPlayerZeroSumGameMovesEngine movesEngine)
            : base(boardState, movesEngine)
        {
        }

        public override bool IsValidMove(Connect4MoveInfo moveInfo)
        {
            var column = State.Columns[moveInfo.Column];
            return column.Any(player => player == Player.None) &&
                GetWinner() == Player.None;
        }

        public override void MakeMove(Connect4MoveInfo moveInfo)
        {
            var column = State.Columns[moveInfo.Column];
            var position = column.Count(player => player != Player.None);
            column[position] = moveInfo.Player;
        }

        public override bool AreThereValidMoves(Player player)
        {
            var allValues = State.Columns.SelectMany(column => column);
            return allValues.Any(player => player == Player.None) &&
                GetWinner() == Player.None;
        }

        protected override List<Connect4MoveInfo> CalculateValidMoves(Player player)
        {
            return State.Columns
                .Select((column, index) => column.Any(player => player == Player.None)
                    ? new Connect4MoveInfo(player, index)
                    : null)
                .Where(move => move != null)
                .ToList();
        }

        protected override Player CalculateWinner()
        {
            var winnerInColumns = CalculateWinnerInColumns();
            if (winnerInColumns != Player.None)
            {
                return winnerInColumns;
            }

            var winnerInRows = CalculateWinnerInRows();
            if (winnerInRows != Player.None)
            {
                return winnerInRows;
            }

            var winnerInDiagonals = CalculateWinnerInDiagonals();
            if (winnerInDiagonals != Player.None)
            {
                return winnerInDiagonals;
            }

            return Player.None;
        }

        private Player CalculateWinnerInColumns()
        {
            var numberOfRows = State.Columns[0].Length;
            for (int column = 0; column < State.Columns.Length; column++)
            {
                var winner = Player.None;
                var contiguousOccurrences = 0;
                for (int row = 0; row < numberOfRows; row++)
                {
                    var remainingIterations = numberOfRows - row - 1;
                    var finishIteration = TryCalculateWinnerInIteration(
                        column, row, remainingIterations, ref winner, ref contiguousOccurrences);
                    if (finishIteration)
                    {
                        if (winner != Player.None)
                        {
                            return winner;
                        }

                        break;
                    }
                }
            }

            return Player.None;
        }

        private Player CalculateWinnerInRows()
        {
            var numberOfRows = State.Columns[0].Length;
            for (int row = 0; row < numberOfRows; row++)
            {
                var winner = Player.None;
                var contiguousOccurrences = 0;
                for (int column = 0; column < State.Columns.Length; column++)
                {
                    var remainingIterations = State.Columns.Length - column - 1;
                    var finishIteration = TryCalculateWinnerInIteration(
                        column, row, remainingIterations, ref winner, ref contiguousOccurrences);
                    if (finishIteration)
                    {
                        if (winner != Player.None)
                        {
                            return winner;
                        }

                        break;
                    }
                }
            }

            return Player.None;
        }

        private bool TryCalculateWinnerInIteration(
            int column,
            int row,
            int remainingIterations,
            ref Player winnerCandidate,
            ref int contiguousOccurrences)
        {
            var player = State.Columns[column][row];
            if (player != winnerCandidate)
            {
                winnerCandidate = player;
                contiguousOccurrences = 0;
            }

            if (winnerCandidate != Player.None && ++contiguousOccurrences == 4)
            {
                // We found the winner; no need to continue iterating
                return true;
            }

            if ((4 - contiguousOccurrences) > remainingIterations)
            {
                // There aren't enough iterations remaining to be able to find a winner
                winnerCandidate = Player.None;
                return true;
            }

            // We have to keep iterating in order to keep looking for a winner
            return false;
        }

        private Player CalculateWinnerInDiagonals()
        {
            var numberOfRows = State.Columns[0].Length;
            for (int row = 0; row < numberOfRows - 3; row++)
            {
                for (int column = 0; column < State.Columns.Length; column++)
                {
                    var winnerCandidate = State.Columns[column][row];
                    if (winnerCandidate == Player.None)
                    {
                        continue;
                    }

                    if ((column + 3) < State.Columns.Length)
                    {
                        var diagonal = new Player[4]
                        {
                            State.Columns[column][row],
                            State.Columns[column + 1][row + 1],
                            State.Columns[column + 2][row + 2],
                            State.Columns[column + 3][row + 3],
                        };
                        if (diagonal.Distinct().Count() == 1)
                        {
                            return winnerCandidate;
                        }
                    }

                    if ((column - 3) >= 0)
                    {
                        var diagonal = new Player[4]
                        {
                            State.Columns[column][row],
                            State.Columns[column - 1][row + 1],
                            State.Columns[column - 2][row + 2],
                            State.Columns[column - 3][row + 3],
                        };
                        if (diagonal.Distinct().Count() == 1)
                        {
                            return winnerCandidate;
                        }
                    }
                }
            }

            return Player.None;
        }

        protected override double CalculateHeuristicGameScore(Player player)
        {
            return (0.75d * CalculateGameScore(player)) - (0.25d * CalculateGameScore(player.ToOppositePlayer()));
        }

        private double CalculateGameScore(Player player)
        {
            var totalScore = 0d;

            var numberOfIterations = 0;
            var numberOfRows = State.Columns[0].Length;
            for (int column = 0; column < State.Columns.Length; column++)
            {
                for (int row = 0; row < numberOfRows; row++)
                {
                    if ((row + 3) < numberOfRows)
                    {
                        // Compute vertical sequence
                        totalScore += GetCandidateSequenceScore(player, new Player[4]
                        {
                            State.Columns[column][row],
                            State.Columns[column][row + 1],
                            State.Columns[column][row + 2],
                            State.Columns[column][row + 3],
                        });
                        numberOfIterations++;

                        if ((column + 3) < State.Columns.Length)
                        {
                            // Compute diagonal sequence
                            totalScore += GetCandidateSequenceScore(player, new Player[4]
                            {
                                State.Columns[column][row],
                                State.Columns[column + 1][row + 1],
                                State.Columns[column + 2][row + 2],
                                State.Columns[column + 3][row + 3],
                            });
                            numberOfIterations++;
                        }

                        if ((column - 3) >= 0)
                        {
                            // Compute alternative diagonal sequence
                            totalScore += GetCandidateSequenceScore(player, new Player[4]
                            {
                                State.Columns[column][row],
                                State.Columns[column - 1][row + 1],
                                State.Columns[column - 2][row + 2],
                                State.Columns[column - 3][row + 3],
                            });
                            numberOfIterations++;
                        }
                    }

                    if ((column + 3) < State.Columns.Length)
                    {
                        // Compute horizontal sequence
                        totalScore += GetCandidateSequenceScore(player, new Player[4]
                        {
                            State.Columns[column][row],
                            State.Columns[column + 1][row],
                            State.Columns[column + 2][row],
                            State.Columns[column + 3][row],
                        });
                        numberOfIterations++;
                    }
                }
            }

            var normalisedScore = totalScore / numberOfIterations;
            var correctedScore = normalisedScore <= 0d ? 0.00001d : normalisedScore;
            correctedScore = correctedScore >= 1d ? 0.99999d : correctedScore;
            return correctedScore;
        }

        private int GetCandidateSequenceScore(Player player, Player[] candidateSequence)
        {
            var noPlayerOccurrences = 0;
            var candidatePlayer = Player.None;

            for (var index = 0; index < 4; index++)
            {
                var playerInSequence = candidateSequence[index];
                if (playerInSequence == Player.None)
                {
                    noPlayerOccurrences++;
                    if (noPlayerOccurrences > 1)
                    {
                        // More than one empty space found, so we discard the sequence
                        return 0;
                    }
                }
                else
                {
                    if (playerInSequence != candidatePlayer && candidatePlayer != Player.None)
                    {
                        // Different players found in the sequence, so we discard it
                        return 0;
                    }

                    candidatePlayer = playerInSequence;
                }
            }

            // The sequence contains a player only and no more than one empty space
            return player == candidatePlayer ? 1 : -1;
        }

        public static string FromPlayerToString(Player player)
        {
            switch (player)
            {
                case Player.First:
                    return "o";

                case Player.Second:
                    return "*";

                case Player.None:
                default:
                    return " ";
            }
        }

        protected override BaseBoard<Connect4MoveInfo, Connect4BoardState> CreateNew(Connect4BoardState boardState, ITwoPlayerZeroSumGameMovesEngine movesEngine)
        {
            return new Connect4Board(boardState, movesEngine);
        }
    }
}
