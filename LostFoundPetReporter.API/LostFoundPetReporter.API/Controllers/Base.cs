

using LostFoundPetReporter.CoreDb.Repos;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using LostFoundPetReporter.API.DTO;

namespace LostFoundPetReporter.API.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseCrudController<TEntity, TController , TResponseDto, TCreateDto> : ControllerBase
        where TEntity : BaseModel, new()
        where TController : class
    {
        protected readonly IBaseRepo<TEntity> MainRepo;
        //protected readonly IAppLogging

        protected BaseCrudController(IBaseRepo<TEntity> repo)
        {
            MainRepo = repo;
        }

        protected abstract TResponseDto MapToResponseDto(TEntity entity);
        protected abstract TEntity MapToEntity(TCreateDto createDto);

        /// <summary>
        /// GETS ALL RECORDS.
        /// <summary>
        /// <returns> ALL records</returns>>
        [ApiVersion("1.0")]
        [HttpGet]
        public ActionResult<IEnumerable<TResponseDto>> GetAll()
        {
            var entities = MainRepo.GetAllIgnoreQueryFillters();
            var dtos = entities.Select(MapToResponseDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Gets a single record.
        /// <summary>
        /// <param name="id"> Primary key of the record</param>
        /// <returns> Single Record</returns>
        [ApiVersion("1.0")]
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<TResponseDto>> GetOne(int id)
        {
            var entity = MainRepo.Find(id);
          
            if (entity == null)
            {
                return NoContent();
            }

            return Ok(MapToResponseDto(entity));
        }


        /// <summary>
        /// Updates a single record.
        /// <summary>
        /// <remarks>
        /// Sample Body:
        /// <pre>
        /// {
        ///     "Id": 1,
        ///     "TimeStamp": "AAAAAAAB+E="  
        /// }
        /// </pre>
        /// </remarks>
        /// <param name="id"> Primary key of the record to update</param>
        /// <param name="entity"> Entity to update</param>
        /// <returns> Single Record</returns>
        [ApiVersion("1.0")]
        [HttpPut("{id}")]
        public ActionResult<IEnumerable<TEntity>> UpdateOne(int id,TEntity entity)
        {
            if(id != entity.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            try
            {
                MainRepo.Update(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok(entity);
        }



        /// <summary>
        /// Adds a single record.
        /// <summary>
        /// <remarks>
        /// Sample Body:
        /// <pre>
        /// {
        ///     "Id": 1,
        ///     "TimeStamp": "AAAAAAAB+E="  
        /// }
        /// </pre>
        /// </remarks>
        /// <returns> Added Record</returns>
        [ApiVersion("1.0")]
        [HttpPost()]
        public ActionResult<IEnumerable<TEntity>> AddOne(TCreateDto createDto)
        {
            var entity = MapToEntity(createDto);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            try
            {
                
                MainRepo.Add(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return CreatedAtAction(nameof(GetOne),new { id = entity.Id},entity);
        }



        /// <summary>
        /// Deletes a single record.
        /// <summary>
        /// <remarks>
        /// Sample Body:
        /// <pre>
        /// {
        ///     "Id": 1,
        ///     "TimeStamp": "AAAAAAAB+E="  
        /// }
        /// </pre>
        /// </remarks>
        /// <returns> Nothing</returns>
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
                return BadRequest(ex.GetBaseException()?.Message);
            }

            return NoContent();
        }
    }
}
