using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models;
using CalendarioMantenimientoPreventivo.Service;
using Microsoft.EntityFrameworkCore;
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
    /// Lógica de interacción para MantenimientosWindow.xaml
    /// </summary>
    public partial class MantenimientosWindow : Window
    {
        private readonly Local _local;
        private readonly MantenimientoService _service;
        private bool _esEdicion;
        public MantenimientosWindow(Local local, AppDbContext contextl)
        {
            InitializeComponent();
            _local = local;

            Title = $"Mantenimientos - {_local.Nombre}";
            NombreLocalTextBlock.Text = _local.Nombre;
            _service = new MantenimientoService(contextl, _local.Id);
            MantenimientosListBox.ItemsSource = _service.Mantenimientos;

            ActualizarEstadoBotones();
        }

        private void GuardarButton_Click(object sender, RoutedEventArgs e)
        {
            if (_esEdicion)
            {
                MessageBox.Show(
                    "Actualmente está en modo de edición. Por favor, utilice el botón MODIFICAR para guardar los cambios o CANCELAR la edición.",
                    "Modo de Edición Activo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                _service.Agregar(
                    NombreTextBox.Text,
                    DescripcionTextBox.Text,
                    MesComboBox.SelectedIndex + 1,
                    int.Parse(AñoTextBox.Text),
                    decimal.Parse(CostoTextBox.Text)
                );

                MessageBox.Show(
                    "Mantenimiento guardado exitosamente.",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al guardar el mantenimiento: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void EditarButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_esEdicion)
            {
                MessageBox.Show(
                    "No está en modo de edición. Haga doble clic en un mantenimiento para editarlo.",
                    "Modo de Edición Inactivo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Mantenimiento? mantenimientoSeleccionado = MantenimientosListBox.SelectedItem as Mantenimiento;
            if (mantenimientoSeleccionado == null)
            {
                MessageBox.Show(
                    "Por favor, seleccione una actividad de mantenimiento de la lista para MODIFICAR.",
                    "Selección Requerida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                _service.Actualizar(
                    mantenimientoSeleccionado,
                    NombreTextBox.Text,
                    DescripcionTextBox.Text,
                    MesComboBox.SelectedIndex + 1,
                    int.Parse(AñoTextBox.Text),
                    decimal.Parse(CostoTextBox.Text)
                );

                MessageBox.Show(
                    "Mantenimiento actualizado exitosamente.",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DesactivarModoEdicion();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al actualizar el mantenimiento: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void EliminarButton_Click(object sender, RoutedEventArgs e)
        {
            Mantenimiento? mantenimientoSeleccionado = MantenimientosListBox.SelectedItem as Mantenimiento;
            if (mantenimientoSeleccionado == null)
            {
                MessageBox.Show(
                    "Por favor, seleccione una actividad de mantenimiento de la lista para ELIMINAR.",
                    "Selección Requerida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            MessageBoxResult resultado = MessageBox.Show(
                $"¿Está seguro que desea eliminar el mantenimiento '{mantenimientoSeleccionado.Nombre}'?",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    _service.Eliminar(mantenimientoSeleccionado);

                    MessageBox.Show(
                        "Mantenimiento eliminado exitosamente.",
                        "Éxito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    DesactivarModoEdicion();
                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error al eliminar el mantenimiento: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void LimpiarFormulario()
        {
            NombreTextBox.Clear();
            DescripcionTextBox.Clear();
            MesComboBox.SelectedIndex = -1;
            AñoTextBox.Clear();
            CostoTextBox.Clear();
            MantenimientosListBox.SelectedIndex = -1;
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultado = MessageBox.Show(
                "¿Está seguro que desea cancelar la edición? Los cambios no guardados se perderán.",
                "Confirmar Cancelación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                DesactivarModoEdicion();
                LimpiarFormulario();
            }
        }

        private void MantenimientosListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MantenimientosListBox.SelectedItem is not Mantenimiento mantenimientoSeleccionado) return;

            NombreTextBox.Text = mantenimientoSeleccionado.Nombre;
            DescripcionTextBox.Text = mantenimientoSeleccionado.Descripcion;
            MesComboBox.SelectedIndex = mantenimientoSeleccionado.Mes - 1;
            AñoTextBox.Text = mantenimientoSeleccionado.Anio.ToString();
            CostoTextBox.Text = mantenimientoSeleccionado.Costo.ToString();

            ActivarModoEdicion();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(NombreTextBox.Text))
            {
                MessageBox.Show(
                    "El nombre del mantenimiento es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                NombreTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(DescripcionTextBox.Text))
            {
                MessageBox.Show(
                    "La descripción del mantenimiento es obligatoria.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                DescripcionTextBox.Focus();
                return false;
            }

            if (MesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Debe seleccionar un mes.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                MesComboBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(AñoTextBox.Text) || !int.TryParse(AñoTextBox.Text, out int anio))
            {
                MessageBox.Show(
                    "El año debe ser un número válido.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                AñoTextBox.Focus();
                return false;
            }

            if (anio < 2000 || anio > 2100)
            {
                MessageBox.Show(
                    "El año debe estar entre 2000 y 2100.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                AñoTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(CostoTextBox.Text) || !decimal.TryParse(CostoTextBox.Text, out decimal costo))
            {
                MessageBox.Show(
                    "El costo debe ser un número decimal válido.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                CostoTextBox.Focus();
                return false;
            }

            if (costo < 0)
            {
                MessageBox.Show(
                    "El costo no puede ser negativo.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                CostoTextBox.Focus();
                return false;
            }

            return true;
        }

        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            if (_esEdicion)
            {
                MessageBoxResult resultado = MessageBox.Show(
                    "Tiene cambios sin guardar. ¿Está seguro que desea cerrar?",
                    "Confirmar Cierre",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.No)
                {
                    return;
                }
            }

            this.Close();
        }

        private void MantenimientosListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizarEstadoBotones();
        }

        private void ActivarModoEdicion()
        {
            _esEdicion = true;
            GuardarButton.IsEnabled = false;
            EditarButton.IsEnabled = true;
            CancelarButton.Visibility = Visibility.Visible;
            ModoEdicionBorder.Visibility = Visibility.Visible;
        }

        private void DesactivarModoEdicion()
        {
            _esEdicion = false;
            GuardarButton.IsEnabled = true;
            EditarButton.IsEnabled = false;
            CancelarButton.Visibility = Visibility.Collapsed;
            ModoEdicionBorder.Visibility = Visibility.Collapsed;
        }

        private void ActualizarEstadoBotones()
        {
            bool haySeleccion = MantenimientosListBox.SelectedItem != null;
            EliminarButton.IsEnabled = haySeleccion && !_esEdicion;
            if (!_esEdicion)
            {
                EditarButton.IsEnabled = false;
            }
        }
    }
}
