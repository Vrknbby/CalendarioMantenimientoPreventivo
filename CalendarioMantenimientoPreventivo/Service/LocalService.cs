using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Service
{
    public class LocalService
    {
        private readonly AppDbContext _context;
        public ObservableCollection<Local> Locales { get; } = new();

        public LocalService(AppDbContext context)
        {
            _context = context;
            CargarLocales();
        }

        public void CargarLocales()
        {
            Locales.Clear();
            var locales = _context.Locales
                .OrderByDescending(l => l.FechaRegistro)
                .ToList();

            foreach (var local in locales)
                Locales.Add(local);
        }

        public Local AgregarLocal(string nombre)
        {
            var local = new Local
            {
                Nombre = nombre,
                FechaRegistro = DateTime.Now
            };

            _context.Locales.Add(local);
            _context.SaveChanges();

            Locales.Insert(0, local);

            return local;
        }

        public void EliminarLocal(Local local)
        {
            if (local == null) return;

            _context.Locales.Remove(local);
            _context.SaveChanges();

            Locales.Remove(local);
        }

        public void ActualizarLocal(Local local, string nuevoNombre)
        {
            if (local == null) return;

            local.Nombre = nuevoNombre;
            _context.SaveChanges();
        }

        public List<Local> BuscarLocales(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                return _context.Locales
                    .Include(l => l.Mantenimientos)
                    .OrderByDescending(l => l.FechaRegistro)
                    .ToList();
            }
            string busquedaPattern = $"%{textoBusqueda}%";

            return _context.Locales
                .Include(l => l.Mantenimientos)
                .Where(l => EF.Functions.Like(l.Nombre, busquedaPattern))
                .OrderByDescending(l => l.FechaRegistro)
                .ToList();
        }

        public int ObtenerTotalLocales()
        {
            return _context.Locales.Count();
        }

        public int ObtenerTotalLocalesFiltrados(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
                return ObtenerTotalLocales();

            return _context.Locales
                .Count(l => l.Nombre.Contains(textoBusqueda));
        }
    }
}
