using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Models
{
    public class MantenimientoNotificacion
    {
        public int Id { get; set; }
        public int MantenimientoId { get; set; }
        public string Tipo { get; set; } = null!;

        public DateTime FechaMostrada { get; set; }
        public Mantenimiento Mantenimiento { get; set; } = null!;
    }
}
