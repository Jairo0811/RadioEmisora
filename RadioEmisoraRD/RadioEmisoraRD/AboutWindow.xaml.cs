using System.Reflection;
using System.Windows;

namespace RadioEmisoraRD
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            VersionText.Text = $"Versión {GetDisplayVersion()} · WPF · .NET 10";
        }

        internal static string GetDisplayVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string? informational = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            if (!string.IsNullOrWhiteSpace(informational))
                return informational.Split('+')[0];

            return assembly.GetName().Version?.ToString(3) ?? "desconocida";
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
