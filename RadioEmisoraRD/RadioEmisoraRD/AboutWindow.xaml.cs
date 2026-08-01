using System.Reflection;
using System.Windows;

namespace RadioEmisoraRD
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionText.Text = version is null
                ? "Versión 3.1.0 (WPF)"
                : $"Versión {version.ToString(3)} (WPF)";
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
