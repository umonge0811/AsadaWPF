using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using H2OPure.Models;
using H2OPure.Repository;
using H2OPure.Services;
using H2OPure.Utilities;

namespace H2OPure.Views
{
    /// <summary>
    /// Lógica de interacción para frmUsers.xaml
    /// </summary>
    public partial class frmUsers : Page
    {
        //almacena el id del usuario buscado
        string? userName = null;

        private bool isModified = false;

        UsersRepository usersRepository = new UsersRepository();
        

        public frmUsers()
        {
            InitializeComponent();
            
            btn_DeleteUser.IsEnabled = false;
            btn_ModifyUser.IsEnabled = false;

            #region Simplificacion de la verificacion si el texto de usuarios cambia para habilitar el boton de modificar
            txt_NewName.TextChanged += Input_Changed;
            txt_NewId.TextChanged += Input_Changed;
            txt_NewUser.TextChanged += Input_Changed;
            txt_NewEmail.TextChanged += Input_Changed;
            #endregion
        }

        #region Metodos para deteccion de Cambios en los TextBox y en combobox para que se habilite el boton de Editar
        private void Input_Changed(object sender, EventArgs e)
        {
            if (isModified)
            {
                btn_ModifyUser.IsEnabled = true;
            }

        }
        #endregion

        #region Metodo para actualizar la Informacion despues de algun cambio
        //metodo para ejecutar el llebado del datagrid con los datos de los clientes
        private void loaddatagrid()
        {
            dtg_Usuarios.ItemsSource = null;
            dtg_Usuarios.ItemsSource =usersRepository.GetAllUser();
            dtg_Usuarios.Items.Refresh(); // Esta línea actualiza la vista del DataGrid

        }
        #endregion

        #region Metodo para dar doble Click en el datagrid y cargar los datos en los textbox
        //Metodo para seleccionar con doble click un cliente del datagrid y cargar los datos en los textbox
        private void dtgUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            clsUser user = (clsUser)dtg_Usuarios.SelectedItem;
            if (user != null)
            {
                txt_NewId.Text = user.DNI;
                txt_NewName.Text = user.Name;
                txt_NewUser.Text = user.UserName;
                txt_NewEmail.Text = user.Email;
                btn_DeleteUser.IsEnabled = true;
                btn_CreateNewUser.IsEnabled = false;
                userName = user.UserName;
                txt_NewPass.Visibility = Visibility.Hidden;
                txt_NewRePass.Visibility = Visibility.Hidden;
                isModified = true;
                lblPass.Visibility = Visibility.Visible;
                tglPass.Visibility = Visibility.Hidden;
                tglrePass.Visibility = Visibility.Hidden;

            } 
        }
        #endregion

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
            userName = null;
            txtBuscarUsuario.Clear();
            btn_DeleteUser.IsEnabled = false;
            btn_ModifyUser.IsEnabled = false;

            txt_VisiblePass.Clear();
            txt_VisibleRePass.Clear();
            txt_NewPass.Clear();
            txt_NewRePass.Clear();
            txt_VisiblePass.Visibility = Visibility.Hidden;
            txt_VisibleRePass.Visibility = Visibility.Hidden;
            txt_NewPass.Visibility = Visibility.Visible;
            txt_NewRePass.Visibility = Visibility.Visible;
            tglPass.IsChecked = false;
            tglrePass.IsChecked = false;

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
           


