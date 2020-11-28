using System;
using System.Collections.Generic;
using System.Linq;
using Ernestoyaquello.Chess.Data;
using Ernestoyaquello.Chess.Util;
using Ernestoyaquello.Chess.Util.MoveCalculators;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Util;

namespace Ernestoyaquello.Chess.Models
{
    public class ChessBoard : BaseBoard<ChessMoveInfo, ChessBoardState>
    {
        public ChessBoard(ChessBoardLayoutType boardLayoutType, ITwoPlayerZeroSumGameMovesEngine gameEngine)
            : this(new ChessBoardState(ChessBoardLayoutFactory.CreateChessBoardLayout(boardLayoutType), capturedPiecesPerPlayer: ChessBoardLayoutFactory.GetCapturedPiecesInLayout(boardLayoutType)), gameEngine)
        {
        }

        private ChessBoard(ChessBoardState state, ITwoPlayerZeroSumGameMovesEngine gameEngine)
            : base(state, gameEngine)
        {
        }

        public Piece[][] GetBoardLayout()
        {
            return State.Layout.Select(column => column.ToArray()).ToArray();
        }

        protected override double CalculateHeuristicGameScore(Player player)
        {
            return CalculatePlayerScore(player) - CalculatePlayerScore(player.ToOppositePlayer());
        }

        private double CalculatePlayerScore(Player player)
        {
            var availableMoves = GetValidMoves(player);
            var availableMovesValue = availableMoves.Sum(move => move.Score);
            var piecesValue = 0;
            var numberOfPieces = 0;

            IterateOverPlayerPieces(player, (piece) =>
            {
                piecesValue += piece.HeuristicValue;
                numberOfPieces++;
                return false;
            });

            return (0.65d * (piecesValue / 11371d)) +
                (0.2d * (availableMovesValue / 218)) +
                (0.1d * (availableMoves.Count / 218) +
                (0.05d * (numberOfPieces / 16d)));
        }

        public bool IsKingInCheck(Player player)
        {
            var availableMovesForOpponent = GetValidMoves(player.ToOppositePlayer());
            return availableMovesForOpponent.Any(move => move.WouldCaptureKing);
        }

        protected override List<ChessMoveInfo> CalculateValidMoves(Player player)
        {
            return CalculateValidMoves(player, true);
        }

        private List<ChessMoveInfo> CalculateValidMoves(Player player, bool filterOutInvalidKingMoves)
        {
            var moves = new List<ChessMoveInfo>();

            IterateOverPlayerPieces(player, (piece) =>
            {
                var movesForPiece = CalculateValidMovesForPiece(piece, filterOutInvalidKingMoves);
                moves.AddRange(movesForPiece);

                return false;
            });

            return moves;
        }

        public List<ChessMoveInfo> CalculateValidMovesForPiece(Piece piece)
        {
            return CalculateValidMovesForPiece(piece, true);
        }

        private List<ChessMoveInfo> CalculateValidMovesForPiece(Piece piece, bool filterOutInvalidKingMoves)
        {
            var forwardMotion = piece.Player == Player.First ? -1 : 1;
            var moves = piece.Type switch
            {
                PieceType.Rook => this.CalculateValidMovesForRook(piece, forwardMotion),
                PieceType.Knight => this.CalculateValidMovesForKnight(piece, forwardMotion),
                PieceType.Bishop => this.CalculateValidMovesForBishop(piece, forwardMotion),
                PieceType.Queen => this.CalculateValidMovesForQueen(piece, forwardMotion),
                PieceType.Pawn => this.CalculateValidMovesForPawn(piece, forwardMotion),
                PieceType.King => filterOutInvalidKingMoves
                    ? FilterOutInvalidKingMoves(piece.Player, this.CalculateMovesForKing(piece, forwardMotion), forwardMotion)
                    : this.CalculateMovesForKing(piece, forwardMotion),
                _ => new List<ChessMoveInfo>(),
            };

            return moves;
        }

