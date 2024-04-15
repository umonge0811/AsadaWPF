using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using wpfASADACore.Utilities;
using static wpfASADACore.Repository.ReadingRepository;


namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para frmPay.xaml
    /// </summary>
    public partial class frmPay : Page
    {
        BillingsRepository billingsRepository = new BillingsRepository();
        bool isLocal = false;

        public int Id { get; set; }
        public string SubscriberNum { get; set; }
        public string FullName { get; set; }
        public bool Pay { get; set; }





        public frmPay()
        {
            InitializeComponent();
        }



        #region Eventos loaded 
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var db = new BillingsRepository();
            var clients = db.GetClientsWithReadings("Pendiente de Pago");
            if (clients.Count > 0)
            {
                cmb_Client.Items.Clear(); // Vacía la colección Items
                cmb_Client.ItemsSource = clients;
                cmb_Client.DisplayMemberPath = "DisplayText";
                cmb_Client.SelectedValuePath = "Id";
            }
            else
            {
                // Mostrar un mensaje de error o realizar alguna otra acción si no hay clientes con lecturas
            }
           
        }
        #endregion


        #region Metodo para generar el numero de factura automaticamente 
        private void GenerarNumeroFactura()
        {
            DateTime ahora = DateTime.Now;
            int año = ahora.Year % 10;  // El último dígito del año
            int diaDelAño = ahora.DayOfYear % 100;  // Los últimos dos dígitos del día del año (1 a 366)
            int hora = ahora.Hour;  // La hora del día (0 a 23)
            int minuto = ahora.Minute;  // El minuto de la hora (0 a 59)
            int segundo = ahora.Second / 12;  // El segundo de la hora (0 a 59) dividido por 12 para que no exceda 4

            // Generamos el número de factura como un número de 5 dígitos
            int numeroFactura = año * 10000 + diaDelAño * 100 + hora * 10 + minuto / 6 + segundo;

            // Asignamos el número de factura al label
            lbl_InvoiceNum.Text = numeroFactura.ToString();
        }
        #endregion


        #region Eventos click del boton Cargar Facturas
        private async void btnCargarFact_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_Client.SelectedItem != null)
            {
                LoadClientType();
                LoadPendingReadings();
            }
            else
            {
                await clsUtilities.ShowSnackbarAsync("Por favor, selecciona un cliente antes de cargar las facturas.", new SolidColorBrush(Colors.Yellow));
            }
        }
        #endregion


        #region Eventos keydown para que cuando se da enter se carguen las facturas pendientes
        private async void frmPay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (cmb_Client.SelectedItem != null)
                {
                    LoadClientType();
                    LoadPendingReadings();
                }
                else
                {
                    await clsUtilities.ShowSnackbarAsync("Por favor, selecciona un cliente antes de cargar las facturas.", new SolidColorBrush(Colors.Yellow));
                }
            }
        }
        #endregion


        #region evento para que cuando se borre el texto del combobox se limpie el datagrid y otras cosas
        private void cmb_Client_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmb_Client.Text))
            {
                // Vacía el DataGrid
                dtg_Facturas.ItemsSource = null;

                // Cierra el Expander
                exp_Facturas.IsExpanded = false;
                grd_Local.Background = new SolidColorBrush(Colors.White);
                txt_TypeClient.Text = string.Empty;
                txt_RateType.Text = string.Empty;
                txt_RateExc.Text = string.Empty;
            }
        }
        #endregion


        #region metodo para cargar el tipo de cliente en los textbox y validaciones por si es local
        private void LoadClientType()
        {
            if (cmb_Client.SelectedValue != null)
            {
                int clientId = (int)cmb_Client.SelectedValue;
                var typeClient = billingsRepository.GetTypeClientByClientId(clientId);

                if (typeClient != null)
                {
                    txt_TypeClient.Text = typeClient.name;
                    txt_RateType.Text = typeClient.rate.ToString();
                    txt_RateExc.Text = typeClient.rateExc.ToString();

                    // Si el nombre del tipo de cliente es "Local", permitir la modificación de los montos
                    bool isLocal = typeClient.name == "Local";
                    txt_RateType.IsReadOnly = !isLocal;
                    txt_RateExc.IsReadOnly = !isLocal;
                    if (isLocal)
                    {
                        grd_Local.Background = new SolidColorBrush(Colors.LightGreen);
                        txt_RateType.Clear();
                        txt_RateExc.Clear();

                    }
                    else
                    {
                        grd_Local.Background = new SolidColorBrush(Colors.White);
                    }
                }

            }
        }
        #endregion


        #region metodo para cargar las lecturas pendientes en el datagrid
        private void LoadPendingReadings()
        {
            var readings = billingsRepository.GetPendingReadings();
            dtg_Facturas.ItemsSource = readings;
            OpenExpander();
        }
        #endregion

        #region evento para que cuando se seleccione una fila del datagrid se carguen los datos en los textbox CALCULOS MATEMATICOS
        private void dtg_Facturas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedReading = (clsReading)dtg_Facturas.SelectedItem;
            if (selectedReading != null)
            {
                // Pasar la información de la lectura a los cuadros de texto
                txt_LecturaAnt.Text = selectedReading.lastRead.ToString();
                txt_LecturaAct.Text = selectedReading.CurrentRead.ToString();
                txt_TotalM3.Text = selectedReading.TotalConsumption.ToString();

                // Realizar los cálculos matemáticos
                double rateType = double.Parse(txt_RateType.Text);
                double rateExc = double.Parse(txt_RateExc.Text);
                double totalM3 = double.Parse(txt_TotalM3.Text);

                // Crear un CultureInfo personalizado con el símbolo de moneda establecido en ¢
                var cultureInfo = new CultureInfo(CultureInfo.CurrentCulture.Name);
                cultureInfo.NumberFormat.CurrencySymbol = "¢";

                // Usar el CultureInfo personalizado para formatear los números como moneda
                txt_MontoBasePay.Text = rateType.ToString("C", cultureInfo);
                txt_MontoExcPay.Text = (totalM3 * rateExc).ToString("C", cultureInfo);

                // Crear una constante para el porcentaje de impuestos
                const double taxPercentage = 0.0;  // 0% por el momento

                double iva = (rateType + totalM3 * rateExc) * taxPercentage;
                txt_iva.Text = iva.ToString("P", cultureInfo);

                double totalPay = rateType + totalM3 * rateExc + iva;
                txt_TotalPay.Text = totalPay.ToString("C", cultureInfo);
                GenerarNumeroFactura();
            }
        }
        #endregion

        #region Metodo para hacer limpieza de los textbox
        private void ClearTextBoxesPays()
        {
            txt_LecturaAnt.Text = string.Empty;
            txt_LecturaAct.Text = string.Empty;
            txt_TotalM3.Text = string.Empty;
            txt_MontoBasePay.Text = string.Empty;
            txt_MontoExcPay.Text = string.Empty;
            txt_iva.Text = string.Empty;
            txt_TotalPay.Text = string.Empty;
        }
        #endregion


        #region Evento para que cuando se seleccione una fila del datagrid se carguen los datos en los textbox CALCULOS MATEMATICOS y los demas ROW se bloqueen
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var selectedCheckBox = (CheckBox)sender;
            foreach (var item in dtg_Facturas.Items)
            {
                var row = (DataGridRow)dtg_Facturas.ItemContainerGenerator.ContainerFromItem(item);
                var checkBox = GetCheckBoxFromRow(row);
                if (checkBox != selectedCheckBox)
                {
                    row.IsEnabled = false;
                }
               
            }
            CloseExpander();
            btnGenerarFactura.IsEnabled = true;
        }

        private CheckBox GetCheckBoxFromRow(DataGridRow row)
        {
            var cell = (DataGridCell)dtg_Facturas.Columns[6].GetCellContent(row).Parent;
            var contentPresenter = (ContentPresenter)cell.Content;
            var checkBox = (CheckBox)contentPresenter.ContentTemplate.FindName("checkBox", contentPresenter);
            return checkBox;
        }
        #endregion


        #region Evento para que cuando se deseleccione una fila del datagrid se BORREN los datos en los textbox CALCULOS MATEMATICOS y los demas ROW se desbloqueen
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in dtg_Facturas.Items)
            {
                var row = (DataGridRow)dtg_Facturas.ItemContainerGenerator.ContainerFromItem(item);
                row.IsEnabled = true;
            }
            btnGenerarFactura.IsEnabled = false;
            ClearTextBoxesPays();
            LoadPendingReadings();
            // Limpiar el TextBlock factura
            txt_RateExc.Text = "";
        }
        #endregion


        #region METODO/FUNCION  para desplegar / cerrar el Extender y mostrar los datos de la factura
        private void OpenExpander()
        {
            // Para expandir el Expander
            exp_Facturas.IsExpanded = true;


        }
        private void CloseExpander()
        {
            // Para expandir el Expander
            exp_Facturas.IsExpanded = true;


        }
        #endregion


        #region Evento para que mientras los campos de editar los precios en local esten vacios el fondo sera Verde, si se completan sera blanco
        /*cuando el texto en txt_RateType o txt_RateExc cambie, se llamará a los métodos txt_RateType_TextChanged y txt_RateExc_TextChanged respectivamente. Estos métodos a su vez llamarán al método CheckRateFields(), 
         * que verificará si ambos campos de texto están vacíos o no, y cambiará el color de fondo del elemento grd_Local en consecuencia.*/
        private void txt_RateType_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckRateFields();
        }

        private void txt_RateExc_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckRateFields();
        }

        private void CheckRateFields()
        {
            if (!string.IsNullOrEmpty(txt_RateType.Text) && !string.IsNullOrEmpty(txt_RateExc.Text))
            {
                grd_Local.Background = new SolidColorBrush(Colors.White);
            }
        }
        #endregion


    }
}
