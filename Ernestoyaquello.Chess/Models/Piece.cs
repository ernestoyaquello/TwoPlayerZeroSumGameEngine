using System;
using Ernestoyaquello.Chess.Util;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.Chess.Models
{
    public class Piece : IEquatable<Piece>
    {
        private static readonly int[][] _pawnPositionValues = new int[8][]
        {
            new int[8] {  0,  0,  0,  0,  0,  0,  0,  0, },
            new int[8] { 50, 50, 50, 50, 50, 50, 50, 50, },
            new int[8] { 10, 10, 20, 30, 30, 20, 10, 10, },
            new int[8] {  5,  5, 10, 25, 25, 10,  5,  5, },
            new int[8] {  0,  0,  0, 20, 20,  0,  0,  0, },
            new int[8] {  5, -5,-10,  0,  0,-10, -5,  5, },
            new int[8] {  5, 10, 10,-20,-20, 10, 10,  5, },
            new int[8] {  0,  0,  0,  0,  0,  0,  0,  0, },
        };

        private static readonly int[][] _knightPositionValues = new int[8][]
        {
            new int[8] { -50,-40,-30,-30,-30,-30,-40,-50, },
            new int[8] { -40,-20,  0,  0,  0,  0,-20,-40, },
            new int[8] { -30,  0, 10, 15, 15, 10,  0,-30, },
            new int[8] { -30,  5, 15, 20, 20, 15,  5,-30, },
            new int[8] { -30,  0, 15, 20, 20, 15,  0,-30, },
            new int[8] { -30,  5, 10, 15, 15, 10,  5,-30, },
            new int[8] { -40,-20,  0,  5,  5,  0,-20,-40, },
            new int[8] { -50,-40,-30,-30,-30,-30,-40,-50, },
        };

        private static readonly int[][] _bishopPositionValues = new int[8][]
        {
            new int[8] { -20,-10,-10,-10,-10,-10,-10,-20, },
            new int[8] { -10,  0,  0,  0,  0,  0,  0,-10, },
            new int[8] { -10,  0,  5, 10, 10,  5,  0,-10, },
            new int[8] { -10,  5,  5, 10, 10,  5,  5,-10, },
            new int[8] { -10,  0, 10, 10, 10, 10,  0,-10, },
            new int[8] { -10, 10, 10, 10, 10, 10, 10,-10, },
            new int[8] { -10,  5,  0,  0,  0,  0,  5,-10, },
            new int[8] { -20,-10,-10,-10,-10,-10,-10,-20, },
        };

        private static readonly int[][] _rookPositionValues = new int[8][]
        {
             new int[8] {  0,  0,  0,  0,  0,  0,  0,  0, },
             new int[8] {  5, 10, 10, 10, 10, 10, 10,  5, },
             new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
             new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
             new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
             new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
             new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
             new int[8] {  0,  0,  0,  5,  5,  0,  0,  0, },
        };

        private static readonly int[][] _queenPositionValues = new int[8][]
        {
             new int[8] { -20,-10,-10, -5, -5,-10,-10,-20, },
             new int[8] { -10,  0,  0,  0,  0,  0,  0,-10, },
             new int[8] { -10,  0,  5,  5,  5,  5,  0,-10, },
             new int[8] {  -5,  0,  5,  5,  5,  5,  0, -5, },
             new int[8] {   0,  0,  5,  5,  5,  5,  0, -5, },
             new int[8] { -10,  5,  5,  5,  5,  5,  0,-10, },
             new int[8] { -10,  0,  5,  0,  0,  0,  0,-10, },
             new int[8] { -20,-10,-10, -5, -5,-10,-10,-20, },
        };

        public static Piece WhiteRook => new Piece(PieceType.Rook, Player.First);
        public static Piece WhiteKnight => new Piece(PieceType.Knight, Player.First);
        public static Piece WhiteBishop => new Piece(PieceType.Bishop, Player.First);
        public static Piece WhiteQueen => new Piece(PieceType.Queen, Player.First);
        public static Piece WhiteKing => new Piece(PieceType.King, Player.First);
        public static Piece WhitePawn => new Piece(PieceType.Pawn, Player.First);

        public static Piece BlackRook => new Piece(PieceType.Rook, Player.Second);
        public static Piece BlackKnight => new Piece(PieceType.Knight, Player.Second);
        public static Piece BlackBishop => new Piece(PieceType.Bishop, Player.Second);
        public static Piece BlackQueen => new Piece(PieceType.Queen, Player.Second);
        public static Piece BlackKing => new Piece(PieceType.King, Player.Second);
        public static Piece BlackPawn => new Piece(PieceType.Pawn, Player.Second);

        public static Piece None => new Piece(PieceType.None, Player.None);

        public PieceType Type { get; }
        public Player Player { get; }
        public int CanonicValue { get; }
        public int HeuristicValue { get; }
        public PiecePosition Position { get; }
        public int NumberOfPlayedMoves { get; }

        private Piece(
            PieceType type,
            Player player,
            PiecePosition position = default,
            int numberOfPlayedMoves = 0,
            int? canonicValue = null,
            int? heuristicValue = null)
        {
            Type = type;
            Player = player;
            Position = position;
            NumberOfPlayedMoves = numberOfPlayedMoves;
            CanonicValue = canonicValue ?? GetPieceCanonicValue(Type);
            HeuristicValue = heuristicValue ?? CalculatePieceHeuristicValue(Type, Position, Player);
        }

        public Piece WithPosition(PiecePosition position)
        {
            return new Piece(Type, Player, position, NumberOfPlayedMoves, CanonicValue, null);
        }

        public Piece WithNumberOfPlayedMoves(int numberOfPlayedMoves)
        {
            return new Piece(Type, Player, Position, numberOfPlayedMoves, CanonicValue, null);
        }

        public override bool Equals(object other)
        {
            return other is Piece moveInfo && Equals(moveInfo);
        }

        public bool Equals(Piece other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Type, other.Type) &&
                Equals(Player, other.Player) &&
                Equals(Position, other.Position) &&
                Equals(CanonicValue, other.CanonicValue) &&
                Equals(HeuristicValue, other.HeuristicValue) &&
                Equals(NumberOfPlayedMoves, other.NumberOfPlayedMoves);
        }

        public override int GetHashCode()
        {
            return (Type, Player, Position, CanonicValue, HeuristicValue, NumberOfPlayedMoves).GetHashCode();
        }

        private static int GetPieceCanonicValue(PieceType type)
        {
            return type switch
            {
                PieceType.Rook => 5,
                PieceType.Knight => 3,
                PieceType.Bishop => 3,
                PieceType.Queen => 9,
                PieceType.King => 4,
                PieceType.Pawn => 1,
                _ => 0,
            };
        }

        private static int CalculatePieceHeuristicValue(PieceType type, PiecePosition position, Player player)
        {
            var verticalIndex = player == Player.First ? (int)position.VerticalPosition : (7 - (int)position.VerticalPosition);
            var horizontalIndex = player == Player.First ? (int)position.HorizontalPosition : (7 - (int)position.HorizontalPosition);

            return type switch
            {
                PieceType.Rook => 510 + _rookPositionValues[verticalIndex][horizontalIndex],     // Min : 505 // Max : 520
                PieceType.Knight => 320 + _knightPositionValues[verticalIndex][horizontalIndex], // Min : 270 // Max : 340
                PieceType.Bishop => 333 + _bishopPositionValues[verticalIndex][horizontalIndex], // Min : 313 // Max : 343
                PieceType.Queen => 880 + _queenPositionValues[verticalIndex][horizontalIndex],   // Min : 860 // Max : 885
                PieceType.Pawn => 100 + _pawnPositionValues[verticalIndex][horizontalIndex],     // Min :  80 // Max : 150
                PieceType.King => 1000,
                _ => 0,
            };
        }

        public override string ToString()
        {
            if (this.IsNone())
            {
                return "None";
            }

            var playerName = Player == Player.First ? "White" : "Black";
            var pieceName = Type.ToString().ToLower();
            return $"{playerName} {pieceName}";
        }

        public string ToSimplifiedString(bool includePlayerCharacter = true)
        {
            if (this.IsNone())
            {
                return "";
            }

            var piecePlayerCharacter = includePlayerCharacter ? (Player == Player.First ? "W" : "B") : "";
            var pieceCharacter = Type != PieceType.Knight ? Type.ToString().ToUpper().Substring(0, 1) : "N";
            return $"{piecePlayerCharacter}{pieceCharacter}";
        }
    }
}
