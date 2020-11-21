using System.Collections.Generic;
using System.Linq;
using Ernestoyaquello.Chess.Data.Models;
using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.Chess.Util;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Util;

namespace Ernestoyaquello.Chess.Data
{
    public class ChessBoardLayoutFactory
    {
        public static PuzzleSolution GetPuzzleSolution(ChessBoardLayoutType layoutType)
        {
            return layoutType switch
            {
                ChessBoardLayoutType.Puzzle => PuzzleSolution,
                ChessBoardLayoutType.Puzzle2 => Puzzle2Solution,
                ChessBoardLayoutType.Puzzle3 => Puzzle3Solution,
                ChessBoardLayoutType.Puzzle4 => Puzzle4Solution,
                ChessBoardLayoutType.Puzzle5 => Puzzle5Solution,
                ChessBoardLayoutType.Puzzle6 => Puzzle6Solution,
            };
        }

        private static Piece[][] GetBoardLayout(ChessBoardLayoutType layoutType)
        {
            return layoutType switch
            {
                ChessBoardLayoutType.Default => Default,
                ChessBoardLayoutType.PawnReplacementTest => PawnReplacementTest,
                ChessBoardLayoutType.CastlingTest => CastlingTest,
                ChessBoardLayoutType.EnPassantTest => EnPassantTest,
                ChessBoardLayoutType.Puzzle => Puzzle,
                ChessBoardLayoutType.Puzzle2 => Puzzle2,
                ChessBoardLayoutType.Puzzle3 => Puzzle3,
                ChessBoardLayoutType.Puzzle4 => Puzzle4,
                ChessBoardLayoutType.Puzzle5 => Puzzle5,
                ChessBoardLayoutType.Puzzle6 => Puzzle6,
            };
        }

        internal static Dictionary<Player, List<Piece>> GetCapturedPiecesInLayout(ChessBoardLayoutType layoutType)
        {
            var capturedPieces = new Dictionary<Player, List<Piece>>();
            capturedPieces[Player.First] = new List<Piece>();
            capturedPieces[Player.Second] = new List<Piece>();

            var defaultPieces = CreateChessBoardLayout(ChessBoardLayoutType.Default, false)
                .SelectMany(piece => piece)
                .Where(piece => !piece.IsNone())
                .ToList();
            var boardPieces = CreateChessBoardLayout(layoutType, false)
                .SelectMany(piece => piece)
                .Where(piece => !piece.IsNone());

            foreach (var boardPiece in boardPieces)
            {
                var defaultPiece = defaultPieces.FirstOrDefault(defaultPiece => defaultPiece.WithPosition(default).Equals(boardPiece.WithPosition(default).WithNumberOfPlayedMoves(0)));
                defaultPieces.Remove(defaultPiece);
            }

            foreach (var capturedPiece in defaultPieces)
            {
                capturedPieces[capturedPiece.Player.ToOppositePlayer()].Add(capturedPiece);
            }

            return capturedPieces;
        }

        internal static Piece[][] CreateChessBoardLayout(ChessBoardLayoutType layoutType)
        {
            return CreateChessBoardLayout(layoutType, true);
        }

        private static Piece[][] CreateChessBoardLayout(ChessBoardLayoutType layoutType, bool updateNumberOfMoves)
        {
            var chessBoardLayout = GetBoardLayout(layoutType);

            SetInitialPiecesPositions(chessBoardLayout);

            if (layoutType != ChessBoardLayoutType.Default && updateNumberOfMoves)
            {
                chessBoardLayout = UpdateNumberOfMovesOfEachPiece(chessBoardLayout);
            }

            return chessBoardLayout;
        }

        private static void SetInitialPiecesPositions(Piece[][] boardLayout)
        {
            for (int verticalIndex = 0; verticalIndex < 8; verticalIndex++)
            {
                for (int horizontalIndex = 0; horizontalIndex < 8; horizontalIndex++)
                {
                    var piece = boardLayout[verticalIndex][horizontalIndex];
                    var horizontalPosition = (PieceHorizontalPosition)horizontalIndex;
                    var verticalPosition = (PieceVerticalPosition)verticalIndex;
                    var piecePosition = new PiecePosition(horizontalPosition, verticalPosition);
                    boardLayout[verticalIndex][horizontalIndex] = piece.WithPosition(piecePosition);
                }
            }
        }

