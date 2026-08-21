
using LostFoundPetReporter.CoreDb.ReposInterfaces;

namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class MatchingService : IMatchingService
    {
        private readonly IFoundReportRepo _foundRepo;
        private readonly ILostReportRepo _lostRepo;
        private readonly ILostFoundMatchRepo _matchRepo;

        public MatchingService(
            IFoundReportRepo foundRepo,
            ILostReportRepo lostRepo,
            ILostFoundMatchRepo matchRepo)
        {
            _foundRepo = foundRepo;
            _lostRepo = lostRepo;
            _matchRepo = matchRepo;
        }

        public async Task TryMatchLostReportAsync(int foundReportId, CancellationToken cancellationToken = default)
        {
            // 1. Fetch the target found report
            var foundReport = _foundRepo.Find(foundReportId);
            if (foundReport == null) return;

            // 2. Query potential lost reports (e.g., same species, active status)
            var candidateLostReports = _lostRepo.GetAll();

            foreach (var lostReport in candidateLostReports)
            {
                // Skip if a match entry already exists between these two reports
                if (_matchRepo.MatchExists(lostReport.Id, foundReport.Id))
                {
                    continue;
                }
                else
                {
                    var match = new LostFoundMatch
                    {
                        FoundReportId = foundReport.Id,
                        LostReportId = lostReport.Id
                    };

                    _matchRepo.Add(match);
                }

                /*
                // 3. Run scoring algorithm
                double score = CalculateMatchScore(foundReport, lostReport);

                // 4. If confidence threshold is met (e.g. 70%), save a match entity
                if (score >= 0.70)
                {
                    var match = new LostFoundMatch
                    {
                        FoundReportId = foundReport.Id,
                        LostReportId = lostReport.Id,
                        ConfidenceScore = score,
                        CreatedAt = DateTime.UtcNow,
                        Status = MatchStatus.PendingReview
                    };

                    await _matchRepo.AddAsync(match);
                */
            }
        }
    }
}

    /*
        private double CalculateMatchScore(FoundReport found, LostReport lost)
        {
            double score = 0.0;

            // Species match (mandatory baseline)
            if (found.SpeciesId == lost.SpeciesId) score += 0.3;

            // Breed match
            if (!string.IsNullOrEmpty(found.Breed) &&
                string.Equals(found.Breed, lost.Breed, StringComparison.OrdinalIgnoreCase))
            {
                score += 0.3;
            }

            // Color match
            if (!string.IsNullOrEmpty(found.Color) &&
                string.Equals(found.Color, lost.Color, StringComparison.OrdinalIgnoreCase))
            {
                score += 0.2;
            }

            // Geographic proximity criteria (e.g., within ~10km / same zip or city)
            if (found.CityId == lost.CityId)
            {
                score += 0.2;
            }

            return score;
        }
    */

