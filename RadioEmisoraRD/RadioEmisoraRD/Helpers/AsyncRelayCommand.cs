using System.Windows.Input;

namespace RadioEmisoraRD.Helpers;

public sealed class AsyncRelayCommand : ICommand, IDisposable
{
    private readonly Func<object?, CancellationToken, Task> execute;
    private readonly Predicate<object?>? canExecute;
    private readonly Action<Exception>? onError;
    private readonly bool cancelPrevious;
    private CancellationTokenSource? executionCancellation;
    private bool isExecuting;
    private bool disposed;

    public AsyncRelayCommand(
        Func<object?, CancellationToken, Task> execute,
        Predicate<object?>? canExecute = null,
        Action<Exception>? onError = null,
        bool cancelPrevious = false)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
        this.onError = onError;
        this.cancelPrevious = cancelPrevious;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) =>
        !disposed && (cancelPrevious || !isExecuting) && (canExecute?.Invoke(parameter) ?? true);

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        if (cancelPrevious)
            executionCancellation?.Cancel();

        var currentCancellation = new CancellationTokenSource();
        executionCancellation = currentCancellation;
        isExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            await execute(parameter, currentCancellation.Token);
        }
        catch (OperationCanceledException) when (currentCancellation.IsCancellationRequested)
        {
            // La cancelación forma parte del cambio rápido entre emisoras.
        }
        catch (Exception exception)
        {
            onError?.Invoke(exception);
        }
        finally
        {
            if (ReferenceEquals(executionCancellation, currentCancellation))
            {
                executionCancellation = null;
                isExecuting = false;
                RaiseCanExecuteChanged();
            }

            currentCancellation.Dispose();
        }
    }

    public void Cancel() => executionCancellation?.Cancel();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        executionCancellation?.Cancel();
        executionCancellation?.Dispose();
        executionCancellation = null;
        RaiseCanExecuteChanged();
        GC.SuppressFinalize(this);
    }
}
