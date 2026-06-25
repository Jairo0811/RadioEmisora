using System.Windows;
using RadioEmisoraRD.ViewModels;

namespace RadioEmisoraRD
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel();

            viewModel.RequestAbout += AbrirAcerca;
            viewModel.RequestExit += Close;

            DataContext = viewModel;
        }

        private void AbrirAcerca()
        {
            AboutWindow ventana = new AboutWindow();
            ventana.Owner = this;
            ventana.ShowDialog();
        }
    }
}