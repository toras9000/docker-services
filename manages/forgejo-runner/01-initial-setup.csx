#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.69.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

return await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    WriteLine("Prepare volume");
    var volumeName = "forgejo-runner-data";
    var check = await "docker".args("volume", "ls", "--filter", $"name={volumeName}", "--format", "{{.Name}}").silent().result().output();
    if (check.IsWhite())
    {
        await "docker".args(["volume", "create", "--driver", "local", volumeName]);
    }

    WriteLine("Start initialize container");
    var composeFile = ThisSource.RelativeFile("compose-init.yml");
    await "docker".args("compose", "--file", composeFile.FullName, "run", "register").echo().input(In).redirect(Out).result().success();
});
