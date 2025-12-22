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
        public DbSet<MantenimientoNotificacion> MantenimientoNotificaciones { get; set; }
        public DbSet<ParametroSistema> ParametrosSistema { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=mantenimiento.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MantenimientoNotificacion>()
                .HasIndex(e => new { e.MantenimientoId, e.Tipo })
                .IsUnique();

            modelBuilder.Entity<ParametroSistema>()
                .HasKey(p => p.Clave);

            modelBuilder.Entity<ParametroSistema>().HasData(
                new ParametroSistema
                {
                    Clave = "NotificacionesActivas",
                    Valor = "true"
                }
            );
        }
    }
}
