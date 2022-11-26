#!/usr/bin/env dotnet-script
#r "nuget: Lestaly.General, 0.108.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

return await Paved.ProceedAsync(noPause: Args.RoughContains("--no-pause"), async () =>
{
    WriteLine("Create server cert");
    var composeFile = ThisSource.RelativeFile("compose.yml");
    await "docker".args([
        "compose", "--file", composeFile, "exec", "app",
        "step", "ca", "certificate",
            "--provisioner=default",
            "--password-file=/home/step/secrets/password",
            "--san", "myserver.home",
            "--san", "*.myserver.home",
            "--force",
            "myserver.home",
            "/work/certs/server/server.crt",
            "/work/certs/server/server.key"
    ]).result().success();
});
