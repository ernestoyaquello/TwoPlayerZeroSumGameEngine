using System.Collections.Generic;
using System.Threading.Tasks;
using Ernestoyaquello.Chess.Models;

namespace Ernestoyaquello.ChessApp.Services
{
    public interface IPopupService
    {
        Task<Piece> ShowPawnReplacementPopup(List<Piece> availableReplacements);
    }
}
