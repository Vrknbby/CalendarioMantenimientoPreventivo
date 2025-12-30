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
        public ParametroSistemaService ParametroSistemaService { get; private set; } = null!;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                SQLitePCL.Batteries_V2.Init();

                bool inicioConWindows = e.Args.Contains("--startup");

                DbContext = new AppDbContext();
                DbContext.Database.Migrate();

                LocalService = new LocalService(DbContext);
                ParametroSistemaService = new ParametroSistemaService(DbContext);

                var seedService = new SeedService(DbContext, LocalService);
                seedService.SeedInitialData();

                var notificacionService = new NotificacionService(DbContext);
                var notificaciones = notificacionService.ObtenerNotificacionesDelDia();

                if (inicioConWindows)
                {
                    if (notificaciones.Any())
                    {
                        var notificacionesView = new NotificacionesView(notificaciones);
                        notificacionesView.ShowDialog();
                    }

                    Shutdown();
                    return;
                }

                var mainWindow = new MainWindow(LocalService, DbContext, ParametroSistemaService);
                mainWindow.Show();

                if (notificaciones.Any())
                {
                    var notificacionesView = new NotificacionesView(notificaciones);
                    notificacionesView.Owner = mainWindow;
                    notificacionesView.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar la aplicación: " + ex.Message);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DbContext.Dispose();
            base.OnExit(e);
        }
    }

}
