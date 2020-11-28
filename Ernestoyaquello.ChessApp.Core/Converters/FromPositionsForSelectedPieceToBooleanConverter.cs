using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ernestoyaquello.Chess.Models;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.Converters
{
    public class FromPositionsForSelectedPieceToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var validPositions = values[0] as List<PiecePosition>;
            var piece = values[1] as Piece;

            return validPositions != null && validPositions.Any(position => position.Equals(piece.Position));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
