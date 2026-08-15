using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;




namespace LostFoundPetReporter.API.Controllers
{
    public class LostReportController : BaseCrudController<LostReport, LostReportController>
    {
        public LostReportController(ILostReportRepo repo) : base(repo)
        {

        }

        /// <summary>
        /// Gets all LostReport records.
        /// <summary>
        /// <param name="id"> Primary key of the User</param>
        /// <returns> All LostReports for a user</returns>
        [ApiVersion("1.0")]
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<LostReport>> GetLostReportsByUserId(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                return Ok(((ILostReportRepo)MainRepo).GetAllByUserId(id.Value));
            }
            return Ok(MainRepo.GetAllIgnoreQueryFillters());
        }
    }
}
