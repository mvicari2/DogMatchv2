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
        private readonly IBiographyService _service;

        public BiographyController(IBiographyService service) =>
            _service = service;


        #region WebApi Methods
        // GET api/Biography/{id}
        [HttpGet("{id}")]
        public async Task<DogBiography> Get(int id)
        {
            DogBiography bio = await _service.GetDogBiography(id);

            if (bio != null)
            {
                return bio;
            }
            else // create new dog biography if it does not yet exist
            {
                return await _service.CreateBiography(id, GetUserId());
            }
        }

        // PUT api/Biography/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DogBiography bio)
        {
            if (id != bio.DogId)
            {
                return BadRequest();
            }

            bool success = await _service.UpdateBiography(bio, GetUserId());

            if (success)
            {
                return Ok();
            }
            else
            {
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
