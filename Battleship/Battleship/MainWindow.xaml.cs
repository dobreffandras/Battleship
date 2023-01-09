using Battleship.Model;
using Battleship.Services;
using System.Windows;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var communicationService = new CommunicationService();
            this.DataContext = new MainWindowViewModel()
            {
                ViewModel = new LobbyViewModel(
                    communicationService,
                    navigateToGameViewModel: game =>
                    {
                        ChangeViewModel(new GameViewModel(game, communicationService));
                    }),
            };
        }

        private void ChangeViewModel(BaseViewModel viewModel)
        {
            if(DataContext is MainWindowViewModel mainViewModel)
            {
                mainViewModel.ViewModel = viewModel;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainWindowViewModel mainViewModel
                && mainViewModel.ViewModel is GameViewModel gameViewModel)
            {
                gameViewModel.LeaveGame();
            }
        }
    }
}
