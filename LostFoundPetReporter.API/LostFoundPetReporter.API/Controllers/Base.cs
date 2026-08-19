using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;

namespace LostFoundPetReporter.API.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseCrudController<
        TEntity,
        TController,
        TResponseDto,
        TCreateOrUpdateDto> : ControllerBase

        where TEntity : BaseModel, new()

        where TResponseDto :
            IResponseDto<TEntity, TResponseDto>

        where TCreateOrUpdateDto :
            IEntityDto<TEntity>,
            IHasId

        where TController : class
    {
        protected readonly IBaseRepo<TEntity> MainRepo;

        protected BaseCrudController(IBaseRepo<TEntity> repo)
        {
            MainRepo = repo;
        }


        // =========================
        // GET ALL
        // =========================

        [ApiVersion("1.0")]
        [HttpGet]
        public ActionResult<IEnumerable<TResponseDto>> GetAll()
        {
            var entities = MainRepo.GetAllIgnoreQueryFillters();

            var dtos = entities.Select(TResponseDto.FromEntity);

            return Ok(dtos);
        }


        // =========================
        // GET ONE
        // =========================

        [ApiVersion("1.0")]
        [HttpGet("{id}")]
        public ActionResult<TResponseDto> GetOne(int id)
        {
            var entity = MainRepo.Find(id);

            if (entity == null)
            {
                return NoContent();
            }

            return Ok(TResponseDto.FromEntity(entity));

        }


        // =========================
        // PUT
        // =========================

        [ApiVersion("1.0")]
        [HttpPut("{id}")]
        public ActionResult UpdateOne(int id, TCreateOrUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var existingEntity =
                    MainRepo.FindAsNoTracking(id);

                if (existingEntity == null)
                {
                    return NotFound();
                }

                var entity = updateDto.ToEntity();

                MainRepo.Update(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }


        // =========================
        // POST
        // =========================

        [ApiVersion("1.0")]
        [HttpPost]
        public ActionResult<TResponseDto> AddOne(
            TCreateOrUpdateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (createDto.Id.HasValue &&
                createDto.Id.Value > 0)
            {
                return BadRequest(
                    "POST requests cannot specify an existing Id.");
            }

            var entity = createDto.ToEntity();

            try
            {
                MainRepo.Add(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return CreatedAtAction(
                nameof(GetOne),
                new { id = entity.Id },
                TResponseDto.FromEntity(entity));
        }


        // =========================
        // DELETE
        // =========================

        [ApiVersion("1.0")]
        [HttpDelete("{id}")]
        public ActionResult DeleteOne(int id)
        {
            var entity = MainRepo.Find(id);

            if (entity == null)
            {
                return NotFound();
            }

            try
            {
                MainRepo.Delete(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ex.GetBaseException()?.Message);
            }

            return NoContent();
        }
    }
}