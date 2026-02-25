using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CarServiceCenter.Application.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
    password: password,
    salt: salt,
    prf: KeyDerivationPrf.HMACSHA256,
    iterationCount: 10000,
    numBytesRequested: 256 / 8
        ));

        string saltString = Convert.ToBase64String(salt);
        return $"{saltString}.{hashed}";
    }

    //Verify Password
    public static bool VerifyPassword(string hashedPasswordWithSalt, string inputPassword)
    {
        var parts = hashedPasswordWithSalt.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hashOfInput = Convert.ToBase64String(KeyDerivation.Pbkdf2
        (
  password: inputPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        ));
        return hashOfInput == parts[1];
    }
}
