using MemberServices.DTOs.Response;
using MemberServices.Interfaces;
using MemberServices.Models;
using MemberServices.DTOs.Request;
using MemberServices.DTOs.Response;

namespace MemberServices.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        public AuthService(IUserRepository userRepository, IConfiguration configuration, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _jwtService = jwtService;
        }
        public async Task<UserResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.UserExistsAsync(request.Email))
            {
                throw new Exception("User already exists");
            }
            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = request.Password, // In production, hash the password before storing
                Role = request.Role
            };
            var createdUser = await _userRepository.CreateAsync(user);
            return new UserResponse
            {
                UserName = createdUser.UserName,
                Email = createdUser.Email,
                Role = createdUser.Role,
            };
        }
        public async Task<UserResponse> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User Not Found");

            return new UserResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Id = user.Id
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("User Not Found");
            if (request.Password != user.PasswordHash)
                throw new Exception("Invalid Password");

            var token = _jwtService.GenerateToken(user);
            return new AuthResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddHours(1) // Token valid for 1 hour
            };
        }
    }
}
