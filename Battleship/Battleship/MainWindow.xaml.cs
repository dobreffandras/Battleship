using Battleship.Components;
using Battleship.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
