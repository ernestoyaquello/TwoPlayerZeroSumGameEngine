using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ernestoyaquello.Chess.Models;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.Converters
{
    // TODO This converter is a repetition of another one, so they should be merged into the same one
    public class FromLastMovePositionsToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var lastMovePositions = values[0] as List<PiecePosition>;
            var piece = values[1] as Piece;

            return lastMovePositions != null && lastMovePositions.Any(position => position.Equals(piece.Position));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}