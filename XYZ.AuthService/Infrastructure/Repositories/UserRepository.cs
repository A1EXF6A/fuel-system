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
}
