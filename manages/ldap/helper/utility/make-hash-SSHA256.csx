#!/usr/bin/env dotnet-script
#load ".password-util.csx"
#nullable enable
using System.Security.Cryptography;

// This is a C# script for dotnet-script.
// Install .NET8 SDK and `dotnet tool install -g dotnet-script`

while (true)
{
    Write("Input text\n>");
    var input = ReadLine();
    if (string.IsNullOrEmpty(input)) break;

    var value = MakeHashPassword(input);
    WriteLine(value);
    WriteLine();
}
