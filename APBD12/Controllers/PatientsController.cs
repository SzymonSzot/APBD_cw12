using APBD12.DTOs;
using APBD12.Exceptions;
using APBD12.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD12.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IDbService _dbService;
        
        public PatientsController(IDbService dbService)
        {
            _dbService = dbService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string? search = null)
        {
            try
            {
                var res = await _dbService.GetPatients(search);
            
                return Ok(res);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("{pesel}/bedassignments")]
        public async Task<IActionResult> AssignBed(string pesel, [FromBody] BedAssignDto request)
        {
            if (request.To.HasValue && request.From >= request.To.Value)
            {
                return BadRequest("Data rozpoczęcia ('from') musi być wcześniejsza niż data zakończenia ('to').");
            }
            
            try
            {
                await _dbService.AssignBed(pesel, request);
                
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
