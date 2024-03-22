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
using wpfASADACore.Services;

namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para Page1.xaml
    /// </summary>
    public partial class Page1 : Page

        



    {
        public Page1()
        {
            InitializeComponent();
        }

        private async void btn_CreateNewUser_Click(object sender, RoutedEventArgs e)
        {
            //crear variables para almacenar los datos que digite el usuario en los textbox de clientes
            string name = txt_NewNameCli.Text;
            string firstName = txt_NewFirstNameCli.Text;
            string Surname = txt_NewsecondSurnameCli.Text;
            string DNI = txt_NewDNICli.Text;
            string SubscriberNum = txt_NewSubscribe.Text;
            string ClientType = null;

            //Error first 
            if (Name.Equals(""))
            {
                MessageBox.Show("Debe de ingresar el nombre del usuario");
                txt_NewNameCli.Focus();
                return;
            }
            if (firstName.Equals(""))
            {
                MessageBox.Show("Debe de ingresar el primer apellido del usuario");
                txt_NewFirstNameCli.Focus();
                return;
            }
            if (Surname.Equals(""))
            {
                MessageBox.Show("Debe de ingresar el segundo apellido del usuario");
                txt_NewsecondSurnameCli.Focus();
                return;
            }
            if (DNI.Equals(""))
            {
                MessageBox.Show("Debe de ingresar el numero de cedula del usuario");
                txt_NewDNICli.Focus();
                return;
            }   



            if (await ValidatedUserRegister(SubscriberNum))
            {
                MessageBox.Show($"El Abonado: {SubscriberNum} ya se encuentra registrado... Verifique!!!!!");
                ClearAllDaAta();
                return;
            }

            public async Task <bool> ValidatedUserRegister(string SubscriberNum)
            {

                try
                {
                    using (var db = new ContextDataBase())
                    {
                        var userExists = await db.usuarios.AnyAsync(u => u.SubscriberNum == subcribernum);
                        return userExists;
                    }
                }
                catch (Exception ex)
                {
                    // Manejar errores de conexión u otras excepciones
                    Console.WriteLine("Error al validar Cliente: " + ex.Message);
                    throw; // Reenviar la excepción para un tratamiento superior
                }
            }

            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await usersRepository.createUser(newName, newUser, newDNI, newPassword, newEmail);

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

            //CREAR UN IF PARA QUE SEGUN LO QUE SE ELIJA EN CMBOX SE ASIGNE UN VALOR A CLIE TYPE SI ELIJE Residencial = 0, Comercial = 1 local = 2
            if (cmb_TypeClient.SelectedIndex == 0)
            {
                ClientType = "0";
            }
            else if (cmb_TypeClient.SelectedIndex == 1)
            {
                ClientType = "1";
            }
            else if (cmb_TypeClient.SelectedIndex == 2)
            {
                ClientType = "2";
            }     

        }



        

        public void ClearAllData()
        {
            txt_NewNameCli.Clear();
            txt_NewFirstNameCli.Clear();
            txt_NewsecondSurnameCli.Clear();
            txt_NewDNICli.Clear();
            txt_NewSubscribe.Clear();
            cmb_TypeClient.SelectedIndex = 0;
        }

        private void btn_CleanTxt_Click(object sender, RoutedEventArgs e)
        {
            ClearAllData();
        }
    }
}
