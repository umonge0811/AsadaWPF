using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;
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

        private void btn_CleanTxt_Click(object sender, RoutedEventArgs e)
        {
            ClearAllData();

        }


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

        // async es para un motodo asyncrono 
        private async void btn_CreateNewUser_Click(object sender, RoutedEventArgs e)
        {
            string newName = txt_NewName.Text;
            string newEmail = txt_NewEmail.Text;
            string newUser = txt_NewUser.Text;
            string newPassword = txt_NewPass.Password;
            string newRepPassword = txt_NewRePass.Password;
            string newDNI = txt_NewId.Text;
            


            //Error first 
            if (newName.Equals(""))
            {
                MessageBox.Show("Debe de ingresar el nombre del usuario");
                txt_NewName.Focus();
                return;
            }

            if (!clsUtilities.EsCorreoValido(newEmail))
            {
                MessageBox.Show("Debe de ingresar un correo electronico valido");
                txt_NewEmail.Focus();
                return;
            }

            if (newPassword.Equals("") || newRepPassword.Equals(""))
            {
                MessageBox.Show("No debes dejar el campo contraseña vacio");
                txt_NewPass.Focus();
                return;
            }

            if (newPassword != newRepPassword)
            {
                MessageBox.Show("Las contraseñas no coinciden");
                txt_NewPass.Focus();
                return;
            }
            if (newDNI.Equals(""))
            {
                MessageBox.Show("Deben digitar el numero de cedula");
                txt_NewPass.Focus();
                return;
            }


            if (await ValidatedUserRegister(newDNI))
            {
                MessageBox.Show($"El usuario con cedula: {newDNI} ya se encuentra registrado... Verifique!!!!!");
                ClearAllData();
                return;
            }

            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await usersRepository.CreateUser(newName, newUser, newDNI, newPassword, newEmail);

            if (estado)
            {
                MessageBox.Show("Usuario registrado con exito!!");
                ClearAllData();
            }
            else
            {
                MessageBox.Show($"Error al registrar el usuario: {usersRepository.message}");
            }

        }




        //static async Task<bool> createUser(string name, string username, string dni, string password, string email)
        //{


        //    try
        //    {

        //        using (var db = new ContextDataBase())
        //        {

        //            //await db.Database.EnsureCreatedAsync();

        //            var usuario1 = new clsUser(name, email, password, username, dni);

        //            db.usuarios.Add(usuario1);

        //            await db.SaveChangesAsync();

        //            return true;

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return false;
        //    }

        //}

        // este es para cuando se esta creando un usuario nuevo, valide  antes de guardar que no exista uno ya con los datos suministrados

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

        //Es para buscar los usuarios en la Base de Datos
        private async void btnBuscarUsuario_Click(object sender, RoutedEventArgs e)
        {
            //variable para almacenar el parametro de busqueda que se realizara, en este caso se buscala con la Cedula
            string dni = txtBuscarUsuario.Text;

            //si en el txtBuscarUsuario.Text no se ha digitado ninguna cedula, ingresa aca y muestra que se debe ingresar
            if (string.IsNullOrWhiteSpace(dni))
            {
                MessageBox.Show("Debes colocar la cedula del usuario a buscar");
                return;
            }

            //aca se muestra un mensaje si el usuario no se encuentra registrado
            clsUser? userFound = await usersRepository.FindClientByDNI(dni);
            if (userFound == null)
            {

                MessageBox.Show("No existe un usuario con la cedula ingresada!!");
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

        ////este es el metodo que se creo para hacer la busqueda en la DB, retorna  un objeto de tipo clase (clsUser)
        //public async Task<clsUser?> FindClientByDNI(string dni) {

        //    //puede que retorne nula si no se encuentra nada
        //    clsUser? user = null;
        //    try
        //    {              

        //        using (var db = new ContextDataBase())
        //        {

        //            user = await db.usuarios.FirstOrDefaultAsync(u => u.DNI.Equals(dni));

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        user = null;

        //    }
        //    return user;

        //}

        private async void btn_ModifyUser_Click(object sender, RoutedEventArgs e)
        {
            string newName = txt_NewName.Text;
            string newEmail = txt_NewEmail.Text;
            string newUser = txt_NewUser.Text;
            string newPassword = txt_NewPass.Password;
            string newRepPassword = txt_NewRePass.Password;
            string newDNI = txt_NewId.Text;


            //Error first 
            if (newName.Equals(""))
            {
                MessageBox.Show("Debe de ingresar el nombre del usuario");
                txt_NewName.Focus();
                return;
            }

            if (!clsUtilities.EsCorreoValido(newEmail))
            {
                MessageBox.Show("Debe de ingresar un correo electronico valido");
                txt_NewEmail.Focus();
                return;
            }

            if (newPassword.Equals("") || newRepPassword.Equals(""))
            {
                MessageBox.Show("No debes dejar el campo contraseña vacio");
                txt_NewPass.Focus();
                return;
            }

            if (newPassword != newRepPassword)
            {
                MessageBox.Show("Las contraseñas no coinciden");
                txt_NewPass.Focus();
                return;
            }
            if (newDNI.Equals(""))
            {
                MessageBox.Show("Deben digitar el numero de cedula");
                txt_NewPass.Focus();
                return;
            }


           

            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await usersRepository.modifyUser(newName, newUser, newDNI, newPassword, newEmail, idUser);

            if (estado)
            {
                MessageBox.Show("Usuario modificado con exito!!");
                ClearAllData();
            }
            else
            {
                MessageBox.Show($"Error al modificar el usuario: {usersRepository.message}");
            }
        }


      

        private async void btn_DeleteUser_Click(object sender, RoutedEventArgs e)
        {

            if (idUser == null)
            {
                MessageBox.Show("Debes buscar un usuario antes de eliminarlo");
                ClearAllData();
            }
            bool estado = await usersRepository.deleteUser(idUser);

            if (estado)
            {
                MessageBox.Show("Usuario eliminado con Exito!");
                ClearAllData();
            }
            else
            {
                MessageBox.Show($"Error al intentar eliminar el usuario: {usersRepository.message}");

            }
        }
    }



}
