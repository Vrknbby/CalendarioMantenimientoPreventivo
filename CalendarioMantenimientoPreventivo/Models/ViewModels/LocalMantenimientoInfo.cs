using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Models.ViewModels
{
    public class LocalMantenimientoInfo : INotifyPropertyChanged
    {
        private string _nombreLocal = string.Empty;
        public string NombreLocal
        {
            get => _nombreLocal;
            set
            {
                if (_nombreLocal != value)
                {
                    _nombreLocal = value;
                    OnPropertyChanged(nameof(NombreLocal));
                    OnPropertyChanged(nameof(TextoDetalle));
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
                    OnPropertyChanged(nameof(TextoDetalle));
                }
            }
        }

        public string TextoDetalle
        {
            get
            {
                if (CantidadMantenimientos == 1)
                    return $"{NombreLocal}: 1 mant.";
                else
                    return $"{NombreLocal}: {CantidadMantenimientos} mant.";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