            if (await usersRepository.ValidatedUserRegister(newDNI))
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
                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.LightGreen));
                ClearAllData();
                loaddatagrid();
            }
            else
            {
                string message = $"Error al Crear usuario : {usersRepository.message} !";

                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.Red));

                //MessageBox.Show($"Error al registrar el usuario: {usersRepository.message}");
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
            userName = userFound.UserName;
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

            if (newDNI.Equals(""))
            {
                //MessageBox.Show("Deben digitar el numero de cedula");
                await clsUtilities.ShowSnackbarAsync("Debes digitar el número de cédula!", new SolidColorBrush(Colors.Yellow));
                txt_NewPass.Focus();
                return;
            }


           

            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await usersRepository.modifyUser(newName, newUser, newDNI, newEmail, userName);

            if (estado)
            {
                //MessageBox.Show("Usuario modificado con exito!!");
                await clsUtilities.ShowSnackbarAsync("Usuario modificado con Exito!", new SolidColorBrush(Colors.LightGreen));
                ClearAllData();
                loaddatagrid();
                btn_DeleteUser.IsEnabled = false;
                btn_ModifyUser.IsEnabled = false;
                btn_CreateNewUser.IsEnabled = true;
                isModified = false;
                txt_NewPass.Visibility = Visibility.Visible;
                txt_NewRePass.Visibility = Visibility.Visible;
                lblPass.Visibility = Visibility.Hidden;
                tglPass.Visibility = Visibility.Visible;
                tglrePass.Visibility = Visibility.Visible;


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
            // Mostrar un mensaje de confirmación para eliminar el cliente
            var confirmationBox = new clsMessageBox("¿Está seguro de eliminar el Usuario?", "Sí", "No", "AlertOctagonOutline", "Advertencia", Brushes.Yellow);
            var confirmationResult = confirmationBox.ShowDialog();
            if (confirmationResult != true)
            {
                return;
            }

            if (userName == null)
            {
                //MessageBox.Show("Debes buscar un usuario antes de eliminarlo");
                await clsUtilities.ShowSnackbarAsync("Debes buscar un usuario antes de eliminarlo!", new SolidColorBrush(Colors.Yellow));
                ClearAllData();
            }
            bool estado = await usersRepository.deleteUser(userName);

            if (estado)
            {
                //MessageBox.Show("Usuario eliminado con Exito!");
                await clsUtilities.ShowSnackbarAsync("Usuario ELIMINADO con Exito!", new SolidColorBrush(Colors.LightGreen));
                ClearAllData();
                loaddatagrid();
                btn_DeleteUser.IsEnabled = false;
                btn_ModifyUser.IsEnabled = false;
                btn_CreateNewUser.IsEnabled = true;
                txt_NewPass.Visibility = Visibility.Visible;
                txt_NewRePass.Visibility = Visibility.Visible;
                lblPass.Visibility = Visibility.Hidden;
                tglPass.Visibility = Visibility.Visible;
                tglrePass.Visibility = Visibility.Visible;


                isModified = false;
            }
            else
            {
                //MessageBox.Show($"Error al intentar eliminar el usuario: {usersRepository.message}");
                await clsUtilities.ShowSnackbarAsync($"Error al intentar eliminar el usuario: {usersRepository.message}", new SolidColorBrush(Colors.Red));


            }
        }
        #endregion

        #region evento load  para que se carguen datos al iniciar la pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loaddatagrid();
        }
        #endregion

        #region Metodo para Busqueda en el TextBox de Busqueda y predicciones en el datagrid
        //Crear metodo para que al digitar en el txtBuscarCli se vaya filtrando los datos en el datagrid
        private void txtBuscarUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtBuscarUsuario.Text;
            if (search != "")
            {
                dtg_Usuarios.ItemsSource = usersRepository.GetAllUsers().Where(c => c.DNI.ToLower().Contains(search.ToLower()) || c.Name.ToLower().Contains(search.ToLower()) || c.Email.ToLower().Contains(search.ToLower()) || c.UserName.ToLower().Contains(search.ToLower()));
            }
            else
            {
                dtg_Usuarios.ItemsSource = usersRepository.GetAllUsers();
            }
        }
        #endregion

        #region Metodo para mostrar la contraseña en el TextBox

        private void tglPass_Checked(object sender, RoutedEventArgs e)
        {
            txt_VisiblePass.Text = txt_NewPass.Password; // Copia el texto de la contraseña al TextBox
            txt_VisiblePass.Visibility = Visibility.Visible; // Muestra el TextBox
            txt_NewPass.Visibility = Visibility.Hidden; // Oculta el PasswordBox
        }

        private void tglPass_Unchecked(object sender, RoutedEventArgs e)
        {
            txt_NewPass.Visibility = Visibility.Visible; // Muestra el PasswordBox
            txt_VisiblePass.Visibility = Visibility.Hidden; // Oculta el TextBox
        }

        private void tglPass2_Checked(object sender, RoutedEventArgs e)
        {
            txt_VisibleRePass.Text = txt_NewRePass.Password; // Copia el texto de la contraseña al TextBox
            txt_VisibleRePass.Visibility = Visibility.Visible; // Muestra el TextBox
            txt_NewRePass.Visibility = Visibility.Hidden; // Oculta el PasswordBox
        }

        private void tglPass2_Unchecked(object sender, RoutedEventArgs e)
        {
            txt_NewRePass.Visibility = Visibility.Visible; // Muestra el PasswordBox
            txt_VisibleRePass.Visibility = Visibility.Hidden; // Oculta el TextBox
        }
        #endregion

    }



}
