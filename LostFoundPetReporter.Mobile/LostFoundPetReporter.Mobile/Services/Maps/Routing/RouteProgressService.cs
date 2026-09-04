using LostFoundPetReporter.Mobile.Models.Map;

using Mapsui;
using Mapsui.Projections;
using System.Diagnostics;


namespace LostFoundPetReporter.Mobile.Services.Maps.Routing
{
    public class RouteProgressService : IRouteProgressService
    {
        public RouteProgress GetProgress(MapPoint currentLocation, IReadOnlyList<MapPoint> routePoints)
        {
            if (routePoints.Count < 2)
            {
                return new RouteProgress
                {
                    ClosestPoint = currentLocation,
                    RemainingRoute = routePoints.ToList(),
                    DistanceFromRouteMeters = 0,
                    SegmentIndex = 0
                };
            }

            var closest = FindClosestPointOnRoute(
                currentLocation,
                routePoints);


            if (closest == null)
            {
                return new RouteProgress
                {
                    ClosestPoint = currentLocation,
                    RemainingRoute = routePoints.ToList(),
                    DistanceFromRouteMeters = double.MaxValue,
                    SegmentIndex = 0
                };
            }

            var remainingRoute = BuildRemainingRoute(
                closest.Value.Point,
                closest.Value.SegmentIndex,
                routePoints);

            return new RouteProgress
            {
                ClosestPoint = closest.Value.Point,
                RemainingRoute = remainingRoute,
                DistanceFromRouteMeters =
                    closest.Value.DistanceMeters,
                SegmentIndex =
                    closest.Value.SegmentIndex
            };
        }




        private static List<MapPoint> BuildRemainingRoute(MapPoint closestPoint, int segmentIndex, IReadOnlyList<MapPoint> routePoints)
        {
            var result = new List<MapPoint> { closestPoint };
            
            for (var i = segmentIndex + 1;
                 i < routePoints.Count;
                 i++)
            {
                result.Add(routePoints[i]);
            }

            return result;
        }




        private static (MapPoint Point, int SegmentIndex, double DistanceMeters)? FindClosestPointOnRoute(MapPoint currentLocation, IReadOnlyList<MapPoint> routePoints)
        {
            var current = ToMapPoint(currentLocation);

            double bestDistanceSquared = double.MaxValue;

            MPoint? bestPoint = null;
            var bestSegmentIndex = -1;

            for (var i = 0; i < routePoints.Count - 1; i++)
            {
                var a = ToMapPoint(routePoints[i]);
                var b = ToMapPoint(routePoints[i + 1]);

                var closest =
                    ClosestPointOnSegment(
                        current,
                        a,
                        b);

                var dx = current.X - closest.X;
                var dy = current.Y - closest.Y;

                var distanceSquared =
                    dx * dx +
                    dy * dy;

                if (distanceSquared >= bestDistanceSquared)
                    continue;

                bestDistanceSquared = distanceSquared;
                bestPoint = closest;
                bestSegmentIndex = i;
            }

            if (bestPoint == null)
                return null;

            var geographic =
                SphericalMercator.ToLonLat(bestPoint);

            var closestMapPoint =
                new MapPoint(
                    geographic.Y,
                    geographic.X);

            return (
                closestMapPoint,
                bestSegmentIndex,
                ProjectedDistanceToMeters(
                    current,
                    bestPoint));
        }





        private static MPoint ToMapPoint(MapPoint point)
        {
            return SphericalMercator.FromLonLat(
                new MPoint(
                    point.Longitude,
                    point.Latitude));
        }



        private static MPoint ClosestPointOnSegment(MPoint p, MPoint a, MPoint b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;

            if (dx == 0 && dy == 0)
                return a;

            var t =
                ((p.X - a.X) * dx +
                 (p.Y - a.Y) * dy)
                /
                (dx * dx + dy * dy);

            t = Math.Clamp(t, 0, 1);

            return new MPoint(
                a.X + t * dx,
                a.Y + t * dy);
        }

        private static double ProjectedDistanceToMeters(MPoint a, MPoint b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;

            var projectedDistance =
                Math.Sqrt(dx * dx + dy * dy);

            // Web Mercator is distorted depending on latitude.
            var latitude = SphericalMercator.ToLonLat(a).Y;


            var latitudeRadians =
                latitude * Math.PI / 180.0;

            return projectedDistance *
                   Math.Cos(latitudeRadians);
        }


        public static double DistanceMeters(MapPoint a, MapPoint b)
        {
            const double earthRadius = 6371000;

            var lat1 = a.Latitude * Math.PI / 180;
            var lat2 = b.Latitude * Math.PI / 180;

            var deltaLat =
                (b.Latitude - a.Latitude) *
                Math.PI / 180;

            var deltaLon =
                (b.Longitude - a.Longitude) *
                Math.PI / 180;

            var sinLat = Math.Sin(deltaLat / 2);
            var sinLon = Math.Sin(deltaLon / 2);

            var h =
                sinLat * sinLat +
                Math.Cos(lat1) *
                Math.Cos(lat2) *
                sinLon * sinLon;

            return 2 * earthRadius *
                   Math.Asin(Math.Sqrt(h));
        }
    }
}
