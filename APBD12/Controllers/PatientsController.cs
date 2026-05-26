using APBD12.Exceptions;
using APBD12.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD12.Controllers
{
    [Route("api/patients")]
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
    }
}
