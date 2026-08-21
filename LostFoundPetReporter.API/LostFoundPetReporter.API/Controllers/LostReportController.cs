using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;




namespace LostFoundPetReporter.API.Controllers
{
    public class LostReportController : BaseCrudController<LostReport, LostReportController ,LostReportDto, CreateLostReportDto>
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
        [HttpGet("ByUser/{id}")]
        public ActionResult<IEnumerable<LostReportDto>> GetLostReportsByUserId(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var entities = ((ILostReportRepo)MainRepo).GetAllByUserId(id.Value);
                var dtos = entities.Select(LostReportDto.FromEntity);
                return Ok(dtos);
            }

            var allEntities = MainRepo.GetAllIgnoreQueryFillters();
            var allDtos = allEntities.Select(LostReportDto.FromEntity);
            return Ok(allDtos);
        }

        
    }
}
