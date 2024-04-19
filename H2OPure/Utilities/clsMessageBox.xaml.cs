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
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace H2OPure.Utilities
{
    /// <summary>
    /// Lógica de interacción para clsMessageBox.xaml
    /// </summary>
    public partial class clsMessageBox : Window
    {
        public clsMessageBox(string message, string okButtonText, string cancelButtonText, string logoKind,string barText, Brush barColor)
        {
            InitializeComponent();

            // Asigna el mensaje y el texto de los botones
            MessageTextBlock.Text = message;
            OkButton.Content = okButtonText;
            CancelButton.Content = cancelButtonText;

            // Asigna el logo
            Logo.Kind = (MaterialDesignThemes.Wpf.PackIconKind)Enum.Parse(typeof(MaterialDesignThemes.Wpf.PackIconKind), logoKind);

            // Asigna el texto y el color de la barra
            BarText.Text = barText;
            BarColor.Background = barColor;

            // Maneja el evento del clic de los botones
            OkButton.Click += (sender, e) => { DialogResult = true; Close(); };
            CancelButton.Click += (sender, e) => { DialogResult = false; Close(); };
        }

    }
}
