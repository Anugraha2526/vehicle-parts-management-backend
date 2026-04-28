namespace VehicleParts.Application.Modules.AdminCore.Interfaces;

// abstracts password hashing so infrastructure details stay out of application layer
public interface IPasswordHasher
{
    string Hash(string plainTextPassword);
}
