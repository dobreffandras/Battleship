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
    }
}
