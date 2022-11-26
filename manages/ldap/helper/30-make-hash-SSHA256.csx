#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.83.0"
#nullable enable
using System.Security.Cryptography;
using Lestaly;

while (true)
{
    Write("Input text\n>");
    var input = ReadLine();
    if (string.IsNullOrEmpty(input)) break;

    var value = LdapExtensions.MakePasswordHash.SSHA256(input);
    WriteLine(value);
    WriteLine();
}