        private List<ChessMoveInfo> FilterOutInvalidKingMoves(Player player, List<ChessMoveInfo> uncheckedKingMoves, int forwardMotion)
        {
            var validKingMoves = new List<ChessMoveInfo>();

            if (!uncheckedKingMoves.Any())
            {
                return validKingMoves;
            }

            var opponent = player.ToOppositePlayer();
            var opponentMoves = CalculateValidMoves(opponent, filterOutInvalidKingMoves: false);
            foreach (var potentialKingMove in uncheckedKingMoves)
            {
                if (!potentialKingMove.IsCastling)
                {
                    var pieceToCapture = potentialKingMove.MoveSteps.First().PieceToCapture;
                    if (!pieceToCapture.IsNone())
                    {
                        // If the king would capture a piece with his move, then we cannot use the current valid moves of the opponent to verify
                        // this king move, as said valid moves would be altered after the piece is captured. Hence, we need to pretend to make
                        // the move and then recalculate the opponent's moves in order to verify whether the king would be in check or not.
                        MovesEngine.TryMakeDummyMove<ChessBoard, ChessMoveInfo, ChessBoardState>(this, potentialKingMove, tempBoard =>
                        {
                            var opponentMovesAfterKingMove = tempBoard.CalculateValidMoves(opponent, filterOutInvalidKingMoves: false);
                            var kingIsInCheckAfterMove = opponentMovesAfterKingMove.Any(move => move.WouldCaptureKing);
                            if (!kingIsInCheckAfterMove)
                            {
                                validKingMoves.Add(potentialKingMove);
                            }
                        }, shouldBeValid: false);
                    }
                    else
                    {
                        var kingMoveDestination = potentialKingMove.MoveSteps.First().NewPosition;
                        if (!opponentMoves.Any(move => move.MoveSteps.First().PieceToMove.Type != PieceType.Pawn && move.MoveSteps.First().NewPosition.Equals(kingMoveDestination)))
                        {
                            // There is no move that would cause the opponent's piece to land on this new position of the king,
                            // so it is likely that this king move won't result on a check. However, we still have to look for
                            // pawns, as they move differently to capture pieces than they do to advance.
                            var upLeftPosition = new PiecePosition(kingMoveDestination.HorizontalPosition + forwardMotion, kingMoveDestination.VerticalPosition + forwardMotion);
                            var upLeftPiece = upLeftPosition.IsValid() ? GetPiece(upLeftPosition) : null;
                            if (!upLeftPiece.IsNone() && upLeftPiece.Type == PieceType.Pawn && upLeftPiece.Player == opponent)
                            {
                                // After this potential king move, the piece in front of the king and to his left would be an opponent's pawn,
                                // so this move isn't allowed for the king because it would result in a check
                                continue;
                            }

                            var upRightPosition = new PiecePosition(kingMoveDestination.HorizontalPosition - forwardMotion, kingMoveDestination.VerticalPosition + forwardMotion);
                            var upRightPiece = upRightPosition.IsValid() ? GetPiece(upRightPosition) : null;
                            if (!upRightPiece.IsNone() && upRightPiece.Type == PieceType.Pawn && upRightPiece.Player == opponent)
                            {
                                // After this potential king move, the piece in front of the king and to his right would be an opponent's pawn,
                                // so this move isn't allowed for the king because it would result in a check
                                continue;
                            }

                            validKingMoves.Add(potentialKingMove);
                        }
                    }
                }
                else
                {
                    // The castling move can only be made if the king is not currently in check
                    var kingIsInCheck = opponentMoves.Any(move => move.WouldCaptureKing);
                    if (!kingIsInCheck)
                    {
                        validKingMoves.Add(potentialKingMove);
                    }
                }
            }

            return validKingMoves;
        }

        public override bool IsValidMove(ChessMoveInfo moveInfo)
        {
            return IsPlayerTurn(moveInfo.Player) &&
                GetGameResult(moveInfo.Player) == GameResult.StillGame &&
                GetValidMoves(moveInfo.Player).Any(move => move.Equals(moveInfo));
        }

        public bool IsPositionValidToMoveTo(PiecePosition position, Func<Piece, bool> isFoundPieceValid)
        {
            return position.IsValid() && isFoundPieceValid(GetPiece(position));
        }

        protected override Player CalculateWinner()
        {
            var winner = Player.None;

            var currentPlayer = IsPlayerTurn(Player.First) ? Player.First : Player.Second;
            var currentPlayerMoves = GetValidMoves(currentPlayer);
            if (currentPlayerMoves.Any(move => move.WouldCaptureKing))
            {
                // The opponent has their king in check and cannot move because
                // it is not their turn anymore, so they have lost the match
                winner = currentPlayer;
            }
            else
            {
                var opponentPlayer = currentPlayer.ToOppositePlayer();
                var opponentPlayerMoves = GetValidMoves(opponentPlayer);
                var kingIsInCheck = opponentPlayerMoves.Any(move => move.WouldCaptureKing);
                if (kingIsInCheck)
                {
                    winner = opponentPlayer;

                    // Current player is in check, we have to see whether they can avoid the checkmate
                    var shouldBreakLoop = false;
                    foreach (var currentPlayerMove in currentPlayerMoves)
                    {
                        MovesEngine.TryMakeDummyMove<ChessBoard, ChessMoveInfo, ChessBoardState>(this, currentPlayerMove, tempBoard =>
                        {
                            var newOpponentPlayerMoves = tempBoard.GetValidMoves(opponentPlayer);
                            var kingRemainsInCheck = newOpponentPlayerMoves.Any(move => move.WouldCaptureKing);
                            if (!kingRemainsInCheck)
                            {
                                // The player is able to avoid the check thanks to this move
                                winner = Player.None;
                                shouldBreakLoop = true;
                            }
                        }, shouldBeValid: false);

                        if (shouldBreakLoop)
                        {
                            break;
                        }
                    }
                }
            }

            return winner;
        }

