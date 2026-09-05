using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundPetReporter.API.Controllers
{
    public class LostFoundMatchController
        : BaseCrudController<
            LostFoundMatch,
            LostFoundMatchController,
            LostFoundMatchDto,
            CreateLostFoundMatchDto>
    {
        private readonly ILostFoundMatchRepo _repo;

        public LostFoundMatchController(
            ILostFoundMatchRepo repo)
            : base(repo)
        {
            _repo = repo;
        }

        [ApiVersion("1.0")]
        [HttpDelete("{lostReportId}/{foundReportId}")]
        public ActionResult RemoveMatch(
            int lostReportId,
            int foundReportId)
        {
            var match = _repo
                .GetByLostReportId(lostReportId)
                .FirstOrDefault(
                    m => m.FoundReportId == foundReportId);

            if (match == null)
                return NotFound();

            _repo.Delete(match);

            return NoContent();
        }
    }
}