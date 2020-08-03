using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DogMatch.Server.Services;
using DogMatch.Shared.Globals;

namespace DogMatch.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DoggoController : ControllerBase
    {
        #region DI
        private readonly IDogService _service;
        public DoggoController(IDogService service) => _service = service;
        #endregion DI

        #region WebApi Methods
        // GET: api/Doggo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dog>>> GetDogs()
        {
            var dogs = await _service.GetAllDogs();        

            if (dogs.Count() < 1)
            {
                return BadRequest();
            }
            return Ok(dogs);
        }        

        // GET: api/Doggo/{id} 
        [HttpGet("{id}")]
        public async Task<ActionResult<Dog>> Get(int id)
        {            
            Dog dog = await _service.GetDog(id);

            if (dog == null)
            {
                return NotFound();
            }

            return dog;
        }

        // PUT: api/Doggo/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, Dog dog)
        {
            if (id != dog.Id)
            {
                return BadRequest();
            }   

            var success = await _service.UpdateDog(id, dog, GetUserId());

            if (success)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }           
        }

        // POST: api/Doggo 
        [HttpPost]
        public async Task<ActionResult<Dog>> Post(Dog dog) =>
            await _service.CreateDog(dog, GetUserId());
        
        // DELETE: api/Doggo/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteDogResponse>> Delete(int id) =>
            await _service.DeleteDog(id, GetUserId());

        #endregion WebApi Methods

        #region Internal 
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        #endregion Internal
    }
}
