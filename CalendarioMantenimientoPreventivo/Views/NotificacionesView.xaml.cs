using CalendarioMantenimientoPreventivo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace CalendarioMantenimientoPreventivo.Views
{
    /// <summary>
    /// Lógica de interacción para NotificacionesView.xaml
    /// </summary>
    public partial class NotificacionesView : Window
    {
        public ObservableCollection<NotificacionInfo> Notificaciones { get; set; }
        public NotificacionesView(List<NotificacionInfo> notificaciones)
        {
            InitializeComponent();
            Notificaciones = new ObservableCollection<NotificacionInfo>(notificaciones);
            DataContext = this;
        }

        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
