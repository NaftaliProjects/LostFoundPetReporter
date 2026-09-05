using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class LostCoordinate
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override string ToString() => $"{Latitude:F5}, {Longitude:F5}";
    }
}
