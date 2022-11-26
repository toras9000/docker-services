#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly.General, 0.106.0"
#load ".target-composes.csx"
#nullable enable
using Lestaly;
using Lestaly.Cx;

await "dotnet".args("script", ThisSource.RelativeFile("C0-prepare-networks.csx"));
await "dotnet".args("script", ThisSource.RelativeFile("C0-prepare-volumes.csx"));

var endpoints = new List<ServiceEndpoint>();
foreach (var info in ManageServices.GetContainers())
{
    WriteLine($">{info.Compose.File.Directory?.Name}");
    await "docker".args(["compose", "--file", info.Compose.File, "up", "-d", .. info.Compose.UpArgs ?? []]);

    if (info.Endpoints != null) endpoints.AddRange(info.Endpoints);
}

WriteLine();
WriteLine("Service addresses");
var nameWidth = endpoints.Max(s => s.Name.Length);
foreach (var ep in endpoints)
{
    var link = Poster.Link[ep.Url, $"{ep.Name.PadRight(nameWidth)} - {ep.Url}"];
    WriteLine($" {link}");
}
WriteLine();

if (!IsInputRedirected)
{
    WriteLine("(press any key to exit)");
    ReadKey(intercept: false);
}
