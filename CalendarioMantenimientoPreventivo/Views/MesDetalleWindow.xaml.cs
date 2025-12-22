using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models;
using CalendarioMantenimientoPreventivo.Models.ViewModels;
using CalendarioMantenimientoPreventivo.Service;
using Microsoft.EntityFrameworkCore;
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
    /// Lógica de interacción para MesDetalleWindow.xaml
    /// </summary>
    public partial class MesDetalleWindow : Window, INotifyPropertyChanged
    {

        private readonly AppDbContext _context;
        private readonly MantenimientoService _servicio;
        private readonly int _anio;
        private readonly int _mes;
        private bool _hayDragActivo;
        public bool HayDragActivo
        {
            get => _hayDragActivo;
            set
            {
                if (_hayDragActivo != value)
                {
                    _hayDragActivo = value;
                    NotifyPropertyChanged(nameof(HayDragActivo));
                }
            }
        }

        private Point _dragStartPoint;
        private DiaInfo? _diaArrastrado;
        public DiaInfo? DiaArrastrado
        {
            get => _diaArrastrado;
            set
            {
                _diaArrastrado = value;
                NotifyPropertyChanged(nameof(DiaArrastrado));
            }
        }

        private string _mesAnioTexto;
        public string MesAnioTexto
        {
            get => _mesAnioTexto;
            set
            {
                if (_mesAnioTexto != value)
                {
                    _mesAnioTexto = value;
                    NotifyPropertyChanged(nameof(MesAnioTexto));
                }
            }
        }

        public ObservableCollection<DiaInfo> Dias { get; set; }

        public MesDetalleWindow(int anio, int mes, AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            _anio = anio;
            _mes = mes;

            Dias = new ObservableCollection<DiaInfo>();
            string[] nombresMeses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                                     "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            MesAnioTexto = $"{nombresMeses[mes - 1]} {anio}";

            CargarCalendario();
            _servicio = new MantenimientoService(_context, 0);
            DataContext = this;
        }

        private void CargarCalendario()
        {
            Dias.Clear();

            var mantenimientosDelMes = _context.Mantenimientos
                .Include(m => m.Local)
                .Where(m => m.Anio == _anio && m.Mes == _mes)
                .ToList();

            var mantenimientosPorDia = mantenimientosDelMes
                .GroupBy(m => m.Dia)
                .ToDictionary(g => g.Key, g => g.ToList());

            DateTime primerDiaMes = new DateTime(_anio, _mes, 1);
            int diasEnMes = DateTime.DaysInMonth(_anio, _mes);

            int diaSemana = (int)primerDiaMes.DayOfWeek;
            int diasMesAnterior = diaSemana == 0 ? 6 : diaSemana - 1;

            DateTime mesAnterior = primerDiaMes.AddMonths(-1);
            int diasEnMesAnterior = DateTime.DaysInMonth(mesAnterior.Year, mesAnterior.Month);

            for (int i = diasMesAnterior - diasMesAnterior + 1; i <= diasEnMesAnterior; i++)
            {
                if (Dias.Count >= diasMesAnterior) break;
                Dias.Insert(0, new DiaInfo
                {
                    Dia = diasEnMesAnterior - diasMesAnterior + Dias.Count + 1,
                    EsDelMesActual = false,
                    TieneMantenimientos = false,
                    CantidadMantenimientos = 0,
                    Mantenimientos = new ObservableCollection<Mantenimiento>()
                });
            }

            for (int dia = 1; dia <= diasEnMes; dia++)
            {
                var mantenimientosDelDia = mantenimientosPorDia.ContainsKey(dia)
                    ? mantenimientosPorDia[dia]
                    : new List<Mantenimiento>();

                Dias.Add(new DiaInfo
                {
                    Dia = dia,
                    EsDelMesActual = true,
                    TieneMantenimientos = mantenimientosDelDia.Count > 0,
                    CantidadMantenimientos = mantenimientosDelDia.Count,
                    Mantenimientos = new ObservableCollection<Mantenimiento>(mantenimientosDelDia)
                });
            }

            int diasAgregar = 42 - Dias.Count;
            for (int dia = 1; dia <= diasAgregar; dia++)
            {
                Dias.Add(new DiaInfo
                {
                    Dia = dia,
                    EsDelMesActual = false,
                    TieneMantenimientos = false,
                    CantidadMantenimientos = 0,
                    Mantenimientos = new ObservableCollection<Mantenimiento>()
                });
            }
        }

        public void ActualizarCalendario()
        {
            CargarCalendario();
        }

        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Dia_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is DiaInfo diaInfo)
            {
                if (diaInfo.EsDelMesActual && diaInfo.TieneMantenimientos)
                {
                    MessageBox.Show(
                        $"Día {diaInfo.Dia}: {diaInfo.CantidadMantenimientos} mantenimiento(s) programado(s)",
                        "Mantenimientos del Día",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Dia_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);

            if (sender is Border border && border.DataContext is DiaInfo dia)
            {
                _diaArrastrado = dia;
            }
        }

        private void Dia_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _diaArrastrado == null)
                return;

            Point position = e.GetPosition(null);

            if (Math.Abs(position.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (_diaArrastrado.EsDelMesActual && _diaArrastrado.TieneMantenimientos)
                {
                    var diaActual = _diaArrastrado;

                    HayDragActivo = true;
                    diaActual.EsArrastrado = true;

                    DragDrop.DoDragDrop(
                        (DependencyObject)sender,
                        diaActual,
                        DragDropEffects.Move);

                    diaActual.EsArrastrado = false;
                    HayDragActivo = false;
                }
            }
        }

        private void Dia_Drop(object sender, DragEventArgs e)
        {
            if (sender is not Border border)
                return;

            if (border.DataContext is not DiaInfo diaDestino)
                return;

            diaDestino.EsDestinoDrop = false;

            if (_diaArrastrado == null)
                return;

            if (!diaDestino.EsDelMesActual)
                return;

            if (diaDestino.Dia == _diaArrastrado.Dia)
                return;

            var mantenimiento = _context.Mantenimientos
                .FirstOrDefault(m => m.Anio == _anio
                                  && m.Mes == _mes
                                  && m.Dia == _diaArrastrado.Dia);

            if (mantenimiento != null)
            {
                _servicio.MoverMantenimiento(mantenimiento, diaDestino.Dia);
                ActualizarCalendario();
            }

            _diaArrastrado = null;
        }

        private void Dia_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border border &&
                border.DataContext is DiaInfo dia)
            {
                dia.EsDestinoDrop = false;
            }
        }

        private void Dia_DragEnter(object sender, DragEventArgs e)
        {
            if (_diaArrastrado == null)
                return;

            if (sender is Border border &&
                border.DataContext is DiaInfo diaDestino)
            {
                if (diaDestino.EsDelMesActual &&
                    diaDestino.Dia != _diaArrastrado.Dia)
                {
                    diaDestino.EsDestinoDrop = true;
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }
    }
}
