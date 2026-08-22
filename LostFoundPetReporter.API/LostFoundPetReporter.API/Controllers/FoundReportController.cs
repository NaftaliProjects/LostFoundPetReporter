using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.API.Services.BackgroundServices;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.Repos;
using LostFoundPetReporter.CoreDb.ReposInterfaces;




namespace LostFoundPetReporter.API.Controllers
{
    public class FoundReportController : BaseCrudController<
        FoundReport,
        FoundReportController,
        FoundReportDto,
        CreateFoundReportDto>
    {
        private readonly IMatchingQueue _matchingQueue;

        // Inject IMatchingQueue alongside your repository
        public FoundReportController(
            IFoundReportRepo repo,
            IMatchingQueue matchingQueue)
            : base(repo)
        {
            _matchingQueue = matchingQueue;
        }


        [ApiVersion("1.0")]
        [HttpPost]
        public override ActionResult<FoundReportDto> AddOne(CreateFoundReportDto createDto)
        {
            var actionResult = base.AddOne(createDto);

            if (actionResult.Result is CreatedAtActionResult createdResult && createdResult.Value is FoundReportDto createdDto)

            {
                _matchingQueue.QueueForMatchingAsync(createdDto.Id.Value, ReportType.Found);
            }

            return actionResult;
        }


        /// <summary>
        /// Gets all FoundReport records for a user.
        /// </summary>
        /// <param name="id">Primary key of the User</param>
        /// <returns>All FoundReports for a user</returns>
        [ApiVersion("1.0")]
        [HttpGet("ByUser/{id}")]
        public ActionResult<IEnumerable<FoundReportDto>> GetFoundReportsByUserId(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var entities = ((IFoundReportRepo)MainRepo).GetAllByUserId(id.Value);
                var dtos = entities.Select(FoundReportDto.FromEntity);
                return Ok(dtos);
            }

            var allEntities = MainRepo.GetAllIgnoreQueryFillters();

            var allDtos = allEntities.Select(FoundReportDto.FromEntity);

            return Ok(allDtos);
        }
    }
}