using Microsoft.AspNetCore.Mvc;
using MovieApp.Interfaces;
using MovieApp.Models;
using System.Text.RegularExpressions;

namespace MovieApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registrationData"></param>
        [HttpPost]
        public IActionResult Post([FromBody] UserMaster registrationData)
        {
            // Validación de correo electrónico
            if (!IsValidEmail(registrationData.Username))
            {
                return BadRequest("Invalid email format.");
            }

            bool isRegistered = _userService.RegisterUser(registrationData).Result;
            if (isRegistered)
            {
                return Ok("User registered successfully.");
            }
            else
            {
                return Conflict("Username is already taken.");
            }
        }

        /// <summary>
        /// Check the availability of the username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("{userName}")]
        public IActionResult ValidateUserName(string userName)
        {
            bool isAvailable = _userService.CheckUserNameAvailability(userName);
            if (isAvailable)
            {
                return Ok(true);
            }
            else
            {
                return Conflict(false);
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Usar Regex para validar el formato de correo electrónico
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}
