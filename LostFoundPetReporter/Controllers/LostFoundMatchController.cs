using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundPetReporter.API.Controllers
{
    public class LostFoundMatchController : BaseCrudController<LostFoundMatch, LostFoundMatchDto, CreateLostFoundMatchDto>
    {
        private readonly ILostFoundMatchRepo _repo;

        public LostFoundMatchController(ILostFoundMatchRepo repo) : base(repo)
        {
            _repo = repo;
        }

        [ApiVersion("1.0")]
        [HttpDelete("{lostReportId}/{foundReportId}")]
        public ActionResult RemoveMatch(int lostReportId, int foundReportId)
        {


            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(); // 401: User is not logged in
            }

            var match = _repo
                .GetByLostReportId(lostReportId)
                .FirstOrDefault(m => m.FoundReportId == foundReportId);

            if (match == null)
                return NotFound();

            //check if the lostReport is owned by the user 
            var lostReportOwnerId = match.LostReportNevigation?.UserNevigation?.Id;

            if (lostReportOwnerId == null || userId.Value != lostReportOwnerId.Value)
            {
                return Forbid(); 
            }

            _repo.Delete(match);

            return NoContent();
        }
    }
}