using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Service
{
    public class ParametroSistemaService
    {
        private readonly AppDbContext _context;
        public ParametroSistemaService(AppDbContext context)
        {
            _context = context;
        }

        public bool NotificacionesActivas()
        {
            var parametro = _context.ParametrosSistema
                .FirstOrDefault(p => p.Clave == "NotificacionesActivas");
            if (parametro == null)
                return true;

            return parametro.Valor == "true";
        }

        public void CambiarNotificaciones(bool activo)
        {
            var parametro = _context.ParametrosSistema
                .FirstOrDefault(p => p.Clave == "NotificacionesActivas");

            if (parametro == null)
            {
                parametro = new ParametroSistema
                {
                    Clave = "NotificacionesActivas",
                    Valor = activo ? "true" : "false"
                };
                _context.ParametrosSistema.Add(parametro);
            }
            else
            {
                parametro.Valor = activo ? "true" : "false";
            }

            _context.SaveChanges();
        }
    }
}
