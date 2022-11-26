#!/usr/bin/env dotnet-script
#r "nuget: Lestaly.General, 0.100.0"
#r "nuget: Lestaly.Ldap, 0.100.0"
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
