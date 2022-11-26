#!/usr/bin/env dotnet-script
#nullable enable

// This is a C# script for dotnet-script.
// Install .NET8 SDK and `dotnet tool install -g dotnet-script`
using (var hasher = System.Security.Cryptography.SHA256.Create())
{
    var salt = new byte[4];
    while (true)
    {
        Write("Input text\n>");
        var input = ReadLine();
        if (string.IsNullOrEmpty(input)) break;

        Random.Shared.NextBytes(salt);
        var source = Encoding.UTF8.GetBytes(input);
        var hashed = hasher.ComputeHash(source.Concat(salt).ToArray());
        var encoded = Convert.ToBase64String(hashed.Concat(salt).ToArray());
        var value = $"{{SSHA256}}{encoded}";
        WriteLine(value);
        WriteLine();
    }
}
