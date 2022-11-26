#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly, 0.69.0"
#load ".target-composes.csx"
#nullable enable
using Lestaly;
using Lestaly.Cx;

await "dotnet".args("script", ThisSource.RelativeFile("C2-stop-composes.csx").FullName);

WriteLine($"Delete volumes");
foreach (var name in ManageServices.GetDockerVolumeNames())
{
    var check = await "docker".args("volume", "ls", "--filter", $"name={name}", "--format", "{{.Name}}").silent().result().output();
    if (check.IsNotWhite())
    {
        Write(" ");
        await "docker".args(["volume", "rm", name]);
    }
}
