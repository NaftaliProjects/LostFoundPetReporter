using LostFoundPetReporter.Mobile.Models.Map;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace LostFoundPetReporter.Mobile.Services.Maps.Routing
{
    public class RouteService : IRouteService
    {
        private readonly HttpClient _httpClient;

        public RouteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RouteResult?> GetRouteAsync(
    MapPoint start,
    MapPoint destination)
        {
            var startLongitude =
                start.Longitude.ToString(CultureInfo.InvariantCulture);

            var startLatitude =
                start.Latitude.ToString(CultureInfo.InvariantCulture);

            var destinationLongitude =
                destination.Longitude.ToString(CultureInfo.InvariantCulture);

            var destinationLatitude =
                destination.Latitude.ToString(CultureInfo.InvariantCulture);

            var url =
                $"route/v1/driving/" +
                $"{startLongitude},{startLatitude};" +
                $"{destinationLongitude},{destinationLatitude}" +
                "?overview=full&geometries=geojson";

            Debug.WriteLine($"ROUTING URL: {_httpClient.BaseAddress}{url}");

            var response = await _httpClient.GetAsync(url);

            Debug.WriteLine(
                $"ROUTING STATUS: {(int)response.StatusCode} {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("ROUTING RESPONSE:");
            Debug.WriteLine(json);

            if (!response.IsSuccessStatusCode)
                return null;

            try
            {
                var result = JsonSerializer.Deserialize<OsrmResponse>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );


                if (result == null)
                {
                    Debug.WriteLine("ROUTING: Deserialized result is NULL.");
                    return null;
                }

                Debug.WriteLine($"ROUTING CODE: {result.Code}");
                Debug.WriteLine(
                    $"ROUTING ROUTES COUNT: {result.Routes?.Count ?? 0}");

                if (result.Code != "Ok" ||
                    result.Routes == null ||
                    result.Routes.Count == 0)
                {
                    Debug.WriteLine("ROUTING: No valid route returned.");
                    return null;
                }

                var route = result.Routes[0];

                var routeResult = new RouteResult
                {
                    DistanceMeters = route.Distance,
                    DurationSeconds = route.Duration
                };

                if (route.Geometry?.Coordinates != null)
                {
                    foreach (var coordinate in route.Geometry.Coordinates)
                    {
                        if (coordinate.Count < 2)
                            continue;

                        // GeoJSON = [longitude, latitude]
                        routeResult.Points.Add(
                            new MapPoint(
                                coordinate[1],
                                coordinate[0]));
                    }
                }

                Debug.WriteLine(
                    $"ROUTING: Points parsed = {routeResult.Points.Count}");

                return routeResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"ROUTING DESERIALIZATION ERROR: {ex}");

                return null;
            }
        }

        private class OsrmResponse
        {
            public string? Code { get; set; }

            public List<OsrmRoute>? Routes { get; set; }
        }

        private class OsrmRoute
        {
            public double Distance { get; set; }

            public double Duration { get; set; }

            public OsrmGeometry? Geometry { get; set; }
        }

        private class OsrmGeometry
        {
            public string? Type { get; set; }

            public List<List<double>>? Coordinates { get; set; }
        }
    }
}
