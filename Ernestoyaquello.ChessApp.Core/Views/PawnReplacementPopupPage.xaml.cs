using System.Collections.Generic;
using System.Windows.Input;
using Ernestoyaquello.Chess.Models;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ernestoyaquello.ChessApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PawnReplacementPopupPage : PopupPage
    {
        public static readonly BindableProperty PiecesProperty =
            BindableProperty.Create(nameof(Pieces), typeof(List<Piece>), typeof(PawnReplacementPopupPage), default(List<Piece>));

        public static readonly BindableProperty ConfirmSelectionCommandProperty =
            BindableProperty.Create(nameof(ConfirmSelectionCommand), typeof(ICommand), typeof(PawnReplacementPopupPage), default(ICommand));

        public List<Piece> Pieces
        {
            get => (List<Piece>)GetValue(PiecesProperty);
            set => SetValue(PiecesProperty, value);
        }

        public ICommand ConfirmSelectionCommand
        {
            get => (ICommand)GetValue(ConfirmSelectionCommandProperty);
            set => SetValue(ConfirmSelectionCommandProperty, value);
        }

        public PawnReplacementPopupPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }
    }
}