using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace H2OPure.Utilities
{
    public class clsUtilities
    {
        //variable para almacenar el usuario logueado
        public static int UserIdLog = 0;
        //variable para almacenar el rol del usuario logueado
        public static int TypeUserLog = 0;

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
                // Accede a tu SnackbarMessagePrincipal MaterialDesignThemes.Wpf.Snackbar
                MaterialDesignThemes.Wpf.Snackbar snackbar = mainWindow.SnackbarMessageGlobal;

                // Actualiza el Snackbar en el subproceso principal
                Application.Current.Dispatcher.Invoke(() =>
                {
                    snackbar.IsActive = true;
                    snackbar.Message.FontSize = 60; // Establecer el tamaño de fuente directamente en el Snackbar
                    snackbar.Message.Content = message;
                    snackbar.Background = backgroundColor;
                    snackbar.Foreground = new SolidColorBrush(Colors.Black);
                });

                await Task.Delay(delay);

                // Actualiza el Snackbar en el subproceso principal
                Application.Current.Dispatcher.Invoke(() =>
                {
                    snackbar.IsActive = false;
                });
            }
            else
            {
                // Manejo de error cuando mainWindow es null
                Console.WriteLine("MainWindow es null");
            }
        }
        #endregion

        #region Metodo para mostrar la barra de progreso
        public static async Task ShowProgressBarAsync(int durationInMilliseconds = 2000)
        {
            // Obtén una referencia a tu MainWindow
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (mainWindow != null)
            {
                // Accede a tu ProgressBarPrincipal
                ProgressBar progressBar = mainWindow.GlobalProgressBar;
                progressBar.IsIndeterminate = true;
                progressBar.Visibility = Visibility.Visible;

                await Task.Delay(durationInMilliseconds);
                progressBar.Visibility = Visibility.Collapsed;
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
