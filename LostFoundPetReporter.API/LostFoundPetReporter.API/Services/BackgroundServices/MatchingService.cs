
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

        // Direction 1: One FoundReport against many LostReports
        public async Task TryMatchLostReportAsync(int lostReportId, CancellationToken cancellationToken = default)
        {
            var lostReport = _lostRepo.Find(lostReportId);
            if (lostReport == null) return;

            var candidateLostReports = _foundRepo.GetAll();
            if (!candidateLostReports.Any()) return;

            // 1. Fetch all existing match LostReportIds for this found report in ONE query
            var newMatches = new List<LostFoundMatch>();

            // 2. Filter and score in-memory
            foreach (var foundReport in candidateLostReports)
            {
                if (_matchRepo.MatchExists(foundReport.Id, lostReportId)) continue;

                double score = CalculateMatchScore(foundReport, lostReport);

                score = 1;

                if (score >= 0.70)
                {
                    newMatches.Add(new LostFoundMatch
                    {
                        FoundReportId = foundReport.Id,
                        LostReportId = lostReport.Id,
                    });
                }
            }

            // 3. Batch insert all matches at once
            if (newMatches.Count > 0)
            {
                _matchRepo.AddRange(newMatches);
            }
        }

        // Direction 2: One LostReport against many FoundReports
        public async Task TryMatchFoundReportAsync(int foundReportId, CancellationToken cancellationToken = default)
        {
            var foundReport =  _foundRepo.Find(foundReportId);
            if (foundReport == null) return;

            var candidateLostReports =  _lostRepo.GetAll();
            if (!candidateLostReports.Any()) return;

            // 1. Fetch all existing match LostReportIds for this found report in ONE query
            var newMatches = new List<LostFoundMatch>();

            // 2. Filter and score in-memory
            foreach (var lostReport in candidateLostReports)
            {
                if (_matchRepo.MatchExists(lostReport.Id, foundReportId)) continue;

                double score = CalculateMatchScore(foundReport, lostReport);

                score = 1;

                if (score >= 0.70)
                {
                    newMatches.Add(new LostFoundMatch
                    {
                        FoundReportId = foundReport.Id,
                        LostReportId = lostReport.Id,
                    });
                }
            }

            // 3. Batch insert all matches at once
            if (newMatches.Count > 0)
            {
                _matchRepo.AddRange(newMatches);       
            }
        }

        // Shared symmetric logic
        private async Task EvaluateAndSaveMatchAsync(FoundReport found, LostReport lost)
        {
            if ( _matchRepo.MatchExists(lost.Id, found.Id)) return;

            double score = CalculateMatchScore(found, lost);

            //secretly for now everyone gett a 100% score 
            score = 1;

            if (score >= 0.70)
            {
                var match = new LostFoundMatch
                {
                    FoundReportId = found.Id,
                    LostReportId = lost.Id,
                };

                _matchRepo.Add(match);
            }
        }

        private double CalculateMatchScore(FoundReport found, LostReport lost)
        {
            double score = 0.0;
            if (found.PetDescription?.Type == lost.PetDescription?.Type) score += 0.4;
            if (found.PetDescription?.Breed == lost.PetDescription?.Breed) score += 0.3;
            if (found.PetDescription?.Colors == lost.PetDescription?.Colors) score += 0.3;
            return score;
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

