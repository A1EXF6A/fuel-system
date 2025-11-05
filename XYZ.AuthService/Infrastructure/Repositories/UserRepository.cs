using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XYZ.AuthService.Domain.Entities;
using XYZ.AuthService.Infrastructure.Persistence;

namespace XYZ.AuthService.Infrastructure.Repositories;

public class UserRepository
{
    private readonly AuthDbContext _db;

    public UserRepository(AuthDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<User>> ListAsync()
    {
        return await _db.Users.AsNoTracking().ToListAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }
}
