using Battleship.Model;
using Battleship.Services;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CommunicationService communicationService;

        public MainWindow(IConfiguration configuration)
        {
            InitializeComponent();
            var host = configuration.GetRequiredSection("host").Value!;
            communicationService = new CommunicationService(host);
            var navigationService = new NavigationService(ChangeViewModel, communicationService);
            this.DataContext = new MainWindowViewModel()
            {
                ViewModel = new LobbyViewModel(communicationService, navigationService),
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
                communicationService.Close();
            }
        }
    }
}
