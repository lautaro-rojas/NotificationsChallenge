using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NotificationsChallenge.Application.Models;
using NotificationsChallenge.Domain.DTOs;
using NotificationsChallenge.Application.Services;

namespace NotificationsChallenge.WebApi.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints in this controller with JWT authentication
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserViewModel>>> UserGetAll()
        {
            var usersDto = await _userService.GetAllAsync();

            // Mapping DTO -> ViewModel
            var usersViewModel = usersDto.Select(dto => new UserViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                DateActivationFormated = dto.DateActivation.ToShortDateString()
            }).ToList();

            return Ok(usersViewModel);
        }

        // GET: api/users/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserViewModel>> UserGetByID(int id)
        {
            var userDto = await _userService.GetByIdAsync(id);
            
            if (userDto == null)
            {
                return NotFound(new { message = $"Usuario con ID {id} no encontrado." });
            }
            
            // Mapping DTO to ViewModel
            var userViewModel = new UserViewModel
            {
                Id = userDto.Id,
                Name = userDto.Name,
                Email = userDto.Email,
                DateActivationFormated = userDto.DateActivation.ToShortDateString()
            };

            return Ok(userViewModel);
        }

        // POST: api/users
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserViewModel>> UserCreate([FromBody] UserCreationDto userCreationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int newUserId = await _userService.AddAsync(userCreationDto);

                var userFromDb = await _userService.GetByIdAsync(newUserId);

                var userViewModel = new UserViewModel
                {
                    Id = userFromDb.Id,
                    Name = userFromDb.Name,
                    Email = userFromDb.Email,
                    DateActivationFormated = userFromDb.DateActivation.ToShortDateString()
                };

                return CreatedAtAction(nameof(UserGetByID), new { id = newUserId }, userViewModel); // Code 201: User created

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
            }
        }

        // PUT: api/users/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UserUpdate([FromRoute] int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (id != userUpdateDto.Id)
            {
                return BadRequest(new { message = "The ID in the route does not match the ID in the request body." }); //Code 400
            }

            var success = await _userService.UpdateAsync(id, userUpdateDto);

            if (!success)
            {
                return NotFound(new { message = $"User with ID {id} not found." }); //Code 404
            }

            return NoContent(); // Code 204: The update was successful but there is no content to return
        }

        // DELETE: api/users/5
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UserDelete(int id)
        {
            var success = await _userService.DeleteAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"User with ID {id} not found." }); //Code 404
            }

            return NoContent(); // Code 204: The update was successful but there is no content to return
        }
    }
}