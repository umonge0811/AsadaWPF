using Microsoft.EntityFrameworkCore;
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
using wpfASADACore.Models;
using wpfASADACore.Repository;
using wpfASADACore.Services;

namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para frmClient.xaml
    /// </summary>
    public partial class frmClient : Page
    {
        ClientsRepository clientsRepository;
        TypeClientRepository typeClientRepository;

        public frmClient()
        {
            InitializeComponent();
            clientsRepository = new ClientsRepository();
            typeClientRepository = new TypeClientRepository();
        
        }
        

        public async Task<bool> ValidatedUserRegister(string Subscribernum)
        {

            try
            {
                using (var db = new ContextDataBase())
                {

                    var clientExists = await db.clientes.AnyAsync(u => u.SubscriberNum == Subscribernum);
                    return clientExists;
                }
            }
            catch (Exception ex)
            {
                // Manejar errores de conexión u otras excepciones
                Console.WriteLine("Error al validar Cliente: " + ex.Message);
                throw; // Reenviar la excepción para un tratamiento superior
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

        private void cargarDatos() {
            try {

                List<clsTypeClient> lista = typeClientRepository.GetAllTypeCliente();
                cmb_TypeClient.Items.Clear();   
                cmb_TypeClient.ItemsSource = lista; 
                cmb_TypeClient.DisplayMemberPath = "name";  
                cmb_TypeClient.SelectedValuePath = "id";
                cmb_TypeClient.SelectedIndex = 0;
            }
            catch (Exception e) {
               MessageBox.Show( e.Message.ToString());   
            }


        }

        private async void btn_CreateNewClient_Click(object sender, RoutedEventArgs e)
        {
            //crear variables para almacenar los datos que digite el usuario en los textbox de clientes
            string name = txt_NewNameCli.Text;
            string firstName = txt_NewFirstNameCli.Text;
            string Surname = txt_NewsecondSurnameCli.Text;
            string DNI = txt_NewDNICli.Text;
            string SubscriberNum = txt_NewSubscribe.Text;
            int ClientType = int.Parse(cmb_TypeClient.SelectedValue.ToString()??"1");

            //Error first 

            //if (await ValidatedUserRegister(SubscriberNum))
            //{
            //    MessageBox.Show($"El Abonado: {SubscriberNum} ya se encuentra registrado... Verifique!!!!!");
            //    ClearAllData();
            //    return;
            //}

            if (name.Equals(""))
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
            if (SubscriberNum.Equals(""))
            {
                {
                    //Crear una validacion de que si presiona si, le pregunte si desea asignar el numero de Cedula como numero de abonado, y si presiona no, que le permita ingresar el numero de abonado
                    if (MessageBox.Show("¿Desea asignar el numero de cedula como numero de abonado?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        txt_NewSubscribe.Text = txt_NewDNICli.Text;
                    }

                    else
                    {
                        txt_NewSubscribe.Text = "Ingrese Numero de Abonado";

                    }
                    return;
                }

            }

            //if (cmb_TypeClient.SelectedIndex == 0)
            //{
            //    ClientType = 0;
            //}
            //else if (cmb_TypeClient.SelectedIndex == 1)
            //{
            //    ClientType = 1;
            //}
            //else if (cmb_TypeClient.SelectedIndex == 2)
            //{
            //    ClientType = 2;
            //}



            //await es para esperar a que la tarea termine, en este caso, la funcion/ Metodo ejecute para que avance a la siguiente tarea

            bool estado = await clientsRepository.CreateClient(name, DNI, firstName, Surname , SubscriberNum, ClientType);
      
        
            if (estado)
            {
                MessageBox.Show("Usuario registrado con exito!!");
                ClearAllData();
            }
            else
            {
                MessageBox.Show($"Error al registrar el usuario: {clientsRepository.message}");
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            cargarDatos();

            dtgClientes.ItemsSource = clientsRepository.GetAllClients();
        }
    }
}
