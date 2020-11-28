using System;
using Ernestoyaquello.Chess.Data;
using Prism.Mvvm;

namespace Ernestoyaquello.ChessApp.ViewModels.Models
{
    public class ChessTestItem : BindableBase
    {
        private ChessBoardLayoutType _testType;
        public ChessBoardLayoutType TestType
        {
            get => _testType;
            set => SetProperty(ref _testType, value);
        }

        private double _progress;
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private ChessTestStatus _status;
        public ChessTestStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private TimeSpan _elapsed;
        public TimeSpan Elapsed
        {
            get => _elapsed;
            set => SetProperty(ref _elapsed, value);
        }
    }
}
