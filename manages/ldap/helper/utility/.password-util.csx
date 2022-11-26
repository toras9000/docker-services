#nullable enable
using System.Security.Cryptography;

string MakeHashPassword(string input)
{
    var salt = new byte[4];
    Random.Shared.NextBytes(salt);
    var source = Encoding.UTF8.GetBytes(input);
    var hashed = SHA256.HashData(source.Concat(salt).ToArray());
    var encoded = Convert.ToBase64String(hashed.Concat(salt).ToArray());
    var value = $"{{SSHA256}}{encoded}";

    return value;
}