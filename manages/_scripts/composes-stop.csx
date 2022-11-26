#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.58.0"
#load ".target-composes.csx"
#nullable enable
using Lestaly;
using Lestaly.Cx;

foreach (var compose in manageComposes.Reverse())
{
    WriteLine($">{compose.File.Directory?.Name}");
    await "docker".args("compose", "--file", compose.File.FullName, "down", "--volumes", "--remove-orphans");
}
