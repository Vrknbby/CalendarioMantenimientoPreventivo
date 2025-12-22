using CalendarioMantenimientoPreventivo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<Local> Locales { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=mantenimiento.db");
        }
    }
}
