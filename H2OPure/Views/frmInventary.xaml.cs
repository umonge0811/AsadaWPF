using H2OPure.Models;
using H2OPure.Repository;
using H2OPure.Utilities;
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

namespace H2OPure.Views
{
    /// <summary>
    /// Lógica de interacción para frmInventary.xaml
    /// </summary>
    public partial class frmInventary : Page
    {

        InventoryRepository inventoryRepository = new InventoryRepository();
        public frmInventary()
        {
            InitializeComponent();
        }



        public async void UpdateDataGrid()
        {
            // Crear una nueva instancia de InventoryRepository
            InventoryRepository inventoryRepository = new InventoryRepository();

            // Obtener todas las transacciones de inventario de la base de datos
            var transactions = await inventoryRepository.GetAllInventoryTransactions();

            // Actualizar el DataGrid
            dgInventory.ItemsSource = transactions;
        }


        private async void btn_InventoryIN_Click(object sender, RoutedEventArgs e)
        {

            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync();
            // Verificar si los campos necesarios están llenos
            if (string.IsNullOrEmpty(cmb_MaterialIn.Text) || string.IsNullOrEmpty(txt_QuantityIn.Text) || cmb_DepartIN.SelectedItem == null)
            {
                await clsUtilities.ShowSnackbarAsync("Por favor, ingresa todos los datos necesarios.", new SolidColorBrush(Colors.Yellow));
                return;
            }

            // Obtener el departamento seleccionado y el material ingresado
            string selectedDepartment = ((ComboBoxItem)cmb_DepartIN.SelectedItem).Content.ToString();
            string enteredMaterial = cmb_MaterialIn.Text;

            // Crear un nuevo objeto clsInventory con los datos del formulario
            clsInventory newInventory = new clsInventory
            {
                materialName = enteredMaterial,
                quantity = int.Parse(txt_QuantityIn.Text),
                entryDate = DateTime.Now,
                Department = selectedDepartment,
                userId = Utilities.clsUtilities.UserIdLog,
                Remarks = txt_RemarkIN.Text,
                action = "Entrada"
            };

            // Crear una nueva instancia de InventoryRepository
            InventoryRepository inventoryRepository = new InventoryRepository();

            // Guardar el nuevo inventario en la base de datos
            await inventoryRepository.AddInventory(newInventory);

            // Crear una nueva transacción de entrada
            clsInventoryTransaction newTransaction = new clsInventoryTransaction
            {
                MaterialName = newInventory.materialName,
                Quantity = newInventory.quantity,
                TransactionDate = DateTime.Now,
                Department = newInventory.Department,
                UserId = newInventory.userId,
                Remarks = newInventory.Remarks,
                Action = "Entrada"
            };
            await inventoryRepository.AddInventoryTransaction(newTransaction);

            await clsUtilities.ShowSnackbarAsync("Inventario Actualizado exitosamente", new SolidColorBrush(Colors.LightGreen));

            // Actualizar el DataGrid
            UpdateDataGrid();
            LoadAvailableMaterialsOUT();
            ClearControlsIN();
        }


        private void ClearControlsIN()
        {
            cmb_MaterialIn.Text = "";
            txt_QuantityIn.Text = "";
            cmb_DepartIN.SelectedIndex = -1;
            txt_RemarkIN.Text = "";
        }
        private void ClearControlsOUT()
        {
            // Limpiar los ComboBox
            cmb_ItemsOUT.Text = "";
            cmb_DepartOUT.Text = "";

            // Limpiar el NumberBox
            nbx_CantidadOUT.Value = 0;

            // Aquí puedes agregar más controles a limpiar si es necesario
        }



        //carga los materiales disponibles en el combobox INGRESO
        private async void LoadMaterialsIN()
        {
            // Obtener todos los materiales disponibles
            var availableMaterials = await inventoryRepository.GetAllMaterialsIN();

            // Limpiar el ComboBox
            cmb_MaterialIn.Items.Clear();

            // Verificar si hay materiales disponibles
            if (availableMaterials != null && availableMaterials.Count > 0)
            {
                // Agregar los nombres de los materiales al ComboBox
                foreach (var item in availableMaterials)
                {
                    cmb_MaterialIn.Items.Add(item.materialName);
                }
            }
            else
            {
                // Mostrar un mensaje al usuario
                await clsUtilities.ShowSnackbarAsync("No Hay Materiales Disponibles en Inventario", new SolidColorBrush(Colors.Yellow));
            }
        }
        //CARGA LOS MATERIALES DISPONIBLES EN EL COMBOBOX SALIDA
        private async void LoadAvailableMaterialsOUT()
        {
            // Verificar si se seleccionó un departamento
            if (cmb_DepartOUT.SelectedItem != null)
            {
                // Obtener el departamento seleccionado
                string selectedDepartment = ((ComboBoxItem)cmb_DepartOUT.SelectedItem).Content.ToString();

                // Obtener todos los materiales para el departamento seleccionado
                var allMaterials = await inventoryRepository.GetAllMaterialsIN();

                // Limpiar el ComboBox
                cmb_ItemsOUT.Items.Clear();

                // Verificar si hay materiales disponibles
                if (allMaterials != null && allMaterials.Count > 0)
                {
                    // Para cada material, calcular la cantidad disponible y agregarlo al ComboBox
                    foreach (var item in allMaterials)
                    {
                        int availableQuantity = await inventoryRepository.GetAvailableQuantity(item.materialName, selectedDepartment);
                        if (availableQuantity > 0)
                        {
                            cmb_ItemsOUT.Items.Add($"{item.materialName} - Cantidad disponible: {availableQuantity}");
                        }
                    }
                }
                else
                {
                    // Mostrar un mensaje al usuario
                    await clsUtilities.ShowSnackbarAsync("Sin Inventario Disponible!", new SolidColorBrush(Colors.Yellow));
                }
            }
        }




        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid();
            LoadMaterialsIN();

           


        }

