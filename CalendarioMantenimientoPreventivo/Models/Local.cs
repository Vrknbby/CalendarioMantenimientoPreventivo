using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Models
{
    public class Local : INotifyPropertyChanged
    {
        private string _nombre = string.Empty;

        public int Id { get; set; }

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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
    }
}
