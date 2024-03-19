using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para frmUsers.xaml
    /// </summary>
    public partial class frmUsers : Page
    {
        public frmUsers()
        {
            InitializeComponent();
        }

        private void btn_CleanTxt_Click(object sender, RoutedEventArgs e)
        {
            txt_NewEmail.Clear();
            txt_NewId.Clear();
            txt_NewName.Clear();
            txt_NewPass.Clear();
            txt_NewRePass.Clear();
            txt_NewUser.Clear();
            
        }
    }
}
