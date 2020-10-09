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
    public class DogProfileController : ControllerBase
    {
        #region DI
        private readonly IDogProfileService _service;
        private readonly IUserService _userService;
        private readonly ILogger<DogProfileController> _logger;

        public DogProfileController(IDogProfileService service, IUserService userService, ILogger<DogProfileController> logger)
        {
            _service = service;
            _userService = userService;
            _logger = logger;
        }
        #endregion DI

        #region WebApi Methods
        // GET api/DogProfile/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DogProfile>> Get(int id)
        {
            DogProfile profile = await _service.GetDogProfile(id);

            if (profile != null)
                return Ok(profile);
            else
                return NotFound();
        }
        #endregion WebApi Methods
    }
}
