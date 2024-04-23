using H2OPure.Models;
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
    public partial class frmReportes : Page
    {
        UsersRepository usersRepository = new UsersRepository();

        public frmReportes()
        {
            InitializeComponent();
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable(), "Todos los usuarios");
        }

        private void UpdateDataGrid(IEnumerable<clsUser> users, string filterTitle)
        {
            dtg_Usuarios.ItemsSource = users.ToList();
            txt_FilterTitle.Text = filterTitle;
        }

        private void btn_FilterActiveUsers_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable().Where(u => u.isActive), "Usuarios activos");
        }

        private void btn_FilterInactiveUsers_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable().Where(u => !u.isActive), "Usuarios inactivos");
        }

        private void btn_FilterAdminUsers_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable().Where(u => u.typeUser == 1), "Usuarios administradores");
        }

        private void btn_FilterNormalUsers_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable().Where(u => u.typeUser == 1), "Usuarios normales");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable(), "Todos");

        }
    }
}