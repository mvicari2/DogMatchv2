using System.Security.Claims;
using System.Threading.Tasks;
using DogMatch.Domain.Services;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public BiographyController(IBiographyService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
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
                // ensure user requesting biography is the dog's owner
                if (requestUser == bio.OwnerId)
                {
                    return Ok(bio);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else // create new dog biography if it does not yet exist
            {
                // ensure that user attempting to create new biography is dog owner
                string ownerId = await _userService.GetOwnerIdByDogId(id);

                if (ownerId == requestUser)
                {
                    return Ok(await _service.CreateBiography(id, requestUser));
                }
                else
                {
                    return Unauthorized();
                }                
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
