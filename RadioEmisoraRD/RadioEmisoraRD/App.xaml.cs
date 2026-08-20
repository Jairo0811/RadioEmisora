using System.Windows;
using System.Windows.Threading;
using RadioEmisoraRD.Services;

namespace RadioEmisoraRD;

public partial class App : Application
{
    private readonly AppLogger logger = AppLogger.Current;

    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        base.OnStartup(e);

        if (TryGetCaptureDirectory(e.Args, out string captureDirectory))
        {
            ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            _ = RunPortfolioCaptureAsync(captureDirectory);
            return;
        }

        try
        {
            var window = new MainWindow();
            MainWindow = window;
            window.Show();
            logger.Info("RadioEmisora RD inició correctamente.");
        }
        catch (Exception exception)
        {
            logger.LogError("No se pudo iniciar la aplicación.", exception);
            MessageBox.Show(
                "RadioEmisora RD no pudo iniciar. Revisa el archivo de registro para obtener detalles.",
                "Error de inicio",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        DispatcherUnhandledException -= OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException -= OnDomainUnhandledException;
        TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        logger.Info("RadioEmisora RD finalizó.");
        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        logger.LogError("Excepción no controlada en la interfaz.", e.Exception);
        e.Handled = true;
        MessageBox.Show(
            "Ocurrió un problema inesperado, pero la aplicación se recuperó. " +
            "Si vuelve a suceder, consulta el registro local.",
            "RadioEmisora RD",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private void OnDomainUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            logger.LogError("Excepción no controlada del proceso.", exception);
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        logger.LogError("Excepción no observada en una tarea asíncrona.", e.Exception);
        e.SetObserved();
    }

    private async Task RunPortfolioCaptureAsync(string outputDirectory)
    {
        try
        {
            await PortfolioCaptureService.CaptureAsync(outputDirectory);
            logger.Info($"Capturas de portafolio generadas en '{outputDirectory}'.");
            Shutdown(0);
        }
        catch (Exception exception)
        {
            logger.LogError("No se pudieron generar las capturas de portafolio.", exception);
            Shutdown(2);
        }
    }

    private static bool TryGetCaptureDirectory(string[] arguments, out string directory)
    {
        if (arguments.Length >= 2 &&
            string.Equals(arguments[0], "--capture-docs", StringComparison.OrdinalIgnoreCase))
        {
            string candidate = Path.GetFullPath(arguments[1]);
            string workingDirectory = Path.GetFullPath(Environment.CurrentDirectory)
                .TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (candidate.StartsWith(workingDirectory, StringComparison.OrdinalIgnoreCase))
            {
                directory = candidate;
                return true;
            }
        }

        directory = string.Empty;
        return false;
    }
}
