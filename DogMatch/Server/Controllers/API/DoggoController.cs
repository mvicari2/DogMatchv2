using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogMatch.Server.Data;
using DogMatch.Shared.Models;
using DogMatch.Server.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DogMatch.Server.Services;

namespace DogMatch.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DoggoController : ControllerBase
    {
        private readonly DogMatchDbContext _context;
        private readonly IDogService _service;

        public DoggoController(DogMatchDbContext context, IDogService service)
        {
            _context = context;
            _service = service;
        }       

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
        public async Task<IActionResult> Put(int id, Dog dog)
        {
            if (id != dog.Id)
            {
                return BadRequest();
            }            

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _service.UpdateDog(id, dog, userId);

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
        public async Task<ActionResult<Dog>> Post(Dog dog)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Dog newDog = await _service.CreateDog(dog, userId);

            return newDog;
        }

        // DELETE: api/Doggo/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Dogs>> DeleteDog(int id)
        {
            var dog = await _context.Dogs.FindAsync(id);
            if (dog == null)
            {
                return NotFound();
            }

            _context.Dogs.Remove(dog);
            await _context.SaveChangesAsync();

            return dog;
        }   
    }
}
