using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;

[Route("auth")]
public class AuthController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => new { Description = e.ErrorMessage }).ToList();
            return BadRequest(errors);
        }

        var user = new IdentityUser { UserName = request.Username, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var username = user.UserName ?? throw new InvalidOperationException("Username cannot be null.");
            var token = GenerateJwtToken(username);

            _logger.LogInformation("Generated JWT token: {Token}", token);

            // Create HttpOnly Cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set to true in production with HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            };
            Response.Cookies.Append("jwt", token, cookieOptions);

            return Ok(new { Message = "Login successful" });
        }

        return Unauthorized(new { Message = "Login failed. Please check your credentials and try again." });
    }

    [HttpGet("auth-state")]
    public IActionResult CheckAuthState()
    {
        _logger.LogInformation("Checking authentication state");

        if (Request.Cookies.TryGetValue("jwt", out var token))
        {
            _logger.LogInformation("JWT cookie found: {Token}", token);
            var validatedToken = ValidateJwtToken(token);
            if (validatedToken != null)
            {
                var usernameClaim = validatedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (usernameClaim != null)
                {
                    var username = usernameClaim.Value;
                    _logger.LogInformation("Username claim found: {Username}", username);
                    return Ok(new { Username = username, Authenticated = true });
                }
                else
                {
                    _logger.LogWarning("Username claim not found in the token");
                    return Unauthorized(new { Message = "Username claim not found in the token", Authenticated = false });
                }
            }
            else
            {
                _logger.LogWarning("Invalid token");
                return Unauthorized(new { Message = "Invalid token", Authenticated = false });
            }
        }
        else
        {
            _logger.LogWarning("JWT cookie not found");
            return Unauthorized(new { Message = "JWT cookie not found", Authenticated = false });
        }
    }

    private string GenerateJwtToken(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            _logger.LogError("Username cannot be null or empty");
            throw new ArgumentNullException(nameof(username), "Username cannot be null or empty");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username), // Use ClaimTypes.NameIdentifier
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        _logger.LogInformation("Generating token for: {Username} with claims: {Claims}", username, string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));

        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "Secret key cannot be null or empty"));
        var securityKey = new SymmetricSecurityKey(key);
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation("Generated token: {Token}", tokenString);

        return tokenString;
    }

    private ClaimsPrincipal? ValidateJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Token cannot be null or empty");
            throw new ArgumentNullException(nameof(token), "Token cannot be null or empty");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "Secret key cannot be null or empty"));
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Log all claims
            foreach (var claim in principal.Claims)
            {
                _logger.LogInformation("Claim Type: {ClaimType}, Value: {ClaimValue}", claim.Type, claim.Value);
            }

            _logger.LogInformation("Token validated successfully");
            return principal;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning("Token validation failed: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while validating token: {Message}", ex.Message);
            return null;
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { Message = "Logout successful" });
    }
}

public class RegisterRequest
{
    [UsernameValidation]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
