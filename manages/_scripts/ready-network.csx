#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.58.0"
using Lestaly;
using Lestaly.Cx;

async ValueTask ensureNetworkAsync(string name, string subnet, params string[] additional)
{
    var check = await "docker".args("network", "ls", "--filter", $"name={name}", "--format", "{{.Name}}").silent().result().output();
    if (check.IsWhite())
    {
        await "docker".args(["network", "create", name, "--subnet", subnet, .. additional]);
    }
}

await ensureNetworkAsync("composes-frontend", "172.31.0.0/16");
