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
using wpfASADACore.Services;
using wpfASADACore.Utilities;

namespace wpfASADACore.Views
{
    /// <summary>
    /// Lógica de interacción para frmLectura.xaml
    /// </summary>
    public partial class frmLectura : Page
    {
        ReadingRepository readingRepository =new ReadingRepository();

        public frmLectura()
        {
            InitializeComponent();
            dtp_Lectura.BlackoutDates.Add(new CalendarDateRange(DateTime.Now.AddDays(1), DateTime.MaxValue));

        }
        private int selectedClientId;
        private int selectedReadingId;


        private void LoadReadingsToDataGrid()
        {
            selectedClientId = (int)cmb_Client.SelectedValue;
            dtg_Lecturas.ItemsSource = readingRepository.GetReadingsByClient(selectedClientId);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

                var db = new ReadingRepository();
                var clients = db.GetClientsWithReadings();
                if (clients.Count > 0)
                {
                    cmb_Client.Items.Clear(); // Vacía la colección Items
                    cmb_Client.ItemsSource = clients;
                cmb_Client.DisplayMemberPath = "DisplayText";
                cmb_Client.SelectedValuePath = "Id";
                }
                else
                {
                    cmb_Client.DisplayMemberPath = "No hay Lecturas Registradas!";
                }           

        }

        private void ClearControls()
        {
            txtLecturaActual.Clear();
            dtp_Lectura.SelectedDate = null;
            txtLecturaAnterior.Clear();
            txtLecturaActual.Clear();

            
        }


        // En el controlador del evento del botón
        private async void btnCargarLectura_Click(object sender, RoutedEventArgs e)
        {
            LoadReadingsToDataGrid();
            var lastReading = readingRepository.GetLastReading(selectedClientId);
            if (lastReading != null)
            {
                txtLecturaAnterior.Text = lastReading.lastRead.ToString();
                selectedReadingId = lastReading.id;
            }
            else
            {
                await clsUtilities.ShowSnackbarAsync("El cliente seleccionado no tiene ninguna lectura actual pendiente", new SolidColorBrush(Colors.Yellow));
                txtLecturaAnterior.Text = "0";
                selectedReadingId = 0;
            }
        }

      
        private void cmb_Client_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnCargarLectura_Click(sender, e);
                txtLecturaActual.Focus();   
            }
        }

        private async void btnGenerarLectura_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtén la lectura actual del formulario
                int currentRead;
                if (!int.TryParse(txtLecturaActual.Text, out currentRead))
                {
                    await clsUtilities.ShowSnackbarAsync("Debe ingresar una lectura actual válida!", new SolidColorBrush(Colors.Yellow));
                    txtLecturaActual.Clear();
                    txtLecturaActual.Focus();
                    return;
                }

                DateTime? currentReadingDate = dtp_Lectura.SelectedDate;             


                // Actualiza la lectura actual
                var lastReading = readingRepository.GetLastReading(selectedClientId);
                if (lastReading != null)
                {
                    if (currentRead < lastReading.lastRead)
                    {
                        await clsUtilities.ShowSnackbarAsync("La lectura actual no puede ser menor que la última lectura!", new SolidColorBrush(Colors.Yellow));
                        txtLecturaActual.Clear();
                        txtLecturaActual.Focus();
                        return;
                    }

                    lastReading.CurrentRead = currentRead;
                    lastReading.CurrentReadingDate = currentReadingDate.Value;
                    lastReading.TotalConsumption = currentRead - lastReading.lastRead;
                    lastReading.ReadActiva = true; // Marca la lectura como inactiva
                    lastReading.Remarks = "Pendiente de Pago"; // Àctualiza
                    lastReading.IsChecked = true;
                    
                    
                    await readingRepository.UpdateReading(lastReading); // Asegúrate de implementar este método en tu repositorio
                }

                // Crea una nueva lectura
                await readingRepository.CreateReading(
                    currentReadingDate.Value, // Fecha de la última lectura
                    currentReadingDate.Value, // Fecha de la lectura actual
                    0, // Consumo total
                    currentRead, // Lectura anterior
                    0, // Lectura actual
                    "A espera de Lectura Final", // Observaciones
                    true, // Lectura Activa
                    lastReading.UserId, // ID del usuario
                    lastReading.idClient, // ID del cliente
                    lastReading.TypeClientId, // ID del tipo de cliente
                    lastReading.IsChecked // Estado de la lectura

                );

                // Actualiza el DataGrid
                LoadReadingsToDataGrid();

                // Muestra un mensaje de éxito
                await clsUtilities.ShowSnackbarAsync("Lectura creada con éxito", new SolidColorBrush(Colors.Green));
                ClearControls();
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error
                await clsUtilities.ShowSnackbarAsync($"Error al crear la lectura: {ex.Message}", new SolidColorBrush(Colors.Red));
            }
        }

        private void txtBuscarLectura_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtBuscarLectura.Text;
            if (search != "")
            {
                dtg_Lecturas.ItemsSource = readingRepository.GetReadingsByClient(selectedClientId).Where(r =>
                    r.Remarks.ToLower().Contains(search.ToLower()) ||
                    r.DateLastReading.ToString("d").Contains(search) ||
                    r.CurrentReadingDate.ToString("d").Contains(search)
                /* Agrega aquí otras propiedades de clsReading que quieras buscar */
                ).ToList();
            }
            else
            {
                dtg_Lecturas.ItemsSource = readingRepository.GetReadingsByClient(selectedClientId);
            }

            /*Este código busca en las propiedades Remarks, DateLastReading y CurrentReadingDate de clsReading.
             * Las fechas se convierten a cadenas con el formato “d” (que es la fecha corta) antes de buscar en ellas.
             * Esto significa que puedes buscar por fecha en el formato “MM/dd/yyyy”.*/
        }

        private void cmb_ClientRead_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            cmb_Client.IsDropDownOpen = true;

        }

        
    }
}
