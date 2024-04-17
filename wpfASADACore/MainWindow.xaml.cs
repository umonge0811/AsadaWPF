using MaterialDesignThemes.Wpf;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wpfASADACore.Models;
using wpfASADACore.Repository;
using wpfASADACore.Services;


namespace wpfASADACore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        TypeClientRepository typeclientRepository = new TypeClientRepository();
        public MainWindow()
        {

            InitializeComponent();           
            //_ = CrearUsuario();
            lbl_FechaPrincipal.Text = DateTime.Now.ToString("U");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Loaded += OnLoaded;
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

        private void NavigationViewItem_Click_2(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(Views.TypeClient));
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
    }
}