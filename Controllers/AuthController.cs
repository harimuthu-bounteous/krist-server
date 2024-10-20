using AutoMapper;
using krist_server.DTO.AuthDTOs;
using krist_server.DTO.ResponseDTO;
using krist_server.Interfaces;
using krist_server.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase;
using Supabase.Gotrue.Exceptions;

namespace krist_server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly Client _client;

        public AuthController(Client client, IMapper mapper, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _client = client;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            try
            {
                Console.WriteLine(JsonConvert.SerializeObject(registerDto));
                if (string.IsNullOrEmpty(registerDto.Email) || !registerDto.Email.Contains("@"))
                {
                    return BadRequest("Invalid email address.");
                }

                if (string.IsNullOrEmpty(registerDto.Password) || registerDto.Password.Length < 6)
                {
                    return BadRequest("Password must be at least 6 characters long.");
                }

                // Check if the user already exists (assuming _userRepository has a method for this)
                var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return Conflict("A user with this email already exists.");
                }

                var user = _mapper.Map<User>(registerDto);

                var response = await _client.Auth.SignUp(registerDto.Email, registerDto.Password);
                if (response?.User == null)
                {
                    return BadRequest("Registration failed. Please try again.");
                }

                var userID = response.User.Id;
                if (string.IsNullOrEmpty(userID))
                {
                    return BadRequest("User ID is null, cannot create user record.");
                }

                user.UId = userID;
                await _userRepository.CreateUserAsync(user, userID);

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception e)
            {
                // Log the exception and return an internal server error
                Console.WriteLine("Exception: " + e.Message);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<MutationResponseDTO>> Login(LoginDTO loginDto)
        {
            try
            {
                var session = await _client.Auth.SignInWithPassword(loginDto.Email, loginDto.Password);
                var token = session?.AccessToken;
                if (token is null)
                {
                    return Unauthorized("Invalid login credentials");
                }
                var foundUser = await _userRepository.GetUserByEmailAsync(loginDto.Email);

                if (foundUser == null)
                {
                    return NotFound(new ErrorResponseDTO("User not found", 404));
                }

                return Ok(new MutationResponseDTO(
                        "Login Successful",
                        new
                        {
                            token,
                            user = JsonConvert.SerializeObject(foundUser)
                        }
                    ));
            }
            catch (GotrueException ge)
            {
                Console.WriteLine("Error in Login : " + ge);
                return Unauthorized(new ErrorResponseDTO("Invalid Credentials", 401));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Login : " + e);
                return BadRequest(new ErrorResponseDTO("Internal Server Error", 500));
            }

        }
    }

}