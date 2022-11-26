#!/usr/bin/env dotnet-script
#r "nuget: BCrypt.Net-Next, 4.0.3"
#r "nuget: Lestaly, 0.68.0"
#nullable enable
using BCrypt.Net;
using Lestaly;
using Lestaly.Cx;

await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    await Task.CompletedTask;

    Write("Entry Password:");
    var password = ReadLine().CancelIfWhite();

    var hash = BCrypt.Net.BCrypt.HashPassword(password);
    WriteLine("BCrypted");
    WriteLine(hash);
});
