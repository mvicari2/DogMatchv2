using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DogMatch.Domain.Services;
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
        private readonly IUserService _userService;

        public DoggoController(IDogService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }
        #endregion DI

        #region WebApi Methods
        // GET: api/Doggo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dog>>> GetDogs() =>
            Ok(await _service.GetAllDogs());

        // GET: api/Doggo/{id} 
        [HttpGet("{id}")]
        public async Task<ActionResult<Dog>> Get(int id)
        {            
            Dog dog = await _service.GetDog(id);

            if (dog == null)
                return NotFound();

            return dog;
        }

        // PUT: api/Doggo/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, Dog dog)
        {
            if (id != dog.Id)
                return BadRequest();

            // ensure user attempting to update dog is the dog owner
            string dogOwnerId = await _userService.GetOwnerIdByDogId(dog.Id);
            string requestUser = GetUserId();
            
            if (dogOwnerId == requestUser)
            {
                bool success = await _service.UpdateDog(id, dog, requestUser);

                if (success)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                // unauthorized: user attempting to update dog is not the owner
                return Unauthorized();
            }                     
        }

        // POST: api/Doggo 
        [HttpPost]
        public async Task<ActionResult<Dog>> Post(Dog dog) =>
            await _service.CreateDog(dog, GetUserId());

        // DELETE: api/Doggo/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            DeleteDogResponse response = await _service.DeleteDog(id, GetUserId());

            switch (response)
            {
                case DeleteDogResponse.Success:
                    return Ok();
                case DeleteDogResponse.Unauthorized:
                    return Unauthorized();                    
                case DeleteDogResponse.Failed:
                    return NotFound();
                default:
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
