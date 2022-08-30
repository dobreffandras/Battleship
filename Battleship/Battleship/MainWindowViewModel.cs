namespace Battleship
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private object? viewModel;

        public object? ViewModel
        {
            get => viewModel;
            set
            {
                viewModel = value;
                NotifyPropertyChanged();
            }
        }
    }
}
