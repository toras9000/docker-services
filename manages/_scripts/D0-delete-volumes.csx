#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly.General, 0.100.0"
#load ".target-composes.csx"
#nullable enable
using Lestaly;
using Lestaly.Cx;

await "dotnet".args("script", ThisSource.RelativeFile("C2-stop-composes.csx"));

WriteLine($"Delete volumes");
var hasFail = false;
foreach (var name in ManageServices.GetDockerVolumeNames())
{
    var vollist = await "docker".args("volume", "ls", "--filter", $"name={name}", "--format", "{{.Name}}").silent().result().output();
    var volumes = vollist.AsTextLines().DropWhite().ToArray();
    var exists = volumes.Contains(name);
    if (exists)
    {
        Write(" ");
        var result = await "docker".args(["volume", "rm", name]).result();
        if (result.ExitCode != 0)
        {
            hasFail = true;
            WriteLine($".. Failed");
        }
    }
}
if (hasFail && !Console.IsInputRedirected)
{
    WriteLine();
    WriteLine("(Press any key to exit)");
    ReadKey(intercept: true);
}
