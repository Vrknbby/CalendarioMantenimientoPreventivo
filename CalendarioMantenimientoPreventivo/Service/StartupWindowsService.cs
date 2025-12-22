using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CalendarioMantenimientoPreventivo.Service
{
    public class StartupWindowsService
    {
        private const string RUN_KEY =
        @"Software\Microsoft\Windows\CurrentVersion\Run";

        private const string APP_NAME = "CalendarioMantenimientoPreventivo";

        public void Activar()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true);

            var exePath = Process.GetCurrentProcess().MainModule!.FileName!;
            var comando = $"\"{exePath}\" --startup";

            key.SetValue(APP_NAME, comando);
        }

        public void Desactivar()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true);

            if (key.GetValue(APP_NAME) != null)
                key.DeleteValue(APP_NAME);
        }

        public bool EstaRegistrado()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RUN_KEY);
            return key.GetValue(APP_NAME) != null;
        }
    }
}
