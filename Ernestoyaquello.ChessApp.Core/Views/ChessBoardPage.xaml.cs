using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ernestoyaquello.ChessApp.ViewModels;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.Views
{
    public partial class ChessBoardPage
    {
        private ChessBoardPageViewModel _viewModel;

        private double _lastWidth;
        private double _lastHeight;

        public ChessBoardPage()
        {
            InitializeComponent();
        }

        // HACK Not ideal to do all of this to scroll down on the list whenever it updates, but the ChildAdded
        // event doesn't work in UWP yet, and the observable collection was causiung issues... so here we are
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _viewModel = null;
            }

            if (BindingContext is ChessBoardPageViewModel viewModel)
            {
                _viewModel = viewModel;
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChessBoardPageViewModel.History))
            {
                // Small delay to give the UI time to update
                await Task.Delay(300);

                var lastElement = _viewModel?.History?.LastOrDefault();
                if (lastElement != null)
                {
                    Device.BeginInvokeOnMainThread(() => MovesList.ScrollTo(lastElement, ScrollToPosition.End, true));
                }
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > 0 && height > 0 && (width != _lastWidth || height != _lastHeight))
            {
                _lastWidth = width;
                _lastHeight = height;

                ResizeBoard(width, height);
            }
        }

        private void ResizeBoard(double width, double height)
        {
            var screenRatio = width / height;
            var boardSize = screenRatio >= 1.75d ? height : (width * (1d / 1.75d));
            var trackerWidth = boardSize * 0.75d;

            ChessBoard.HorizontalOptions = LayoutOptions.End;
            ChessBoard.VerticalOptions = LayoutOptions.Center;
            ChessBoard.WidthRequest = boardSize;
            ChessBoard.HeightRequest = boardSize;
            ChessBoard.ForceLayout();

            GameTracker.HorizontalOptions = LayoutOptions.Start;
            GameTracker.VerticalOptions = LayoutOptions.Center;
            GameTracker.WidthRequest = trackerWidth;
            GameTracker.HeightRequest = boardSize;
            GameTracker.ForceLayout();

            ForceLayout();
        }
    }
}
