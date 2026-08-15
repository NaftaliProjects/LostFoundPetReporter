using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;




namespace LostFoundPetReporter.API.Controllers
{
    public class LostReportController : BaseCrudController<LostReport, LostReportController>
    {
        public LostReportController(ILostReportRepo repo) : base(repo)
        {

        }
    }
}
