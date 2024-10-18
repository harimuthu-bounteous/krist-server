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
                var user = _mapper.Map<User>(registerDto);
                var response = await _client.Auth.SignUp(registerDto.Email, registerDto.Password);

                if (response?.User == null)
                {
                    return BadRequest("Registration failed");
                }

                var UserID = response.User.Id;

                if (string.IsNullOrEmpty(UserID))
                {
                    return BadRequest("User ID is null, cannot create user record.");
                }

                user.UId = UserID;

                await _userRepository.CreateUserAsync(user, UserID);
                return Ok("User created successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Execption : " + e.Message);
                return Ok();
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