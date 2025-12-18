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
        public ObservableCollection<Local> Locales { get; } = new();

        public void AgregarLocal(string nombre)
        {
            Locales.Add(new Local
            {
                Id = Locales.Count + 1,
                Nombre = nombre
            });
        }

        public void EliminarLocal(Local local)
        {
            if (local != null)
                Locales.Remove(local);
        }

        public void ActualizarLocal(Local local, string nuevoNombre)
        {
            if (local != null)
                local.Nombre = nuevoNombre;
        }
    }
}
