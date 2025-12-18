using CalendarioMantenimientoPreventivo.Service;
using CalendarioMantenimientoPreventivo.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalendarioMantenimientoPreventivo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LocalService _localService;

        public MainWindow()
        {
            InitializeComponent();
            _localService = new LocalService();
        }

        private void AbrirLocalesWindow_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();

            var localesWindow = new LocalesWindow(_localService);
            localesWindow.Owner = this;
            localesWindow.Closed += (s, args) => this.Show();
            localesWindow.Show();
        }
    }
}