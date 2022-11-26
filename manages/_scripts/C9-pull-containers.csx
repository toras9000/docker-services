#!/usr/bin/env dotnet-script
#load ".target-composes.csx"
#nullable enable
using Lestaly;
using Lestaly.Cx;

foreach (var info in ManageServices.GetContainers().Reverse())
{
    WriteLine($">{info.Compose.File.Directory?.Name}");
    await "docker".args("compose", "--file", info.Compose.File.FullName, "pull");
}
