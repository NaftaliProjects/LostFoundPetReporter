using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models.Map
{
    public class RouteResult
    {
        public List<MapPoint> Points { get; set; } = new();

        public double DistanceMeters { get; set; }

        public double DurationSeconds { get; set; }
    }
}