        private static Piece[][] UpdateNumberOfMovesOfEachPiece(Piece[][] chessBoardLayout)
        {
            var defaultLayout = CreateChessBoardLayout(ChessBoardLayoutType.Default, false);

            for (int verticalIndex = 0; verticalIndex < 8; verticalIndex++)
            {
                for (int horizontalIndex = 0; horizontalIndex < 8; horizontalIndex++)
                {
                    var piece = chessBoardLayout[verticalIndex][horizontalIndex];
                    var expectedPiece = defaultLayout[verticalIndex][horizontalIndex];
                    if (piece.Type != expectedPiece.Type || piece.Player != expectedPiece.Player)
                    {
                        // The piece is not on its original position, so we assume that it has moved at least once
                        var correctedPiece = piece.WithNumberOfPlayedMoves(1);
                        chessBoardLayout[verticalIndex][horizontalIndex] = correctedPiece;
                    }
                }
            }

            return chessBoardLayout;
        }

        private static Piece[][] Default => new Piece[8][]
        {
            new Piece[8] { Piece.BlackRook, Piece.BlackKnight, Piece.BlackBishop, Piece.BlackQueen, Piece.BlackKing, Piece.BlackBishop, Piece.BlackKnight, Piece.BlackRook },
            new Piece[8] { Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn },
            new Piece[8] { Piece.WhiteRook, Piece.WhiteKnight, Piece.WhiteBishop, Piece.WhiteQueen, Piece.WhiteKing, Piece.WhiteBishop, Piece.WhiteKnight, Piece.WhiteRook },
        };

