#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly, 0.83.0"
#load ".target-composes.csx"
#nullable enable
using Kokuban;
using Lestaly;
using Lestaly.Cx;

async ValueTask ensureNamedVolumeAsync(string name)
{
    var vollist = await "docker".args("volume", "ls", "--filter", $"name={name}", "--format", "{{.Name}}").silent().result().output();
    var volumes = vollist.AsTextLines().DropWhite().ToArray();
    var exists = volumes.Contains(name);
    if (exists)
    {
        WriteLine(Chalk.Gray[$" {name} - already exists"]);
    }
    else
    {
        Write(" ");
        await "docker".args(["volume", "create", "--driver", "local", name]);
    }
}

WriteLine("Prepare volumes");
foreach (var name in ManageServices.GetDockerVolumeNames())
{
    await ensureNamedVolumeAsync(name);
}
