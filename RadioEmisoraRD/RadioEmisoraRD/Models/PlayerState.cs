namespace RadioEmisoraRD.Models;

public enum PlayerState
{
    Detenido,
    Conectando,
    Buffering,
    Reproduciendo,
    Pausado,
    Reconectando,
    Error
}

public static class PlayerStateExtensions
{
    public static string ToDisplayText(this PlayerState state) => state switch
    {
        PlayerState.Detenido => "DETENIDO",
        PlayerState.Conectando => "CONECTANDO",
        PlayerState.Buffering => "BUFFERING",
        PlayerState.Reproduciendo => "REPRODUCIENDO",
        PlayerState.Pausado => "PAUSADO",
        PlayerState.Reconectando => "RECONECTANDO",
        PlayerState.Error => "ERROR",
        _ => "DESCONOCIDO"
    };
}
