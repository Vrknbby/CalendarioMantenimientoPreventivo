using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models;
using CalendarioMantenimientoPreventivo.Models.ViewModels;
using CalendarioMantenimientoPreventivo.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class LocalesWindow : Window, INotifyPropertyChanged
    {
        private readonly LocalService _localService;
        private readonly AppDbContext _context;

        private const int REGISTROS_POR_PAGINA = 10;

        private int _paginaActual = 1;
        public int PaginaActual
        {
            get => _paginaActual;
            set
            {
                if (_paginaActual != value)
                {
                    _paginaActual = value;
                    NotifyPropertyChanged(nameof(PaginaActual));
                    NotifyPropertyChanged(nameof(TextoPaginacion));
                    NotifyPropertyChanged(nameof(PuedeIrAnterior));
                    NotifyPropertyChanged(nameof(PuedeIrSiguiente));
                    CargarPagina();
                }
            }
        }

        private int _totalPaginas = 1;
        public int TotalPaginas
        {
            get => _totalPaginas;
            set
            {
                if (_totalPaginas != value)
                {
                    _totalPaginas = value;
                    NotifyPropertyChanged(nameof(TotalPaginas));
                    NotifyPropertyChanged(nameof(TextoPaginacion));
                    NotifyPropertyChanged(nameof(PuedeIrAnterior));
                    NotifyPropertyChanged(nameof(PuedeIrSiguiente));
                }
            }
        }

        private int _totalRegistros;
        public int TotalRegistros
        {
            get => _totalRegistros;
            set
            {
                if (_totalRegistros != value)
                {
                    _totalRegistros = value;
                    NotifyPropertyChanged(nameof(TotalRegistros));
                    NotifyPropertyChanged(nameof(TextoContador));
                }
            }
        }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (_textoBusqueda != value)
                {
                    _textoBusqueda = value;
                    NotifyPropertyChanged(nameof(TextoBusqueda));
                    NotifyPropertyChanged(nameof(HayFiltroBusqueda));
                }
            }
        }

        public bool HayFiltroBusqueda => !string.IsNullOrWhiteSpace(TextoBusqueda);

        public bool PuedeIrAnterior => PaginaActual > 1;
        public bool PuedeIrSiguiente => PaginaActual < TotalPaginas;

        public string TextoPaginacion => $"Página {PaginaActual} de {TotalPaginas}";
        public string TextoContador => $"Total: {TotalRegistros} {(TotalRegistros == 1 ? "local" : "locales")} {(HayFiltroBusqueda ? "encontrados" : "registrados")}";

        public ObservableCollection<Local> LocalesPaginados { get; set; }

        public LocalesWindow(LocalService localService, AppDbContext context)
        {
            InitializeComponent();
            _localService = localService;
            _context = context;

            LocalesPaginados = new ObservableCollection<Local>();

            DataContext = this;

            CalcularTotales();
            CargarPagina();
        }

        private void CalcularTotales()
        {
            TotalRegistros = _localService.ObtenerTotalLocalesFiltrados(TextoBusqueda);
            TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / REGISTROS_POR_PAGINA);

            if (TotalPaginas == 0) TotalPaginas = 1;
        }

        private void CargarPagina()
        {
            LocalesPaginados.Clear();

            var locales = _localService.BuscarLocales(TextoBusqueda)
                .Skip((PaginaActual - 1) * REGISTROS_POR_PAGINA)
                .Take(REGISTROS_POR_PAGINA)
                .ToList();

            foreach (var local in locales)
            {
                LocalesPaginados.Add(local);
            }
        }

        private void AgregarButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new LocalDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && dialog.FueGuardado)
            {
                var localCreado = _localService.AgregarLocal(dialog.NombreLocal);
                TextoBusqueda = string.Empty;
                BuscarTextBox.Text = string.Empty;
                PaginaActual = 1;
                CalcularTotales();
                CargarPagina();

                this.Hide();

                var mantenimientosWindow = new MantenimientosWindow(localCreado, _context);
                mantenimientosWindow.Owner = this;
                mantenimientosWindow.Closed += (s, args) =>
                {
                    this.Show();
                };
                mantenimientosWindow.Show();
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
                CargarPagina();
            }
        }

        private void EliminarButton_Click(object sender, RoutedEventArgs e)
        {
            Local? localSeleccionado = LocalesListBox.SelectedItem as Local;
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
                CalcularTotales();
                if (LocalesPaginados.Count == 1 && PaginaActual > 1)
                {
                    PaginaActual--;
                }
                else
                {
                    CargarPagina();
                }
            }
        }

        private void LocalesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LocalesListBox.SelectedItem is not Local localSeleccionado)
                return;
            this.Hide();

            var mantenimientosWindow = new MantenimientosWindow(localSeleccionado, _context);
            mantenimientosWindow.Owner = this;
            mantenimientosWindow.Closed += (s, args) =>
            {
                this.Show();
            };
            mantenimientosWindow.Show();
        }

        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BuscarButton_Click(object sender, RoutedEventArgs e)
        {
            TextoBusqueda = BuscarTextBox.Text?.Trim() ?? string.Empty;

            PaginaActual = 1;
            CalcularTotales();
            CargarPagina();
        }

        private void LimpiarBusquedaButton_Click(object sender, RoutedEventArgs e)
        {
            TextoBusqueda = string.Empty;
            BuscarTextBox.Text = string.Empty;
            PaginaActual = 1;
            CalcularTotales();
            CargarPagina();
        }

        private void PaginaAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (PuedeIrAnterior)
            {
                PaginaActual--;
            }
        }

        private void PaginaSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (PuedeIrSiguiente)
            {
                PaginaActual++;
            }
        }

        private void BuscarTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BuscarButton_Click(sender, e);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
