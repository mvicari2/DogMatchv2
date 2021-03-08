using DogMatch.Domain.Services;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DogMatch.Server.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        #region DI
        private readonly IMatchesService _matchesService;
        private readonly IUserService _userService;
        private readonly ILogger<MatchesController> _logger;

        public MatchesController(IMatchesService matchesService, IUserService userService, ILogger<MatchesController> logger)
        {
            _matchesService = matchesService;
            _userService = userService;   
            _logger = logger;
        }
        #endregion DI


        #region WebApi Methods
        /// <summary>
        /// HTTP GET URL: api/Matches/{id}
        /// Calls matches service to get top dog matches for a single dog
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns><see cref="DogMatches"/> object containing dog matches</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DogMatches>> GetMatches(int id)
        {
            string requestUserId = GetUserId();
            string ownerId = await _userService.GetOwnerIdByDogId(id);

            if (requestUserId == ownerId)
            {
                return Ok(
                    await _matchesService.GetDogMatchesByDogId(id, ownerId)
                );
            }
            else
            {
                _logger.LogWarning($"Request user ({requestUserId}) does not have the permission (non-owner) to get dog matches for dog Id: {id}");
                return Unauthorized();
            }
        }
        #endregion WebApi Methods

        #region Internal
        /// <summary>
        /// Get user id <see cref="string"> for user making api request
        /// </summary>
        /// <returns>user id <see cref="string"/></returns>
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);
        #endregion Internal
    }
}
