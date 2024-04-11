﻿using MaterialDesignThemes.Wpf;
using System.Text;
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
using wpfASADACore.Services;


namespace wpfASADACore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //_ = CrearUsuario();
        }


        // Propiedad pública para exponer tu ProgressBarPrincipal
        public ProgressBar GlobalProgressBar
        {
            get { return ProgressBarPrincipal; }
        }

        // Propiedad pública para exponer tu SnackbarMessagePrincipal
        public Snackbar SnackbarMessageGlobal
        {
            get { return SnackbarMessagePrincipal; }
        }


        private void NavigationViewItem_Click(object sender, RoutedEventArgs e)
        {
            //RootNavigation.Navigate(typeof(View.frmUsuarios));
            RootNavigation.Navigate(typeof(Views.frmUsers));

        }

        private void NavigationViewItem_Click_1(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(Views.frmClient));

        }

        private void NavigationViewItem_Click_2(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(Views.TypeClient));
        }
    }
}