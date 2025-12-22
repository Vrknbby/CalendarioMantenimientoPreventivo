using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Models.ViewModels
{
    public class NotificacionInfo
    {
        public int MantenimientoId { get; set; }
        public string NombreLocal { get; set; } = string.Empty;
        public string NombreMantenimiento { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaMantenimiento { get; set; }
        public int DiasRestantes { get; set; }
        public bool EsUrgente { get; set; }
        public string TipoNotificacion { get; set; } = string.Empty;
    }
}
