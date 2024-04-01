using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        //Se almacena ell id del cliente buscado
        int? idClient = null;

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
            idClient = null;
            txt_NewSubscribe.Clear();
            cmb_TypeClient.SelectedIndex = 0;
        }

        private void btn_CleanTxt_Click(object sender, RoutedEventArgs e)
        {
            ClearAllData();
        }


        //Este Metodo es para cargar los datos de los tipos de clientes en el combobox
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

        //btn_CreateNewClient_Click es para crear un nuevo cliente
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




            //await es para esperar a que la tarea termine, en este caso, la funcion/ Metodo ejecute para que avance a la siguiente tarea

            bool estado = await clientsRepository.CreateClient(name, DNI, firstName, Surname , SubscriberNum, ClientType);
      
        
            if (estado)
            {
                MessageBox.Show("Usuario registrado con exito!!");
                ClearAllData();
                loaddatagrid();
                
            }
            else
            {
                MessageBox.Show($"Error al registrar el usuario: {clientsRepository.message}");
            }

        }

        //metodo para ejecutar el llebado del datagrid con los datos de los clientes
        private void loaddatagrid()
        {
            dtgClientes.ItemsSource = null;
            dtgClientes.ItemsSource = clientsRepository.GetAllClients();
            dtgClientes.Items.Refresh(); // Esta línea actualiza la vista del DataGrid

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            cargarDatos();

            loaddatagrid();
            btn_ModifyClient.IsEnabled = false;
            btn_DeleteClient.IsEnabled = false;
        }

        //Metodo para seleccionar con doble click un cliente del datagrid y cargar los datos en los textbox
        private void dtgClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            clsCliente client = (clsCliente)dtgClientes.SelectedItem;
            if (client != null)
            {
                txt_NewNameCli.Text = client.name;
                txt_NewFirstNameCli.Text = client.lastName;
                txt_NewsecondSurnameCli.Text = client.secondSurname;
                txt_NewDNICli.Text = client.DNI;
                txt_NewSubscribe.Text = client.SubscriberNum;
                cmb_TypeClient.SelectedValue = client.TypeClientId;
                btn_CreateNewClient.IsEnabled = false;
                btn_ModifyClient.IsEnabled = true;
                btn_DeleteClient.IsEnabled = true;
            }
        }

       

        //Metodo para el btn_ModifyClient_Click para modificar un cliente
        private async void btn_ModifyClient_Click(object sender, RoutedEventArgs e)

        {
            //crear variables para almacenar los datos que digite el usuario en los textbox de clientes

            string name = txt_NewNameCli.Text;
            string lastName = txt_NewFirstNameCli.Text;
            string Surname = txt_NewsecondSurnameCli.Text;
            string DNI = txt_NewDNICli.Text;
            string SubscriberNum = txt_NewSubscribe.Text;
            int ClientType = int.Parse(cmb_TypeClient.SelectedValue.ToString() ?? "1");


            //aca se muestra un mensaje si el usuario no se encuentra registrado
            clsCliente? userFound = await clientsRepository.FindClientBySubscriberNum(SubscriberNum);
            if (userFound == null)
            {

                MessageBox.Show("No existe un usuario con la cedula ingresada!!");
                return;
            }

            //si ninguna de las validaciones se cumplen entonces encuentra el id cliente segun su Numero de Abonado y carga los datos en los tXTBox

            //Guardo el id del cliente encontrado
            idClient = userFound.id;            

            //Error first       

            if (name.Equals(""))
            {
                MessageBox.Show("Debe de ingresar el nombre del usuario");
                txt_NewNameCli.Focus();
                return;
            }
            if (lastName.Equals(""))
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



            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await clientsRepository.UpdateClient(idClient, name,DNI, lastName,Surname, SubscriberNum,ClientType);

            if (estado)
            {
                MessageBox.Show("Cliente modificado con exito!!");
                ClearAllData();
                //Recargar los datos del datagrid
                loaddatagrid();

            }
            else
            {
                MessageBox.Show($"Error al modificar el Cliente: {clientsRepository.message}");
            }

            



        }


         


    }
}
