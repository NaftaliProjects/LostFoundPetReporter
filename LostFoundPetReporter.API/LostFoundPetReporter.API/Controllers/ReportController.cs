using LostFoundPetReporter.CoreDb.Models;




namespace LostFoundPetReporter.API.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : BaseCrudController
    {
        [HttpGet("{id?}")]
        public async Task<IActionResult> GetReports(int? id)
        {
       
            return Ok(new String("Hi"));
            
        }

        [HttpPost("{id?}")]
        public async Task<IActionResult> CreateNewReport(int? id)
        {
            //return a view
            return Ok(new String("Hi"));
        }

        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateReport(int? id)
        {
            //return a view
            return Ok(new String("Hi"));
        }
    }
}
