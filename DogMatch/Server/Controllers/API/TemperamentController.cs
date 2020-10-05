using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DogMatch.Domain.Services;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<TemperamentController> _logger;

        public TemperamentController(ITemperamentService service, IUserService userService, ILogger<TemperamentController> logger)
        {
            _service = service;
            _userService = userService;
            _logger = logger;
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
                    return Ok(temperament);
                else
                    _logger.LogWarning($"Request user ({requestUser}) does not have permission (non-owner) to get dog temperament for dog id {temperament.DogId} owned by {temperament.OwnerId}");
                    return Unauthorized();
            }
            else // create new dog temperament if it does not yet exist
            {                
                string ownerId = await _userService.GetOwnerIdByDogId(id);

                // create and return new temperament if requester is dog owner
                if (ownerId == requestUser)
                    return Ok(await _service.CreateTemperament(id, requestUser));
                else
                    _logger.LogWarning($"Request user ({requestUser}) does not have permission (non-owner) to create new dog temperament for dog owned by {ownerId}");
                    return Unauthorized();
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
                    return Ok();
                else
                    _logger.LogError($"Failed to save Temperament for {temperament.DogId} by {requestUser}");
                    return BadRequest();
            }
            else
            {
                // unauthorized: user attempting to update temperament is not the owner
                _logger.LogWarning($"Request user ({requestUser}) does not have the permission (non-owner) to update dog temperament for dog id {temperament.DogId} owned by {dogOwnerId}");
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
