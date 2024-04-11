using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace wpfASADACore.Utilities
{
    public class clsUtilities
    {

        public static bool EsCorreoValido(string email)
        {
            // Patrón de expresión regular para un correo electrónico
            const string regexPattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

            // Valida el correo electrónico con la expresión regular
            return Regex.IsMatch(email, regexPattern);
        }

        #region Metodo para mostrar Snackbar de Material Design
        public static async Task ShowSnackbarAsync(string message, SolidColorBrush backgroundColor, int delay = 2000)
        {
            // Obtén una referencia a tu MainWindow
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (mainWindow != null)
            {
                // Accede a tu SnackbarMessagePrincipal
                MaterialDesignThemes.Wpf.Snackbar snackbar = mainWindow.SnackbarMessageGlobal;
                snackbar.IsActive = true;
                snackbar.Message.Content = message;
                snackbar.Background = backgroundColor;
                snackbar.Foreground = new SolidColorBrush(Colors.Black);
                await Task.Delay(delay);
                snackbar.IsActive = false;
            }
            else
            {
                // Manejo de error cuando mainWindow es null
                Console.WriteLine("MainWindow es null");
            }
        }
        #endregion



    }
}
