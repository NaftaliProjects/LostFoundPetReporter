using LostFoundPetReporter.Mobile.Models.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Maps.Routing
{
    public interface IRouteProgressService
    {
        RouteProgress GetProgress(MapPoint currentLocation, IReadOnlyList<MapPoint> routePoints);

        double GetDistanceMeters(MapPoint a, MapPoint b);


    }
}
