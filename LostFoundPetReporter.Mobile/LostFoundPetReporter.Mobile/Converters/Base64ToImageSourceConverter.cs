using System.Globalization;

namespace LostFoundPetReporter.Mobile.Converters;

public class Base64ToImageSourceConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not string base64 ||
            string.IsNullOrWhiteSpace(base64))
        {
            System.Diagnostics.Debug.WriteLine(
                "Base64 converter: empty value");

            return null;
        }

        try
        {
            System.Diagnostics.Debug.WriteLine(
                $"Base64 converter: length = {base64.Length}");

            // Handle:
            // data:image/jpeg;base64,/9j/4AAQ...
            if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                var commaIndex = base64.IndexOf(',');

                if (commaIndex >= 0)
                    base64 = base64[(commaIndex + 1)..];
            }

            byte[] bytes = System.Convert.FromBase64String(base64);

            System.Diagnostics.Debug.WriteLine(
                $"Base64 converter: decoded {bytes.Length} bytes");

            return ImageSource.FromStream(() =>
            {
                return new MemoryStream(bytes);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Base64 converter ERROR: {ex}");

            return null;
        }
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}