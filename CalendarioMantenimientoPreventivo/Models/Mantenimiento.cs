using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Models
{
    public class Mantenimiento : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public int LocalId { get; set; }
        public Local Local { get; set; } = null!;

        private string _nombre = string.Empty;
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }

        private string _descripcion = string.Empty;
        public string Descripcion
        {
            get => _descripcion;
            set
            {
                if (_descripcion != value)
                {
                    _descripcion = value;
                    OnPropertyChanged(nameof(Descripcion));
                }
            }
        }

        private int _dia;
        public int Dia
        {
            get => _dia;
            set
            {
                if (_dia != value)
                {
                    _dia = value;
                    OnPropertyChanged(nameof(Dia));
                }
            }
        }

        private int _mes;
        public int Mes
        {
            get => _mes;
            set
            {
                if (_mes != value)
                {
                    _mes = value;
                    OnPropertyChanged(nameof(Mes));
                }
            }
        }

        private int _anio;
        public int Anio
        {
            get => _anio;
            set
            {
                if (_anio != value)
                {
                    _anio = value;
                    OnPropertyChanged(nameof(Anio));
                }
            }
        }

        private decimal _costo;
        public decimal Costo
        {
            get => _costo;
            set
            {
                if (_costo != value)
                {
                    _costo = value;
                    OnPropertyChanged(nameof(Costo));
                }
            }
        }

        public ICollection<MantenimientoNotificacion> Notificaciones { get; set; }
        = new List<MantenimientoNotificacion>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
