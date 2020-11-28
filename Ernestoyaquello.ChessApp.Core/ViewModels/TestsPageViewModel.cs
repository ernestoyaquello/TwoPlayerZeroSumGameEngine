using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ernestoyaquello.Chess.Data;
using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.ChessApp.ViewModels.Models;
using Ernestoyaquello.ChessApp.Views;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.ViewModels
{
    public class TestsPageViewModel : BaseViewModel
    {
        public const string NavParamRunTestWithParallelAlgorithmKey = "RunTestWithParallelAlgorithm";

        private List<ChessTestItem> _automaticTests;
        public List<ChessTestItem> AutomaticTests
        {
            get => _automaticTests;
            set => SetProperty(ref _automaticTests, value);
        }

        private bool _canRunAutomaticTests;
        public bool CanRunAutomaticTests
        {
            get => _canRunAutomaticTests;
            set => SetProperty(ref _canRunAutomaticTests, value);
        }

        public DelegateCommand RunAutomaticTestsCommand { get; }
        public DelegateCommand OpenCastlingTestCommand { get; }
        public DelegateCommand OpenPawnReplacementTestCommand { get; }
        public DelegateCommand OpenEnPassantTestCommand { get; }

        public TestsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Chess Engine Tests";

            AutomaticTests = new List<ChessTestItem>
            {
                new ChessTestItem { TestType = ChessBoardLayoutType.Puzzle },
                new ChessTestItem { TestType = ChessBoardLayoutType.Puzzle2 },
                new ChessTestItem { TestType = ChessBoardLayoutType.Puzzle3 },
                new ChessTestItem { TestType = ChessBoardLayoutType.Puzzle4 },
                new ChessTestItem { TestType = ChessBoardLayoutType.Puzzle5 },
                new ChessTestItem { TestType = ChessBoardLayoutType.Puzzle6 },
            };
            CanRunAutomaticTests = true;
            RunAutomaticTestsCommand = new DelegateCommand(async () => await Task.Run(async () => await RunAutomaticTests()), () => CanRunAutomaticTests)
                .ObservesCanExecute(() => CanRunAutomaticTests);

            OpenCastlingTestCommand = new DelegateCommand(OpenCastlingTest);
            OpenPawnReplacementTestCommand = new DelegateCommand(OpenPawnReplacementTest);
            OpenEnPassantTestCommand = new DelegateCommand(OpenEnPassantTest);
        }

        private async Task RunAutomaticTests()
        {
            CanRunAutomaticTests = false;

            AutomaticTests.ForEach(test => test.Status = ChessTestStatus.NotStarted);
            AutomaticTests = AutomaticTests.ToList();

            foreach (var test in AutomaticTests)
            {
                await RunPuzzleTest(test);
            }

            CanRunAutomaticTests = true;
        }

        private void OpenCastlingTest()
        {
            StartChessGame(ChessBoardLayoutType.CastlingTest);
        }

        private void OpenPawnReplacementTest()
        {
            StartChessGame(ChessBoardLayoutType.PawnReplacementTest);
        }

        private void OpenEnPassantTest()
        {
            StartChessGame(ChessBoardLayoutType.EnPassantTest);
        }

        private void StartChessGame(ChessBoardLayoutType layoutType)
        {
            var navParams = new NavigationParameters
            {
                { ChessBoardPageViewModel.NavParamHumanPlayerKey, Player.First },
                { ChessBoardPageViewModel.NavParamBoardLayoutTypeKey, layoutType },
                { ChessBoardPageViewModel.NavParamMaxTreeDepthKey, 4 },
            };
            Device.BeginInvokeOnMainThread(async () => await NavigationService.NavigateAsync(nameof(ChessBoardPage), navParams));
        }

        private async Task RunPuzzleTest(ChessTestItem puzzleTest)
        {
            puzzleTest.Status = ChessTestStatus.Running;
            puzzleTest.Elapsed = TimeSpan.Zero;

            var puzzleSolution = ChessBoardLayoutFactory.GetPuzzleSolution(puzzleTest.TestType);
            var gameEngine = new TwoPlayerZeroSumGameMovesEngine();
            var chessBoard = new ChessBoard(puzzleTest.TestType, gameEngine);
            gameEngine.Initialize(maxTreeDepth: puzzleSolution.RequiredTreeDepth);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var bestMoveResult = await gameEngine.CalculateBestMove<ChessBoard, ChessMoveInfo, ChessBoardState>(
                chessBoard,
                Player.First,
                onProgressMade: progress => Device.BeginInvokeOnMainThread(() =>
                {
                    puzzleTest.Progress = progress;
                    puzzleTest.Elapsed = stopWatch.Elapsed;
                }));

            stopWatch.Stop();

            var wasSuccessful = bestMoveResult.BestMove.Equals(puzzleSolution.BestMove);

            puzzleTest.Elapsed = stopWatch.Elapsed;
            puzzleTest.Status = wasSuccessful ? ChessTestStatus.Success : ChessTestStatus.Fail;
        }
    }
}
