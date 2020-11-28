using System;
using System.Globalization;
using Ernestoyaquello.ChessApp.ViewModels.Models;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.Converters
{
    public class FromAutomaticTestStatusToSuccessTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChessTestStatus status)
            {
                return status switch
                {
                    ChessTestStatus.Running => "RUNNING",
                    ChessTestStatus.Success => "SUCCESS",
                    ChessTestStatus.Fail => "FAIL",
                    ChessTestStatus.NotStarted => "NOT STARTED",
                };
            }

            return "NOT STARTED";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
