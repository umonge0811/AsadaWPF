using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using wpfASADACore.Models;
using wpfASADACore.Repository;
using wpfASADACore.Services;
using wpfASADACore.Utilities;

namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para TypeClient.xaml
    /// </summary>
    public partial class TypeClient : Page
    {
        int idTypeClient = 0;
        bool isModified = false;
        TypeClientRepository typeClientRepository;
        public TypeClient()
        {
            InitializeComponent();
            typeClientRepository = new TypeClientRepository();
            //btn_ModifyTypeCli.IsEnabled = false;
            //btn_DeleteTypeCli.IsEnabled = false;

        }


        #region Metodo para cargar los datos de la tabla de tipos de clientes en el data grid
        private void CargarDatosTypeClient()
        {
            var allTypeClients = typeClientRepository.GetAllTypeCliente();
            dgvTypeClient.ItemsSource = allTypeClients;
        }
        #endregion

        #region Evento LOAD de la pagina
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatosTypeClient();
   
        }
        #endregion

        //#region Evento doble Click en el datagrid y cargar los datos en los textbox
        ////Metodo para seleccionar con doble click un cliente del datagrid y cargar los datos en los textbox
        //private void dgvTypeClient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    clsTypeClient typeclient = (clsTypeClient)dgvTypeClient.SelectedItem;
        //    if (typeclient != null)
        //    {
        //        txt_NewTyCli.Text = typeclient.name;
        //        txt_NewDescriptionType.Text = typeclient.description;
        //        txt_NewRateBaseType.Text = typeclient.rate.ToString();
        //        txt_NewRateExcType.Text = typeclient.rateExc.ToString();
        //        idTypeClient = typeclient.id;
        //        btn_CreateNewTypeCli.IsEnabled = false;
        //        //btn_ModifyTypeCli.IsEnabled = true;
        //        //btn_DeleteTypeCli.IsEnabled = true;
        //        CargarDatosTypeClient();
        //        isModified = true;
        //    }
        //}
        //#endregion

        #region Metodo que llamar el btn de limpiar los TextBox
        //funcion para limpirar todos los TextBlox 
        private void ClearAllDataTypeCli()
        {
            txt_NewTyCli.Clear();
            txt_NewDescriptionType.Clear();
            txt_NewRateBaseType.Clear();
            txt_NewRateExcType.Clear();
            //btn_ModifyTypeCli.IsEnabled = false;
            //btn_DeleteTypeCli.IsEnabled = false;
            isModified = false;
            CargarDatosTypeClient();


        }
        #endregion

        #region Metodo para validar si el tipo de cliente ya existe
        public async Task<bool> ValidatedTypeCliRegister(string NewtypeClient)
        {

            try
            {
                using (var db = new ContextDataBase())
                {
                    var TypeCliExists = await db.typeClients.AnyAsync(u => u.name == NewtypeClient);
                    return TypeCliExists;
                }
            }
            catch (Exception ex)
            {
                // Manejar errores de conexión u otras excepciones
                Console.WriteLine("Error al validar: " + ex.Message);
                throw; // Reenviar la excepción para un tratamiento superior
            }
        }
        #endregion

        #region Evento click para crear un nuevo tipo de cliente
        private async void btn_CreateNewTypeCli_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync(1000);

            string NewtypeClient = txt_NewTyCli.Text;
            string NewDescription = txt_NewDescriptionType.Text;

            // Validación de campos vacíos
            if (string.IsNullOrEmpty(NewtypeClient) && string.IsNullOrEmpty(NewDescription) && string.IsNullOrEmpty(txt_NewRateBaseType.Text) && string.IsNullOrEmpty(txt_NewRateExcType.Text))
            {
                await clsUtilities.ShowSnackbarAsync("No hay ningún dato cargado!", new SolidColorBrush(Colors.Yellow));
                return;
            }

            
            double RateBase = Convert.ToDouble(txt_NewRateBaseType.Text);
            double RateExtra = Convert.ToDouble(txt_NewRateExcType.Text);

            //Error first 
           
            if (NewtypeClient.Equals(""))
            {
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar el nombre del tipo de Cliente Valido!", new SolidColorBrush(Colors.Yellow));

                //MessageBox.Show("Debe de ingresar el nombre del tipo de Cliente Valido");
                txt_NewTyCli.Focus();
                return;
            }            

            if (RateBase.Equals(""))
            {
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar la tarifa base del tipo de Cliente Valido!", new SolidColorBrush(Colors.Yellow));

                //MessageBox.Show("Debe de ingresar la tarifa base del tipo de Cliente Valido");
                txt_NewRateBaseType.Focus();
                return;
            }

            if (RateExtra.Equals(""))
            {
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar la tarifa extra del tipo de Cliente Valido!", new SolidColorBrush(Colors.Yellow));

                //MessageBox.Show("Debe de ingresar la tarifa extra del tipo de Cliente Valido");
                txt_NewRateExcType.Focus();
                return;
            }

            if (await ValidatedTypeCliRegister(NewtypeClient))
            {
                string message = $"El Tipo de Cliente: {NewtypeClient} ya se encuentra registrado... Verifique!";
                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.Red));
                //MessageBox.Show($"El Tipo de Cliente: {NewtypeClient} ya se encuentra registrado... Verifique!!!!!");

                ClearAllDataTypeCli();
                return;
            }
           

            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await typeClientRepository.CreateTypeClient(NewtypeClient, NewDescription, RateBase, RateExtra);

            if (estado)
            {
                string message = $"{NewtypeClient} fue  registrado con Exito!";
                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.LightGreen));

                //MessageBox.Show("Tipo de Cliente registrado con exito!!");
                ClearAllDataTypeCli();
            }
            else
            {
                string message = $"Error al Crear usuario : {typeClientRepository.message} !";

                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.Red));

                //MessageBox.Show($"Error al registrar el usuario: {typeClientRepository.message}");
            }




        }
        #endregion

        #region Evento click del boton de limpiar los TextBox
        private void btn_CleanTxtCli_Click(object sender, RoutedEventArgs e)
        {
            ClearAllDataTypeCli();

        }
        #endregion             

        #region Metodo para restringir el ingreso de caracteres en el TextBox
        //Metodo para restringir el ingreso de caracteres en el TextBox
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text.FirstOrDefault()))
            {
                e.Handled = true;
            }
        }
        #endregion

        //#region Evento click para editar el tipo de cliente
        //private async void btn_ModifyTypeClient_Click(object sender, RoutedEventArgs e)
        //{
        //    // Mostrar barra de progreso
        //    await clsUtilities.ShowProgressBarAsync();

        //    string newNameType = txt_NewTyCli.Text;
        //    string newDescription = txt_NewDescriptionType.Text;
        //    int newRate = int.Parse(txt_NewRateBaseType.Text);
        //    int newExtRate = int.Parse(txt_NewRateExcType.Text);
            
 

        //    //Error first 
        //    if (txt_NewTyCli.Equals(""))
        //    {
        //        //MessageBox.Show("Debe de ingresar el nombre del usuario");
        //        await clsUtilities.ShowSnackbarAsync("Debe de ingresar el Tipo de Usuarios!", new SolidColorBrush(Colors.Yellow));

        //        txt_NewTyCli.Focus();
        //        return;
        //    }


        //    if (txt_NewRateBaseType.Equals(""))
        //    {
        //        //MessageBox.Show("Deben digitar el numero de cedula");
        //        await clsUtilities.ShowSnackbarAsync("Debes Ingresar el Precio TarifaBase (Si es ¢0 debe indicarlo)", new SolidColorBrush(Colors.Yellow));
        //        txt_NewRateBaseType.Focus();
        //        return;
        //    }

        //    if (txt_NewRateExcType.Equals(""))
        //    {
        //        //MessageBox.Show("Deben digitar el numero de cedula");
        //        await clsUtilities.ShowSnackbarAsync("Debes Ingresar el Precio Tarifa de Excedente por M³ (Si es ¢0 debe indicarlo)", new SolidColorBrush(Colors.Yellow));
        //        txt_NewRateBaseType.Focus();
        //        return;
        //    }




        //    //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
        //    bool estado = await typeClientRepository.UpdateTypeClient(newNameType, newDescription, newRate, newExtRate,idTypeClient);

        //    if (estado)
        //    {
        //        //MessageBox.Show("Usuario modificado con exito!!");
        //        await clsUtilities.ShowSnackbarAsync("Tipo de Cliente modificado con Exito!", new SolidColorBrush(Colors.Green));
        //        ClearAllDataTypeCli();
        //        CargarDatosTypeClient();
        //        btn_ModifyTypeCli.IsEnabled = false;
        //        btn_DeleteTypeCli.IsEnabled = false;
        //        btn_CreateNewTypeCli.IsEnabled = true;
        //        isModified = false;


        //    }
        //    else
        //    {
        //        //MessageBox.Show($"Error al modificar el usuario: {usersRepository.message}");
        //        await clsUtilities.ShowSnackbarAsync($"Error al modificar el usuario: {typeClientRepository.message}", new SolidColorBrush(Colors.Red));

        //    }
        //}
        //#endregion

        //#region Evento click del boton Eliminar usuario
        //private async void btn_DeleteTypeClient_Click(object sender, RoutedEventArgs e)
        //{
        //    // Mostrar barra de progreso
        //    await clsUtilities.ShowProgressBarAsync();
        //    // Mostrar un mensaje de confirmación para eliminar el cliente
        //    var confirmationBox = new clsMessageBox("¿Está seguro de eliminar el tipo de Cliente", "Sí", "No", "AlertOctagonOutline", "Advertencia", Brushes.Yellow);
        //    var confirmationResult = confirmationBox.ShowDialog();
        //    if (confirmationResult != true)
        //    {
        //        return;
        //    }

        //    bool estado = await typeClientRepository.DeleteTypeClient(idTypeClient);

        //    if (estado)
        //    {
        //        //MessageBox.Show("Usuario eliminado con Exito!");
        //        await clsUtilities.ShowSnackbarAsync("Usuario ELIMINADO con Exito!", new SolidColorBrush(Colors.Green));
        //        ClearAllDataTypeCli();
        //        CargarDatosTypeClient();
        //        btn_DeleteTypeCli.IsEnabled = false;
        //        btn_ModifyTypeCli.IsEnabled = false;
        //        btn_CreateNewTypeCli.IsEnabled = true;
        //        isModified = false;      
        //    }
        //    else
        //    {
        //        //MessageBox.Show($"Error al intentar eliminar el usuario: {usersRepository.message}");
        //        await clsUtilities.ShowSnackbarAsync($"Error al intentar eliminar el usuario: {typeClientRepository.message}", new SolidColorBrush(Colors.Red));
        //    }
        //}
        //#endregion




    }
}
