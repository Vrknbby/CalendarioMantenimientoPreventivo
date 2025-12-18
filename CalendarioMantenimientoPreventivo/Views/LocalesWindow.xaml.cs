using CalendarioMantenimientoPreventivo.Models;
using CalendarioMantenimientoPreventivo.Service;
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
using System.Windows.Shapes;

namespace CalendarioMantenimientoPreventivo.Views
{
    /// <summary>
    /// Lógica de interacción para LocalesWindow.xaml
    /// </summary>
    public partial class LocalesWindow : Window
    {
        private readonly LocalService _localService;
        public LocalesWindow(LocalService localService)
        {
            InitializeComponent();
            _localService = localService;
            cargarLocales();
        }

        private void cargarLocales()
        {
            LocalesListBox.ItemsSource = null;
            LocalesListBox.ItemsSource = _localService.Locales;
        }
        private void AgregarButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new LocalDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && dialog.FueGuardado)
            {
                _localService.AgregarLocal(dialog.NombreLocal);
                cargarLocales();
            }
        }

        private void EditarButton_Click(object sender, RoutedEventArgs e)
        {
            Local? localSeleccionado = LocalesListBox.SelectedItem as Local;
            if (localSeleccionado == null)
            {
                MessageBox.Show(
                    "Por favor, seleccione un local de la lista para editar.",
                    "Selección Requerida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var dialog = new LocalDialog(localSeleccionado.Nombre);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && dialog.FueGuardado)
            {
                _localService.ActualizarLocal(localSeleccionado, dialog.NombreLocal);
                cargarLocales();
            }
        }

        private void EliminarButton_Click(object sender, RoutedEventArgs e)
        {
            Local localSeleccionado = LocalesListBox.SelectedItem as Local;
            if (localSeleccionado == null)
            {
                MessageBox.Show(
                    "Por favor, seleccione un local de la lista para eliminar.",
                    "Selección Requerida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"¿Está seguro de que desea eliminar el local '{localSeleccionado.Nombre}'?\n\nEsta acción no se puede deshacer.",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _localService.EliminarLocal(localSeleccionado);
                cargarLocales();
            }
        }

        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
