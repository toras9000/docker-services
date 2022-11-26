#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly, 0.69.0"
#load ".target-composes.csx"
#nullable enable
using Lestaly;
using Lestaly.Cx;

await "dotnet".args("script", ThisSource.RelativeFile("C2-stop-composes.csx").FullName);

var archiveDir = ThisSource.RelativeDirectory("../../volumes");

WriteLine($"Archive volumes data");
foreach (var name in ManageServices.GetDockerVolumeNames())
{
    WriteLine($">{name}");
    await "docker".args(
        "run", "--rm", "-i",
        "--mount", $"type=volume,source={name},target=/volume-data,readonly",
        "--mount", $"type=bind,source={archiveDir.FullName},target=/arch-data",
        "--workdir", "/volume-data",
        "busybox",
        "tar", "zcf", $"/arch-data/{name}.tgz", "."
    );
}
