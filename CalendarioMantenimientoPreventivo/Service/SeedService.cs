using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Service
{
    public class SeedService
    {
        private readonly AppDbContext _context;
        private readonly LocalService _localService;

        public SeedService(AppDbContext context, LocalService localService)
        {
            _context = context;
            _localService = localService;
        }
        public void SeedInitialData()
        {
            if (_context.Locales.Any())
            {
                return;
            }
            SeedLocalesYMantenimientos();
        }

        private void SeedLocalesYMantenimientos()
        {
            var local1 = CrearLocal("CISTERNA PRINCIPAL DE LA SEDE");
            CrearMantenimiento(local1, "MANTENIMIENTO DE SISTEMA DE BOMBEO", "INSPECCIÓN Y MANTENIMIENTO DE BOMBAS, TANQUE HIDRONEUMÁTICO, VÁLVULAS Y CONEXIONES SANITARIAS", 2026, 1, 550);
            CrearMantenimiento(local1, "MANTENIMIENTO DEL TABLERO ELÉCTRICO GENERAL Y CIRCUITOS", "INSPECCIÓN Y MANTENIMIENTO DE COMPONENTES DEL TABLERO DE BOMBAS (LIMPIEZA, AJUSTES), CONEXIONES ELÉCTRICAS A BOMBAS Y PUESTA A TIERRA", 2026, 1, 600);
            CrearMantenimiento(local1, "LIMPIEZA Y DESINFECCIÓN DE CISTERNA", "LIMPIEZA Y DESINFECCIÓN DE CISTERNA  QUE INCLUYA ELIMINACIÓN DE CONTAMINANTES Y AGENTES PATÓGENOS, CON PRODUCTO AUTORIZADO POR LA DIGESA. DEBE INCLUIR CERTIFICADO.", 2026, 1, 800);
            RecalcularDiasDelLocal(local1.Id, 2026, 1);

            var local2 = CrearLocal("CASA PROVINCIAL");
            CrearMantenimiento(local2, "MANTENIMIENTO DE SISTEMA DE BOMBEO Y TANQUE HIDRONEUMÁTICO", "INSPECCIÓN Y MANTENIMIENTO DE BOMBAS, VÁLVULAS Y CONEXIONES SANITARIAS", 2026, 7, 400);
            CrearMantenimiento(local2, "MANTENIMIENTO DEL TABLERO ELÉCTRICO DE BOMBAS Y CIRCUITOS", "INSPECCIÓN Y MANTENIMIENTO DE COMPONENTES DEL TABLERO DE BOMBAS (LIMPIEZA, AJUSTES), CONEXIONES ELÉCTRICAS A BOMBAS, CONTROL DE NIVEL Y PUESTA A TIERRA ", 2026, 7, 600);
            CrearMantenimiento(local2, "LIMPIEZA Y DESINFECCIÓN DE CISTERNA", "LIMPIEZA Y DESINFECCIÓN DE CISTERNA  QUE INCLUYA ELIMINACIÓN DE CONTAMINANTES Y AGENTES PATÓGENOS, CON PRODUCTO AUTORIZADO POR LA DIGESA. DEBE INCLUIR CERTIFICADO.", 2026, 7, 600);
            RecalcularDiasDelLocal(local2.Id, 2026, 7);

            CrearMantenimiento(local2, "LIMPIEZA Y DESINFECCIÓN DE TANQUE ELEVADO", "LIMPIEZA Y DESINFECCIÓN DE TANQUE ELEVADO QUE INCLUYA ELIMINACIÓN DE CONTAMINANTES Y AGENTES PATÓGENOS, CON PRODUCTO AUTORIZADO POR LA DIGESA. DEBE INCLUIR CERTIFICADO.", 2026, 12, 450);
            RecalcularDiasDelLocal(local2.Id, 2026, 12);

            var local3 = CrearLocal("CASA DE MAYORES");
            CrearMantenimiento(local3, "MANTENIMIENTO DE SISTEMA DE BOMBEO", "INSPECCIÓN Y MANTENIMIENTO DE BOMBAS, VÁLVULAS Y CONEXIONES SANITARIAS", 2026, 7, 400);
            CrearMantenimiento(local3, "MANTENIMIENTO DEL TABLERO ELÉCTRICO DE BOMBAS Y CIRCUITOS", "INSPECCIÓN Y MANTENIMIENTO DE COMPONENTES DEL TABLERO DE BOMBAS (LIMPIEZA, AJUSTES), CONEXIONES ELÉCTRICAS A BOMBAS, CONTROL DE NIVEL Y PUESTA A TIERRA ", 2026, 7, 600);
            CrearMantenimiento(local3, "LIMPIEZA Y DESINFECCIÓN DE CISTERNA Y TANQUE ELEVADO", "LIMPIEZA Y DESINFECCIÓN DE CISTERNA  QUE INCLUYA ELIMINACIÓN DE CONTAMINANTES Y AGENTES PATÓGENOS, CON PRODUCTO AUTORIZADO POR LA DIGESA. DEBE INCLUIR CERTIFICADO.", 2026, 7, 1150);
            RecalcularDiasDelLocal(local3.Id, 2026, 7);

            var local4 = CrearLocal("ENFERMERÍA NUEVA");
            CrearMantenimiento(local4, "MANTENIMIENTO DE SISTEMA DE BOMBEO", "INSPECCIÓN Y MANTENIMIENTO DE BOMBAS, VÁLVULAS Y CONEXIONES SANITARIAS", 2026, 10, 400);
            CrearMantenimiento(local4, "MANTENIMIENTO DEL TABLERO ELÉCTRICO DE BOMBAS Y CIRCUITOS", "INSPECCIÓN Y MANTENIMIENTO DE COMPONENTES DEL TABLERO DE BOMBAS (LIMPIEZA, AJUSTES), CONEXIONES ELÉCTRICAS A BOMBAS, CONTROL DE NIVEL Y PUESTA A TIERRA ", 2026, 10, 600);
            CrearMantenimiento(local4, "LIMPIEZA Y DESINFECCIÓN DE CISTERNA Y TANQUES ELEVADOS ROTOPLAS", "LIMPIEZA Y DESINFECCIÓN DE CISTERNA  QUE INCLUYA ELIMINACIÓN DE CONTAMINANTES Y AGENTES PATÓGENOS, CON PRODUCTO AUTORIZADO POR LA DIGESA. DEBE INCLUIR CERTIFICADO.", 2026, 10, 950);
            RecalcularDiasDelLocal(local4.Id, 2026, 10);

        }


        private Local CrearLocal(string nombre)
        {
            var local = _localService.AgregarLocal(nombre);
            return local;
        }

        private void CrearMantenimiento(Local local, string nombre, string descripcion, int anio, int mes, decimal costo)
        {
            var mantenimiento = new Mantenimiento
            {
                LocalId = local.Id,
                Nombre = nombre,
                Descripcion = descripcion,
                Anio = anio,
                Mes = mes,
                Dia = 1,
                Costo = costo
            };

            _context.Mantenimientos.Add(mantenimiento);
            _context.SaveChanges();

        }

        private void RecalcularDiasDelLocal(int localId, int anio, int mes)
        {
            var mantenimientos = _context.Mantenimientos
                .Where(m => m.LocalId == localId && m.Anio == anio && m.Mes == mes)
                .OrderBy(m => m.Id)
                .ToList();

            if (!mantenimientos.Any())
                return;

            var diasAsignados = CalcularDias(mantenimientos.Count, anio, mes);

            for (int i = 0; i < mantenimientos.Count; i++)
            {
                mantenimientos[i].Dia = diasAsignados[i];
            }

            _context.SaveChanges();
            Console.WriteLine($"    → Días asignados para {mes}/{anio}: {string.Join(", ", diasAsignados)}");
        }


        private List<int> CalcularDias(int cantidad, int anio, int mes)
        {
            int ultimoDia = DateTime.DaysInMonth(anio, mes);

            if (cantidad == 1)
                return new List<int> { 15 };

            if (cantidad == 2)
                return new List<int> { 15, ultimoDia };

            if (cantidad == 3)
                return new List<int> { 1, 15, ultimoDia };

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
    }
}
