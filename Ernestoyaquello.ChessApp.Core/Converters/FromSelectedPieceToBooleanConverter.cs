using System;
using System.Globalization;
using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.Chess.Util;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.Converters
{
    public class FromSelectedPieceToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedPiece = values[0] as Piece;
            var piece = values[1] as Piece;

            return !selectedPiece.IsNone() && selectedPiece.Equals(piece);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
