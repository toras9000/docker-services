#!/usr/bin/env dotnet-script
#r "nuget: Lestaly.General, 0.106.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

return await Paved.ProceedAsync(noPause: true, async () =>
{
    WriteLine("Renew server cert");
    var composeFile = ThisSource.RelativeFile("compose.yml");
    await "docker".args([
        "compose",
            "--file", composeFile,
        "exec", "app", "certbot", "renew",
            "--non-interactive",
            "--cert-name", "myserver.home",
            "--force-renewal",
    ]).result().success();

    WriteLine("Replace server cert");
    await "docker".args([
        "compose",
            "--file", composeFile,
        "exec", "app", "cp",
            "/etc/letsencrypt/live/myserver.home/fullchain.pem",
            "/work/certs/server/server.crt",
    ]).result().success();
    await "docker".args([
        "compose",
            "--file", composeFile,
        "exec", "app", "cp",
            "/etc/letsencrypt/live/myserver.home/privkey.pem",
            "/work/certs/server/server.key",
    ]).result().success();

});
