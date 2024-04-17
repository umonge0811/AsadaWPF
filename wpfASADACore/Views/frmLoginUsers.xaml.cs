using System.Windows;
using System.Windows.Input;
using wpfASADACore.Services;

namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para LoginUsers.xaml
    /// </summary>
    public partial class LoginUsers : Window
    {
        //Declaración del objeto
        AutenticacionService autenticacionService;

        public LoginUsers()
        {
            InitializeComponent();
            //Instancias 
            autenticacionService = new AutenticacionService();  

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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
          

        }

        public void GrantAccess()
        { 
            MainWindow main = new MainWindow();
            main.Show();
        
        }
    }
}
