using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.ChessApp.Converters;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ernestoyaquello.ChessApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChessBoardView : Grid
    {
        public static readonly BindableProperty BoardProperty =
            BindableProperty.Create(nameof(Board), typeof(Piece[][]), typeof(ChessBoardView), default(Piece[][]),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardView)?.RefreshBoard());

        public static readonly BindableProperty HumanPlayerProperty =
            BindableProperty.Create(nameof(HumanPlayer), typeof(Player), typeof(ChessBoardView), default(Player),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardView)?.RefreshBoard());
        
        public static readonly BindableProperty PlayerInCheckProperty =
            BindableProperty.Create(nameof(PlayerInCheck), typeof(Player?), typeof(ChessBoardView), default(Player?),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardView)?.RefreshBoard());

        public static readonly BindableProperty SelectedPieceProperty =
            BindableProperty.Create(nameof(SelectedPiece), typeof(Piece), typeof(ChessBoardView), default(Piece));
        
        public static readonly BindableProperty PositionsForSelectedPieceProperty =
            BindableProperty.Create(nameof(PositionsForSelectedPiece), typeof(List<PiecePosition>), typeof(ChessBoardView), default(List<PiecePosition>));
        
        public static readonly BindableProperty LastMovePositionsProperty =
            BindableProperty.Create(nameof(LastMovePositions), typeof(List<PiecePosition>), typeof(ChessBoardView), default(List<PiecePosition>));

        public static readonly BindableProperty PieceTappedCommandProperty =
            BindableProperty.Create(nameof(PieceTappedCommand), typeof(ICommand), typeof(ChessBoardView), default(ICommand));

        public static readonly BindableProperty LightPositionColorProperty =
            BindableProperty.Create(nameof(LightPositionColor), typeof(Color), typeof(ChessBoardView), new Color(0.98, 0.84, 0.73),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardView)?.RefreshBoard());

        public static readonly BindableProperty DarkPositionColorProperty =
            BindableProperty.Create(nameof(DarkPositionColor), typeof(Color), typeof(ChessBoardView), new Color(0.82, 0.55, 0.28),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardView)?.RefreshBoard());

        public static readonly BindableProperty LightPositionHighlightedColorProperty =
            BindableProperty.Create(nameof(LightPositionHighlightedColor), typeof(Color), typeof(ChessBoardView), new Color(0.84, 0.90, 0.59),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardView)?.RefreshBoard());

        public static readonly BindableProperty DarkPositionHighlightedColorProperty =
            BindableProperty.Create(nameof(DarkPositionHighlightedColor), typeof(Color), typeof(ChessBoardView), new Color(0.69, 0.78, 0.30),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardView)?.RefreshBoard());

        public static readonly BindableProperty LightPositionDangerColorProperty =
            BindableProperty.Create(nameof(LightPositionDangerColor), typeof(Color), typeof(ChessBoardView), new Color(1.0, 0.65, 0.6),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardView)?.RefreshBoard());

        public static readonly BindableProperty DarkPositionDangerColorProperty =
            BindableProperty.Create(nameof(DarkPositionDangerColor), typeof(Color), typeof(ChessBoardView), new Color(0.82, 0.34, 0.28),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardView)?.RefreshBoard());

        public static readonly BindableProperty IsClickableProperty =
            BindableProperty.Create(nameof(IsClickable), typeof(bool), typeof(ChessBoardView), default(bool));

        public static readonly BindableProperty ShowLoaderProperty =
            BindableProperty.Create(nameof(ShowLoader), typeof(bool), typeof(ChessBoardView), default(bool));

        public static readonly BindableProperty LoaderProgressProperty =
            BindableProperty.Create(nameof(LoaderProgress), typeof(double), typeof(ChessBoardView), default(double));

        public Piece[][] Board
        {
            get => (Piece[][])GetValue(BoardProperty);
            set => SetValue(BoardProperty, value);
        }

        public Player HumanPlayer
        {
            get => (Player)GetValue(HumanPlayerProperty);
            set => SetValue(HumanPlayerProperty, value);
        }
        
        public Player? PlayerInCheck
        {
            get => (Player?)GetValue(PlayerInCheckProperty);
            set => SetValue(PlayerInCheckProperty, value);
        }
        
        public Piece SelectedPiece
        {
            get => (Piece)GetValue(SelectedPieceProperty);
            set => SetValue(SelectedPieceProperty, value);
        }

        public List<PiecePosition> PositionsForSelectedPiece
        {
            get => (List<PiecePosition>)GetValue(PositionsForSelectedPieceProperty);
            set => SetValue(PositionsForSelectedPieceProperty, value);
        }

        public List<PiecePosition> LastMovePositions
        {
            get => (List<PiecePosition>)GetValue(LastMovePositionsProperty);
            set => SetValue(LastMovePositionsProperty, value);
        }

        public ICommand PieceTappedCommand
        {
            get => (ICommand)GetValue(PieceTappedCommandProperty);
            set => SetValue(PieceTappedCommandProperty, value);
        }

        public Color LightPositionColor
        {
            get => (Color)GetValue(LightPositionColorProperty);
            set => SetValue(LightPositionColorProperty, value);
        }

        public Color DarkPositionColor
        {
            get => (Color)GetValue(DarkPositionColorProperty);
            set => SetValue(DarkPositionColorProperty, value);
        }
        
        public Color LightPositionHighlightedColor
        {
            get => (Color)GetValue(LightPositionHighlightedColorProperty);
            set => SetValue(LightPositionHighlightedColorProperty, value);
        }

        public Color DarkPositionHighlightedColor
        {
            get => (Color)GetValue(DarkPositionHighlightedColorProperty);
            set => SetValue(DarkPositionHighlightedColorProperty, value);
        }

        public Color LightPositionDangerColor
        {
            get => (Color)GetValue(LightPositionDangerColorProperty);
            set => SetValue(LightPositionDangerColorProperty, value);
        }

        public Color DarkPositionDangerColor
        {
            get => (Color)GetValue(DarkPositionDangerColorProperty);
            set => SetValue(DarkPositionDangerColorProperty, value);
        }

        public bool IsClickable
        {
            get => (bool)GetValue(IsClickableProperty);
            set => SetValue(IsClickableProperty, value);
        }
        
        public bool ShowLoader
        {
            get => (bool)GetValue(ShowLoaderProperty);
            set => SetValue(ShowLoaderProperty, value);
        }
        
        public double LoaderProgress
        {
            get => (double)GetValue(LoaderProgressProperty);
            set => SetValue(LoaderProgressProperty, value);
        }

        public ChessBoardView()
        {
            InitializeComponent();

            RefreshBoard();
        }

        private void RefreshBoard()
        {
            var isFirstTime = !Children.Any();
            if (isFirstTime)
            {
                SetUpEmptyPieceViews();
                SetUpProgressBar();
            }

            BackgroundColor = LightPositionColor;
            RefreshPieceViews();
        }

        private void SetUpEmptyPieceViews()
        {
            for (int verticalIndex = 0; verticalIndex < 8; verticalIndex++)
            {
                for (int horizontalIndex = 0; horizontalIndex < 8; horizontalIndex++)
                {
                    SetUpEmptyPieceViewForPosition(verticalIndex, horizontalIndex);
                }
            }
        }

        private void SetUpEmptyPieceViewForPosition(int verticalIndex, int horizontalIndex)
        {
            var pieceView = new ChessBoardPieceView();

            var positionBackgroundColor = verticalIndex % 2 == 0
                ? (horizontalIndex % 2 == 0 ? LightPositionColor : DarkPositionColor)
                : (horizontalIndex % 2 != 0 ? LightPositionColor : DarkPositionColor);

            pieceView.SetBinding(
                ChessBoardPieceView.OnTappedCommandProperty,
                new Binding(nameof(PieceTappedCommand)));
            pieceView.SetBinding(
                ChessBoardPieceView.IsSelectedProperty,
                new MultiBinding
                {
                    Converter = new FromSelectedPieceToBooleanConverter(),
                    Bindings = new List<BindingBase>
                    {
                        new Binding(nameof(SelectedPiece), mode: BindingMode.OneWay),
                        new Binding(nameof(ChessBoardPieceView.Piece), mode: BindingMode.OneWay, source: pieceView),
                    },
                });
            pieceView.SetBinding(
                ChessBoardPieceView.IsCandidateProperty,
                new MultiBinding
                {
                    Converter = new FromPositionsForSelectedPieceToBooleanConverter(),
                    Bindings = new List<BindingBase>
                    {
                        new Binding(nameof(PositionsForSelectedPiece), mode: BindingMode.OneWay),
                        new Binding(nameof(ChessBoardPieceView.Piece), mode: BindingMode.OneWay, source: pieceView),
                    },
                });
            pieceView.SetBinding(
                ChessBoardPieceView.HighlightBackgroundProperty,
                new MultiBinding
                {
                    Converter = new FromLastMovePositionsToBooleanConverter(),
                    Bindings = new List<BindingBase>
                    {
                        new Binding(nameof(LastMovePositions), mode: BindingMode.OneWay),
                        new Binding(nameof(ChessBoardPieceView.Piece), mode: BindingMode.OneWay, source: pieceView),
                    },
                });
            pieceView.SetBinding(
                ChessBoardPieceView.IsClickableProperty,
                new Binding(nameof(IsClickable)));
            pieceView.SetBinding(
                ChessBoardPieceView.FillColorProperty,
                new Binding((positionBackgroundColor == LightPositionColor) ? nameof(LightPositionColor) : nameof(DarkPositionColor)));
            pieceView.SetBinding(
                ChessBoardPieceView.TextColorProperty,
                new Binding((positionBackgroundColor == LightPositionColor) ? nameof(DarkPositionColor) : nameof(LightPositionColor)));
            pieceView.SetBinding(
                ChessBoardPieceView.HighlightedBackgroundColorProperty,
                new Binding((positionBackgroundColor == LightPositionColor) ? nameof(LightPositionHighlightedColor) : nameof(DarkPositionHighlightedColor)));
            pieceView.SetBinding(
                ChessBoardPieceView.DangerBackgroundColorProperty,
                new Binding((positionBackgroundColor == LightPositionColor) ? nameof(LightPositionDangerColor) : nameof(DarkPositionDangerColor)));
            pieceView.BindingContext = this;

            // Add the view on the correct position within the board
            Children.Add(pieceView, verticalIndex, horizontalIndex);
        }

        private void SetUpProgressBar()
        {
            var progressBar = new ProgressBar
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
            };
            progressBar.SetBinding(ProgressBar.IsVisibleProperty, new Binding(nameof(ShowLoader)));
            progressBar.SetBinding(ProgressBar.ProgressProperty, new Binding(nameof(LoaderProgress)));
            progressBar.BindingContext = this;

            Children.Add(progressBar, 0, 8, 0, 8);
        }

        private void RefreshPieceViews()
        {
            if (Board != null)
            {
                foreach (var child in Children)
                {
                    if (child is ChessBoardPieceView pieceView)
                    {
                        var columnInGrid = GetColumn(pieceView);
                        var rowInGrid = GetRow(pieceView);
                        var horizontalIndex = HumanPlayer == Player.First ? columnInGrid : (7 - columnInGrid);
                        var verticalIndex = HumanPlayer == Player.First ? rowInGrid : (7 - rowInGrid);
                        var piece = Board[verticalIndex][horizontalIndex];

                        var bottomRightText = rowInGrid == 7
                            ? piece.Position.HorizontalPosition.ToString().Replace("P_", "").ToLower()
                            : string.Empty;
                        var topLeftText = columnInGrid == 0
                            ? piece.Position.VerticalPosition.ToString().Replace("P_", "")
                            : string.Empty;

                        pieceView.Piece = piece;
                        pieceView.BottomRightText = bottomRightText;
                        pieceView.TopLeftText = topLeftText;
                        pieceView.ShowDangerBackground = piece.Type == PieceType.King && piece.Player == PlayerInCheck;
                    }
                }
            }
        }
    }
}