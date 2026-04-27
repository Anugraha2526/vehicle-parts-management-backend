namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

public interface IPasswordHasher
{
    string Hash(string plainTextPassword);
}
