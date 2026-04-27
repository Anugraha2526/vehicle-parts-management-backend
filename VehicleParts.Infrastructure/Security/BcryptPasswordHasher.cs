using VehicleParts.Application.Modules.AdminCore.Interfaces;

namespace VehicleParts.Infrastructure.Security;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plainTextPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
}
