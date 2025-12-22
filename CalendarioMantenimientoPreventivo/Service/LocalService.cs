using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models;
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
            foreach (var local in _context.Locales)
                Locales.Add(local);
        }

        public void AgregarLocal(string nombre)
        {
            var local = new Local
            {
                Nombre = nombre
            };

            _context.Locales.Add(local);
            _context.SaveChanges();

            Locales.Add(local);
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
    }
}
