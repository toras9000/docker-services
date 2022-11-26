#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly.General, 0.106.0"
#load ".target-composes.csx"
#nullable enable
using Kokuban;
using Lestaly;
using Lestaly.Cx;

async ValueTask ensureNetworkAsync(string name, string subnet, params string[] additional)
{
    var check = await "docker".args("network", "ls", "--filter", $"name={name}", "--format", "{{.Name}}").silent().result().output();
    if (check.IsWhite())
    {
        Write(" ");
        await "docker".args(["network", "create", name, "--subnet", subnet, .. additional]);
    }
    else
    {
        WriteLine(Chalk.Gray[$" {name} - already exists"]);
    }
}

WriteLine("Prepare networks");
foreach (var nw in ManageServices.GetNetworks())
{
    await ensureNetworkAsync(nw.Name, nw.Subnet);
}

