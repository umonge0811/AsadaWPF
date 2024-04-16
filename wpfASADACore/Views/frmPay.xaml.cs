using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Document = iTextSharp.text.Document;
using static wpfASADACore.Repository.ReadingRepository;
using System.Net.Sockets;
using static wpfASADACore.Repository.BillingsRepository;


namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para frmPay.xaml
    /// </summary>
    public partial class frmPay : Page
    {
        BillingsRepository billingsRepository = new BillingsRepository();
      ReadingRepository readingRepository = new ReadingRepository();

        bool isLocal = false;

        public int Id { get; set; }
        public string SubscriberNum { get; set; }
        public string FullName { get; set; }
        public bool Pay { get; set; }
        private int selectedReadingId;  // Añade esta línea




        public frmPay()
        {
            InitializeComponent();
        }

        private void CargarCmbClient()
        {
            var db = new BillingsRepository();
            var clients = db.GetClientsWithReadings("Pendiente de Pago");
            if (clients.Count > 0)
            {
                cmb_Client.ItemsSource = null; // Limpia la colección existente
                cmb_Client.ItemsSource = clients; // Vuelve a vincularla con los nuevos datos
                cmb_Client.DisplayMemberPath = "DisplayText";
                cmb_Client.SelectedValuePath = "Id";
            }
            else
            {
                cmb_Client.Background = new SolidColorBrush(Colors.LightGreen);
                cmb_Client.Text = "No hay clientes con Lecturas Pendientes de Pago.";
                btnCargarFact.IsEnabled = false;
            }
        }


        #region Eventos loaded 
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CargarCmbClient();
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


        //#region Eventos keydown para que cuando se da enter se carguen las facturas pendientes
        //private async void frmPay_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        if (cmb_Client.SelectedItem != null)
        //        {
        //            LoadClientType();
        //            LoadPendingReadings();
        //        }
        //        else
        //        {
        //            await clsUtilities.ShowSnackbarAsync("Por favor, selecciona un cliente antes de cargar las facturas.", new SolidColorBrush(Colors.Yellow));
        //        }
        //    }
        //}
        //#endregion


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


        #region evento para que se despliegue lista de clientes
        private void cmb_ClientPay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            cmb_Client.IsDropDownOpen = true;

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
                        grd_Local.Background = new SolidColorBrush(Color.FromArgb(150, 198, 235, 197));
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
            var readings = billingsRepository.GetPendingReadings((int)cmb_Client.SelectedValue);
            dtg_Facturas.ItemsSource = readings;
            OpenExpander();
        }
        #endregion

        private void RealizarCalculos()
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

                double subtotal = rateType + totalM3 * rateExc;
                double iva = subtotal * taxPercentage;

                // Usar el CultureInfo personalizado para formatear el monto del IVA como moneda
                txt_iva.Text = iva.ToString("C", cultureInfo);

                double totalPay = subtotal + iva;
                txt_TotalPay.Text = totalPay.ToString("C", cultureInfo);

                GenerarNumeroFactura();
            }
        }


        #region evento para que cuando se seleccione una fila del datagrid se carguen los datos en los textbox CALCULOS MATEMATICOS
        private async void dtg_Facturas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txt_TypeClient.Text == "Local" && (txt_RateType.Text == "" || txt_RateExc.Text == ""))
            {
                await clsUtilities.ShowSnackbarAsync("Debes Ingresar las Tarifas de cobro.", new SolidColorBrush(Colors.Yellow));
                exp_Facturas.IsExpanded = false;
                txt_RateType.Focus();

                // Obtén la fila seleccionada
                var selectedRow = (DataGridRow)dtg_Facturas.ItemContainerGenerator.ContainerFromItem(dtg_Facturas.SelectedItem);

                // Obtén el CheckBox de la fila seleccionada
                var checkBox = GetCheckBoxFromRow(selectedRow);

                // Desmarca el CheckBox
                if (checkBox != null)
                {
                    checkBox.IsChecked = false;
                }

                return;
            }
            else
            {
                RealizarCalculos();
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
            txt_TypeClient.Text = string.Empty;
            txt_RateType.Text = string.Empty;
            txt_RateExc.Text = string.Empty;
            grd_Local.Background = new SolidColorBrush(Colors.White);
            exp_Facturas.IsExpanded = false;
            CargarCmbClient();
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
                else
                {
                    // Obtén el ID de la lectura seleccionada
                    var selectedReading = (clsReading)row.Item;
                    selectedReadingId = selectedReading.id;
                }
            }
            CloseExpander();
            btnGenerarFactura.IsEnabled = true;
        }
        #endregion


        #region Evento para que se detecte el checkbox de una fila del datagrid
        // Método para obtener el CheckBox de una fila del DataGrid
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
            lbl_InvoiceNum.Text = "";
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
            else
            {
                grd_Local.Background = new SolidColorBrush(Colors.Green);
            }
        }

        #endregion

        #region Eventos keydown para que cuando se da enter se carguen las facturas pendientes
        private async void cmb_Client_KeyUp(object sender, KeyEventArgs e)
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

        #region Evento click del boton Generar Factura
        private async void btnGenerarFactura_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync();

            if (cmb_Client.SelectedItem != null)
            {
                if (dtg_Facturas.SelectedItem != null)
                {
                    var selectedReading = (clsReading)dtg_Facturas.SelectedItem;
                    if (selectedReading != null)
                    {
                        string dateLastReading = selectedReading.DateLastReading.ToString("dd/MM/yyyy");
                        string currentReadingDate = selectedReading.CurrentReadingDate.ToString("dd/MM/yyyy");
                        string totalConsumption = selectedReading.TotalConsumption.ToString();
                        string typeClient= txt_TypeClient.Text;
                        // Eliminar el símbolo de moneda y los espacios en blanco
                        string amountIvaText = txt_iva.Text.Replace("¢", "").Trim();
                        double amountIva = double.Parse(amountIvaText);

                        string amountBaseText = txt_MontoBasePay.Text.Replace("¢", "").Trim();
                        double amountBase = double.Parse(amountBaseText);

                        string amountExcText = txt_MontoExcPay.Text.Replace("¢", "").Trim();
                        double amountExc = double.Parse(amountExcText);

                        string totalPayText = txt_TotalPay.Text.Replace("¢", "").Trim();
                        double totalPay = double.Parse(totalPayText);

                        // Crear una nueva factura
                        var newBilling = new clsBilling
                        {
                            InvoiceNum = lbl_InvoiceNum.Text,
                            BillingDate = DateTime.Now,
                            AmountIva = amountIva,
                            AmountBase = amountBase,
                            AmountExc = amountExc,
                            AmountTotal = totalPay,
                            UserId = 0, // Aquí debes poner el ID del usuario actual
                            Remarks = "Pagada",
                            idClient = selectedReading.idClient
                          

                        };
                        
                        // Guardar la nueva factura en la base de datos
                        billingsRepository.AddBilling(newBilling);

                        // Actualizar la lectura como pagada
                        var readingToUpdate = readingRepository.GetReadingById(selectedReadingId);
                        if (readingToUpdate != null)
                        {
                            readingToUpdate.Remarks = "Pagada";
                            readingToUpdate.ReadActiva = false;
                            readingRepository.UpdateReading(readingToUpdate);
                        }

                        // Actualizar el DataGrid
                        LoadPendingReadings();

                        // Limpiar los TextBoxes
                        ClearTextBoxesPays();

                        // Limpiar el TextBlock factura
                        lbl_InvoiceNum.Text = "";

                        // Mostrar un mensaje de éxito
                        await clsUtilities.ShowSnackbarAsync("La factura se ha generado correctamente.", new SolidColorBrush(Colors.LightGreen));
                        // Generar el PDF de la factura
                        GenerarFacturaPdf(newBilling, dateLastReading, currentReadingDate, totalConsumption, typeClient);
                    }
                }
                else
                {
                    await clsUtilities.ShowSnackbarAsync("Por favor, selecciona una lectura antes de generar la factura.", new SolidColorBrush(Colors.Yellow));
                }
            }
            else
            {
                await clsUtilities.ShowSnackbarAsync("Por favor, selecciona un cliente antes de generar la factura.", new SolidColorBrush(Colors.Yellow));
            }
        }
        #endregion

        private void GenerarFacturaPdf(clsBilling billing, string dateLastReading, string currentReadingDate,string totalConsumption, string typeClient)
        {
            var billingDetails = billingsRepository.GetBillingDetails(billing.id);
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Cobro de Recibo Agua " + "Factura_{0}_Abonado_{1}.pdf", billing.InvoiceNum, billingDetails.Client.SubscriberNum);
            if (savefile.ShowDialog() == true)
            {
                var billingsRepository = new BillingsRepository();
               

                string htmlTemplate = Properties.Resources.facturaAsada.ToString();

                if (billing != null && billingDetails != null)
                {                  

                    htmlTemplate = htmlTemplate.Replace("{InvoiceNum}", billing.InvoiceNum);
                    htmlTemplate = htmlTemplate.Replace("{BillingDate}", billing.BillingDate.ToString());
                    htmlTemplate = htmlTemplate.Replace("{AmountIva}", billing.AmountIva.ToString());
                    htmlTemplate = htmlTemplate.Replace("{AmountBase}",billing.AmountBase.ToString());
                    htmlTemplate = htmlTemplate.Replace("{AmountExc}", billing.AmountExc.ToString());
                    htmlTemplate = htmlTemplate.Replace("{AmountTotal}",  billing.AmountTotal.ToString());
                    htmlTemplate = htmlTemplate.Replace("{Remarks}", billing.Remarks);
                    htmlTemplate = htmlTemplate.Replace("{idClient}", billing.idClient.ToString());
                    htmlTemplate = htmlTemplate.Replace("{ClientName}", billingDetails.Client.name);
                    htmlTemplate = htmlTemplate.Replace("{ClientLastName}", billingDetails.Client.lastName);
                    htmlTemplate = htmlTemplate.Replace("{ClientSecondSurname}", billingDetails.Client.secondSurname);
                    htmlTemplate = htmlTemplate.Replace("{ClientDNI}", billingDetails.Client.DNI);
                    htmlTemplate = htmlTemplate.Replace("{ClientSubscriberNum}", billingDetails.Client.SubscriberNum);
                    htmlTemplate = htmlTemplate.Replace("{TypeClientId}",typeClient);
                    htmlTemplate = htmlTemplate.Replace("{DateLastReading}", dateLastReading);
                    htmlTemplate = htmlTemplate.Replace("{CurrentReadingDate}", currentReadingDate);
                    htmlTemplate = htmlTemplate.Replace("{TotalConsumption}", totalConsumption);

                    using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                    {
                        Document pdfDoc = new Document(PageSize.A4);
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        pdfDoc.Add(new Phrase(""));

                        using (StringReader sr = new StringReader(htmlTemplate))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                        }

                        pdfDoc.Close();
                        stream.Close();
                    }
                }
            }
        }


    }
}
