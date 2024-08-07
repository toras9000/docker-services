#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.65.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    var composeFile = ThisSource.RelativeFile("../docker-compose.yml");
    await "docker".args("compose", "-f", composeFile.FullName, "exec", "app", "registry", "garbage-collect", "/etc/docker/registry/config.yml");
});
