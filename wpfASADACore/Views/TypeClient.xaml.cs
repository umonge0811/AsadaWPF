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
using System.Xml.Linq;
using wpfASADACore.Models;
using wpfASADACore.Repository;
using wpfASADACore.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


        //funcion para limpirar todos los TextBlox 
        private void ClearAllDataTypeCli()
        {
           txt_NewTyCli.Text= string.Empty;
            txt_NewDescriptionType.Text= string.Empty;
            txt_NewRateBaseType.Text=string.Empty;
            txt_NewRateExcType.Text= string.Empty;
        }


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
                Console.WriteLine("Error al validar usuario: " + ex.Message);
                throw; // Reenviar la excepción para un tratamiento superior
            }
        }

        private async void btn_CreateNewTypeCli_Click(object sender, RoutedEventArgs e)
        {
            string NewtypeClient = txt_NewTyCli.Text;
            string NewDescription = txt_NewDescriptionType.Text;
            double RateBase = Convert.ToDouble(txt_NewRateBaseType.Text);
            double RateExtra = Convert.ToDouble(txt_NewRateExcType.Text);

            //Error first 
            if (NewtypeClient.Equals(""))
            {
                MessageBox.Show("Debe de ingresar el nombre del tipo de Cliente Valido");
                txt_NewTyCli.Focus();
                return;
            }

            if (NewDescription.Equals(""))
            {
                MessageBox.Show("Debe de ingresar la descripcion del tipo de Cliente Valido");
                txt_NewDescriptionType.Focus();
                return;
            }

            if (RateBase.Equals(""))
            {
                MessageBox.Show("Debe de ingresar la tarifa base del tipo de Cliente Valido");
                txt_NewRateBaseType.Focus();
                return;
            }

            if (RateExtra.Equals(""))
            {
                MessageBox.Show("Debe de ingresar la tarifa extra del tipo de Cliente Valido");
                txt_NewRateExcType.Focus();
                return;
            }

            if (await ValidatedTypeCliRegister(NewtypeClient))
            {
                MessageBox.Show($"El Tipo de Cliente: {NewtypeClient} ya se encuentra registrado... Verifique!!!!!");

                ClearAllDataTypeCli();
                return;
            }

            //await es para esperar a que la tarea termine, en este caso, la funcion/Metodo ejecute para que avance a la siguiente tarea 
            bool estado = await typeClientRepository.CreateTypeClient(NewtypeClient, NewDescription, RateBase, RateExtra);

            if (estado)
            {
                MessageBox.Show("Tipo de Cliente registrado con exito!!");
                ClearAllDataTypeCli();
            }
            else
            {
                MessageBox.Show($"Error al registrar el usuario: {typeClientRepository.message}");
            }




        }

        private void btn_CleanTxtCli_Click(object sender, RoutedEventArgs e)
        {
            ClearAllDataTypeCli();

        }


        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            //Cargar los datos de la tabla de tipos de clientes
            dgvTypeClient.ItemsSource = typeClientRepository.GetAllTypeCliente();
        }

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
