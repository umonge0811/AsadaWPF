using H2OPure.Repository;
using H2OPure.Utilities;
using H2OPure.Views;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace H2OPure
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        clsUtilities utilities = new clsUtilities();
        TypeClientRepository typeclientRepository = new TypeClientRepository();
        public MainWindow()
        {

            InitializeComponent();           
            //_ = CrearUsuario();
            lbl_FechaPrincipal.Text = DateTime.Now.ToString("U");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Loaded += OnLoaded;

            // Crea un estilo personalizado para el Snackbar
            Style snackbarStyle = new Style(typeof(MaterialDesignThemes.Wpf.Snackbar));
            snackbarStyle.Setters.Add(new Setter(MaterialDesignThemes.Wpf.Snackbar.FontSizeProperty, 100.0));
            SnackbarMessageGlobal.Style = snackbarStyle;
        }


        // Propiedad pública para exponer tu ProgressBarPrincipal
        public ProgressBar GlobalProgressBar
        {
            get { return ProgressBarPrincipal; }
        }

        // Propiedad pública para exponer tu SnackbarMessagePrincipal
        public Snackbar SnackbarMessageGlobal
        {
            get { return SnackbarMessagePrincipal; }
        }


        private void NavigationViewItem_Click(object sender, RoutedEventArgs e)
        {
           
            //RootNavigation.Navigate(typeof(View.frmUsuarios));
            RootNavigation.Navigate(typeof(Views.frmUsers));

        }

        private void NavigationViewItem_Click_1(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(Views.frmClient));

        }

        private async void NavigationViewItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (Utilities.clsUtilities.TypeUserLog != 1)
            {
                await clsUtilities.ShowSnackbarAsync("Acceso restringido, unicamente Administradores", new SolidColorBrush(Colors.Yellow));

                return;

            }
            else
            { 
                RootNavigation.Navigate(typeof(Views.TypeClient));
            }
        }


        private void NavigationViewItem_Click_3(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(Views.frmPay));
        }

        private void nvLecturas_Click(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(Views.frmLectura));
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Crear los clientes por defecto
            await typeclientRepository.CreateTypeClient("Local", "Cliente tarifa editable", 0, 0);
            await typeclientRepository.CreateTypeClient("Residencial", "", 3200, 200);
            await typeclientRepository.CreateTypeClient("Comercial", "", 3200, 400);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //RootNavigation.Navigate(typeof(View.frmUsuarios));
            RootNavigation.Navigate(typeof(Views.frmInicio));
            

        }

        private async void NavigationViewItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (Utilities.clsUtilities.TypeUserLog != 1)
            {
                await clsUtilities.ShowSnackbarAsync("Acceso restringido, unicamente Administradores", new SolidColorBrush(Colors.Yellow));
                
                return;

            }
            else
            {
                RootNavigation.Navigate(typeof(Views.frmUsers));
            }

        }

        private void NavigationViewItem_Click_5(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(Views.frmInventary));

        }

        private void NavigationViewItem_Click_6(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(Views.frmAcercade));

        }

        private async void NavigationViewItem_Click_7(object sender, RoutedEventArgs e)
        {
            if (Utilities.clsUtilities.TypeUserLog != 1)
            {
                await clsUtilities.ShowSnackbarAsync("Acceso restringido, unicamente Administradores", new SolidColorBrush(Colors.Yellow));

                return;

            }
            else
            {
                RootNavigation.Navigate(typeof(Views.frmReportes));
            }

                

        }

        private void NavigationViewItem_Click_8(object sender, RoutedEventArgs e)
        {
            LoginUsers loginUsers = new LoginUsers();
            loginUsers.Show();  
            this.Close();
        }
    }
}