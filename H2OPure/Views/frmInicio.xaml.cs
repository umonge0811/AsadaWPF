using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace H2OPure.Views
{
    /// <summary>
    /// Lógica de interacción para frmInicio.xaml
    /// </summary>
    public partial class frmInicio : Page
    {
        public frmInicio()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(3))
            };
            txt_Block.BeginAnimation(Wpf.Ui.Controls.TextBlock.OpacityProperty, fadeInAnimation);

            // Espera un segundo después de que la animación de aparición termine
            await Task.Delay(TimeSpan.FromSeconds(2));

            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(3))
            };
            txt_Block.BeginAnimation(Wpf.Ui.Controls.TextBlock.OpacityProperty, fadeOutAnimation);


        }
    }
}
