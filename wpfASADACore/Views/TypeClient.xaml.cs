using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        TypeClientRepository typeClientRepository;
        public TypeClient()
        {
            InitializeComponent();
            typeClientRepository = new TypeClientRepository();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            //Cargar los datos de la tabla de tipos de clientes
            dgvTypeClient.ItemsSource = typeClientRepository.GetAllTypeCliente();
        }

        #region Metodo que llamar el btn de limpiar los TextBox
        //funcion para limpirar todos los TextBlox 
        private void ClearAllDataTypeCli()
        {
           txt_NewTyCli.Text= string.Empty;
            txt_NewDescriptionType.Text= string.Empty;
            txt_NewRateBaseType.Text=string.Empty;
            txt_NewRateExcType.Text= string.Empty;
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
            string NewtypeClient = txt_NewTyCli.Text;
            string NewDescription = txt_NewDescriptionType.Text;
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

            if (NewDescription.Equals(""))
            {
                await clsUtilities.ShowSnackbarAsync("Debe de ingresar la descripcion del tipo de Cliente Valido!", new SolidColorBrush(Colors.Yellow));

                //MessageBox.Show("Debe de ingresar la descripcion del tipo de Cliente Valido");
                txt_NewDescriptionType.Focus();
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
                await clsUtilities.ShowSnackbarAsync(message, new SolidColorBrush(Colors.Green));

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

    }
}
