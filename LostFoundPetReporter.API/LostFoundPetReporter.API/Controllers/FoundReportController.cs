using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.Repos;
using LostFoundPetReporter.CoreDb.ReposInterfaces;




namespace LostFoundPetReporter.API.Controllers
{
    public class FoundReportController : BaseCrudController<FoundReport, FoundReportController>
    {
        public FoundReportController(IFoundReportRepo repo) : base(repo)
        {

        }

        /// <summary>
        /// Gets all FoundReport records.
        /// <summary>
        /// <param name="id"> Primary key of the User</param>
        /// <returns> All FoundReport for a user</returns>
        [ApiVersion("1.0")]
        [HttpGet("ByUser/{id}")]
        public ActionResult<IEnumerable<FoundReport>> GetFoundReportsByUserId(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                return Ok(((IFoundReportRepo)MainRepo).GetAllByUserId(id.Value));
            }
            return Ok(MainRepo.GetAllIgnoreQueryFillters());
        }
    }
}
