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


        public frmPay()
        {
            InitializeComponent();
        }

        #region Metodo para cargar los datos de los tipos de clientes en el combobox
        ////Este Metodo es para cargar los datos de los tipos de clientes en el combobox
        //private void cargarDatosClientBill()
        //{
        //    try
        //    {

        //        List<clsBilling> lista = billingsRepository.GetAllClientBilling();
        //        cmb_ClientBill.Items.Clear();
        //        cmb_ClientBill.ItemsSource = lista;
        //        cmb_ClientBill.DisplayMemberPath = "";
        //        cmb_ClientBill.SelectedValuePath = "id";
        //        cmb_ClientBill.SelectedIndex = 0;
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message.ToString());
        //    }


        //}
        #endregion



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var clientsForBilling = billingsRepository.GetClientsForBilling();

            cmb_ClientBill.Items.Clear();
            foreach (var (id,subscribeNum, name) in clientsForBilling)
            {
                //cmb_ClientBill.Items.Add(new { SubscriberNum = subscribeNum, Name = name });
                cmb_ClientBill.Items.Add(new { SubscriberNum = subscribeNum, Name = name });

            }



            // Habilitar el texto predictivo
            cmb_ClientBill.IsTextSearchEnabled = true;
            //cmb_ClientBill.TextSearch.TextPath = "Name";

            /*
             Este código:

            Llama al método GetClientsForBilling() para obtener la lista de clientes con registros en la tabla de facturación.
            Limpia el combobox cmb_ClientBill.
            Agrega cada cliente a la lista del combobox, utilizando un objeto anónimo con las propiedades Id y Name.
            Habilita el texto predictivo en el combobox, utilizando IsTextSearchEnabled = true y TextSearch.TextPath = "Name".*/

        }
    }
}
