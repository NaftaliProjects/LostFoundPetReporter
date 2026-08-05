using LostFoundPetReporter.CoreDb.Models;
using Microsoft.AspNetCore.Mvc;



namespace LostFoundPetReporter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        [HttpGet("{id?}")]
        public async IActionResult GetReports(int? id)
        {
       
            return Ok(new String("Hi"));
            
        }

        [HttpPost("{id?}")]
        public async IActionResult CreateNewReport(int? id)
        {
            //return a view
        }

        [HttpPut("{id?}")]
        public async IActionResult UpdateReport(int? id)
        {
            //return a view
        }
    }
}