        public override void MakeMove(ChessMoveInfo moveInfo)
        {
            State.PositionedPiecesPerPlayer.Clear();

            foreach (var step in moveInfo.MoveSteps)
            {
                var pieceToCapture = step.PieceToCapture;
                if (pieceToCapture?.Type == PieceType.King && pieceToCapture.Player == moveInfo.Player.ToOppositePlayer())
                {
                    // Instead of actually capturing the king, we just declare the player a winner
                    SetWinner(moveInfo.Player);
                    break;
                }
                else
                {
                    if (!pieceToCapture.IsNone())
                    {
                        if (!State.CapturedPiecesPerPlayer.ContainsKey(moveInfo.Player))
                        {
                            State.CapturedPiecesPerPlayer[moveInfo.Player] = new List<Piece>();
                        }

                        State.CapturedPiecesPerPlayer[moveInfo.Player].Add(pieceToCapture);
                    }

                    var emptyPiece = Piece.None;
                    emptyPiece = emptyPiece.WithPosition(step.OldPosition);
                    SetPiece(emptyPiece);

                    var pieceToMove = step.PieceToMove;
                    pieceToMove = pieceToMove.WithPosition(step.NewPosition).WithNumberOfPlayedMoves(pieceToMove.NumberOfPlayedMoves + 1);
                    SetPiece(pieceToMove);

                    if (!pieceToCapture.IsNone() && !pieceToCapture.Position.Equals(step.NewPosition))
                    {
                        emptyPiece = Piece.None;
                        emptyPiece = emptyPiece.WithPosition(pieceToCapture.Position);
                        SetPiece(emptyPiece);
                    }
                }
            }
        }

        public List<Piece> GetCapturedPieces(Player player)
        {
            if (!State.CapturedPiecesPerPlayer.ContainsKey(player))
            {
                State.CapturedPiecesPerPlayer[player] = new List<Piece>();
            }

            return State.CapturedPiecesPerPlayer[player].ToList();
        }

        internal ChessMoveInfo CreateSimpleMove(Piece pieceToMove, PiecePosition pieceDestination, Piece pieceToCapture = null)
        {
            pieceToCapture ??= GetPiece(pieceDestination);
            pieceToCapture = !pieceToCapture.IsNone() && pieceToCapture.Player != pieceToMove.Player ? pieceToCapture : null;
            var singleMoveStep = new ChessMoveStepInfo(pieceToMove.Position, pieceDestination, pieceToMove, pieceToCapture);
            return CreateMove(singleMoveStep);
        }

        private ChessMoveInfo CreateMove(params ChessMoveStepInfo[] moveSteps)
        {
            var player = moveSteps[0].PieceToMove.Player;
            var wouldCaptureKing = moveSteps.Any(step =>
            {
                var pieceToCapture = GetPiece(step.NewPosition);
                return pieceToCapture.Type == PieceType.King && pieceToCapture.Player != player;
            });

            return new ChessMoveInfo(player, moveSteps.ToList(), wouldCaptureKing);
        }

        internal Piece GetPiece(PiecePosition piecePosition)
        {
            return State.Layout[(int)piecePosition.VerticalPosition][(int)piecePosition.HorizontalPosition];
        }

        internal void SetPiece(Piece piece)
        {
            State.Layout[(int)piece.Position.VerticalPosition][(int)piece.Position.HorizontalPosition] = piece;
        }

        private void IterateOverPlayerPieces(Player player, Func<Piece, bool> action = null)
        {
            if (State.PositionedPiecesPerPlayer.ContainsKey(player))
            {
                var playerPieces = State.PositionedPiecesPerPlayer[player];
                foreach (var piece in playerPieces)
                {
                    if (action?.Invoke(piece) == true)
                    {
                        return;
                    }
                }
            }
            else
            {
                State.PositionedPiecesPerPlayer[player] = new List<Piece>();

                for (int verticalIndex = 0; verticalIndex < 8; verticalIndex++)
                {
                    for (int horizontalIndex = 0; horizontalIndex < 8; horizontalIndex++)
                    {
                        var piece = State.Layout[verticalIndex][horizontalIndex];
                        if (piece.IsNone() || piece.Player != player)
                        {
                            continue;
                        }

                        if (!State.PositionedPiecesPerPlayer[player].Contains(piece))
                        {
                            State.PositionedPiecesPerPlayer[player].Add(piece);
                        }

                        if (action?.Invoke(piece) == true)
                        {
                            return;
                        }
                    }
                }
            }
        }

        protected override BaseBoard<ChessMoveInfo, ChessBoardState> CreateNew(ChessBoardState boardState, ITwoPlayerZeroSumGameMovesEngine movesEngine)
        {
            return new ChessBoard(boardState, movesEngine);
        }
    }
}
