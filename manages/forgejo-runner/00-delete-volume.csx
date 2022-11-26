#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.69.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

return await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    WriteLine("Stop containers");
    await "docker".args("compose", "--file", ThisSource.RelativeFile("compose-init.yml").FullName, "down", "--volumes", "--remove-orphans").echo().result().success();
    await "docker".args("compose", "--file", ThisSource.RelativeFile("compose.yml").FullName, "down", "--volumes", "--remove-orphans").echo().result().success();

    WriteLine("Delete volume");
    var volumeName = "forgejo-runner-data";
    var check = await "docker".args("volume", "ls", "--filter", $"name={volumeName}", "--format", "{{.Name}}").silent().result().output();
    if (check.IsNotWhite())
    {
        await "docker".args(["volume", "rm", volumeName]);
    }
});
