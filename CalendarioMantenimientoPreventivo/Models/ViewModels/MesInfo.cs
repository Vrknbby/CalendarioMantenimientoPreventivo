using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Models.ViewModels
{
    public class MesInfo: INotifyPropertyChanged
    {

        private int _numeroMes;
        public int NumeroMes
        {
            get => _numeroMes;
            set
            {
                if (_numeroMes != value)
                {
                    _numeroMes = value;
                    OnPropertyChanged(nameof(NumeroMes));
                }
            }
        }

        private string _nombreMes = string.Empty;
        public string NombreMes
        {
            get => _nombreMes;
            set
            {
                if (_nombreMes != value)
                {
                    _nombreMes = value;
                    OnPropertyChanged(nameof(NombreMes));
                }
            }
        }

        private int _cantidadMantenimientos;
        public int CantidadMantenimientos
        {
            get => _cantidadMantenimientos;
            set
            {
                if (_cantidadMantenimientos != value)
                {
                    _cantidadMantenimientos = value;
                    OnPropertyChanged(nameof(CantidadMantenimientos));
                    OnPropertyChanged(nameof(TextoMantenimientos));
                }
            }
        }

        private bool _tieneMantenimientos;
        public bool TieneMantenimientos
        {
            get => _tieneMantenimientos;
            set
            {
                if (_tieneMantenimientos != value)
                {
                    _tieneMantenimientos = value;
                    OnPropertyChanged(nameof(TieneMantenimientos));
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

        public string TextoMantenimientos
        {
            get
            {
                if (CantidadMantenimientos == 0)
                    return "—";
                else if (CantidadMantenimientos == 1)
                    return "1 mantenimiento";
                else
                    return $"{CantidadMantenimientos} mantenimientos";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
