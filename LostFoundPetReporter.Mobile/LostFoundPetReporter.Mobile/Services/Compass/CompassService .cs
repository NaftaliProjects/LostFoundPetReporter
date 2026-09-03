using Microsoft.Maui.Devices.Sensors;

namespace LostFoundPetReporter.Mobile.Services.Compass;

public class CompassService : ICompassService
{
    public bool IsSupported =>
        global::Microsoft.Maui.Devices.Sensors.Compass.Default.IsSupported;

    public event EventHandler<double>? HeadingChanged;

    public void Start()
    {
        var compass =
            global::Microsoft.Maui.Devices.Sensors.Compass.Default;

        if (!compass.IsSupported)
            return;

        if (compass.IsMonitoring)
            return;

        compass.ReadingChanged += OnReadingChanged;

        compass.Start(
            SensorSpeed.UI,
            applyLowPassFilter: true);
    }

    public void Stop()
    {
        var compass =
            global::Microsoft.Maui.Devices.Sensors.Compass.Default;

        if (!compass.IsMonitoring)
            return;

        compass.ReadingChanged -= OnReadingChanged;

        compass.Stop();
    }

    private void OnReadingChanged(
        object? sender,
        CompassChangedEventArgs e)
    {
        var heading =
            e.Reading.HeadingMagneticNorth;

        HeadingChanged?.Invoke(this, heading);
    }
}