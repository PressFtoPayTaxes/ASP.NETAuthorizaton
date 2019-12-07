using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AuthJWTLesson.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthJWTLesson.Conrollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly TokenValidationService validator;

        public HomeController(TokenValidationService validator)
        {
            this.validator = validator;
        }

        [HttpGet]
        public IActionResult GetSecureInfo()
        {
            return Ok(validator.Validate(Request.Headers["Authorization"].ToString().Split(' ')[1]));
        }
    }
}