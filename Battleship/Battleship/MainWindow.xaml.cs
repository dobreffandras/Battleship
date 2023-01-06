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
                    navigateToGameViewModel: (model) =>
                    {
                        ChangeViewModel(new GameViewModel(model, new PlayfieldModel(), communicationService));
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
    }
}
