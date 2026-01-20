using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TriviaGame.Api.DTOs.Users;
using TriviaGame.Api.Models;
using TriviaGame.Api.Services.Interfaces;
using AutoMapper;
using TriviaGame.Api.Common;

namespace TriviaGame.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Registro de un nuevo usuario
        /// </summary>
        /// <param name="dto">DTO con Gmail y Password</param>
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterUserRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Llamada al service, solo se pasa la contraseña en texto plano
            var (exists, message) = await _userService.CreateUserAsync(dto.Gmail, dto.Password);

            // Preparar DTO de respuesta
            var response = new UserResponseDto
            {
                Gmail = dto.Gmail,
                Success = !exists,           // true si no existía y se creo correctamente
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            if (exists)
                return Conflict(response); // Usuario ya existe

            return Ok(response);
        }


        /// <summary>
        /// Login de usuario
        /// </summary>
        /// <param name="dto">DTO con Gmail y Password</param>
        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginUserRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.LoginAsync(dto.Gmail, dto.Password);

                var response = _mapper.Map<UserResponseDto>(user);
                response.Success = true;
                response.Message = "Login exitoso";

                return Ok(response);
            }
            catch (BusinessException ex)
            {
                return Unauthorized(new UserResponseDto
                {
                    Gmail = dto.Gmail,
                    Success = false,
                    Message = ex.Message
                });
            }
        }


    }
}
