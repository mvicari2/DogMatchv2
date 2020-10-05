using System.Security.Claims;
using System.Threading.Tasks;
using DogMatch.Domain.Services;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DogMatch.Server.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BiographyController : ControllerBase
    {
        #region DI
        private readonly IBiographyService _service;
        private readonly IUserService _userService;
        private readonly ILogger<BiographyController> _logger;

        public BiographyController(IBiographyService service, IUserService userService, ILogger<BiographyController> logger)
        {
            _service = service;
            _userService = userService;
            _logger = logger;
        }
        #endregion DI


        #region WebApi Methods
        // GET api/Biography/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DogBiography>> Get(int id)
        {
            string requestUser = GetUserId();
            DogBiography bio = await _service.GetDogBiography(id);

            if (bio != null)
            {
                // return bio if user requesting biography is the dog's owner
                if (requestUser == bio.OwnerId)                
                    return Ok(bio);
                else
                    _logger.LogWarning($"Request user ({requestUser}) does not have the permission (non-owner) to get dog biography for dog Id: {bio.DogId}");
                    return Unauthorized();
                
            }
            else // create new dog biography if it does not yet exist
            {                
                string ownerId = await _userService.GetOwnerIdByDogId(id);

                // create and return new biography if requester is dog owner
                if (ownerId == requestUser)                
                    return Ok(await _service.CreateBiography(id, requestUser));
                else
                    _logger.LogWarning($"Request user ({requestUser}) does not have the permission (non-owner) to create new biography for dog owned by {ownerId}");
                    return Unauthorized();
            }
        }

        // PUT api/Biography/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DogBiography bio)
        {
            if (id != bio.DogId)
                return BadRequest();

            // ensure user attempting to update biography is the dog owner
            string dogOwnerId = await _userService.GetOwnerIdByDogId(bio.DogId);
            string requestUser = GetUserId();

            if (dogOwnerId == requestUser)
            {
                bool success = await _service.UpdateBiography(bio, requestUser);

                if (success)                
                    return Ok();
                else
                    _logger.LogError($"Failed to save Biography for {bio.DogId} by {requestUser}");
                    return BadRequest();                
            }
            else
            {
                // unauthorized: user attempting to update dog is not the owner
                _logger.LogWarning($"Request user ({requestUser}) does not have permission (non-owner) to update dog biography for dog id {bio.DogId}");

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
