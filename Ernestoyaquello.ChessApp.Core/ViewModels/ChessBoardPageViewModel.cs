using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ernestoyaquello.Chess.Data;
using Ernestoyaquello.Chess.Models;
using Ernestoyaquello.Chess.Util;
using Ernestoyaquello.ChessApp.Services;
using Ernestoyaquello.ChessApp.ViewModels.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Util;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace Ernestoyaquello.ChessApp.ViewModels
{
    public class ChessBoardPageViewModel : BaseViewModel
    {
        public const string NavParamHumanPlayerKey = "HumanPlayer";
        public const string NavParamBoardLayoutTypeKey = "BoardLayoutType";
        public const string NavParamMaxTreeDepthKey = "MaxTreeDepth";

        private Player _humanPlayer;
        public Player HumanPlayer
        {
            get => _humanPlayer;
            set
            {
                if (SetProperty(ref _humanPlayer, value))
                {
                    RaisePropertyChanged(nameof(GameTurnStatusAsString));
                }
            }
        }

        private Player? _playerInCheck;
        public Player? PlayerInCheck
        {
            get => _playerInCheck;
            set => SetProperty(ref _playerInCheck, value);
        }

        private Piece[][] _board;
        public Piece[][] Board
        {
            get => _board;
            set => SetProperty(ref _board, value);
        }

        private Piece _selectedPiece;
        public Piece SelectedPiece
        {
            get => _selectedPiece;
            set
            {
                if (SetProperty(ref _selectedPiece, value))
                {
                    PositionsForSelectedPiece = !SelectedPiece.IsNone()
                        ? _chessBoard.CalculateValidMovesForPiece(SelectedPiece)
                            .Select(move => move.MoveSteps.First(step => step.OldPosition.Equals(SelectedPiece.Position)).NewPosition)
                            .ToList()
                        : null;
                }
            }
        }

        private List<PiecePosition> _positionsForSelectedPiece;
        public List<PiecePosition> PositionsForSelectedPiece
        {
            get => _positionsForSelectedPiece;
            set => SetProperty(ref _positionsForSelectedPiece, value);
        }

        private List<Piece> _piecesCapturesByWhite;
        public List<Piece> PiecesCapturesByWhite
        {
            get => _piecesCapturesByWhite;
            set => SetProperty(ref _piecesCapturesByWhite, value);
        }

        private List<Piece> _piecesCapturesByBlack;
        public List<Piece> PiecesCapturesByBlack
        {
            get => _piecesCapturesByBlack;
            set => SetProperty(ref _piecesCapturesByBlack, value);
        }

        private GameResult _gameResult;
        public GameResult GameResult
        {
            get => _gameResult;
            set
            {
                if (SetProperty(ref _gameResult, value))
                {
                    RaisePropertyChanged(nameof(PlayerCanMove));
                    RaisePropertyChanged(nameof(GameResultAsString));
                    RaisePropertyChanged(nameof(GameTurnStatusAsString));
                }
            }
        }

        private bool _isMachineMakingMove;
        public bool IsMachineMakingMove
        {
            get => _isMachineMakingMove;
            set
            {
                if (SetProperty(ref _isMachineMakingMove, value))
                {
                    RaisePropertyChanged(nameof(PlayerCanMove));
                }
            }
        }
        
        private bool _isHumanPlayerTurn;
        public bool IsHumanPlayerTurn
        {
            get => _isHumanPlayerTurn;
            set
            {
                if (SetProperty(ref _isHumanPlayerTurn, value))
                {
                    RaisePropertyChanged(nameof(PlayerCanMove));
                    RaisePropertyChanged(nameof(GameTurnStatusAsString));
                }
            }
        }

        private double _progressMadeByMachine;
        public double ProgressMadeByMachine
        {
            get => _progressMadeByMachine;
            set => SetProperty(ref _progressMadeByMachine, value);
        }
        
        private int _maxTreeDepth;
        public int MaxTreeDepth
        {
            get => _maxTreeDepth;
            set
            {
                if (SetProperty(ref _maxTreeDepth, value))
                {
                    _gameEngine.MaxTreeDepth = _maxTreeDepth;
                }
            }
        }
        
        private List<PiecePosition> _lastMovePositions;
        public List<PiecePosition> LastMovePositions
        {
            get => _lastMovePositions;
            set => SetProperty(ref _lastMovePositions, value);
        }

        private List<ChessMoveItem> _history;
        public List<ChessMoveItem> History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        public DelegateCommand<Piece> PieceTappedCommand { get; }

        public List<int> TreeDepthOptions => new List<int> { 1, 2, 3, 4, 5, 6 };

        public bool PlayerCanMove => !IsMachineMakingMove && IsHumanPlayerTurn && GameResult == GameResult.StillGame;

        public string GameResultAsString => GameResult != GameResult.StillGame ? "Game Finished" : "Game Ongoing";
        public string GameTurnStatusAsString => GetTurnStatusAsString();

        private readonly IPopupService _popupService;
        private readonly ITwoPlayerZeroSumGameMovesEngine _gameEngine;

        private ChessBoard _chessBoard;

        public ChessBoardPageViewModel(INavigationService navigationService, IPopupService popupService, ITwoPlayerZeroSumGameMovesEngine gameEngine)
            : base(navigationService)
        {
            _popupService = popupService;
            _gameEngine = gameEngine;

            Title = "Chess Game";
            GameResult = GameResult.StillGame;
            MaxTreeDepth = 5;

            PieceTappedCommand = new DelegateCommand<Piece>(async (piece) => await Task.Run(async () => await OnPieceTapped(piece)), _ => PlayerCanMove)
                .ObservesCanExecute(() => PlayerCanMove);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(NavParamHumanPlayerKey, out Player humanPlayer) &&
                parameters.TryGetValue(NavParamBoardLayoutTypeKey, out ChessBoardLayoutType layoutType) &&
                parameters.TryGetValue(NavParamMaxTreeDepthKey, out int maxTreeDepth))
            {
                // HACK Using this callback instead of Initialize() to avoid freezing the UI
                await Task.Run(async () => await InitializeChessBoard(humanPlayer, layoutType, maxTreeDepth));
            }
        }

        private async Task InitializeChessBoard(Player humanPlayer, ChessBoardLayoutType layoutType, int maxTreeDepth)
        {
            _chessBoard = new ChessBoard(layoutType, _gameEngine);
            _gameEngine.Initialize(MaxTreeDepth: maxTreeDepth);

            HumanPlayer = humanPlayer;
            MaxTreeDepth = maxTreeDepth;
            UpdateBoardData();

            // If the machine has to make the first move, we trigger it here
            if (!IsHumanPlayerTurn)
            {
                IsMachineMakingMove = true;
                ProgressMadeByMachine = 0d;

                await Task.Delay(150);

                var bestMoveForMachineResult = await _gameEngine.CalculateBestMove<ChessBoard, ChessMoveInfo, ChessBoardState>(
                    _chessBoard,
                    Player.First,
                    onProgressMade: OnProgressMadeByMachine);

                var bestMoveForMachine = bestMoveForMachineResult.BestMove;
                _gameEngine.TryMakeMove<ChessBoard, ChessMoveInfo, ChessBoardState>(_chessBoard, bestMoveForMachine, shouldBeValid: false);
                await Task.Delay(350);

                UpdateBoardData();

                IsMachineMakingMove = false;
            }
        }

        private async Task<ChessMoveInfo> OnPlayerMustSelectPawnPromotion(List<ChessMoveInfo> possibleChoices)
        {
            var selectedPiece = await _popupService.ShowPawnReplacementPopup(possibleChoices.Select(move => move.MoveSteps[1].PieceToMove).ToList());
            var selectedPieceMove = possibleChoices.First(move => move.MoveSteps[1].PieceToMove.Equals(selectedPiece));
            return selectedPieceMove;
        }

        private async Task OnPieceTapped(Piece tappedPiece)
        {
            if (SelectedPiece.IsNone())
            {
                SelectedPiece = !tappedPiece.IsNone() && tappedPiece.Player == HumanPlayer ? tappedPiece : Piece.None;
            }
            else
            {
                if (!SelectedPiece.Equals(tappedPiece))
                {
                    if (SelectedPiece.Player != tappedPiece.Player)
                    {
                        await TryToMoveSelectedPieceToTappedPosition(tappedPiece.Position);
                    }
                    else
                    {
                        SelectedPiece = tappedPiece;
                    }
                }
                else
                {
                    SelectedPiece = Piece.None;
                }
            }
        }

        private async Task TryToMoveSelectedPieceToTappedPosition(PiecePosition tappedPosition)
        {
            var validMoves = _chessBoard.CalculateValidMovesForPiece(SelectedPiece);
            var movesToMake = validMoves.Where(move =>
                move.MoveSteps.Any(step => step.OldPosition.Equals(SelectedPiece.Position) && step.NewPosition.Equals(tappedPosition)));
            if (movesToMake.Any())
            {
                var moveToMake = movesToMake.Count() == 1 ? movesToMake.First() : await OnPlayerMustSelectPawnPromotion(movesToMake.ToList());

                // We make the move manually already so the user can see it while they wait for the machine to move
                await SimulateMoveInBoard(moveToMake);
                await MoveSelectedPieceToTappedPosition(moveToMake);
            }
        }

        private async Task SimulateMoveInBoard(ChessMoveInfo moveToMake)
        {
            SelectedPiece = null;

            var currentChessBoard = _chessBoard;
            _gameEngine.TryMakeDummyMove<ChessBoard, ChessMoveInfo, ChessBoardState>(_chessBoard, moveToMake, tempBoard =>
            {
                _chessBoard = tempBoard;
                UpdateBoardData();
            }, shouldBeValid: false);

            // Give the UI some time to update
            await Task.Delay(250);

            _chessBoard = currentChessBoard;
        }

        private async Task MoveSelectedPieceToTappedPosition(ChessMoveInfo humanMove)
        {
            IsMachineMakingMove = true;
            ProgressMadeByMachine = 0d;
            IsHumanPlayerTurn = false;

            await Task.Delay(150);

            await _gameEngine.MakeMoveAgainstMachine<ChessBoard, ChessMoveInfo, ChessBoardState>(
                _chessBoard,
                humanMove,
                shouldBeValid: false,
                onProgressMade: OnProgressMadeByMachine);

            await Task.Delay(350);

            IsMachineMakingMove = false;

            UpdateBoardData();
        }

        private void UpdateBoardData()
        {
            Board = _chessBoard.GetBoardLayout();
            IsHumanPlayerTurn = _chessBoard.IsPlayerTurn(HumanPlayer);
            GameResult = _chessBoard.GetGameResult(HumanPlayer);
            PiecesCapturesByWhite = _chessBoard.GetCapturedPieces(Player.First).OrderBy(piece => piece.CanonicValue).ThenBy(piece => piece.Type).ToList();
            PiecesCapturesByBlack = _chessBoard.GetCapturedPieces(Player.Second).OrderBy(piece => piece.CanonicValue).ThenBy(piece => piece.Type).ToList();

            var updatedHistory = _chessBoard.GetMoveHistory();
            History = updatedHistory.Select(move => ChessMoveItem.FromMove(move, HumanPlayer)).ToList();
            UpdateLastMovePositions(updatedHistory.LastOrDefault());

            var playerToMove = IsHumanPlayerTurn ? HumanPlayer : HumanPlayer.ToOppositePlayer();
            var isPlayerToMoveInCheck = _chessBoard.IsKingInCheck(playerToMove);
            PlayerInCheck = isPlayerToMoveInCheck ? (Player?)playerToMove : null;
        }

        private void UpdateLastMovePositions(ChessMoveInfo chessMoveInfo)
        {
            LastMovePositions = chessMoveInfo != null
                ? chessMoveInfo.MoveSteps.SelectMany(step => new PiecePosition[] { step.OldPosition, step.NewPosition }).Distinct().ToList()
                : new List<PiecePosition>();
        }

        private void OnProgressMadeByMachine(double progress)
        {
            Device.BeginInvokeOnMainThread(() => ProgressMadeByMachine = progress);
        }

        private string GetTurnStatusAsString()
        {
            var whitePlayerName = $"White ({(HumanPlayer == Player.First ? "You" : "Machine")})";
            var blackPlayerName = $"Black ({(HumanPlayer == Player.First ? "Machine" : "You")})";
            var humanPlayerName = HumanPlayer == Player.First ? whitePlayerName : blackPlayerName;
            var machinePlayerName = HumanPlayer == Player.First ? blackPlayerName : whitePlayerName;

            return GameResult switch
            {
                GameResult.StillGame => IsHumanPlayerTurn ? $"{humanPlayerName} to move" : $"{machinePlayerName} is thinking...",
                GameResult.Victory => $"{humanPlayerName} won! :)",
                GameResult.Defeat => $"{machinePlayerName} won! :(",
                _ => $"Draw",
            };
        }
    }
}
