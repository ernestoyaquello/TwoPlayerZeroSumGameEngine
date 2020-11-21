using Prism;
using Prism.Ioc;
using Ernestoyaquello.ChessApp.ViewModels;
using Ernestoyaquello.ChessApp.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using Prism.Plugin.Popups;
using Ernestoyaquello.ChessApp.Services;
using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Engine;

namespace Ernestoyaquello.ChessApp
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterForNavigation<NavigationPage>();

            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterSingleton<IPopupService, PopupService>();
            containerRegistry.RegisterSingleton<ITwoPlayerZeroSumGameMovesEngine, TwoPlayerZeroSumGameMovesEngine>();

            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<ChessBoardPage, ChessBoardPageViewModel>();
            containerRegistry.RegisterForNavigation<TestsPage, TestsPageViewModel>();
        }
    }
}
