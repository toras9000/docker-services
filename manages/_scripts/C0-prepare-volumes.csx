#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly, 0.68.0"
#load ".target-composes.csx"
#nullable enable
using Kokuban;
using Lestaly;
using Lestaly.Cx;

async ValueTask ensureNamedVolumeAsync(string name)
{
    var check = await "docker".args("volume", "ls", "--filter", $"name={name}", "--format", "{{.Name}}").silent().result().output();
    if (check.IsWhite())
    {
        Write(" ");
        await "docker".args(["volume", "create", "--driver", "local", name]);
    }
    else
    {
        WriteLine(Chalk.Gray[$" {name} - already exists"]);
    }
}

WriteLine("Prepare volumes");
foreach (var name in ManageServices.GetDockerVolumeNames())
{
    await ensureNamedVolumeAsync(name);
}
