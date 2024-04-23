using H2OPure.Models;
using H2OPure.Repository;
using H2OPure.Utilities;
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

namespace H2OPure.Views
{
    public partial class frmReportes : Page
    {
        BillingsRepository billingsRepository = new BillingsRepository();
        InventoryRepository inventoryRepository = new InventoryRepository();
        UsersRepository usersRepository = new UsersRepository();
        ClientsRepository clientsRepository = new ClientsRepository();

        public frmReportes()
        {
            InitializeComponent();
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable(), "Todos los usuarios");
            UpdateDataGrid(clientsRepository.GetAllClients(), "Todos los clientes");
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable(), "Todos los usuarios");

            var clients = clientsRepository.GetClientsForReport();
            if (clients.Any())
            {
                UpdateDataGrid(clients, "Todos los clientes");
            }
            else
            {
                await clsUtilities.ShowSnackbarAsync(clientsRepository.message, new SolidColorBrush(Colors.Yellow));
            }

            var transactions = inventoryRepository.GetInventoryTransactionsForReport().Result;
            if (transactions.Any())
            {
                UpdateDataGrid(transactions, "Todas las transacciones");
            }
            else
            {
                await clsUtilities.ShowSnackbarAsync("No se pudieron recuperar las transacciones de inventario.", new SolidColorBrush(Colors.Yellow));
            }

            var billingDetails = billingsRepository.GetAllBillingDetails().Result;
            if (billingDetails.Any())
            {
                UpdateDataGrid(billingDetails, "Todos los detalles de facturación");
            }
            else
            {
                await clsUtilities.ShowSnackbarAsync("No se pudieron recuperar los detalles de facturación.", new SolidColorBrush(Colors.Yellow));
            }
        }

        private void UpdateDataGrid(IEnumerable<BillingsRepository.BillingDetails> billingDetails, string filterTitle)
        {
            dtg_Billings.ItemsSource = billingDetails.ToList();
            txt_FilterTitle.Text = filterTitle;
        }


        private void UpdateDataGrid(IEnumerable<clsInventoryTransaction> transactions, string filterTitle)
        {
            dtg_Inventory.ItemsSource = transactions.ToList();
            txt_FilterTitle.Text = filterTitle;
        }

        private void UpdateDataGridClients(IEnumerable<clsCliente> clients, string filterTitle)
        {
            dtg_Clientes.ItemsSource = clients.ToList();
            txt_FilterTitleClient.Text = filterTitle; // Utiliza el TextBlock de clientes
        }

        private void UpdateDataGridTransactions(IEnumerable<clsInventoryTransaction> transactions, string filterTitle)
        {
            dtg_Inventory.ItemsSource = transactions.ToList();
            txt_FilterTitleINV.Text = filterTitle; // Utiliza el TextBlock de inventarios
        }

        #region Botones de filtro de usuarios
        private void UpdateDataGrid(IEnumerable<clsUser> users, string filterTitle)
        {
            dtg_Usuarios.ItemsSource = users.ToList();
            txt_FilterTitle.Text = filterTitle;
        }



        private void btn_FilterActiveUsers_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable().Where(u => u.isActive), "Usuarios activos");
        }

        private void btn_FilterInactiveUsers_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable().Where(u => !u.isActive), "Usuarios inactivos");
        }

        private void btn_FilterAdminUsers_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable().Where(u => u.typeUser == 1), "Usuarios administradores");
        }

        private void btn_FilterNormalUsers_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(usersRepository.GetAllUsersAsEnumerable().Where(u => u.typeUser == 1), "Usuarios normales");
        }

       
        #endregion

        #region Botones de filtro de clientes
        private void UpdateDataGrid(IEnumerable<clsCliente> clients, string filterTitle)
        {
            dtg_Clientes.ItemsSource = clients.ToList();
            txt_FilterTitle.Text = filterTitle;
        }

       

        private void btn_FilterAdminClients_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGridClients(clientsRepository.GetAllClients().Where(c => c.TypeClientId == 1), "Clientes administradores");
        }

        private void btn_FilterNormalClients_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid(clientsRepository.GetAllClients().Where(c => c.TypeClientId == 2), "Clientes normales");
        }



        #endregion

        private void btn_FilterEntryTransactions_Click(object sender, RoutedEventArgs e)
        {
            var entryTransactions = inventoryRepository.GetInventoryTransactionsForReport().Result.Where(t => t.Action == "Entrada");
            UpdateDataGridTransactions(entryTransactions, "Transacciones de entrada");
        }

        private void btn_FilterExitTransactions_Click(object sender, RoutedEventArgs e)
        {
            var exitTransactions = inventoryRepository.GetAllInventoryTransactions().Result.Where(t => t.Action == "Salida");
            UpdateDataGrid(exitTransactions, "Transacciones de salida");
        }

        private void btn_ShowAllTransactions_Click(object sender, RoutedEventArgs e)
        {
            var transactions = inventoryRepository.GetInventoryTransactionsForReport().Result;
            if (transactions.Any())
            {
                UpdateDataGrid(transactions, "Todas las transacciones");
            }
            else
            {
                MessageBox.Show("No se pudieron recuperar las transacciones de inventario.");
            }
        }

        private void btn_FilterZeroQuantity_Click(object sender, RoutedEventArgs e)
        {
            var zeroQuantityMaterials = inventoryRepository.GetInventoryTransactionsForReport().Result.Where(t => t.Quantity == 0);
            UpdateDataGrid(zeroQuantityMaterials, "Materiales con cantidad 0");
        }

        private void btn_FilterAvailableMaterials_Click(object sender, RoutedEventArgs e)
        {
            var availableMaterials = inventoryRepository.GetInventoryTransactionsForReport().Result.Where(t => t.Quantity > 0);
            UpdateDataGrid(availableMaterials, "Materiales disponibles");
        }

        private void btn_FilterPaidBillings_Click(object sender, RoutedEventArgs e)
        {
            var paidBillings = billingsRepository.GetAllBillingDetails().Result.Where(b => b.Billing.Remarks == "Pagado");
            UpdateDataGrid(paidBillings, "Facturas pagadas");
        }

        private void btn_FilterUnpaidBillings_Click(object sender, RoutedEventArgs e)
        {
            var unpaidBillings = billingsRepository.GetAllBillingDetails().Result.Where(b => b.Billing.Remarks != "Pagado");
            UpdateDataGrid(unpaidBillings, "Facturas sin pagar");
        }

    }
}