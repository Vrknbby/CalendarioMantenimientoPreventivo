using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Models.ViewModels
{
    public class DiaInfo: INotifyPropertyChanged
    {
        private int _dia;
        public int Dia
        {
            get => _dia;
            set
            {
                if (_dia != value)
                {
                    _dia = value;
                    NotifyPropertyChanged(nameof(Dia));
                }
            }
        }

        private bool _esDelMesActual;
        public bool EsDelMesActual
        {
            get => _esDelMesActual;
            set
            {
                if (_esDelMesActual != value)
                {
                    _esDelMesActual = value;
                    NotifyPropertyChanged(nameof(EsDelMesActual));
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
                    NotifyPropertyChanged(nameof(TieneMantenimientos));
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
                    NotifyPropertyChanged(nameof(CantidadMantenimientos));
                    NotifyPropertyChanged(nameof(TextoMantenimientos));
                }
            }
        }

        private bool _esArrastrado;
        public bool EsArrastrado
        {
            get => _esArrastrado;
            set
            {
                if (_esArrastrado != value)
                {
                    _esArrastrado = value;
                    NotifyPropertyChanged(nameof(EsArrastrado));
                }
            }
        }

        private bool _esDestinoDrop;
        public bool EsDestinoDrop
        {
            get => _esDestinoDrop;
            set
            {
                _esDestinoDrop = value;
                NotifyPropertyChanged(nameof(EsDestinoDrop));
            }
        }

        public string TextoMantenimientos
        {
            get
            {
                if (!TieneMantenimientos || CantidadMantenimientos == 0)
                    return "";
                else if (CantidadMantenimientos == 1)
                    return "🔧 1 mant.";
                else
                    return $"🔧 {CantidadMantenimientos} mant.";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
