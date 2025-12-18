using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Models
{
    public class Mantenimiento
    {
        public int Id { get; set; }
        public int LocalId { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public int Mes { get; set; }
        public int Anio { get; set; }

        public decimal Costo { get; set; }
    }
}
