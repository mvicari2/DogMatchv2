using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DogMatch.Domain.Services;

namespace DogMatch.Server.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TemperamentController : ControllerBase
    {
        #region DI
        private readonly ITemperamentService _service;
        private readonly IUserService _userService;

        public TemperamentController(ITemperamentService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }
        #endregion DI

        #region WebApi Methods
        // GET api/Temperament/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DogTemperament>> Get(int id)
        {
            string requestUser = GetUserId();
            DogTemperament temperament = await _service.GetDogTemperament(id);            

            if (temperament != null)
            {
                // ensure user requesting temperament is the dog's owner
                if (requestUser == temperament.OwnerId)
                {
                    return Ok(temperament);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else // create new dog temperament if it does not yet exist
            {
                // ensure that user attempting to create new temperament is dog owner
                string ownerId = await _userService.GetOwnerIdByDogId(id);

                if (ownerId == requestUser)
                {
                    return Ok(await _service.CreateTemperament(id, requestUser));
                }
                else
                {
                    return Unauthorized();
                }                
            }
        }

        // PUT api/Temperament/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DogTemperament temperament)
        {
            if (id != temperament.DogId)
                return BadRequest();

            // ensure user attempting to update temperament is the dog owner
            string dogOwnerId = await _userService.GetOwnerIdByDogId(temperament.DogId);
            string requestUser = GetUserId();

            if (dogOwnerId == requestUser)
            {
                bool success = await _service.UpdateTemperament(temperament, requestUser);

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
        #endregion WebApi Methods

        #region Internal
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);
        #endregion Internal
    }
}
