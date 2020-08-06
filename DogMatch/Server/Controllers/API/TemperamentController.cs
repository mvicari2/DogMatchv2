using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DogMatch.Server.Services;

namespace DogMatch.Server.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TemperamentController : ControllerBase
    {
        private readonly ITemperamentService _service;

        public TemperamentController(ITemperamentService service) =>        
            _service = service;
        

        #region WebApi Methods
        // GET api/Temperament/{id}
        [HttpGet("{id}")]
        public async Task<DogTemperament> Get(int id)
        {
            DogTemperament temperament = await _service.GetDogTemperament(id);

            if (temperament != null)
            {
                return temperament;
            }
            else // create new temperament
            {
                return await _service.CreateTemperament(id, GetUserId());
            }
        }

        // PUT api/Temperament/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DogTemperament temperament)
        {
            if (id != temperament.DogId)
            {
                return BadRequest();
            }

            bool success = await _service.UpdateTemperament(temperament, GetUserId());

            if (success)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        #endregion WebApi Methods

        #region Internal
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        #endregion Internal
    }
}
