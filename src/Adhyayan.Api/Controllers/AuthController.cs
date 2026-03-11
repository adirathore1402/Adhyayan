using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Adhyayan.Core.DTOs;
using Adhyayan.Core.Interfaces;
using Adhyayan.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Adhyayan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;

    public AuthController(IUserRepository userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existing = await _userRepo.GetParentByEmailAsync(request.Email);
        if (existing != null)
            return BadRequest(new { message = "Email already registered." });

        var parent = new Parent
        {
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Name = request.Name,
            Phone = request.Phone
        };

        await _userRepo.CreateParentAsync(parent);
        var token = GenerateJwtToken(parent);

        return Ok(new AuthResponse
        {
            Token = token,
            Name = parent.Name,
            Email = parent.Email,
            ParentId = parent.Id
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var parent = await _userRepo.GetParentByEmailAsync(request.Email);
        if (parent == null || !VerifyPassword(request.Password, parent.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var token = GenerateJwtToken(parent);

        return Ok(new AuthResponse
        {
            Token = token,
            Name = parent.Name,
            Email = parent.Email,
            ParentId = parent.Id
        });
    }

    private string GenerateJwtToken(Parent parent)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "AdhyayanSuperSecretKey2026!@#$%^&*()"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, parent.Id.ToString()),
            new Claim(ClaimTypes.Email, parent.Email),
            new Claim(ClaimTypes.Name, parent.Name)
        };

        var expiryDays = int.Parse(_config["Jwt:ExpiryInDays"] ?? "30");
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "Adhyayan",
            audience: _config["Jwt:Audience"] ?? "AdhyayanApp",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expiryDays),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
