using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfASADACore.Models;
using wpfASADACore.Repository;
using wpfASADACore.Services;
using wpfASADACore.Utilities;

namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para frmUsers.xaml
    /// </summary>
    public partial class frmUsers : Page
    {
        //almacena el id del usuario buscado
        int? idUser = null;

        UsersRepository usersRepository = new UsersRepository();

        public frmUsers()
        {
            InitializeComponent();
            
            btn_DeleteUser.IsEnabled = false;
            btn_ModifyUser.IsEnabled = false;
        }

        #region evento click del boton de limpiar los campos de texto
        private void btn_CleanTxt_Click(object sender, RoutedEventArgs e)
        {
            ClearAllData();

        }
        #endregion

        #region metodo para limpiar los campos de texto
        private void ClearAllData()
        {
            txt_NewEmail.Clear();
            txt_NewId.Clear();
            txt_NewName.Clear();
            txt_NewPass.Clear();
            txt_NewRePass.Clear();
            txt_NewUser.Clear();
            idUser = null;
            txtBuscarUsuario.Clear();
            btn_DeleteUser.IsEnabled = false;
            btn_ModifyUser.IsEnabled = false;
        }
        #endregion

        #region Evento click del boton de crear un Usuario Nuevo
        // async es para un motodo asyncrono 
        private async void btn_CreateNewUser_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync();

            string newName = txt_NewName.Text;
            string newEmail = txt_NewEmail.Text;
            string newUser = txt_NewUser.Text;
            string newPassword = txt_NewPass.Password;
            string newRepPassword = txt_NewRePass.Password;
            string newDNI = txt_NewId.Text;
            


            //Error first 
            if (newName.Equals(""))
            {
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar el nombre del usuario!", new SolidColorBrush(Colors.Yellow));
                //MessageBox.Show("Debe de ingresar el nombre del usuario");
                txt_NewName.Focus();
                return;
            }
            if (newDNI.Equals(""))
            {
                //MessageBox.Show("Deben digitar el numero de cedula");
                await clsUtilities.ShowSnackbarAsync("Debes digitar el número de cédula!", new SolidColorBrush(Colors.Yellow));

                txt_NewId.Focus();
                return;
            }

            if (!clsUtilities.EsCorreoValido(newEmail))
            {
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar un correo electronico valido!", new SolidColorBrush(Colors.Yellow));

                //MessageBox.Show("Debe de ingresar un correo electronico valido");
                txt_NewEmail.Focus();
                return;
            }

            if (newUser.Equals(""))
            {
                //MessageBox.Show("Deben digitar el numero de cedula");
                await clsUtilities.ShowSnackbarAsync("Debes digitar un nombre de Usuario Valido!", new SolidColorBrush(Colors.Yellow));

                txt_NewUser.Focus();
                return;
            }


            if (newPassword.Equals("") || newRepPassword.Equals(""))
            {
                //MessageBox.Show("No debes dejar el campo contraseña vacio");
                await clsUtilities.ShowSnackbarAsync("No debes dejar el campo contraseña vacio!", new SolidColorBrush(Colors.Yellow));

                txt_NewPass.Focus();
                return;
            }

            if (newPassword != newRepPassword)
            {
                //MessageBox.Show("Las contraseñas no coinciden");
                await clsUtilities.ShowSnackbarAsync("Las contraseñas no coinciden!", new SolidColorBrush(Colors.Yellow));

                txt_NewPass.Focus();
                return;
            }
           


            if (await ValidatedUserRegister(newDNI))
            {
                string message = $"El usuario con cedula: {newDNI} ya se encuentra registrado... Verifique!";
                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.Red));
                ClearAllData();
                txt_NewId.Focus();
                return;
            }

            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await usersRepository.CreateUser(newName, newUser, newDNI, newPassword, newEmail);

            if (estado)
            {
                string message = $"El usuario : {newUser} fue registrado con Exito!";                
                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.Green));

                ClearAllData();
            }
            else
            {
                string message = $"Error al Crear usuario : {usersRepository.message} !";

                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.Red));

                //MessageBox.Show($"Error al registrar el usuario: {usersRepository.message}");
            }

        }
        #endregion

        #region metodo para validar si el usuario ya se encuentra registrado
        public async Task<bool> ValidatedUserRegister(string dni)
        {

            try
            {
                using (var db = new ContextDataBase())
                {
                    var userExists = await db.usuarios.AnyAsync(u => u.DNI == dni);
                    return userExists;
                }
            }
            catch (Exception ex)
            {
                // Manejar errores de conexión u otras excepciones
                Console.WriteLine("Error al validar usuario: " + ex.Message);
                throw; // Reenviar la excepción para un tratamiento superior
            }
        }
        #endregion

        #region Evento click del boton de Buscar Usuario
        //Es para buscar los usuarios en la Base de Datos
        private async void btnBuscarUsuario_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync();

            //variable para almacenar el parametro de busqueda que se realizara, en este caso se buscala con la Cedula
            string dni = txtBuscarUsuario.Text;

            //si en el txtBuscarUsuario.Text no se ha digitado ninguna cedula, ingresa aca y muestra que se debe ingresar
            if (string.IsNullOrWhiteSpace(dni))
            {
                //MessageBox.Show("Debes colocar la cedula del usuario a buscar");
                await clsUtilities.ShowSnackbarAsync("Debes colocar la cédula del Usuario a buscar!", new SolidColorBrush(Colors.Yellow));

                return;
            }

            //aca se muestra un mensaje si el usuario no se encuentra registrado
            clsUser? userFound = await usersRepository.FindClientByDNI(dni);
            if (userFound == null)
            {

                //MessageBox.Show("No existe un usuario con la cedula ingresada!!");
                await clsUtilities.ShowSnackbarAsync("No existe usuario con la cédula ingresada!", new SolidColorBrush(Colors.Yellow));

                return;
            }

            //si ninguna de las validaciones se cumplen entonces encuentra el usuario segun su DNI y carga los datos en los tXTBox

            txt_NewName.Text = userFound.Name;
            txt_NewEmail.Text = userFound.Email;
            txt_NewUser.Text = userFound.UserName;
            txt_NewId.Text = userFound.DNI;
            idUser = userFound.Id;
            btn_DeleteUser.IsEnabled = true;
            btn_ModifyUser.IsEnabled = true;

        }
        #endregion

        #region Evento click para editar el usuario
        private async void btn_ModifyUser_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync();

            string newName = txt_NewName.Text;
            string newEmail = txt_NewEmail.Text;
            string newUser = txt_NewUser.Text;
            string newPassword = txt_NewPass.Text;
            string newRepPassword = txt_NewRePass.Text;
            string newDNI = txt_NewId.Text;


            //Error first 
            if (newName.Equals(""))
            {
                //MessageBox.Show("Debe de ingresar el nombre del usuario");
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar el nombre del usuario!", new SolidColorBrush(Colors.Yellow));

                txt_NewName.Focus();
                return;
            }

            if (!clsUtilities.EsCorreoValido(newEmail))
            {
                //MessageBox.Show("Debe de ingresar un correo electronico valido");
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar un correo electronico valido!", new SolidColorBrush(Colors.Yellow));

                txt_NewEmail.Focus();
                return;
            }

            if (newPassword.Equals("") || newRepPassword.Equals(""))
            {
                //MessageBox.Show("No debes dejar el campo contraseña vacio");
                await clsUtilities.ShowSnackbarAsync("No debes dejar el campo contraseña vacio!", new SolidColorBrush(Colors.Yellow));

                txt_NewPass.Focus();
                return;
            }

            if (newPassword != newRepPassword)
            {
                //MessageBox.Show("Las contraseñas no coinciden");
                await clsUtilities.ShowSnackbarAsync("Las contraseñas no coinciden!", new SolidColorBrush(Colors.Yellow));

                txt_NewPass.Focus();
                return;
            }
            if (newDNI.Equals(""))
            {
                //MessageBox.Show("Deben digitar el numero de cedula");
                await clsUtilities.ShowSnackbarAsync("Debes digitar el número de cédula!", new SolidColorBrush(Colors.Yellow));
                txt_NewPass.Focus();
                return;
            }


           

            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await usersRepository.modifyUser(newName, newUser, newDNI, newPassword, newEmail, idUser);

            if (estado)
            {
                //MessageBox.Show("Usuario modificado con exito!!");
                await clsUtilities.ShowSnackbarAsync("Usuario modificado con Exito!", new SolidColorBrush(Colors.Green));
                ClearAllData();
            }
            else
            {
                //MessageBox.Show($"Error al modificar el usuario: {usersRepository.message}");
                await clsUtilities.ShowSnackbarAsync($"Error al modificar el usuario: {usersRepository.message}", new SolidColorBrush(Colors.Red));

            }
        }
        #endregion

        #region Evento click del boton Eliminar usuario
        private async void btn_DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync();

            if (idUser == null)
            {
                //MessageBox.Show("Debes buscar un usuario antes de eliminarlo");
                await clsUtilities.ShowSnackbarAsync("Debes buscar un usuario antes de eliminarlo!", new SolidColorBrush(Colors.Yellow));
                ClearAllData();
            }
            bool estado = await usersRepository.deleteUser(idUser);

            if (estado)
            {
                //MessageBox.Show("Usuario eliminado con Exito!");
                await clsUtilities.ShowSnackbarAsync("Usuario ELIMINADO con Exito!", new SolidColorBrush(Colors.Green));
                ClearAllData();
            }
            else
            {
                //MessageBox.Show($"Error al intentar eliminar el usuario: {usersRepository.message}");
                await clsUtilities.ShowSnackbarAsync($"Error al intentar eliminar el usuario: {usersRepository.message}", new SolidColorBrush(Colors.Red));


            }
        }
        #endregion
    }



}
