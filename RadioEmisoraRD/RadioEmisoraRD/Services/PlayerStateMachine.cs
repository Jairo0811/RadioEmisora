using RadioEmisoraRD.Models;

namespace RadioEmisoraRD.Services;

public sealed class PlayerStateChangedEventArgs : EventArgs
{
    public PlayerStateChangedEventArgs(PlayerState previous, PlayerState current, string? detail)
    {
        Previous = previous;
        Current = current;
        Detail = detail;
    }

    public PlayerState Previous { get; }

    public PlayerState Current { get; }

    public string? Detail { get; }
}

public sealed class PlayerStateMachine
{
    private readonly object syncRoot = new();
    private PlayerState currentState = PlayerState.Detenido;

    public event EventHandler<PlayerStateChangedEventArgs>? StateChanged;

    public PlayerState CurrentState
    {
        get
        {
            lock (syncRoot)
            {
                return currentState;
            }
        }
    }

    public bool TransitionTo(PlayerState nextState, string? detail = null)
    {
        PlayerState previousState;

        lock (syncRoot)
        {
            if (currentState == nextState && string.IsNullOrWhiteSpace(detail))
                return false;

            previousState = currentState;
            currentState = nextState;
        }

        StateChanged?.Invoke(
            this,
            new PlayerStateChangedEventArgs(previousState, nextState, detail));
        return true;
    }
}
