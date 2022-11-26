#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.68.0"
#r "nuget: Kokuban, 0.2.0"
#nullable enable
using System.Net.Http;
using Lestaly;
using Lestaly.Cx;

return await Paved.RunAsync(config: o => o.AnyPause(), action: async () =>
{
    var serviceFeed = "https://bagetter.myserver.home/v3/index.json";
    var serviceApiKey = "NUGET-SERVER-API-KEY";

    WriteLine("Enter the package file path to be pushed.");
    while (true)
    {
        Write(">");
        var input = Console.ReadLine();
        if (input.IsWhite()) continue;

        await "dotnet".args("nuget", "push", "-k", serviceApiKey, "-s", serviceFeed, input);
    }
});

