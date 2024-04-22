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
using System.Windows.Shapes;

namespace H2OPure.Utilities
{
    /// <summary>
    /// Lógica de interacción para msb_ChangePass.xaml
    /// </summary>
    public partial class msb_ChangePass : Window
    {
        UsersRepository usersRepository = new UsersRepository();

        public msb_ChangePass()
        {
            InitializeComponent();
        }


        #region Metodo para mostrar la contraseña en el TextBox

        private void tglPass_Checked(object sender, RoutedEventArgs e)
        {
            txt_VisibleAdminPass.Text = txt_NewAdminPass.Password; // Copia el texto de la contraseña al TextBox
            txt_VisibleAdminPass.Visibility = Visibility.Visible; // Muestra el TextBox
            txt_NewAdminPass.Visibility = Visibility.Hidden; // Oculta el PasswordBox
        }

        private void tglPass_Unchecked(object sender, RoutedEventArgs e)
        {
            txt_NewAdminPass.Visibility = Visibility.Visible; // Muestra el PasswordBox
            txt_VisibleAdminPass.Visibility = Visibility.Hidden; // Oculta el TextBox
        }

        private void tglPass2_Checked(object sender, RoutedEventArgs e)
        {
            txt_VisibleAdminRePass.Text = txt_NewAdminRePass.Password; // Copia el texto de la contraseña al TextBox
            txt_VisibleAdminRePass.Visibility = Visibility.Visible; // Muestra el TextBox
            txt_NewAdminRePass.Visibility = Visibility.Hidden; // Oculta el PasswordBox
        }

        private void tglPass2_Unchecked(object sender, RoutedEventArgs e)
        {
            txt_NewAdminRePass.Visibility = Visibility.Visible; // Muestra el PasswordBox
            txt_VisibleAdminRePass.Visibility = Visibility.Hidden; // Oculta el TextBox
        }

        #endregion


        private async void btn_ChangePassAdmin_Click(object sender, RoutedEventArgs e)
        {

            // Obtener la nueva contraseña del TextBox
            string newPassword = txt_NewAdminPass.Password;

            //VALIDACIONES
            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Por favor, ingrese una contraseña");
                return;
            }

            //if (newPassword.Length < 8)
            //{
            //    MessageBox.Show("La contraseña debe tener al menos 8 caracteres");
            //    return;
            //}

            if (newPassword != txt_NewAdminRePass.Password)
            {
                MessageBox.Show("Las contraseñas no coinciden");
                return;
            }

            // Cambiar la contraseña del usuario
            bool result = await usersRepository.ChangeUserPassword(Utilities.clsUtilities.UserIdLog, newPassword);
            if (result)
            {
               
                this.Close();
                //MessageBox.Show("Contraseña cambiada exitosamente");
            }
            else
            {
                MessageBox.Show("Error al cambiar la contraseña");
            }
        }


    }
}
