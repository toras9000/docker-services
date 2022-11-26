#!/usr/bin/env dotnet-script
#r "nuget: Lestaly.General, 0.105.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

return await Paved.ProceedAsync(noPause: Args.RoughContains("--no-pause"), async () =>
{
    WriteLine("Renew server cert");
    var composeFile = ThisSource.RelativeFile("compose.yml");
    await "docker".args([
        "compose", "--file", composeFile, "exec", "app",
        "step", "ca", "renew",
            "--mtls=false",
            "--force",
            "/work/certs/server/server.crt",
            "/work/certs/server/server.key"
    ]).result().success();
});
