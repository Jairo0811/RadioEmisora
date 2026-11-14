using System.Globalization;
using System.Windows.Data;

namespace RadioEmisoraRD.Helpers;

/// <summary>
/// Devuelve true cuando todos los valores enlazados son iguales (comparación ordinal de texto).
/// Se usa para marcar el filtro activo sin duplicar propiedades en el ViewModel.
/// </summary>
public sealed class EqualityMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length < 2)
            return false;

        string? first = values[0]?.ToString();
        return values.Skip(1).All(value => string.Equals(first, value?.ToString(), StringComparison.Ordinal));
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
