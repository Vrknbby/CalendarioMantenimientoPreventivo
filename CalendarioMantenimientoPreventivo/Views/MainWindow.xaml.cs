using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models.ViewModels;
using CalendarioMantenimientoPreventivo.Service;
using CalendarioMantenimientoPreventivo.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Cryptography.Xml;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly LocalService _localService;
        private readonly AppDbContext _context;
        private readonly MantenimientoService _mantenimientoService;
        private readonly ParametroSistemaService _parametroService;
        private readonly StartupWindowsService _startupService;

        private int _anioSeleccionado;
        public int AnioSeleccionado
        {
            get => _anioSeleccionado;
            set
            {
                if (_anioSeleccionado != value)
                {
                    _anioSeleccionado = value;
                    NotifyPropertyChanged(nameof(AnioSeleccionado));
                    CargarCalendario();
                }
            }
        }

        private bool _iniciarConWindows;
        public bool IniciarConWindows
        {
            get => _iniciarConWindows;
            set
            {
                if (_iniciarConWindows != value)
                {
                    _iniciarConWindows = value;
                    NotifyPropertyChanged(nameof(IniciarConWindows));

                    _parametroService.CambiarNotificaciones(value);

                    if (value)
                        _startupService.Activar();
                    else
                        _startupService.Desactivar();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<MesInfo> Meses { get; set; }

        public MainWindow(LocalService localService, AppDbContext context, ParametroSistemaService parametroService)
        {
            InitializeComponent();
            _localService = localService;
            _context = context;
            _mantenimientoService = new MantenimientoService(_context, 0);
            _startupService = new StartupWindowsService();
            _parametroService = parametroService;

            Meses = new ObservableCollection<MesInfo>();
            AnioSeleccionado = DateTime.Now.Year;

            DataContext = this;
            CargarParametros();

        }

        private void CargarParametros()
        {
            IniciarConWindows = _parametroService.NotificacionesActivas();
        }

        private void CargarCalendario()
        {
            Meses.Clear();

            var mantenimientosPorMesYLocal = _context.Mantenimientos
                .Include(m => m.Local)
                .Where(m => m.Anio == AnioSeleccionado)
                .GroupBy(m => m.Mes)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(m => m.Local)
                          .Select(lg => new { Local = lg.Key, Cantidad = lg.Count() })
                          .ToList()
                );

            string[] nombresMeses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                                     "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

            for (int i = 1; i <= 12; i++)
            {
                var detalleLocales = new ObservableCollection<LocalMantenimientoInfo>();
                int totalMantenimientos = 0;

                if (mantenimientosPorMesYLocal.ContainsKey(i))
                {
                    var localesDelMes = mantenimientosPorMesYLocal[i];
                    foreach (var localInfo in localesDelMes)
                    {
                        detalleLocales.Add(new LocalMantenimientoInfo
                        {
                            NombreLocal = localInfo.Local.Nombre,
                            CantidadMantenimientos = localInfo.Cantidad
                        });
                        totalMantenimientos += localInfo.Cantidad;
                    }
                }

                Meses.Add(new MesInfo
                {
                    NumeroMes = i,
                    NombreMes = nombresMeses[i - 1],
                    CantidadMantenimientos = totalMantenimientos,
                    TieneMantenimientos = totalMantenimientos > 0,
                    Anio = AnioSeleccionado,
                    DetalleLocales = detalleLocales
                });
            }
        }

        public void ActualizarCalendario()
        {
            CargarCalendario();
        }


        private void AbrirLocalesWindow_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();

            var localesWindow = new LocalesWindow(_localService, _context);
            localesWindow.Owner = this;
            localesWindow.Closed += (s, args) =>
            {
                ActualizarCalendario();
                this.Show();
            };
            localesWindow.Show();
        }

        private void AnteriorAnio_Click(object sender, RoutedEventArgs e)
        {
            AnioSeleccionado--;
        }

        private void SiguienteAnio_Click(object sender, RoutedEventArgs e)
        {
            AnioSeleccionado++;
        }

        private void AnioTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && int.TryParse(textBox.Text, out int anio))
            {
                if (anio >= 1900 && anio <= 2100)
                {
                    AnioSeleccionado = anio;
                }
                else
                {
                    textBox.Text = AnioSeleccionado.ToString();
                }
            }
        }

        private void Mes_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.DataContext is MesInfo mesInfo)
            {
                this.Hide();

                var mesDetalleWindow = new MesDetalleWindow(mesInfo.Anio, mesInfo.NumeroMes, _context);
                mesDetalleWindow.Owner = this;
                mesDetalleWindow.Closed += (s, args) =>
                {
                    ActualizarCalendario();
                    this.Show();
                };
                mesDetalleWindow.Show();
            }
        }

        private void AyudaButton_Click(object sender, RoutedEventArgs e)
        {
            var ayudaWindow = new AyudaWindow();
            ayudaWindow.Owner = this;
            ayudaWindow.ShowDialog();
        }
    }
}