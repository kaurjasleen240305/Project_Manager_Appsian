using Microsoft.EntityFrameworkCore;
using MiniProjectManager.Application.DTOs.Auth;
using MiniProjectManager.Application.Interfaces;
using MiniProjectManager.Infrastructure.Data;
using MiniProjectManager.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace MiniProjectManager.Application.Services;
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtHelper _jwt;

    public AuthService(AppDbContext context, JwtHelper jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            throw new Exception("Username already exists");

        var user = new User
        {
            Username = request.Username,
            PasswordHash = Hash(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse { Username = user.Username, Token = _jwt.GenerateToken(user) };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username)
            ?? throw new Exception("Invalid credentials");

        if (user.PasswordHash != Hash(request.Password))
            throw new Exception("Invalid credentials");

        return new AuthResponse { Username = user.Username, Token = _jwt.GenerateToken(user) };
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
