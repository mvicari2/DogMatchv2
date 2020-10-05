﻿using System.Collections.Generic;
using System.Linq;
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
    public class OwnersController : ControllerBase
    {
        #region DI
        private readonly IDogService _service;
        public OwnersController(IDogService service) => _service = service;
        #endregion DI

        #region Owners API
        // GET: api/Owners 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dog>>> Get() =>
            Ok(await _service.GetDogsByOwner(GetUserId()));
        #endregion Owners API

        #region Internal 
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        #endregion Internal
    }
}
