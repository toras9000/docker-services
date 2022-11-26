#!/usr/bin/env dotnet-script
#r "nuget: Lestaly.General, 0.106.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

return await Paved.ProceedAsync(noPause: true, async () =>
{
    WriteLine("Create server cert");
    var composeFile = ThisSource.RelativeFile("compose.yml");
    await "docker".args([
        "compose",
            "--file", composeFile,
        "exec", "app", "certbot", "certonly",
            "--non-interactive",
            "--standalone",
            "--server", "https://ca.myserver.home/acme/acme/directory",
            "--preferred-challenges", "http",
            "--cert-name", "myserver.home",
            "--domains", "myserver.home",
            "--domains", "keycloak.myserver.home",
            "--domains", "goaccess.myserver.home",
            "--domains", "goaccess-ws.myserver.home",
            "--domains", "ca.myserver.home",
            "--domains", "forgejo.myserver.home",
            "--domains", "nextcloud.myserver.home",
            "--domains", "bookstack.myserver.home",
            "--domains", "vaultwarden.myserver.home",
            "--domains", "it-tools.myserver.home",
            "--domains", "mini-qr.myserver.home",
            "--domains", "mermaid-ink.myserver.home",
            "--domains", "mermaid-editor.myserver.home",
            "--domains", "planka.myserver.home",
            "--domains", "upsnap.myserver.home"
    ]).result().success();
});

