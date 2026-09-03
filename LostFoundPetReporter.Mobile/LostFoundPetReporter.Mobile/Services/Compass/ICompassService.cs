using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Compass
{
    public interface ICompassService
    {
        bool IsSupported { get; }

        event EventHandler<double>? HeadingChanged;

        void Start();

        void Stop();
    }
}
