using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using RadioEmisoraRD.ViewModels;

namespace RadioEmisoraRD;

public partial class MainWindow : Window
{
    private readonly MainViewModel viewModel;

    public MainWindow()
        : this(new MainViewModel())
    {
    }

    internal MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        this.viewModel.RequestAbout += OpenAbout;
        this.viewModel.RequestExit += OnExitRequested;
        DataContext = this.viewModel;
        KeyDown += OnMainWindowKeyDown;
        Closed += OnWindowClosed;
    }

    private void OpenAbout()
    {
        var window = new AboutWindow
        {
            Owner = this
        };
        window.ShowDialog();
    }

    private void OnExitRequested() => Close();

    internal FrameworkElement GetHistoryCaptureTarget() =>
        Dashboard.FindName("HistoryPanel") as FrameworkElement ?? Dashboard;

    private void OnMainWindowKeyDown(object sender, KeyEventArgs e)
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

        if (e.Key == Key.Space &&
            Keyboard.FocusedElement is not TextBoxBase and not ButtonBase and not Slider)
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

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        Closed -= OnWindowClosed;
        KeyDown -= OnMainWindowKeyDown;
        viewModel.RequestAbout -= OpenAbout;
        viewModel.RequestExit -= OnExitRequested;
        viewModel.Dispose();
    }
}
