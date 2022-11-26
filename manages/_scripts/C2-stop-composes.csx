#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly, 0.83.0"
#load ".target-composes.csx"
#nullable enable
using Lestaly;
using Lestaly.Cx;

foreach (var info in ManageServices.GetContainers().Reverse())
{
    WriteLine($">{info.Compose.File.Directory?.Name}");
    await "docker".args("compose", "--file", info.Compose.File, "down", "--volumes", "--remove-orphans");
}
