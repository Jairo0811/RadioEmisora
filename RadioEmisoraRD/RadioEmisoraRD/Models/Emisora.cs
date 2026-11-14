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
        set
        {
            if (SetField(ref esFavorita, value))
                OnPropertyChanged(nameof(DescripcionAccesible));
        }
    }

    public bool EstaReproduciendo
    {
        get => estaReproduciendo;
        set
        {
            if (!SetField(ref estaReproduciendo, value))
                return;

            OnPropertyChanged(nameof(EstadoCard));
            OnPropertyChanged(nameof(DescripcionAccesible));
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
            OnPropertyChanged(nameof(DescripcionAccesible));
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

    /// <summary>
    /// Texto que leen los lectores de pantalla para cada tarjeta del catálogo.
    /// </summary>
    public string DescripcionAccesible
    {
        get
        {
            string estado = EstaReproduciendo ? "sonando" : EstaPausada ? "en pausa" : "en vivo";
            string favorita = EsFavorita ? ", favorita" : string.Empty;
            return $"{Nombre}, {Frecuencia}, {Categoria}, {estado}{favorita}";
        }
    }

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
