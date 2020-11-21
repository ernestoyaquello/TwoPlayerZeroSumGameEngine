using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Ernestoyaquello.Chess.Data;
using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.ChessApp.Views;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private bool _isWhitePieceSelected;
        public bool IsWhitePieceSelected
        {
            get => _isWhitePieceSelected;
            set
            {
                if (SetProperty(ref _isWhitePieceSelected, value))
                {
                    RaisePropertyChanged(nameof(IsBlackPieceSelected));
                }
            }
        }

        private List<ChessBoardLayoutType> _boardLayoutOptions;
        public List<ChessBoardLayoutType> BoardLayoutOptions
        {
            get => _boardLayoutOptions;
            set => SetProperty(ref _boardLayoutOptions, value);
        }
        
        private ChessBoardLayoutType _selectedBoardLayoutOption;
        public ChessBoardLayoutType SelectedBoardLayoutOption
        {
            get => _selectedBoardLayoutOption;
            set => SetProperty(ref _selectedBoardLayoutOption, value);
        }

        private int _selectedMaxTreeDepth;
        public int SelectedMaxTreeDepth
        {
            get => _selectedMaxTreeDepth;
            set => SetProperty(ref _selectedMaxTreeDepth, value);
        }

        public List<int> TreeDepthOptions => new List<int> { 1, 2, 3, 4, 5, 6 };

        public bool IsBlackPieceSelected => !IsWhitePieceSelected;

        public Piece WhitePiece => Piece.WhitePawn;

        public Piece BlackPiece => Piece.BlackPawn;

        public ICommand OnWhitePieceTappedCommand { get; }
        public ICommand OnBlackPieceTappedCommand { get; }
        public ICommand PlayMatchButtonCommand { get; }
        public ICommand OpenTestsScreen { get; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Solo Chess";
            IsWhitePieceSelected = true;
            BoardLayoutOptions = Enum.GetValues(typeof(ChessBoardLayoutType)).OfType<ChessBoardLayoutType>().ToList();
            SelectedBoardLayoutOption = ChessBoardLayoutType.Default;
            SelectedMaxTreeDepth = 5;

            OnWhitePieceTappedCommand = new DelegateCommand(OnWhitePieceClicked);
            OnBlackPieceTappedCommand = new DelegateCommand(OnBlackPieceClicked);
            PlayMatchButtonCommand = new DelegateCommand(OnPlayMatchButtonClicked);
            OpenTestsScreen = new DelegateCommand(OpenTestsScreenButtonClicked);
        }

        private void OnWhitePieceClicked()
        {
            IsWhitePieceSelected = true;
        }
        
        private void OnBlackPieceClicked()
        {
            IsWhitePieceSelected = false;
        }

        private void OnPlayMatchButtonClicked()
        {
            var navParams = new NavigationParameters
            {
                { ChessBoardPageViewModel.NavParamHumanPlayerKey, IsWhitePieceSelected ? Player.First : Player.Second },
                { ChessBoardPageViewModel.NavParamBoardLayoutTypeKey, SelectedBoardLayoutOption },
                { ChessBoardPageViewModel.NavParamMaxTreeDepthKey, SelectedMaxTreeDepth },
            };
            Device.BeginInvokeOnMainThread(async () => await NavigationService.NavigateAsync(nameof(ChessBoardPage), navParams));
        }

        private void OpenTestsScreenButtonClicked()
        {
            Device.BeginInvokeOnMainThread(async () => await NavigationService.NavigateAsync(nameof(TestsPage)));
        }
    }
}
