using System.Windows.Controls;
using System.Windows.Input;
using RadioEmisoraRD.ViewModels;

namespace RadioEmisoraRD.Controls.Sidebar
{
    public partial class SidebarControl : UserControl
    {
        public SidebarControl()
        {
            InitializeComponent();
        }

        public void EnfocarBuscador()
        {
            txtBuscar.Focus();
            txtBuscar.SelectAll();
        }

        private void ListBoxEmisoras_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            ReproducirDesdeSidebar();
            e.Handled = true;
        }

        private void ListBoxEmisoras_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReproducirDesdeSidebar();
            e.Handled = true;
        }

        private void ReproducirDesdeSidebar()
        {
            if (DataContext is MainViewModel vm && vm.ReproducirCommand.CanExecute(null))
            {
                vm.ReproducirCommand.Execute(null);
            }
        }
    }
}