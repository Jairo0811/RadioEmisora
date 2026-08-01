using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace RadioEmisoraRD.Models;

public sealed class Emisora : INotifyPropertyChanged
{
    private bool estaReproduciendo;
    private bool estaPausada;
    private bool esFavorita;

    public string Id { get; }

    public string Nombre { get; }

    public string Frecuencia { get; }

    public string Categoria { get; }

    public string Provincia { get; }

    public string Grupo { get; }

    public string Logo { get; }

    public string StreamUrl { get; }

    public Color ColorTema { get; }

    public bool EsFavorita
    {
        get => esFavorita;
        set => SetField(ref esFavorita, value);
    }

    public bool EstaReproduciendo
    {
        get => estaReproduciendo;
        set
        {
            if (!SetField(ref estaReproduciendo, value))
                return;

            OnPropertyChanged(nameof(EstadoCard));
        }
    }

    public bool EstaPausada
    {
        get => estaPausada;
        set
        {
            if (!SetField(ref estaPausada, value))
                return;

            OnPropertyChanged(nameof(EstadoCard));
        }
    }

    public string EstadoCard
    {
        get
        {
            if (EstaReproduciendo)
                return "▶ SONANDO";

            if (EstaPausada)
                return "⏸ PAUSADA";

            return Estado;
        }
    }

    public string Estado => string.IsNullOrWhiteSpace(StreamUrl) ? "● OFFLINE" : "● EN VIVO";

    public Emisora(
        string id,
        string nombre,
        string frecuencia,
        string categoria,
        string provincia,
        string grupo,
        string logo,
        Color colorTema,
        string streamUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        Id = id;
        Nombre = nombre;
        Frecuencia = frecuencia;
        Categoria = categoria;
        Provincia = provincia;
        Grupo = grupo;
        Logo = logo;
        ColorTema = colorTema;
        StreamUrl = streamUrl;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField(ref bool field, bool value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
