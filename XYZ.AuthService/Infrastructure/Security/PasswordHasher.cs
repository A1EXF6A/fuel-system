using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace XYZ.AuthService.Infrastructure.Security;

public class PasswordHasher
{
    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32));

        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = parts[1];

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32));

        return hashed == hash;
    }
}
