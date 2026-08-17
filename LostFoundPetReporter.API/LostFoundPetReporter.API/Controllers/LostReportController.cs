using LostFoundPetReporter.API.DTO;
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
                var dtos = entities.Select(MapToResponseDto);
                return Ok(((ILostReportRepo)MainRepo).GetAllByUserId(id.Value));
            }
            return Ok(MainRepo.GetAllIgnoreQueryFillters());
        }

        protected override LostReportDto MapToResponseDto(LostReport entity)
        {
            return new LostReportDto
            {
                Id = entity.Id,
                Coordinates = entity.Coordinates,
                dateTime = entity.dateTime,
                UserId = entity.UserId
            };
        }

        protected override LostReport MapToEntity(CreateLostReportDto createDto)
        {
            return new LostReport
            {
                Id = createDto.Id ?? 0,
                Coordinates = createDto.Coordinates,
                dateTime = createDto.dateTime,
                UserId = createDto.UserId
            };
        }
    }
}
