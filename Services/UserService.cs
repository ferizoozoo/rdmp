using Data.Database;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Services;

public interface IUserService
{
    Task<LoginResponse> Login(LoginRequest request);
    Task<RegisterResponse> Register(RegisterRequest request);
    Task<RefreshResponse> Refresh(RefreshRequest request);
    Task<User> GetById(int id);
    Task<List<User>> GetUsers();
    Task<User> AddUser(User user);
    Task<User> UpdateUser(int id, User user);
    Task<bool> DeleteUser(int id);
}

public class UserService : IUserService
{
    private readonly RdmpContext _context;
    private readonly IJwtService _jwtService;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(RdmpContext context, IJwtService jwtService)
    {
        _jwtService = jwtService;
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return null;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var refreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateAccessToken(user.Id);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var password = request.Password;

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser is not null)
        {
            return null;
        }

        var user = new User { Email = email };
        var passwordHash = _passwordHasher.HashPassword(user, password);
        user.PasswordHash = passwordHash;
        user.RefreshToken = _jwtService.GenerateRefreshToken();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateAccessToken(user.Id);

        return new RegisterResponse
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken
        };
    }

    public async Task<User> GetById(int id)
            => await _context.Users.FindAsync(id);

    public async Task<RefreshResponse> Refresh(RefreshRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
        if (user is null || !_jwtService.ValidateRefreshToken(request.RefreshToken))
        {
            return null;
        }

        var accessToken = _jwtService.GenerateAccessToken(user.Id);
        var refreshToken = _jwtService.GenerateRefreshToken();

        if (user != null)
        {
            user.RefreshToken = refreshToken;
            await _context.SaveChangesAsync();
        }

        return new RefreshResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<List<User>> GetUsers()
        => await _context.Users.ToListAsync();

    public async Task<User> AddUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUser(int id, User user)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser is null)
        {
            return null;
        }

        existingUser.Firstname = user.Firstname;
        existingUser.Email = user.Email;
        existingUser.Lastname = user.Lastname;

        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }


}
