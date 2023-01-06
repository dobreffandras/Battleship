using Battleship.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Battleship.Components
{
    /// <summary>
    /// Interaction logic for Cell.xaml
    /// </summary>
    public partial class Cell : UserControl
    {
        public Cell()
        {
            InitializeComponent();
        }

        public bool IsShippart
        {
            get { return (bool)GetValue(IsShippartProperty); }
            set { SetValue(IsShippartProperty, value); }
        }

        public static readonly DependencyProperty IsShippartProperty =
            DependencyProperty.Register(
                name: nameof(IsShippart),
                propertyType: typeof(bool),
                ownerType: typeof(Cell),
                typeMetadata: new PropertyMetadata(false));


        public ShootState ShootState
        {
            get { return (ShootState)GetValue(ShootStateProperty); }
            set { SetValue(ShootStateProperty, value); }
        }

        public static readonly DependencyProperty ShootStateProperty =
            DependencyProperty.Register(
                name: nameof(ShootState),
                propertyType: typeof(ShootState),
                ownerType: typeof(Cell),
                typeMetadata: new PropertyMetadata(ShootState.None));


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                name: nameof(Command),
                propertyType: typeof(ICommand),
                ownerType: typeof(Cell),
                typeMetadata: new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                name: nameof(CommandParameter),
                propertyType: typeof(object),
                ownerType: typeof(Cell),
                typeMetadata: new PropertyMetadata(null));

        public PlayingType PlayingType
        {
            get { return (PlayingType)GetValue(PlayingTypeProperty); }
            set { SetValue(PlayingTypeProperty, value); }
        }

        public static readonly DependencyProperty PlayingTypeProperty =
            DependencyProperty.Register(
                name: nameof(PlayingType),
                propertyType: typeof(PlayingType),
                ownerType: typeof(Cell),
                typeMetadata: new PropertyMetadata(PlayingType.Passive));
    }
}
