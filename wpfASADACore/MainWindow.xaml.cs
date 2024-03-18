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
using wpfASADACore.Services;

namespace wpfASADACore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //_ = CrearUsuario();
        }

        

        static async Task CrearUsuario() {
            

            using (var db = new ContextDataBase()) {

                await db.Database.EnsureCreatedAsync();

                var usuario1 = new clsUser("Ulises","umongegds@gmail.com","123","uma","1234567");
            
                db.usuarios.Add(usuario1);

               await db.SaveChangesAsync();

            }
        
        }

        private void NavigationViewItem_Click(object sender, RoutedEventArgs e)
        {
            //RootNavigation.Navigate(typeof(View.frmUsuarios));
            RootNavigation.Navigate(typeof(Views.frmUsers));
        }

        private void AcercaDe_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}