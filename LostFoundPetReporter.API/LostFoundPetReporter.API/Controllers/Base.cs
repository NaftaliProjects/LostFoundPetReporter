using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using LostFoundPetReporter.API.Services.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace LostFoundPetReporter.API.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public abstract class BaseCrudController<TEntity, TResponseDto, TCreateOrUpdateDto> : ControllerBase
        where TEntity : BaseModel, new()
        where TResponseDto : IResponseDto<TEntity, TResponseDto>
        where TCreateOrUpdateDto : IEntityDto<TEntity>, IHasId 
    {
        protected readonly IBaseRepo<TEntity> _mainRepo;

        protected BaseCrudController(IBaseRepo<TEntity> repo)
        {
            _mainRepo = repo;
        }

       
        protected int? GetCurrentUserId()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!int.TryParse(userId, out var id))
            {
                return null;
            }

            return id;
        }


        // =========================
        // GET ALL
        // =========================

        [ApiVersion("1.0")]
        [HttpGet]
        public ActionResult<IEnumerable<TResponseDto>> GetAll()
        {
            var entities = _mainRepo.GetAllIgnoreQueryFillters();

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
            var entity = _mainRepo.Find(id);

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

            try
            {
                var existingEntity = _mainRepo.FindAsNoTracking(id);


                if (existingEntity == null)
                {
                    return NotFound();
                }

                var updatedEntity = updateDto.ToEntity();
                
                _mainRepo.Update(existingEntity,updatedEntity);
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
        public virtual ActionResult<TResponseDto> AddOne(TCreateOrUpdateDto createDto)
        {   
            if (createDto.Id.HasValue && createDto.Id.Value > 0) { return BadRequest("POST requests cannot specify an existing Id."); }

            var entity = createDto.ToEntity();

            try { _mainRepo.Add(entity); }
            catch (Exception ex) { return BadRequest(ex); }

            return CreatedAtAction(nameof(GetOne), new { id = entity.Id }, TResponseDto.FromEntity(entity));
        }


        // =========================
        // DELETE
        // =========================

        [ApiVersion("1.0")]
        [HttpDelete("{id}")]
        public ActionResult DeleteOne(int id)
        {
            var entity = _mainRepo.Find(id);

            if (entity == null)
            {
                return NotFound();
            }

            try
            {
                _mainRepo.Delete(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException()?.Message);

            }

            return NoContent();
        }
    }
}