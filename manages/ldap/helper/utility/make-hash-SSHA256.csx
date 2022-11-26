#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.73.0"
#nullable enable
using System.Security.Cryptography;
using Lestaly;

// This is a C# script for dotnet-script.
// Install .NET8 SDK and `dotnet tool install -g dotnet-script`

while (true)
{
    Write("Input text\n>");
    var input = ReadLine();
    if (string.IsNullOrEmpty(input)) break;

    var value = LdapExtensions.MakePasswordHash.SSHA256(input);
    WriteLine(value);
    WriteLine();
}
