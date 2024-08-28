using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using H2OPure.Models;
using H2OPure.Repository;
using H2OPure.Utilities;
using Document = iTextSharp.text.Document;


namespace H2OPure.Views
{
    /// <summary>
    /// Lógica de interacción para frmPay.xaml
    /// </summary>
    public partial class frmPay : Page
    {
        BillingsRepository billingsRepository = new BillingsRepository();
        ReadingRepository readingRepository = new ReadingRepository();
        // Delegado vacío para forzar la actualización de la UI/UX
        private static Action EmptyDelegate = delegate () { };

        bool isLocal = false;

        public int Id { get; set; }
        public string SubscriberNum { get; set; }
        public string FullName { get; set; }
        public bool Pay { get; set; }
        private int selectedReadingId;
        private clsReading _selectedReading;

        public frmPay()
        {
            InitializeComponent();
        }

        #region para cargar el Combobox de Clientes con lexturas pendientes de pago
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
                cmb_Client.Background = new SolidColorBrush(Color.FromRgb(58, 82, 73));
                cmb_Client.Text = "No hay clientes con Lecturas Pendientes de Pago.";
                btnCargarFact.IsEnabled = false;
            }
        }
        #endregion

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
            int milisegundo = ahora.Millisecond / 166;  // Los milisegundos de la hora (0 a 999) dividido por 166 para que no exceda 5

            // Generamos el número de factura como un número de 6 dígitos
            int numeroFactura = año * 100000 + diaDelAño * 1000 + hora * 100 + minuto * 10 + segundo + milisegundo;

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
                        grd_Local.Background = new SolidColorBrush(Color.FromRgb(45,45,45));
                    }
                }

            }
        }
        #endregion

        #region metodo para cargar las lecturas pendientes en el datagrid
        private void LoadPendingReadings()
        {
            // Verificar si billingsRepository es null
            if (billingsRepository == null)
            {
                // billingsRepository no está inicializado
                return;
            }

            // Verificar si cmb_Client.SelectedValue es null
            if (cmb_Client.SelectedValue == null)
            {
                // No hay ningún cliente seleccionado
                return;
            }

            var readings = billingsRepository.GetPendingReadings((int)cmb_Client.SelectedValue);

            // Verificar si readings es null
            if (readings == null)
            {
                // GetPendingReadings devolvió null
                return;
            }

            dtg_Facturas.ItemsSource = readings;
            OpenExpander();
        }
        #endregion

        #region Metodo para realizar los calculos matematicos
        private void RealizarCalculos()
        {
            var selectedReading = (clsReading)dtg_Facturas.SelectedItem;
            if (selectedReading != null)
            {
                // Obtener la fecha actual
                DateTime currentDate = DateTime.Now;
                double totalRec = 0;               

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

                // Verificar si la fecha actual es posterior al día 8 del mes
                if (currentDate.Day > 8 )
                {
                    // Si es así, agrega el monto por reconexión
                    double reconnectionFee = 2000; // se cambia esto por el monto que se desee cobrar por reconexión
                    totalRec += reconnectionFee;

                    // Actualizar el cuadro de texto txt_MontoRec con el nuevo total
                    txt_MontoRec.Text = totalRec.ToString("C", cultureInfo);
                }
                else
                {
                    // Si no es así, establece el monto por reconexión en 0
                    txt_MontoRec.Text = "¢0";
                }
                

                // Usar el CultureInfo personalizado para formatear los números como moneda
                txt_MontoBasePay.Text = rateType.ToString("C", cultureInfo);
                txt_MontoExcPay.Text = (totalM3 * rateExc).ToString("C", cultureInfo);

                // Crear una constante para el porcentaje de impuestos
                const double taxPercentage = 0.0;  // 0% por el momento

                double subtotal = rateType + totalM3 * rateExc;
                double iva = subtotal * taxPercentage;

                //// Usar el CultureInfo personalizado para formatear el monto del IVA como moneda
                //txt_iva.Text = iva.ToString("C", cultureInfo);

                double totalPay = subtotal + iva + totalRec;
                txt_TotalPay.Text = totalPay.ToString("C", cultureInfo);

                GenerarNumeroFactura();
            }
        }
        #endregion

        
        private bool AreRateFieldsEmpty()
        {
            return string.IsNullOrEmpty(txt_RateType.Text) || string.IsNullOrEmpty(txt_RateExc.Text);
        }

        private async void dtg_Facturas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AreRateFieldsEmpty())
            {
                await clsUtilities.ShowSnackbarAsync("Debes Ingresar las Tarifas de cobro.", new SolidColorBrush(Colors.Yellow));
                exp_Facturas.IsExpanded = false;
                btnGenerarFactura.IsEnabled = false;
                txt_RateType.Focus();

                // Muestra los labels si los campos están vacíos
                lbl_txtRateType.Visibility = string.IsNullOrEmpty(txt_RateType.Text) ? Visibility.Visible : Visibility.Hidden;
                lbl_txtRateExc.Visibility = string.IsNullOrEmpty(txt_RateExc.Text) ? Visibility.Visible : Visibility.Hidden;

                // Desmarca todas las filas seleccionadas
                //dtg_Facturas.SelectedItems.Clear();

                // Desmarca el CheckBox de la fila seleccionada
                if (dtg_Facturas.SelectedItem != null)
                {
                    var selectedRow = (DataGridRow)dtg_Facturas.ItemContainerGenerator.ContainerFromItem(dtg_Facturas.SelectedItem);
                   // var checkBox = GetCheckBoxFromRow(selectedRow);
                    var cellContent = dtg_Facturas.Columns[6].GetCellContent(selectedRow);
                    if (cellContent != null)
                    {
                        var cell = (DataGridCell)cellContent.Parent;
                        var contentPresenter = (ContentPresenter)cell.Content;
                        ((CheckBox)contentPresenter.ContentTemplate.FindName("checkBox", contentPresenter)).IsChecked = false;
                       // checkBox.IsChecked = false;

                    //   // return checkBox;
                    }
                   // checkBox.IsChecked = false;
                }

                return;
            }
            else
            {
                // Oculta los labels si los campos están llenos
                lbl_txtRateType.Visibility = Visibility.Hidden;
                lbl_txtRateExc.Visibility = Visibility.Hidden;

                RealizarCalculos();
            }
        }

        // Evento TextChanged para txt_RateType y txt_RateExc
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Si ambos campos están llenos, expande exp_Facturas
            if (!AreRateFieldsEmpty())
            {
                exp_Facturas.IsExpanded = true;
                
            }
        }


        #region Metodo para hacer limpieza de los textbox
        private void ClearTextBoxesPays()
        {
            txt_LecturaAnt.Text = string.Empty;
            txt_LecturaAct.Text = string.Empty;
            txt_TotalM3.Text = string.Empty;
            txt_MontoBasePay.Text = string.Empty;
            txt_MontoExcPay.Text = string.Empty;
            //txt_iva.Text = string.Empty;
            txt_TotalPay.Text = string.Empty;
            txt_MontoRec.Text = string.Empty;   
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
        private CheckBox GetCheckBoxFromRow(DataGridRow row)
        {
            var cellContent = dtg_Facturas.Columns[6].GetCellContent(row);
            if (cellContent != null)
            {
                var cell = (DataGridCell)cellContent.Parent;
                var contentPresenter = (ContentPresenter)cell.Content;
                var checkBox = (CheckBox)contentPresenter.ContentTemplate.FindName("checkBox", contentPresenter);
                return checkBox;
            }
            return null; // Devuelve null si no se encuentra el contenido de la celda
        }
        #endregion

        #region Evento para que cuando se deseleccione una fila del datagrid se BORREN los datos en los textbox CALCULOS MATEMATICOS y los demas ROW se desbloqueen
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Obtén el CheckBox que disparó el evento
            var checkBox = (CheckBox)sender;

            // Verifica si hay una fila seleccionada actualmente
            if (_selectedReading != null)
            {
                // Desmarca el CheckBox de la fila seleccionada
                _selectedReading.IsChecked = false;
                _selectedReading = null;
            }

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
                grd_Local.Background = new SolidColorBrush(Colors.LightGreen);
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
                        int lastRead = int.Parse(txt_LecturaAnt.Text);
                        int currentRead = int.Parse(txt_LecturaAct.Text);
                        // Eliminar el símbolo de moneda y los espacios en blanco
                        //string amountIvaText = txt_iva.Text.Replace("¢", "").Trim();
                        //double amountIva = double.Parse(amountIvaText);

                        string amountBaseText = txt_MontoBasePay.Text.Replace("¢", "").Trim();
                        double amountBase = double.Parse(amountBaseText);

                        string amountExcText = txt_MontoExcPay.Text.Replace("¢", "").Trim();
                        double amountExc = double.Parse(amountExcText);

                        string totalPayText = txt_TotalPay.Text.Replace("¢", "").Trim();
                        double totalPay = double.Parse(totalPayText);

                        string totalRecText = txt_MontoRec.Text.Replace("¢", "").Trim();
                        double totalRec = double.Parse(totalRecText);


                        // Crear una nueva factura
                        var newBilling = new clsBilling
                        {
                            InvoiceNum = lbl_InvoiceNum.Text,
                            BillingDate = DateTime.Now,
                            AmountIva = 0,
                            AmountBase = amountBase,
                            AmountExc = amountExc,
                            AmountTotal = totalPay,
                            UserId = 0, // Aquí debes poner el ID del usuario actual
                            Remarks = "Pagada",
                            idClient = selectedReading.idClient,
                            AmountRec = totalRec,



                        };

                        // Guardar la nueva factura en la base de datos
                        await billingsRepository.AddBilling(newBilling);

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
                        ClearTextBoxesPays(); // solo limpia los calculos matematicos
                        Clear(); // limpia datagrid y textbox tipo cliente, expander cerrado

                        // Limpiar el TextBlock factura
                        lbl_InvoiceNum.Text = "";

                        // Mostrar un mensaje de éxito
                        await clsUtilities.ShowSnackbarAsync("La factura se ha generado correctamente.", new SolidColorBrush(Colors.LightGreen));
                        // Generar el PDF de la factura
                        GenerarFacturaPdf(newBilling, dateLastReading, currentReadingDate, totalConsumption, typeClient,lastRead,currentRead);
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

        #region Metodo para imprimir la factura en PDF
        private void GenerarFacturaPdf(clsBilling billing, string dateLastReading, string currentReadingDate,string totalConsumption, string typeClient, int lastRead, int currentRead)
        {
            var billingDetails = billingsRepository.GetBillingDetails(billing.id);
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Cobro de Recibo Agua " + "Factura_{0}_Medidor_{1}.pdf", billing.InvoiceNum, billingDetails.Client.SubscriberNum);
            if (savefile.ShowDialog() == true)
            {
                var billingsRepository = new BillingsRepository();
               

                string htmlTemplate = Properties.Resources.facturaAsada5.ToString();

                if (billing != null && billingDetails != null)
                {                  

                    htmlTemplate = htmlTemplate.Replace("{InvoiceNum}", billing.InvoiceNum);
                    htmlTemplate = htmlTemplate.Replace("{BillingDate}", billing.BillingDate.ToString());
                    //htmlTemplate = htmlTemplate.Replace("{AmountIva}", billing.AmountIva.ToString());
                    htmlTemplate = htmlTemplate.Replace("{AmountBase}",billing.AmountBase.ToString());
                    htmlTemplate = htmlTemplate.Replace("{AmountExc}", billing.AmountExc.ToString());
                    htmlTemplate = htmlTemplate.Replace("{AmountTotal}",  billing.AmountTotal.ToString());
                    htmlTemplate = htmlTemplate.Replace("{AmountRec}", billing.AmountRec.ToString());
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
                    htmlTemplate = htmlTemplate.Replace("{LastReading}", lastRead.ToString());
                    htmlTemplate = htmlTemplate.Replace("{CurrentReading}", currentRead.ToString());

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

        #endregion

        #region Metodo para  Limpiar gatagrid y textbox tipo cliente, expander cerrado
        private void Clear()
        {
            dtg_Facturas.ItemsSource = null;
            txt_RateType.Clear();
            txt_RateExc.Clear();
            txt_TypeClient.Clear();
            cmb_Client.Text = string.Empty;
            grd_Local.Background = new SolidColorBrush(Color.FromRgb(45,45,45));
            exp_Facturas.IsExpanded = false;
        }
        #endregion

        #region Evento click del boton Cancelar
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBoxesPays();
            Clear();


        }
        #endregion
    }
}