        private static Piece[][] PawnReplacementTest => new Piece[8][]
        {
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.WhiteKing, Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn },
            new Piece[8] { Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn, Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.BlackKing },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.BlackPawn, Piece.BlackPawn },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
        };

        private static Piece[][] CastlingTest => new Piece[8][]
        {
            new Piece[8] { Piece.BlackRook, Piece.None, Piece.None, Piece.None, Piece.BlackKing, Piece.None, Piece.None, Piece.BlackRook },
            new Piece[8] { Piece.BlackPawn, Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackKnight, Piece.None, Piece.BlackPawn },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.WhitePawn, Piece.None, Piece.WhiteKnight, Piece.None, Piece.None, Piece.None, Piece.None, Piece.WhitePawn },
            new Piece[8] { Piece.WhiteRook, Piece.None, Piece.None, Piece.None, Piece.WhiteKing, Piece.None, Piece.None, Piece.WhiteRook },
        };
        
        private static Piece[][] EnPassantTest => new Piece[8][]
        {
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackKing, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.BlackPawn, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.WhitePawn, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.WhitePawn, Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.WhiteKing, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
        };

        private static Piece[][] Puzzle => new Piece[8][]
        {
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.BlackQueen, Piece.None, Piece.BlackRook, Piece.None, Piece.BlackKing },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.WhiteKnight, Piece.None, Piece.BlackPawn, Piece.BlackPawn },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.WhiteRook, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.BlackKnight, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.WhiteQueen, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.WhitePawn, Piece.WhitePawn, Piece.WhitePawn },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.WhiteKing, Piece.None },
        };

        private static Piece[][] Puzzle2 => new Piece[8][]
        {
            new Piece[8] { Piece.BlackRook, Piece.None, Piece.None, Piece.None, Piece.BlackKing, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.BlackPawn, Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.BlackQueen, Piece.WhitePawn, Piece.None, Piece.WhiteQueen },
            new Piece[8] { Piece.BlackPawn, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.WhitePawn, Piece.None, Piece.None, Piece.WhitePawn, Piece.None, Piece.WhiteBishop, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.WhitePawn, Piece.WhitePawn, Piece.None, Piece.None, Piece.WhiteKing, Piece.WhitePawn, Piece.WhitePawn },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
        };

        private static Piece[][] Puzzle3 => new Piece[8][]
        {
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackRook, Piece.None, Piece.BlackKing, Piece.None },
            new Piece[8] { Piece.None, Piece.BlackQueen, Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.BlackPawn },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackBishop, Piece.None, Piece.WhitePawn, Piece.None },
            new Piece[8] { Piece.None, Piece.BlackPawn, Piece.WhiteBishop, Piece.None, Piece.WhiteQueen, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.BlackRook, Piece.BlackPawn, Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.None, Piece.WhitePawn },
            new Piece[8] { Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.WhitePawn, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.WhiteKing, Piece.None, Piece.WhiteRook, Piece.None, Piece.None, Piece.None, Piece.WhiteRook },
        };

        private static Piece[][] Puzzle4 => new Piece[8][]
        {
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackKing, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackRook, Piece.None, Piece.BlackPawn },
            new Piece[8] { Piece.BlackPawn, Piece.None, Piece.None, Piece.WhiteKnight, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.BlackKnight, Piece.BlackPawn, Piece.BlackPawn, Piece.WhitePawn, Piece.None, Piece.None, Piece.BlackQueen, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.WhiteKnight, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.WhitePawn, Piece.WhiteQueen, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.WhitePawn },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.WhiteKing },
        };

        private static Piece[][] Puzzle5 => new Piece[8][]
        {
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackKing, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.BlackKnight, Piece.None, Piece.BlackPawn, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.BlackPawn, Piece.None, Piece.BlackRook, Piece.BlackPawn },
            new Piece[8] { Piece.None, Piece.None, Piece.BlackQueen, Piece.None, Piece.WhitePawn, Piece.WhiteKnight, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.WhiteQueen, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.None, Piece.WhitePawn, Piece.WhitePawn, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.WhiteRook, Piece.None, Piece.WhiteKing, Piece.None },
        };

        private static Piece[][] Puzzle6 => new Piece[8][]
        {
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.BlackPawn, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.BlackQueen, Piece.None, Piece.BlackKing, Piece.WhiteKnight, Piece.WhiteKnight, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.WhitePawn, Piece.None, Piece.None, Piece.None, Piece.WhitePawn, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.WhitePawn, Piece.None, Piece.WhiteKing, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
            new Piece[8] { Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None },
        };

        private static PuzzleSolution PuzzleSolution => GetPuzzleSolution(
            puzzleType: ChessBoardLayoutType.Puzzle,
            initialPosition: new PiecePosition(PieceHorizontalPosition.P_D, PieceVerticalPosition.P_3),
            finalPosition: new PiecePosition(PieceHorizontalPosition.P_H, PieceVerticalPosition.P_7),
            requiredTreeDepth: 3);

        private static PuzzleSolution Puzzle2Solution => GetPuzzleSolution(
            puzzleType: ChessBoardLayoutType.Puzzle2,
            initialPosition: new PiecePosition(PieceHorizontalPosition.P_H, PieceVerticalPosition.P_6),
            finalPosition: new PiecePosition(PieceHorizontalPosition.P_H, PieceVerticalPosition.P_8),
            requiredTreeDepth: 3);

        private static PuzzleSolution Puzzle3Solution => GetPuzzleSolution(
            puzzleType: ChessBoardLayoutType.Puzzle3,
            initialPosition: new PiecePosition(PieceHorizontalPosition.P_E, PieceVerticalPosition.P_5),
            finalPosition: new PiecePosition(PieceHorizontalPosition.P_E, PieceVerticalPosition.P_6),
            requiredTreeDepth: 5);

        private static PuzzleSolution Puzzle4Solution => GetPuzzleSolution(
            puzzleType: ChessBoardLayoutType.Puzzle4,
            initialPosition: new PiecePosition(PieceHorizontalPosition.P_B, PieceVerticalPosition.P_2),
            finalPosition: new PiecePosition(PieceHorizontalPosition.P_H, PieceVerticalPosition.P_8),
            requiredTreeDepth: 5);

        private static PuzzleSolution Puzzle5Solution => GetPuzzleSolution(
            puzzleType: ChessBoardLayoutType.Puzzle5,
            initialPosition: new PiecePosition(PieceHorizontalPosition.P_E, PieceVerticalPosition.P_1),
            finalPosition: new PiecePosition(PieceHorizontalPosition.P_C, PieceVerticalPosition.P_1),
            requiredTreeDepth: 5);

        private static PuzzleSolution Puzzle6Solution => GetPuzzleSolution(
            puzzleType: ChessBoardLayoutType.Puzzle6,
            initialPosition: new PiecePosition(PieceHorizontalPosition.P_D, PieceVerticalPosition.P_5),
            finalPosition: new PiecePosition(PieceHorizontalPosition.P_B, PieceVerticalPosition.P_6),
            requiredTreeDepth: 5);

        private static PuzzleSolution GetPuzzleSolution(
            ChessBoardLayoutType puzzleType,
            PiecePosition initialPosition,
            PiecePosition finalPosition,
            int requiredTreeDepth)
        {
            var puzzle = CreateChessBoardLayout(puzzleType);
            var pieceToMove = puzzle[(int)initialPosition.VerticalPosition][(int)initialPosition.HorizontalPosition];
            var pieceToCapture = puzzle[(int)finalPosition.VerticalPosition][(int)finalPosition.HorizontalPosition];
            pieceToCapture = !pieceToCapture.IsNone() ? pieceToCapture : null;
            var moveStep = new ChessMoveStepInfo(initialPosition, finalPosition, pieceToMove, pieceToCapture);

            var bestMove = new ChessMoveInfo(
                player: Player.First,
                steps: new List<ChessMoveStepInfo> { moveStep },
                wouldCaptureKing: pieceToCapture?.Type == PieceType.King,
                isCastling: false);

            return new PuzzleSolution(bestMove, requiredTreeDepth);
        }
    }
}
