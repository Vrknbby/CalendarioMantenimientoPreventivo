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
    public class MantenimientoService
    {
        private readonly AppDbContext _context;
        private readonly int _localId;

        public ObservableCollection<Mantenimiento> Mantenimientos { get; }= new();

        public MantenimientoService(AppDbContext context, int localId)
        {
            _context = context;
            _localId = localId;
            CargarMantenimientos();
        }

        public void CargarMantenimientos()
        {
            Mantenimientos.Clear();
            var mantenimientos = _context.Mantenimientos.Where(m => m.LocalId == _localId)
                            .OrderByDescending(m => m.Anio)
                            .ThenByDescending(m => m.Mes)
                            .ToList();
            foreach (var mantenimiento in mantenimientos)
                Mantenimientos.Add(mantenimiento);
        }

        public void Agregar(string nombre, string descripcion, int mes, int anio, decimal costo)
        {
            var mantenimiento = new Mantenimiento
            {
                LocalId = _localId,
                Nombre = nombre,
                Descripcion = descripcion,
                Mes = mes,
                Anio = anio,
                Costo = costo
            };

            _context.Mantenimientos.Add(mantenimiento);
            _context.SaveChanges();
            RecalcularDias(_localId, anio, mes);

            var mantenimientoActualizado = _context.Mantenimientos
                .First(m => m.Id == mantenimiento.Id);

            Mantenimientos.Add(mantenimientoActualizado);
        }

        public void Actualizar(Mantenimiento mantenimiento, string nombre, string descripcion, int mes, int anio, decimal costo)
        {
            if (mantenimiento == null) return;

            int mesAnterior = mantenimiento.Mes;
            int anioAnterior = mantenimiento.Anio;

            mantenimiento.Nombre = nombre;
            mantenimiento.Descripcion = descripcion;
            mantenimiento.Mes = mes;
            mantenimiento.Anio = anio;
            mantenimiento.Costo = costo;

            _context.SaveChanges();
            RecalcularDias(_localId, anioAnterior, mesAnterior);
            RecalcularDias(_localId, anio, mes);
        }

        public void Eliminar(Mantenimiento mantenimiento)
        {
            if (mantenimiento == null) return;

            int mes = mantenimiento.Mes;
            int anio = mantenimiento.Anio;

            _context.Mantenimientos.Remove(mantenimiento);
            _context.SaveChanges();

            RecalcularDias(_localId, anio, mes);

            Mantenimientos.Remove(mantenimiento);
        }

        public List<Mantenimiento> ObtenerPorMes(int anio, int mes)
        {
            return _context.Mantenimientos
            .Where(m => m.LocalId == _localId
                     && m.Anio == anio
                     && m.Mes == mes)
            .OrderBy(m => m.Dia)
            .ToList();
        }

        public List<Mantenimiento> ObtenerPorAnio(int anio)
        {
            return _context.Mantenimientos
                .Where(m => m.LocalId == _localId && m.Anio == anio)
                .OrderBy(m => m.Mes)
                .ThenBy(m => m.Dia)
                .ToList();
        }

        private List<int> CalcularDias(int cantidad, int anio, int mes)
        {
            int ultimoDia = DateTime.DaysInMonth(anio, mes);

            if (cantidad == 1)
                return new() { 15 };

            if (cantidad == 2)
                return new() { 15, ultimoDia };

            if (cantidad == 3)
                return new() { 1, 15, ultimoDia };

            var dias = new List<int>();
            for (int i = 0; i < cantidad; i++)
            {
                int dia = (int)Math.Round(
                    1 + i * (ultimoDia - 1.0) / (cantidad - 1)
                );
                dias.Add(dia);
            }

            return dias;
        }

        public void RecalcularDias(int localId, int anio, int mes)
        {
            var mantenimientos = _context.Mantenimientos
                .Where(m => m.LocalId == localId
                         && m.Anio == anio
                         && m.Mes == mes)
                .OrderBy(m => m.Id)
                .ToList();

            var diasAsignados = CalcularDias(mantenimientos.Count, anio, mes);

            for (int i = 0; i < mantenimientos.Count; i++)
            {
                mantenimientos[i].Dia = diasAsignados[i];
            }

            _context.SaveChanges();
        }

        public void MoverMantenimiento(Mantenimiento mantenimientoOrigen, int diaDestino)
        {
            if (mantenimientoOrigen == null)
                return;

            int diaOrigen = mantenimientoOrigen.Dia;
            if (diaOrigen == diaDestino)
                return;

            var mantenimientoDestino = _context.Mantenimientos.FirstOrDefault(m =>
                m.Id != mantenimientoOrigen.Id &&
                m.LocalId == mantenimientoOrigen.LocalId &&
                m.Anio == mantenimientoOrigen.Anio &&
                m.Mes == mantenimientoOrigen.Mes &&
                m.Dia == diaDestino
            );

            if (mantenimientoDestino == null)
            {
                mantenimientoOrigen.Dia = diaDestino;
                _context.SaveChanges();
                return;
            }

            int diaTemporal = mantenimientoDestino.Dia;
            mantenimientoDestino.Dia = diaOrigen; 
            mantenimientoOrigen.Dia = diaTemporal; 

            _context.SaveChanges();
        }
    }
}
