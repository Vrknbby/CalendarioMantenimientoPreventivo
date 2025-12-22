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
    /// Lógica de interacción para LocalDialog.xaml
    /// </summary>
    public partial class LocalDialog : Window
    {
        public string NombreLocal { get; private set; }
        public bool FueGuardado { get; private set; }
        private readonly bool _esEdicion;
        public LocalDialog() : this(null)
        {
        }
        public LocalDialog(string nombreActual)
        {
            InitializeComponent();

            _esEdicion = !string.IsNullOrWhiteSpace(nombreActual);

            if (_esEdicion)
            {
                TituloTextBlock.Text = "Editar Local";
                GuardarButton.Content = "Actualizar";
                NombreLocalTextBox.Text = nombreActual;
                NombreLocalTextBox.SelectAll();
            }
            else
            {
                TituloTextBlock.Text = "Agregar Local";
                GuardarButton.Content = "Guardar";
            }

            GuardarButton.Click += GuardarButton_Click;
            CancelarButton.Click += CancelarButton_Click;

            Loaded += (s, e) => NombreLocalTextBox.Focus();
        }

        private void GuardarButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreLocalTextBox.Text))
            {
                MessageBox.Show(
                    "Por favor, ingrese un nombre para el local.",
                    "Campo Requerido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                NombreLocalTextBox.Focus();
                return;
            }

            NombreLocal = NombreLocalTextBox.Text.Trim();
            FueGuardado = true;
            try
            {
                DialogResult = true;
            }
            catch (InvalidOperationException)
            {
                Close();
            }
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            FueGuardado = false;
            try
            {
                DialogResult = false;
            }
            catch (InvalidOperationException)
            {
                Close();
            }
        }
    }
}
