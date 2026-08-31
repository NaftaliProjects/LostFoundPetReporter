
using LostFoundPetReporter.API.Services.Notification;
using LostFoundPetReporter.CoreDb.ReposInterfaces;


namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class MatchingService : IMatchingService
    {
        private readonly IFoundReportRepo _foundRepo;
        private readonly ILostReportRepo _lostRepo;
        private readonly ILostFoundMatchRepo _matchRepo;
        private readonly IPushNotificationService _pushNotificationService;

        private const double MinimumMatchScore = 0.70;

        public MatchingService(
            IFoundReportRepo foundRepo,
            ILostReportRepo lostRepo,
            ILostFoundMatchRepo matchRepo,
            IPushNotificationService pushNotificationService)
        {
            _foundRepo = foundRepo;
            _lostRepo = lostRepo;
            _matchRepo = matchRepo;
            _pushNotificationService = pushNotificationService;
        }

        // Direction 1: One FoundReport against many LostReports
        public async Task TryMatchLostReportAsync(int lostReportId, CancellationToken cancellationToken = default)
        {
            var lostReport = _lostRepo.Find(lostReportId);

            if (lostReport == null)
                return;

            cancellationToken.ThrowIfCancellationRequested();

            var fromDate = lostReport.dateTime.AddDays(-1);
            var toDate = lostReport.dateTime.AddMonths(3);

            var candidateFoundReports = _foundRepo.GetReportsInDateRange(fromDate, toDate);




            if (!candidateFoundReports.Any())
                return;

            var newMatches = new List<LostFoundMatch>();

            var existingFoundReportIds = _matchRepo.GetFoundReportIdsForLostReport(lostReport.Id);
            var existingIds = existingFoundReportIds.ToHashSet();

            foreach (var foundReport in candidateFoundReports)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (existingIds.Contains(foundReport.Id))
                    continue;


                var result = CalculateMatchScore(
                    foundReport,
                    lostReport);

                if (result.Score < MinimumMatchScore)
                    continue;

                newMatches.Add(new LostFoundMatch
                {
                    FoundReportId = foundReport.Id,
                    LostReportId = lostReport.Id,
                    Score = result.Score,
                    MatchReason = result.MatchReason
                });
            }

            if (newMatches.Count > 0)
            {
                _matchRepo.AddRange(newMatches);

                await _pushNotificationService.SendMatchNotificationAsync(
                    lostReport.UserId,
                    newMatches,
                    cancellationToken);
            }
        }


        // Direction 2: One LostReport against many FoundReports
        public async Task TryMatchFoundReportAsync(int foundReportId, CancellationToken cancellationToken = default)
        {
            var foundReport = _foundRepo.Find(foundReportId);

            if (foundReport == null)
                return;

            cancellationToken.ThrowIfCancellationRequested();

            var fromDate = foundReport.dateTime.AddMonths(-3);
            var toDate = foundReport.dateTime.AddDays(1);

            var candidateLostReports =
                _lostRepo.GetReportsInDateRange(fromDate, toDate);

            if (!candidateLostReports.Any())
                return;

            var newMatches = new List<LostFoundMatch>();

            foreach (var lostReport in candidateLostReports)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_matchRepo.MatchExists(
                        lostReport.Id,
                        foundReport.Id))
                {
                    continue;
                }

                var result = CalculateMatchScore(
                    foundReport,
                    lostReport);

                if (result.Score < MinimumMatchScore)
                    continue;

                newMatches.Add(new LostFoundMatch
                {
                    FoundReportId = foundReport.Id,
                    LostReportId = lostReport.Id,
                    Score = result.Score,
                    MatchReason = result.MatchReason
                });
            }

            if (newMatches.Count > 0)
            {
                _matchRepo.AddRange(newMatches);
            }
        }

      

        private MatchResult CalculateMatchScore(FoundReport found, LostReport lost)
        {
            var f = found.PetDescription;
            var l = lost.PetDescription;

            if (f == null || l == null)
            {
                return new MatchResult
                {
                    Score = 0,
                    MatchReason = "Missing animal description."
                };
            }

            // Type is a strong constraint.
            if (IsKnown(f.Type) &&
                IsKnown(l.Type) &&
                !AreEqual(f.Type, l.Type))
            {
                return new MatchResult
                {
                    Score = 0,
                    MatchReason = "Animal types do not match."
                };
            }

            double totalWeight = 0;
            double earnedWeight = 0;

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.20,
                CompareText(f.Type, l.Type));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.15,
                CompareText(f.Breed, l.Breed));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.15,
                CompareColors(f.Colors, l.Colors));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.08,
                CompareText(f.Sex, l.Sex));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.07,
                CompareText(f.Size, l.Size));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.05,
                CompareAge(f.Age, l.Age));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.05,
                CompareText(f.CoatLength, l.CoatLength));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.05,
                CompareText(f.CoatType, l.CoatType));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.05,
                CompareText(f.Pattern, l.Pattern));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.05,
                CompareText(f.EyeColor, l.EyeColor));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.03,
                CompareText(f.CollarColor, l.CollarColor));

            AddScore(
                ref earnedWeight,
                ref totalWeight,
                0.07,
                CompareDistance(found, lost));

            var score = totalWeight == 0
                ? 0
                : earnedWeight / totalWeight;

            return new MatchResult
            {
                Score = score,
                MatchReason = BuildMatchReason(found, lost, score)
            };
        }

        private static string BuildMatchReason(FoundReport found, LostReport lost, double score)
        {
            var reasons = new List<string>();

            var f = found.PetDescription;
            var l = lost.PetDescription;

            if (AreEqual(f.Type, l.Type))
                reasons.Add("Same animal type");

            if (AreEqual(f.Breed, l.Breed))
                reasons.Add("Same breed");

            if (CompareColors(f.Colors, l.Colors) > 0)
                reasons.Add("Similar colors");

            if (AreEqual(f.Sex, l.Sex))
                reasons.Add("Same sex");

            if (AreEqual(f.Size, l.Size))
                reasons.Add("Same size");

            if (AreEqual(f.EyeColor, l.EyeColor))
                reasons.Add("Same eye color");

            var distance = GetDistanceKm(found, lost);

            if (distance.HasValue)
            {
                reasons.Add(
                    $"Found approximately {distance.Value:F1} km from lost location");
            }

            if (reasons.Count == 0)
                return $"Match score: {score:P0}";

            return string.Join(". ", reasons) + ".";
        }

        private static double? GetDistanceKm(FoundReport found, LostReport lost)
        {
            var foundCoordinate =
                found.FoundCoordinateNavigation;

            var lostCoordinate =
                lost.LostCoordinateNavigation;

            if (foundCoordinate == null ||
                lostCoordinate == null)
            {
                return null;
            }

            return CalculateDistanceKm(
                lostCoordinate.Latitude,
                lostCoordinate.Longitude,
                foundCoordinate.Latitude,
                foundCoordinate.Longitude);
        }

        private static double CompareAge(double? first, double? second)
        {
            if (!first.HasValue || !second.HasValue)
                return -1;

            var difference = Math.Abs(first.Value - second.Value);

            return difference switch
            {
                0 => 1.0,
                <= 1 => 0.8,
                <= 2 => 0.5,
                <= 4 => 0.2,
                _ => 0.0
            };
        }

        private static HashSet<string> ParseValues(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new HashSet<string>();

            return value
                .Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries)
                .Select(Normalize)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet();
        }


        private static bool AreEqual(string? first, string? second)
        {
            if (!IsKnown(first) || !IsKnown(second))
                return false;

            return string.Equals(
                Normalize(first),
                Normalize(second),
                StringComparison.OrdinalIgnoreCase);
        }


        private static void AddScore(ref double earnedWeight, ref double totalWeight, double weight, double similarity)
        {
            if (similarity < 0)
                return;

            totalWeight += weight;
            earnedWeight += weight * similarity;
        }


        private static double CompareText(string? first, string? second)
        {
            if (!IsKnown(first) || !IsKnown(second))
                return -1;

            return string.Equals(
                Normalize(first),
                Normalize(second),
                StringComparison.OrdinalIgnoreCase)
                ? 1.0
                : 0.0;
        }

        private static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            return value
                .Trim()
                .ToLowerInvariant();
        }

        private static bool IsKnown(string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        private static double CompareColors(string? first, string? second)
        {
            var firstColors = ParseValues(first);
            var secondColors = ParseValues(second);

            if (firstColors.Count == 0 || secondColors.Count == 0)
                return -1;

            var intersection = firstColors
                .Intersect(secondColors)
                .Count();

            if (intersection == 0)
                return 0;

            var union = firstColors
                .Union(secondColors)
                .Count();

            return (double)intersection / union;
        }


        private static double CompareDistance(FoundReport found, LostReport lost)
        {
            var foundCoordinate =
                found.FoundCoordinateNavigation;

            var lostCoordinate =
                lost.LostCoordinateNavigation;

            if (foundCoordinate == null ||
                lostCoordinate == null)
            {
                return -1;
            }

            var distanceKm = CalculateDistanceKm(
                lostCoordinate.Latitude,
                lostCoordinate.Longitude,
                foundCoordinate.Latitude,
                foundCoordinate.Longitude);

            return distanceKm switch
            {
                <= 1 => 1.0,
                <= 3 => 0.9,
                <= 5 => 0.75,
                <= 10 => 0.5,
                <= 20 => 0.25,
                _ => 0.0
            };
        }


        private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371;

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a =
                Math.Sin(dLat / 2) *
                Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) *
                Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(
                Math.Sqrt(a),
                Math.Sqrt(1 - a));

            return earthRadiusKm * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

    }
}

    
        
    

