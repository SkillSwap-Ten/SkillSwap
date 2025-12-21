using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkillSwap.Dtos.User;
using SkillSwap.Interfaces;
using SkillSwap.Models;

namespace SkillSwap.Controllers.V1.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IEmailService _emailService;

    //Constructor
    public AuthController(AppDbContext dbContext, IEmailService emailService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <remarks>
    /// This endpoint allows users to log in by providing their email and password.
    /// If the credentials are valid, a JWT token is generated and returned along with user details.
    /// </remarks>
    /// <param name="userLoginPostDTO">The user's login credentials containing email and password.</param>
    /// <returns>
    /// A 200 OK response with a success message and user details including the JWT token.
    /// A 400 Bad Request response if the email or password fields are empty, with a message indicating "Los campos están vacíos"
    /// A 401 Unauthorized response if the user does not exist, with a message indicating "Autenticación requerida."
    /// A 404 Not Found response if the password is incorrect, with a message indicating "Contraseña incorrecta."
    /// </returns>
    [HttpPost("PostAuthLogin")]
    public async Task<IActionResult> PostAuthLogin([FromBody] AuthDTO userLoginPostDTO)
    {
        // Check if the request body or essential fields (Email and Password) are null or empty.
        if (userLoginPostDTO == null || string.IsNullOrEmpty(userLoginPostDTO.Email) || string.IsNullOrEmpty(userLoginPostDTO.Password))
        {
            return StatusCode(400, ManageResponse.ErrorBadRequest("Los campos están vacíos."));
        }

        // Look for a user in the database with the provided email.
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userLoginPostDTO.Email);
        if (user == null)
        {
            return StatusCode(401, ManageResponse.ErrorUnauthorized());
        }

        // Verify the password using the password hasher.
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, userLoginPostDTO.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return StatusCode(404, ManageResponse.ErrorBadRequest("Contraseña incorrecta"));
        }

        // Generate a JWT token for the authenticated user.
        var token = GenerateJwtToken();

        var response = new
        {
            id = user.Id,
            role = user.IdRol,
            email = user.Email,
            token
        };

        return StatusCode(200, ManageResponse.SuccessfullWithObject("Éxito", response));
    }

    /// <summary>
    /// Request a password reset link via email
    /// </summary>
    /// <remarks>
    /// This endpoint allows a user to request a password reset link. If the email exists in the system,
    /// a reset token will be generated and sent to the user's email.
    /// </remarks>
    /// <param name="model">An object containing the user's email address.</param>
    /// <returns>
    /// Returns an HTTP status code:
    /// - 200 OK: If the reset link is successfully sent.
    /// - 404 Not Found: If no user is found with the provided email address.
    /// </returns>
    [HttpPost("RequestEmail")]
    public async Task<IActionResult> ForgotPassword([FromBody] UserRequestEmail model)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
            return NotFound("No se encontró un usuario con ese correo.");

        // Generar el token de restauración
        var resetToken = GenerateJwtToken();

        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.Now.AddHours(24);

        await _dbContext.SaveChangesAsync();

        // Enviar el correo
        var resetTokenEncoded = Uri.EscapeDataString(resetToken);
        await _emailService.SendPasswordResetEmail(user.Email, resetTokenEncoded);

        return Ok("El enlace de restauración ha sido enviado al correo.");
    }

    /// <summary>
    /// Reset a user's password using a valid reset token
    /// </summary>
    /// <remarks>
    /// This endpoint allows a user to reset their password by providing a valid reset token.
    /// The token must not be expired, and the new password will be hashed before being stored.
    /// </remarks>
    /// <param name="model">An object containing the reset token and the new password.</param>
    /// <returns>
    /// Returns an HTTP status code:
    /// - 200 OK: If the password is successfully reset.
    /// - 400 Bad Request: If the token is invalid or expired.
    /// </returns>
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] UserRequestPassword model)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == model.Token && u.PasswordResetTokenExpiry > DateTime.Now);

        if (user == null)
        {
            return BadRequest("Token inválido o expirado.");
        }


        // Create PasswordHasher<User> instance
        var passwordHasher = new PasswordHasher<User>();

        // Hash the password and assign it to the user's Password property
        user.Password = passwordHasher.HashPassword(user, model.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        await _dbContext.SaveChangesAsync();

        return Ok("Contraseña restablecida correctamente.");
    }

    // Generate JWT token for authenticated users
    private string GenerateJwtToken()
    {
        // Create a security key using the secret key from configuration.
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"]));

        // Create signing credentials using the security key and HMAC-SHA256 algorithm.
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Define the claims to include in the token, such as the user's email and a unique identifier (JTI).
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Build the JWT token with the defined issuer, audience, claims, expiration time, and signing credentials.
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT_ISSUER"],
            audience: _configuration["JWT_AUDIENCE"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JWT_EXPIREMINUTES"])),
            signingCredentials: credentials                    // Signing credentials for token security
        );

        // Return the JWT token as a string.
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}