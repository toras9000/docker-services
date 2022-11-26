#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly.General, 0.105.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

return await Paved.ProceedAsync(noPause: Args.RoughContains("--no-pause"), async () =>
{
    WriteLine("Check state");
    if (ThisSource.RelativeDirectory("assets/step").Exists)
    {
        WriteLine("Already initialized");
        return;
    }

    WriteLine("Init step-ca");
    var initFile = ThisSource.RelativeFile("compose-init.yml");
    await "docker".args(["compose", "--file", initFile, "up"]).result().success();
    await "docker".args(["compose", "--file", initFile, "down", "--volumes", "--remove-orphans"]).result().success();
});
