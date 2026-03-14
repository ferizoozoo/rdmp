using Data.Database;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services;

public interface IUserService
{
    Task<List<User>> GetUsers();
    Task<User> AddUser(User user);
    Task<User> UpdateUser(int id, User user);
    Task<bool> DeleteUser(int id);
}

public class UserService : IUserService
{
    private readonly RdmpContext _context;

    public UserService(RdmpContext context)
        => _context = context;

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