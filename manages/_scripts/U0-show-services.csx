#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly.General, 0.100.0"
#load ".target-composes.csx"
#nullable enable
using Kokuban;
using Lestaly;
using Lestaly.Cx;

return await Paved.ProceedAsync(async () =>
{
    await Task.CompletedTask;
    WriteLine();
    WriteLine("Service addresses");
    var endpoints = ManageServices.GetContainers().SelectMany(c => c.Endpoints ?? []).ToArray();
    var nameWidth = endpoints.Max(s => s.Name.Length);
    foreach (var ep in endpoints)
    {
        var link = Poster.Link[ep.Url, $"{ep.Name.PadRight(nameWidth)} - {ep.Url}"];
        WriteLine($" {link}");
    }
    WriteLine();
});
