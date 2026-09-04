using LostFoundPetReporter.Mobile.Models.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Maps.Routing
{
    public class RouteProgress
    {
        public MapPoint ClosestPoint { get; init; } = default!;

        public List<MapPoint> RemainingRoute { get; init; } = new();

        public double DistanceFromRouteMeters { get; init; }

        public int SegmentIndex { get; init; }
    }
}
