#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.65.0"
#load ".target-composes.csx"
#nullable enable
using Lestaly;
using Lestaly.Cx;

await "dotnet".args("script", ThisSource.RelativeFile("ready-network.csx").FullName);

foreach (var compose in manageComposes)
{
    WriteLine($">{compose.File.Directory?.Name}");
    await "docker".args(["compose", "--file", compose.File.FullName, "up", "-d", .. compose.UpArgs ?? []]);
}

WriteLine();
WriteLine("Service addresses");
var nameWidth = serviceEps.Max(s => s.Name.Length);
foreach (var ep in serviceEps)
{
    var link = Poster.Link[ep.Url, $"{ep.Name.PadRight(nameWidth)} - {ep.Url}"];
    WriteLine($" {link}");
}
if (!IsInputRedirected)
{
    WriteLine("(press any key to exit)");
    ReadKey(intercept: false);
}
