using H2OPure.Models;
using H2OPure.Repository;
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

        private async void LoadMaterials()
        {
            // Obtener los materiales disponibles
            var availableMaterials = await inventoryRepository.GetAvailableMaterials();

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
                MessageBox.Show("No hay materiales disponibles en el inventario.");
            }
        }

        public async void UpdateDataGrid()
        {
            // Crear una nueva instancia de InventoryRepository
            InventoryRepository inventoryRepository = new InventoryRepository();

            // Obtener todos los inventarios de la base de datos
            var inventories = await inventoryRepository.GetAllInventory();

            // Actualizar el DataGrid
            dgInventory.ItemsSource = inventories;
        }


        private async void btn_InventoryIN_Click(object sender, RoutedEventArgs e)
        {
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
                userId = Utilities.clsUtilities.UserIdLog, // Aquí debes poner el ID del usuario actual
                Remarks = txt_RemarkIN.Text // Aquí debes poner el nombre de tu TextBox para las observaciones
            };

            // Crear una nueva instancia de InventoryRepository
            InventoryRepository inventoryRepository = new InventoryRepository();

            // Guardar el nuevo inventario en la base de datos
            await inventoryRepository.AddInventory(newInventory);

            // Actualizar el DataGrid
            UpdateDataGrid();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid();
            LoadMaterials();
        }




        //private async void btn_InventorOUT_Click(object sender, RoutedEventArgs e)
        //{
        //    // Obtener el inventario seleccionado
        //    clsInventory selectedInventory = cmb_ItemsOUT.SelectedItem as clsInventory;

        //    // Actualizar la cantidad y la fecha de salida
        //    selectedInventory.quantity -= int.Parse(txt_QuantityOut.Text);
        //    selectedInventory.exitDate = DateTime.Now;

        //    // Guardar los cambios en la base de datos
        //    await _inventoryRepository.UpdateInventory(selectedInventory);

        //    // Actualizar el DataGrid
        //    dgInventory.ItemsSource = await context.inventories.ToListAsync();
        //}
    }
}
