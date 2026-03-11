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
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public AuthController(IUserRepository userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length < 2)
            return BadRequest(new { message = "Name must be at least 2 characters." });
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Email is required." });
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return BadRequest(new { message = "Password must be at least 6 characters." });

        var existing = await _userRepo.GetParentByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (existing != null)
            return BadRequest(new { message = "Email already registered." });

        var parent = new Parent
        {
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = HashPassword(request.Password),
            Name = request.Name.Trim(),
            Phone = request.Phone?.Trim() ?? ""
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
        var parent = await _userRepo.GetParentByEmailAsync(request.Email?.Trim().ToLowerInvariant() ?? "");
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
        var jwtKey = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key not configured. Set Jwt:Key in appsettings.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
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

    /// <summary>PBKDF2 with 128-bit salt, 100K iterations, SHA256</summary>
    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        var result = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);
        return Convert.ToBase64String(result);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var decoded = Convert.FromBase64String(storedHash);
        if (decoded.Length != SaltSize + HashSize) return false;
        var salt = decoded[..SaltSize];
        var expectedHash = decoded[SaltSize..];
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
