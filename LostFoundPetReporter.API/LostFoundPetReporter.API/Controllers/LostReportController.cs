using LostFoundPetReporter.API.DTO;
using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.API.Services.BackgroundServices;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using System.Reflection.Metadata.Ecma335;




namespace LostFoundPetReporter.API.Controllers
{
    public class LostReportController : BaseCrudController<LostReport, LostReportController ,LostReportDto, CreateLostReportDto>
    {
        private readonly IMatchingQueue _matchingQueue;

        public LostReportController (ILostReportRepo repo, IMatchingQueue matchingQueue) : base(repo)
        {
            _matchingQueue = matchingQueue;
        }


        [ApiVersion("1.0")]
        [HttpPost]
        public override ActionResult<LostReportDto> AddOne(CreateLostReportDto createDto)
        {
            var actionResult = base.AddOne(createDto);

            if (actionResult.Result is CreatedAtActionResult createdResult && createdResult.Value is LostReportDto createdDto)

            {        
               _matchingQueue.QueueForMatchingAsync(createdDto.Id.Value, ReportType.Lost);
            }

            return actionResult;
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
