#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.68.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

return await Paved.RunAsync(config: o => o.AnyPause(), action: async () =>
{
    var registry = "registry.myserver.home";

    await "docker".args("login", registry).echo().result().success();

    async Task pushImage(string name, string dockerfile)
        => await "docker".args("build", "--push", "--tag", $"{registry}/{name}", "-").input(dockerfile).result().success();

    await pushImage("user1/alpine-openssl", """
        FROM alpine
        RUN apk add --no-cache openssl
    """);

    await pushImage("user1/alpine-nodejs", """
        FROM alpine
        RUN apk add --no-cache nodejs
    """);

    await pushImage("dev/alpine-git", """
        FROM alpine
        RUN apk add --no-cache git
    """);

});

