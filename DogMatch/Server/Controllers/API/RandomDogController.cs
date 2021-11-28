using DogMatch.Domain.Services;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DogMatch.Server.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomDogController : ControllerBase
    {
        #region DI
        private readonly IRandomDogService _service;
        private readonly ILogger<RandomDogController> _logger;

        public RandomDogController(IRandomDogService service, ILogger<RandomDogController> logger)
        {
            _service = service;
            _logger = logger;
        }
        #endregion DI


        #region WebApi Methods
        /// <summary>
        /// HTTP GET URL: api/RandomDog
        /// Calls a domain service that fetches a random dog by using a combination of
        /// public apis and data generators to create a random dog that inlcludes a dog
        /// image, name, and other demographic information.
        /// </summary>
        /// <returns>Task containing <see cref="ActionResult{RandomDog}>"/></returns>
        [HttpGet]
        public async Task<ActionResult<RandomDog>> GetRandomDog()
        {
            RandomDog randomDog = await _service.FetchRandomDog();

            if (randomDog.DogFound)
                _logger.LogTrace($"Fetched random dog with image url: {randomDog.ImageUrl}");
            else
                _logger.LogWarning("Error fetching random dog.");

            return Ok(randomDog);
        }
        #endregion WebApi Methods
    }
}
