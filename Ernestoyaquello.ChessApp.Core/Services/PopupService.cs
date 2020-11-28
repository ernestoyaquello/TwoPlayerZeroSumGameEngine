using System.Collections.Generic;
using System.Threading.Tasks;
using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.ChessApp.Views;
using Prism.Commands;
using Rg.Plugins.Popup.Contracts;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.Services
{
    public class PopupService : IPopupService
    {
        private readonly IPopupNavigation _popupNavigation;

        public PopupService(IPopupNavigation popupNavigation)
        {
            _popupNavigation = popupNavigation;
        }

        public async Task<Piece> ShowPawnReplacementPopup(List<Piece> availableReplacements)
        {
            // This allows us to wait until the user closes the popup
            var resultAwaiter = new TaskCompletionSource<Piece>();

            var pawnReplacementPopupPage = new PawnReplacementPopupPage { Pieces = availableReplacements };
            pawnReplacementPopupPage.ConfirmSelectionCommand = new DelegateCommand<Piece>((chosenPiece) =>
            {
                resultAwaiter.SetResult(chosenPiece);
                Device.BeginInvokeOnMainThread(async () => await _popupNavigation.RemovePageAsync(pawnReplacementPopupPage));
            });

            Device.BeginInvokeOnMainThread(async () => await _popupNavigation.PushAsync(pawnReplacementPopupPage));

            return await resultAwaiter.Task;
        }
    }
}
