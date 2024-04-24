using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using H2OPure.Repository;
using H2OPure.Services;
using H2OPure.Utilities;

namespace H2OPure.Views
{
    /// <summary>
    /// Lógica de interacción para LoginUsers.xaml
    /// </summary>
    public partial class LoginUsers : Window
    {
        //Declaración del objeto
        AutenticacionService autenticacionService;
        UsersRepository usersRepository = new UsersRepository();

        public LoginUsers()
        {
            InitializeComponent();
            //Instancias 
            autenticacionService = new AutenticacionService();
            lbl_In.Visibility = Visibility.Hidden;

        }

        //Evento Mouse_Down para permitir que la ventana se pueda mover
        private void Windows_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        //Evento Mouse_Down para permitir que la ventana se pueda mover
        private void btnMinimize_Click(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }
        //metodo para la tecla enter
        private void txt_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btn_Login_Click(sender, e);
            }
        }


        private async void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txt_Username.Text;
            string password = txt_Password.Password;

            var (isValid, userId, userType, isPasswordChangeRequired, errorMessage) = await usersRepository.ValidateUserLogin(username, password);

            if (isValid)
            {
                txt_Username.Background = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
                txb_AvisoLogInco.Visibility = Visibility.Hidden;
                // Muestra el ProgressBar
                DeterminateCircularProgress.IsIndeterminate = true;
                lbl_In.Visibility= Visibility.Visible;

                // Crea un Timer que se ejecuta después de 2 segundos
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                timer.Start();
                timer.Tick += (s, args) =>
                {
                    timer.Stop();

                    // Oculta el ProgressBar
                    DeterminateCircularProgress.IsIndeterminate = true;                   
                    // Guarda el ID del usuario y el tipo de usuario en la clase Utilities
                    Utilities.clsUtilities.UserIdLog = userId;
                    Utilities.clsUtilities.TypeUserLog = userType;
                    if (isPasswordChangeRequired)
                    {
                        msb_ChangePass msb_ChangePass = new msb_ChangePass();
                        msb_ChangePass.ShowDialog();
                        this.Hide();
                    }
                    // Otorga acceso a la aplicación
                    GrantAccess();
                    // Cierra la ventana de inicio de sesión
                    this.Close();
                };
                }
            else
            {
                txt_Username.Background = new SolidColorBrush(Color.FromArgb(30, 255, 0, 0));
                txb_AvisoLogInco.Visibility = Visibility.Visible;
                txt_Password.Password = "";
                txt_Username.Text = "";
                txt_Username.Focus();
                // Muestra el mensaje de error en el Label
                txb_AvisoLogInco.Text = errorMessage;
            }

        }
        public void GrantAccess()
        { 
            MainWindow main = new MainWindow();
            main.Show();
        
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(251, 120, 6));
            }
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                textBlock.Foreground = Brushes.DarkGray;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            var user = new NRecuperarPassword();

            var result = user.recoverPassword(txt_Username.Text);
            messageSnackbar(result);
            //MessageBox.Show(result);
        }

        public void messageSnackbar(string message)
        {
            snackbarMessage.MessageQueue.Enqueue(message);
            //snackbarMessage.Message.Content = message;
            snackbarMessage.IsActive = true;
        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            snackbarMessage.IsActive = false;
        }

    }
}
