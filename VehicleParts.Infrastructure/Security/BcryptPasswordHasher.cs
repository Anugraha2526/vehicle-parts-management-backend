using VehicleParts.Application.Modules.AdminCore.Interfaces;

namespace VehicleParts.Infrastructure.Security;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    // uses bcrypt algorithm to hash passwords before storing
    public string Hash(string plainTextPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
}
