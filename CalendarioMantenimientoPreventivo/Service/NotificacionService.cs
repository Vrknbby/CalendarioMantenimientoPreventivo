using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models;
using CalendarioMantenimientoPreventivo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Service
{
    public class NotificacionService
    {
        private readonly AppDbContext _context;

        public NotificacionService(AppDbContext context)
        {
            _context = context;
        }

        public List<string> ObtenerNotificacionesDelDia()
        {
            var mensajes = new List<string>();

            var parametro = _context.ParametrosSistema
                .FirstOrDefault(p => p.Clave == "NotificacionesActivas");

            if (parametro != null && parametro.Valor == "false")
                return mensajes;

            var hoy = DateTime.Today;

            var mantenimientos = _context.Mantenimientos
                .Where(m =>
                        m.Anio > hoy.Year ||
                        (m.Anio == hoy.Year && m.Mes > hoy.Month) ||
                        (m.Anio == hoy.Year && m.Mes == hoy.Month && m.Dia >= hoy.Day)
                    )
                .ToList();

            foreach (var m in mantenimientos)
            {
                var fechaMantenimiento = new DateTime(m.Anio, m.Mes, m.Dia);
                var diasFaltantes = (fechaMantenimiento - hoy).Days;

                if (diasFaltantes <= 7 && diasFaltantes > 0)
                {
                    if (!NotificacionYaMostrada(m.Id, TiposNotificacion.SemanaAntes))
                    {
                        mensajes.Add(
                            $"⚠ Mantenimiento programado para el {fechaMantenimiento:dd/MM/yyyy} " +
                            $"(faltan {diasFaltantes} días)");

                        RegistrarNotificacion(m.Id, TiposNotificacion.SemanaAntes);
                    }
                }

                if (diasFaltantes == 0)
                {
                    if (!NotificacionYaMostrada(m.Id, TiposNotificacion.DiaExacto))
                    {
                        mensajes.Add(
                            $"🚨 HOY corresponde el mantenimiento del local {m.LocalId}");

                        RegistrarNotificacion(m.Id, TiposNotificacion.DiaExacto);
                    }
                }
            }

            _context.SaveChanges();
            return mensajes;
        }

        private bool NotificacionYaMostrada(int mantenimientoId, string tipo)
        {
            return _context.MantenimientoNotificaciones.Any(n =>
                n.MantenimientoId == mantenimientoId &&
                n.Tipo == tipo);
        }

        private void RegistrarNotificacion(int mantenimientoId, string tipo)
        {
            _context.MantenimientoNotificaciones.Add(new MantenimientoNotificacion
            {
                MantenimientoId = mantenimientoId,
                Tipo = tipo,
                FechaMostrada = DateTime.Now
            });
        }
    }
}
