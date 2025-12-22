using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models;
using CalendarioMantenimientoPreventivo.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
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

        public List<NotificacionInfo> ObtenerNotificacionesDelDia()
        {
            var notificaciones = new List<NotificacionInfo>();
            var hoy = DateTime.Today;

            var mantenimientos = _context.Mantenimientos
                .Include(m => m.Local)
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
                        notificaciones.Add(new NotificacionInfo
                        {
                            MantenimientoId = m.Id,
                            NombreLocal = m.Local.Nombre,
                            NombreMantenimiento = m.Nombre,
                            Descripcion = m.Descripcion,
                            FechaMantenimiento = fechaMantenimiento,
                            DiasRestantes = diasFaltantes,
                            EsUrgente = false,
                            TipoNotificacion = "Próximamente"
                        });

                        RegistrarNotificacion(m.Id, TiposNotificacion.SemanaAntes);
                    }
                }

                if (diasFaltantes == 0)
                {
                    if (!NotificacionYaMostrada(m.Id, TiposNotificacion.DiaExacto))
                    {
                        notificaciones.Add(new NotificacionInfo
                        {
                            MantenimientoId = m.Id,
                            NombreLocal = m.Local.Nombre,
                            NombreMantenimiento = m.Nombre,
                            Descripcion = m.Descripcion,
                            FechaMantenimiento = fechaMantenimiento,
                            DiasRestantes = 0,
                            EsUrgente = true,
                            TipoNotificacion = "¡HOY!"
                        });

                        RegistrarNotificacion(m.Id, TiposNotificacion.DiaExacto);
                    }
                }
            }

            _context.SaveChanges();
            var notificacionesOrdenadas = notificaciones
                .OrderByDescending(n => n.EsUrgente)           
                .ThenBy(n => n.FechaMantenimiento)      
                .ToList();
            return notificaciones;
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
