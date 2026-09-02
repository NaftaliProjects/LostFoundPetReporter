using LostFoundPetReporter.Mobile.Models.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Maps.Routing
{
    public interface IRouteService
    {
        Task<RouteResult?> GetRouteAsync(
            MapPoint start,
            MapPoint destination);
    }
}
