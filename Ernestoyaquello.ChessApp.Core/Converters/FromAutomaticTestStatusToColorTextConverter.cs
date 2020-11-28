using System;
using System.Globalization;
using Ernestoyaquello.ChessApp.ViewModels.Models;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.Converters
{
    public class FromAutomaticTestStatusToColorTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChessTestStatus status)
            {
                return status switch
                {
                    ChessTestStatus.Running => Color.LightYellow,
                    ChessTestStatus.Success => Color.LightGreen,
                    ChessTestStatus.Fail => Color.Red,
                    ChessTestStatus.NotStarted => Color.White,
                };
            }

            return Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
