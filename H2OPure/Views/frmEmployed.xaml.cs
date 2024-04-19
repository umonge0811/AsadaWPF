using H2OPure.Repository;
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

namespace H2OPure.Views
{
    /// <summary>
    /// Lógica de interacción para frmEmployed.xaml
    /// </summary>
    public partial class frmEmployed : Page


    {

        EmployedRepository employedRepository = new EmployedRepository();

        public frmEmployed()
        {
            InitializeComponent();
        }
    }
}
