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
using static wpfASADACore.Repository.ReadingRepository;


namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para frmPay.xaml
    /// </summary>
    public partial class frmPay : Page
    {
        BillingsRepository billingsRepository = new BillingsRepository();

        public int Id { get; set; }
        public string SubscriberNum { get; set; }
        public string FullName { get; set; }
        public bool Pay { get; set; }





        public frmPay()
        {
            InitializeComponent();
        }




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

        private void btnCargarFact_Click(object sender, RoutedEventArgs e)
        {
            LoadClientType();
            LoadPendingReadings();
        }

        private void frmPay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadClientType();
                LoadPendingReadings();
            }
        }

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
                }
            }
        }
        private void LoadPendingReadings()
        {
            var readings = billingsRepository.GetPendingReadings();
            dtg_Facturas.ItemsSource = readings;
        }

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
            }
        }

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
            btnGenerarLectura.IsEnabled = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in dtg_Facturas.Items)
            {
                var row = (DataGridRow)dtg_Facturas.ItemContainerGenerator.ContainerFromItem(item);
                row.IsEnabled = true;
            }
            btnGenerarLectura.IsEnabled = false;
            ClearTextBoxesPays();
            LoadPendingReadings();
        }


        private CheckBox GetCheckBoxFromRow(DataGridRow row)
        {
            var cell = (DataGridCell)dtg_Facturas.Columns[6].GetCellContent(row).Parent;
            var contentPresenter = (ContentPresenter)cell.Content;
            var checkBox = (CheckBox)contentPresenter.ContentTemplate.FindName("checkBox", contentPresenter);
            return checkBox;
        }




    }
}
