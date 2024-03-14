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
            _ = CrearUsuario();
        }

        static async Task CrearUsuario() {

            

            using (var db = new ContextDataBase()) {

                await db.Database.EnsureCreatedAsync();

                var usuario1 = new clsUser()
                {
                    Name = "Jeremy",
                    UserName = "seth",
                    DNI = "123",
                    Password = "123",
                    Email = "seth@seth.com"
                };

                db.usuarios.Add(usuario1);

               await db.SaveChangesAsync();

            }
        
        }
    }
}