using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkillSwap.Dtos.User;
using SkillSwap.Models;
using SkillSwap.Validations;
using SkillSwap.Interfaces;

namespace SkillSwap.Controllers.V1.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersPostController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        // Constructor
        public UsersPostController(AppDbContext dbContext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <remarks>
        /// Endpoint to register a new user with their personal details, abilities, and qualifications.
        /// This method also hashes the user's password before saving it to the database.
        /// If registration is successful, a JWT token is generated and returned with user info.
        /// </remarks>
        [HttpPost("PostUserCreate")]
        public async Task<IActionResult> PostUserCreate([FromBody] UserPostDTO userDTO)
        {
            var response = await UserValidation.GeneralValidationAsync(_dbContext, userDTO);
            if (response != "correcto")
            {
                return StatusCode(400, ManageResponse.ErrorBadRequest(response));
            }

            // Create qualification
            var qualification = new Qualification
            {
                Count = 0,
                AccumulatorAdition = 0
            };
            await _dbContext.Qualifications.AddAsync(qualification);
            await _dbContext.SaveChangesAsync();

            // Create abilities
            var abilities = new Ability
            {
                Category = userDTO.Category,
                Abilities = userDTO.Abilities
            };
            await _dbContext.Abilities.AddAsync(abilities);
            await _dbContext.SaveChangesAsync();

            // Map user
            var user = _mapper.Map<User>(userDTO);
            user.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            user.IdState = 1; // Active by default
            user.IdRol = 2;   // Default role: User
            user.IdQualification = qualification.Id;
            user.IdAbility = abilities.Id;

            // Hash password
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, userDTO.Password);

            // Save user
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // --> SEND WELCOME EMAIL
            try
            {
                await _emailService.SendWelcomeEmail(user.Email, user.Name);
            }
            catch (Exception ex)
            {
                // Optionally log error without stopping registration
                Console.WriteLine($"[EmailService] Error sending welcome email: {ex.Message}");
            }

            // Generate JWT Token
            var token = GenerateJwtToken(user);

            var successResponse = new
            {
                id = user.Id,
                role = user.IdRol,
                email = user.Email,
                token
            };

            return StatusCode(200, ManageResponse.SuccessfullWithObject("Usuario registrado correctamente!", successResponse));
        }

        // Private method to generate JWT Token
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("role", user.IdRol.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT_ISSUER"],
                audience: _configuration["JWT_AUDIENCE"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JWT_EXPIREMINUTES"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}