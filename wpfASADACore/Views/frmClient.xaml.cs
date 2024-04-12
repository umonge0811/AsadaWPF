using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using wpfASADACore.Models;
using wpfASADACore.Repository;
using wpfASADACore.Services;
using wpfASADACore.Utilities;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para frmClient.xaml
    /// </summary>
    public partial class frmClient : Page
    {
        //Se almacena ell id del cliente buscado
        int? idClient = null;




        //Variable para detectar si se ha cargado informacion de un cliente despues de hacer doble click en el datagrid
        private bool isModified = false;

        ClientsRepository clientsRepository;
        TypeClientRepository typeClientRepository;

        public frmClient()
        {
            InitializeComponent();
            clientsRepository = new ClientsRepository();
            typeClientRepository = new TypeClientRepository();

            #region Simplificacion de la verificacion si el texto cambia para habilitar el boton de modificar
            txt_NewNameCli.TextChanged += Input_Changed;
            txt_NewFirstNameCli.TextChanged += Input_Changed;
            txt_NewsecondSurnameCli.TextChanged += Input_Changed;
            txt_NewDNICli.TextChanged += Input_Changed;
            txt_NewSubscribe.TextChanged += Input_Changed;
            cmb_TypeClient.SelectionChanged += Input_Changed;
            #endregion

        }

       

        #region Metodo para validar el numero de abonado
        public async Task<bool> ValidatedClientRegister(string Subscribernum)
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
        #endregion

        #region Metodo para limpiar los textbox
        public void ClearAllData()
        {
            txt_NewNameCli.Clear();
            txt_NewFirstNameCli.Clear();
            txt_NewsecondSurnameCli.Clear();
            txt_NewDNICli.Clear();
            idClient = null;
            txt_NewSubscribe.Clear();
            cmb_TypeClient.SelectedIndex = 0;
            btn_CreateNewClient.IsEnabled = true;
            btn_ModifyClient.IsEnabled = false;
            btn_DeleteClient.IsEnabled = false;
        }
        #endregion

        #region Evento para limpiar los textbox
        private void btn_CleanTxt_Click(object sender, RoutedEventArgs e)
        {
            ClearAllData();
        }
        #endregion

        #region Metodo para cargar los datos de los tipos de clientes en el combobox
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
        #endregion

        #region Metodo para crear los Clientes con el Boton btn_CreateNewClient_Click
        //btn_CreateNewClient_Click es para crear un nuevo cliente
        private async void btn_CreateNewClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mostrar barra de progreso
                await clsUtilities.ShowProgressBarAsync();


                // Variables para almacenar datos del usuario
                string name = txt_NewNameCli.Text;
                string firstName = txt_NewFirstNameCli.Text;
                string Surname = txt_NewsecondSurnameCli.Text;
                string DNI = txt_NewDNICli.Text;
                string SubscriberNum = txt_NewSubscribe.Text;
                int ClientType = int.Parse(cmb_TypeClient.SelectedValue.ToString() ?? "1");

                // Validación de campos
                // Obtén una referencia a tu MainWindow
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

                if (mainWindow != null)
                {
                    //// Accede a tu SnackbarMessagePrincipal
                    //MaterialDesignThemes.Wpf.Snackbar snackbar = mainWindow.SnackbarMessageGlobal;
                    //MaterialDesignThemes.Wpf.SnackbarMessage snackbarMessage = new MaterialDesignThemes.Wpf.SnackbarMessage();


                    if (string.IsNullOrEmpty(name))
                    {

                        await clsUtilities.ShowSnackbarAsync("Debe Ingresar el nombre del usuario!", new SolidColorBrush(Colors.Yellow));
                        txt_NewNameCli.Focus();
                        return;

                    }
                    if (string.IsNullOrEmpty(firstName))
                    {
                        await clsUtilities.ShowSnackbarAsync("Debe Ingresar el primer apellido del usuario", new SolidColorBrush(Colors.Yellow));
                        txt_NewNameCli.Focus();
                        return;
                    }
                    if (string.IsNullOrEmpty(Surname))
                    {
                        await clsUtilities.ShowSnackbarAsync("Debe Ingresar el segundo apellido del usuario", new SolidColorBrush(Colors.Yellow));
                        txt_NewNameCli.Focus();
                        return;

                    }
                    if (string.IsNullOrEmpty(DNI))
                    {
                        await clsUtilities.ShowSnackbarAsync("Debe Ingresar el numero de cédula del usuario", new SolidColorBrush(Colors.Yellow));
                        txt_NewNameCli.Focus();
                        return;
                    }
                    if (string.IsNullOrEmpty(SubscriberNum))
                    {
                        await clsUtilities.ShowSnackbarAsync("Ingrese el número de abonado o active el Switch para asignar la cédula como número de abonado", new SolidColorBrush(Colors.Yellow));
                        txt_NewNameCli.Focus();
                        return;

                    }
                }
                else
                {
                    // Manejo de error cuando mainWindow es null
                    Console.WriteLine("MainWindow es null");
                }


                // Crear cliente
                bool estado = await clientsRepository.CreateClient(name, DNI, firstName, Surname, SubscriberNum, ClientType);

                
                // Mostrar mensaje de éxito o error
                if (estado)
                {
                    await clsUtilities.ShowSnackbarAsync("Cliente creado exitosamente", new SolidColorBrush(Colors.LightGreen));
                    ClearAllData();
                    loaddatagrid();
                }
                else
                {
                    string errorMessage = $"Error al crear cliente: {clientsRepository.message}";
                    await clsUtilities.ShowSnackbarAsync(errorMessage, new SolidColorBrush(Colors.Red));
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error inesperado: {ex.Message}";
                await clsUtilities.ShowSnackbarAsync(errorMessage, new SolidColorBrush(Colors.Red));
            }
        }
        #endregion

        #region Metodo para actualizar la Informacion despues de algun cambio
        //metodo para ejecutar el llebado del datagrid con los datos de los clientes
        private void loaddatagrid()
        {
            dtgClientes.ItemsSource = null;
            dtgClientes.ItemsSource = clientsRepository.GetAllClients();
            dtgClientes.Items.Refresh(); // Esta línea actualiza la vista del DataGrid

        }
        #endregion

        #region Evento Load Para que se ejecute al cargar la pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            cargarDatos();

            loaddatagrid();
            btn_ModifyClient.IsEnabled = false;
            btn_DeleteClient.IsEnabled = false;
            //Hacer que el toggleswitch este desactivado
            copySwitch.IsEnabled = false;
        }
        #endregion

        #region Metodo para dar doble Click en el datagrid y cargar los datos en los textbox
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
                btn_DeleteClient.IsEnabled = true;
                isModified = true;
            }
        }
        #endregion

        #region Modificar la Información del Cliente
        //Metodo para el btn_ModifyClient_Click para modificar un cliente
        private async void btn_ModifyClient_Click(object sender, RoutedEventArgs e)

        {
            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync();
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
                await clsUtilities.ShowSnackbarAsync("No existe Cliente con el Número de Abonado ingresado!!!", new SolidColorBrush(Colors.Yellow));


                //MessageBox.Show("No existe Cliente con el Número de Abonado ingresado!!");
                return;
            }

            //si ninguna de las validaciones se cumplen entonces encuentra el id cliente segun su Numero de Abonado y carga los datos en los tXTBox

            //Guardo el id del cliente encontrado
            idClient = userFound.id;            

            //Error first       

            if (name.Equals(""))
            {
                //MessageBox.Show("Debe de ingresar el nombre del usuario");
                txt_NewNameCli.Focus();
                return;
            }
            if (lastName.Equals(""))
            {
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar el Primer Apellido del Cliente a Modificar!", new SolidColorBrush(Colors.Yellow));

                //MessageBox.Show("Debe de ingresar el primer apellido del usuario");
                txt_NewFirstNameCli.Focus();
                return;
            }
            if (Surname.Equals(""))
            {
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar el Segundo Apellido del Cliente a Modificar!", new SolidColorBrush(Colors.Yellow));
                //MessageBox.Show("Debe de ingresar el segundo apellido del usuario");
                txt_NewsecondSurnameCli.Focus();
                return;
            }
            if (DNI.Equals(""))
            {
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar el Numero de Cédula del Cliente a Modificar!", new SolidColorBrush(Colors.Yellow));
                //MessageBox.Show("Debe de ingresar el numero de cedula del usuario");
                txt_NewDNICli.Focus();
                return;
            }
            



            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await clientsRepository.UpdateClient(idClient, name,DNI, lastName,Surname, SubscriberNum,ClientType);

            if (estado)
            {
                // Utiliza tu MessageBox personalizado
                var messageBox = new clsMessageBox("Cliente modificado con exito!!", "OK", "CANCEL", "CheckCircleOutline", "Éxito", Brushes.Green);
                messageBox.ShowDialog();

                ClearAllData();
                //Recargar los datos del datagrid
                loaddatagrid();
                btn_DeleteClient.IsEnabled = false;
                btn_ModifyClient.IsEnabled = false;
                btn_CreateNewClient.IsEnabled = true;
                isModified = false;
            }
            else
            {
                // Utiliza tu MessageBox personalizado
                var messageBox = new clsMessageBox($"Error al modificar el Cliente: {clientsRepository.message}", "OK", "", "AlertOutline", "Error", Brushes.Red);
                messageBox.ShowDialog();
            }

        }
        #endregion

        #region Eliminar Cliente
        private async void btn_DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync();
            // Mostrar un mensaje de confirmación para eliminar el cliente
            var confirmationBox = new clsMessageBox("¿Está seguro de eliminar el cliente?", "Sí", "No", "AlertOctagonOutline","Advertencia", Brushes.Yellow);
            var confirmationResult = confirmationBox.ShowDialog();
            if (confirmationResult != true)
            {
                return;
            }

            string SubscriberNum = txt_NewSubscribe.Text;

            // Aquí se muestra un mensaje si el usuario no se encuentra registrado
            clsCliente? userFound = await clientsRepository.FindClientBySubscriberNum(SubscriberNum);
            if (userFound == null)
            {
                await clsUtilities.ShowSnackbarAsync("No existe Cliente con el Número de Abonado ingresado!!!", new SolidColorBrush(Colors.Red));
                return;
            }

            //si ninguna de las validaciones se cumplen entonces encuentra el id cliente segun su Numero de Abonado y carga los datos en los tXTBox

            //Guardo el id del cliente encontrado
            idClient = userFound.id;


            if (idClient == null)
            {
                await clsUtilities.ShowSnackbarAsync("Debes buscar un usuario antes de eliminarlo!", new SolidColorBrush(Colors.Yellow));
                //MessageBox.Show("Debes buscar un usuario antes de eliminarlo");
                ClearAllData();
            }
            bool estado = await clientsRepository.deleteClient(idClient);

            if (estado)
            {
                string message = $" Cliente ELIMINADO con Exito!";
                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.Green));
                ClearAllData();
                loaddatagrid();
                btn_DeleteClient.IsEnabled = false;
                btn_ModifyClient.IsEnabled = false;
                btn_CreateNewClient.IsEnabled = true;
                isModified = false;
                return;
            }
            else
            {
                string message = $"Error Intentar ELIMINAR el cliente : {clientsRepository.message} !";

                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.Red));
                //MessageBox.Show($"Error al intentar eliminar el usuario: {clientsRepository.message}");

            }

        }
        #endregion

        #region Metodos para deteccion de Cambios en los TextBox y en combobox para que se habilite el boton de Editar
        private void Input_Changed(object sender, EventArgs e)
        {
            if (isModified)
            {
                btn_ModifyClient.IsEnabled = true;
            }

            if (sender == txt_NewDNICli)
            {
                copySwitch.IsEnabled = !string.IsNullOrEmpty(txt_NewDNICli.Text);
            }

            if (sender == cmb_TypeClient)
            {
            }
               
        }


        
        #endregion

        #region Metodo para Busqueda en el TextBox de Busqueda y predicciones en el datagrid
        //Crear metodo para que al digitar en el txtBuscarCli se vaya filtrando los datos en el datagrid
        private void txtBuscarCli_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtBuscarCli.Text;
            if (search != "")
            {
                dtgClientes.ItemsSource = clientsRepository.GetAllClients().Where(c => c.name.ToLower().Contains(search.ToLower()) || c.lastName.ToLower().Contains(search.ToLower()) || c.secondSurname.ToLower().Contains(search.ToLower()) || c.DNI.ToLower().Contains(search.ToLower()) || c.SubscriberNum.ToLower().Contains(search.ToLower())).ToList();
            }
            else
            {
                dtgClientes.ItemsSource = clientsRepository.GetAllClients();
            }
        }

        #endregion

        #region Metodo para copiar el DNI en el Numero de Abonado
        private void copySwitch_Checked(object sender, RoutedEventArgs e)
        {
            // Copiar el texto
            txt_NewSubscribe.Text = txt_NewDNICli.Text;

        }

        private void copySwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            txt_NewSubscribe.Clear();
        }
        #endregion

       

    }


}
