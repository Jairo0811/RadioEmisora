using System.Windows;
using System.Windows.Input;
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

            KeyDown += MainWindow_KeyDown;
        }

        private void AbrirAcerca()
        {
            AboutWindow ventana = new AboutWindow();
            ventana.Owner = this;
            ventana.ShowDialog();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F)
            {
                Sidebar.EnfocarBuscador();
                e.Handled = true;
                return;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.R)
            {
                viewModel.ActualizarCommand.Execute(null);
                e.Handled = true;
                return;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Q)
            {
                viewModel.SalirCommand.Execute(null);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Space)
            {
                viewModel.ReproducirCommand.Execute(null);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                viewModel.DetenerCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}