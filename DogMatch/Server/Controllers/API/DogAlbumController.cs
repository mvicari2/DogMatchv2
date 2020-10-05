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
    public class DogAlbumController : ControllerBase
    {
        #region DI
        private readonly IImageService _service;
        private readonly IUserService _userService;
        private readonly ILogger<DogAlbumController> _logger;

        public DogAlbumController(IImageService service, IUserService userService, ILogger<DogAlbumController> logger)
        {
            _service = service;
            _userService = userService;
            _logger = logger;
        }
        #endregion DI


        #region WebApi Methods
        // GET api/DogAlbum/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DogAlbumImages>> Get(int id)
        {
            // ensure user making request for dog album is actual dog owner
            string ownerId = await _userService.GetOwnerIdByDogId(id);
            string requestUser = GetUserId();


            if (ownerId != requestUser)
            {
                // unauthorized: user attempting to get dog is not the owner
                _logger.LogWarning($"Request user ({requestUser}) does not have permission (non-owner) to update dog album for dog id {id}");

                return Unauthorized();
            }

            DogAlbumImages album = await _service.GetDogAlbumImages(id);

            if (album != null)
                return Ok(album);
            else
                return NotFound();
        }

        // PUT api/DogAlbum/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DogAlbumImages album)
        {
            if (id != album.DogId)
                return BadRequest();

            // ensure user attempting to update biography is the dog owner
            string dogOwnerId = await _userService.GetOwnerIdByDogId(album.DogId);
            string requestUser = GetUserId();

            if (dogOwnerId == requestUser)
            {
                bool success = await _service.UpdateDogAlbumImages(album, requestUser);

                if (success)
                    return Ok();
                else
                    _logger.LogError($"Failed to save Dog Album for dog id {album.DogId} by {requestUser}");
                    return BadRequest();
            }
            else
            {
                // unauthorized: user attempting to update dog is not the owner
                _logger.LogWarning($"Request user ({requestUser}) does not have permission (non-owner) to update dog album for dog id {album.DogId}");

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
