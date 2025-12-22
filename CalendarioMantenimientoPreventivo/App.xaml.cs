using CalendarioMantenimientoPreventivo.Data;
using CalendarioMantenimientoPreventivo.Service;
using CalendarioMantenimientoPreventivo.Views;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;

namespace CalendarioMantenimientoPreventivo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AppDbContext DbContext { get; private set; } = null!;
        public LocalService LocalService { get; private set; } = null!;
        public MantenimientoService mantenimientoService { get; private set; } = null!;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DbContext = new AppDbContext();
            DbContext.Database.Migrate();

            LocalService = new LocalService(DbContext);

            var mainWindow = new MainWindow(LocalService, DbContext);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DbContext.Dispose();
            base.OnExit(e);
        }
    }

}