        private void cmb_ItemsOUT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            comboBox.IsDropDownOpen = true;
        }

        private void cmb_ItemsIN_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            comboBox.IsDropDownOpen = true;
        }




        private async void btn_InventaOUT_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar barra de progreso
            await clsUtilities.ShowProgressBarAsync();
            // Verificar si los campos necesarios están llenos
            if (string.IsNullOrEmpty(cmb_ItemsOUT.Text) || string.IsNullOrEmpty(nbx_CantidadOUT.Text) || cmb_DepartOUT.SelectedItem == null)
            {
                await clsUtilities.ShowSnackbarAsync("Por favor, ingresa todos los datos necesarios.", new SolidColorBrush(Colors.Yellow));
                return;
            }

            // Obtener la cadena seleccionada
            string selectedText = cmb_ItemsOUT.Text;

            // Dividir la cadena seleccionada en el nombre del material y la cantidad disponible
            string[] parts = selectedText.Split('-');

            // El nombre del material es la primera parte
            string selectedMaterial = parts[0].Trim();

            // Verificar si el material seleccionado es válido
            if (string.IsNullOrEmpty(selectedMaterial))
            {
                await clsUtilities.ShowSnackbarAsync("Por favor, selecciona un material válido.", new SolidColorBrush(Colors.Yellow));
                return;
            }

            // Obtener el departamento seleccionado
            string selectedDepartment = cmb_DepartOUT.Text;

            // Verificar si el departamento seleccionado es válido
            if (string.IsNullOrEmpty(selectedDepartment))
            {
                await clsUtilities.ShowSnackbarAsync("Por favor, selecciona un departamento válido.", new SolidColorBrush(Colors.Yellow));
                return;
            }

            // Crear un nuevo objeto clsInventory con los datos del formulario
            clsInventory inventoryToRemove = new clsInventory
            {
                materialName = selectedMaterial,
                Department = selectedDepartment,
                quantity = int.Parse(nbx_CantidadOUT.Text), // Asegúrate de tener un control para la cantidad a remover
                Remarks = txt_MotivoOUT.Text,
                exitDate = DateTime.Now,
                userId = Utilities.clsUtilities.UserIdLog,
                action = "Salida"
            };

            // Crear una nueva instancia de InventoryRepository
            InventoryRepository inventoryRepository = new InventoryRepository();

            // Obtener la cantidad disponible del material
            int availableQuantity = await inventoryRepository.GetAvailableQuantity(selectedMaterial, selectedDepartment);

            // Verificar si hay suficiente cantidad disponible
            if (availableQuantity >= inventoryToRemove.quantity)
            {
                // Si hay suficiente cantidad, agregar una nueva transacción de salida
                clsInventoryTransaction newTransaction = new clsInventoryTransaction
                {
                    MaterialName = inventoryToRemove.materialName,
                    Quantity = inventoryToRemove.quantity,
                    TransactionDate = DateTime.Now,
                    Department = inventoryToRemove.Department,
                    UserId = inventoryToRemove.userId,
                    Remarks = inventoryToRemove.Remarks,
                    Action = "Salida"
                };
                await inventoryRepository.AddInventoryTransaction(newTransaction);

                // Mostrar un mensaje de éxito
                await clsUtilities.ShowSnackbarAsync("Inventario Actualizado exitosamente", new SolidColorBrush(Colors.LightGreen));

                // Actualizar el DataGrid
                UpdateDataGrid();
                ClearControlsOUT();
            }
            else
            {
                // Si no hay suficiente cantidad, mostrar un mensaje de advertencia
                await clsUtilities.ShowSnackbarAsync("Sin Inventario Disponible!", new SolidColorBrush(Colors.Yellow));
            }
        }

        private async void cmb_DepartOUT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cargar los materiales disponibles para el departamento seleccionado
            LoadAvailableMaterialsOUT();         


        }

       
    }
}
