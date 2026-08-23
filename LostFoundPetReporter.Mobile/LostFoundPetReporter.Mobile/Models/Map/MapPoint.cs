using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models.Map
{
    public class MapPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string? Title { get; set; }
    }
}
