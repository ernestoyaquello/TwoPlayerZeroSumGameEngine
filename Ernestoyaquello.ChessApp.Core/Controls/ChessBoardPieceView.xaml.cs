using System.Windows.Input;
using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.Chess.Util;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ernestoyaquello.ChessApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChessBoardPieceView : Grid
    {
        public static readonly BindableProperty PieceProperty =
            BindableProperty.Create(nameof(Piece), typeof(Piece), typeof(ChessBoardPieceView), default(Piece),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardPieceView)?.UpdateImage());

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(ChessBoardPieceView), default(bool),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardPieceView)?.UpdateBorderColor());

        public static readonly BindableProperty IsCandidateProperty =
            BindableProperty.Create(nameof(IsCandidate), typeof(bool), typeof(ChessBoardPieceView), default(bool));

        public static readonly BindableProperty IsClickableProperty =
            BindableProperty.Create(nameof(IsClickable), typeof(bool), typeof(ChessBoardPieceView), default(bool));

        public static readonly BindableProperty HighlightBackgroundProperty =
            BindableProperty.Create(nameof(HighlightBackground), typeof(bool), typeof(ChessBoardPieceView), default(bool));

        public static readonly BindableProperty ShowDangerBackgroundProperty =
            BindableProperty.Create(nameof(ShowDangerBackground), typeof(bool), typeof(ChessBoardPieceView), default(bool));

        public static readonly BindableProperty FillColorProperty =
            BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(ChessBoardPieceView), default(Color),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as ChessBoardPieceView)?.UpdateBorderColor());

        public static readonly BindableProperty HighlightedBackgroundColorProperty =
            BindableProperty.Create(nameof(HighlightedBackgroundColor), typeof(Color), typeof(ChessBoardPieceView), default(Color));

        public static readonly BindableProperty DangerBackgroundColorProperty =
            BindableProperty.Create(nameof(DangerBackgroundColor), typeof(Color), typeof(ChessBoardPieceView), default(Color));

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ChessBoardPieceView), default(Color));

        public static readonly BindableProperty BottomRightTextProperty =
            BindableProperty.Create(nameof(BottomRightText), typeof(string), typeof(ChessBoardPieceView), string.Empty);

        public static readonly BindableProperty TopLeftTextProperty =
            BindableProperty.Create(nameof(TopLeftText), typeof(string), typeof(ChessBoardPieceView), string.Empty);

        public static readonly BindableProperty OnTappedCommandProperty =
            BindableProperty.Create(nameof(OnTappedCommand), typeof(ICommand), typeof(ChessBoardPieceView), default(ICommand));

        public Piece Piece
        {
            get => (Piece)GetValue(PieceProperty);
            set => SetValue(PieceProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        
        public bool IsCandidate
        {
            get => (bool)GetValue(IsCandidateProperty);
            set => SetValue(IsCandidateProperty, value);
        }

        public bool IsClickable
        {
            get => (bool)GetValue(IsClickableProperty);
            set => SetValue(IsClickableProperty, value);
        }
        
        public bool HighlightBackground
        {
            get => (bool)GetValue(HighlightBackgroundProperty);
            set => SetValue(HighlightBackgroundProperty, value);
        }
        
        public bool ShowDangerBackground
        {
            get => (bool)GetValue(ShowDangerBackgroundProperty);
            set => SetValue(ShowDangerBackgroundProperty, value);
        }

        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }
        
        public Color HighlightedBackgroundColor
        {
            get => (Color)GetValue(HighlightedBackgroundColorProperty);
            set => SetValue(HighlightedBackgroundColorProperty, value);
        }
        
        public Color DangerBackgroundColor
        {
            get => (Color)GetValue(DangerBackgroundColorProperty);
            set => SetValue(DangerBackgroundColorProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public string BottomRightText
        {
            get => (string)GetValue(BottomRightTextProperty);
            set => SetValue(BottomRightTextProperty, value);
        }

        public string TopLeftText
        {
            get => (string)GetValue(TopLeftTextProperty);
            set => SetValue(TopLeftTextProperty, value);
        }

        public ICommand OnTappedCommand
        {
            get => (ICommand)GetValue(OnTappedCommandProperty);
            set => SetValue(OnTappedCommandProperty, value);
        }

        public ChessBoardPieceView()
        {
            InitializeComponent();

            UpdateBorderColor();
            UpdateImage();
        }

        private void UpdateBorderColor()
        {
            BackgroundColor = IsSelected ? Color.Green : FillColor;
            DangerColorOverlay.Margin = IsSelected ? 5 : 0;
        }

        private void UpdateImage()
        {
            var imageNamePrefix = !Piece.IsNone() ? (Piece.Player == Player.First ? "white" : "black") : "";
            var imageSourceName = !Piece.IsNone() ? $"Assets/{imageNamePrefix}_{Piece.Type.ToString().ToLower()}.png" : "Assets/transparent.png";
            PieceImage.Source = imageSourceName;
        }
    }
}