using Microsoft.AspNetCore.Http;
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

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var res = await _dbService.GetPatientWithPrescriptions(id);
            
                return Ok(res);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
