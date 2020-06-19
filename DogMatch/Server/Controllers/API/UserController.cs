﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogMatch.Server.Data;
using DogMatch.Server.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DogMatch.Server.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DogMatchDbContext _context;
        private readonly UserManager<DogMatchUser> _userManager;

        public UserController(DogMatchDbContext context, UserManager<DogMatchUser> userManager)
        {
            _context = context;
            _userManager = userManager;        
        }        

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DogMatchUser>> GetUser(string id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}
