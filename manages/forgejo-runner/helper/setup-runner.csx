#!/usr/bin/env dotnet-script
#r "nuget: AngleSharp, 1.2.0"
#r "nuget: Lestaly, 0.69.0"
#r "nuget: Kokuban, 0.2.0"
#nullable enable
using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Kokuban;
using Lestaly;
using Lestaly.Cx;

var settings = new
{
    ComposeFile = ThisSource.RelativeFile("../compose.yml"),

    RunnerContainer = "runner",

    ForgejoURL = "https://forgejo.myserver.home",

    Labels = new[]
    {
        new { Name = "docker", URI = "docker://ubuntu:noble",     },
        new { Name = "node",   URI = "docker://node:20-bullseye", },
    },
};

await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    using var signal = new SignalCancellationPeriod();
    using var outenc = ConsoleWig.OutputEncodingPeriod(Encoding.UTF8);

    WriteLine("Enter runner token");
    Write(">");
    var token = ReadLine().CancelIfWhite().Trim();

    WriteLine("Enter runner name");
    Write(">");
    var name = ReadLine().CancelIfWhite().Trim();

    WriteLine("Stop runner");
    await "docker".args("compose", "--file", settings.ComposeFile.FullName,
        "rm", "--stop", "--force", settings.RunnerContainer
    ).silent().cancelby(signal.Token).result().success();

    WriteLine("Setup permissions");
    await "docker".args("compose", "--file", settings.ComposeFile.FullName,
        "run", "--rm", "--user", "root", settings.RunnerContainer,
        "bash", "/assets/init/setup.sh"
    ).silent().cancelby(signal.Token).result().success();

    WriteLine("Register runner");
    var labels = settings.Labels.Select(l => $"{l.Name}:{l.URI}").JoinString(",");
    await "docker".args("compose", "--file", settings.ComposeFile.FullName,
       "run", "--rm", settings.RunnerContainer,
       "forgejo-runner", "register", "--no-interactive",
       "--name", name,
       "--instance", settings.ForgejoURL,
       "--token", token,
       "--labels", labels
    ).silent().cancelby(signal.Token).result().success();
    WriteLine(".. Completed");

    WriteLine("Start runner");
    await "docker".args("compose", "--file", settings.ComposeFile.FullName,
        "up", "-d", settings.RunnerContainer
    ).silent().cancelby(signal.Token).result().success();
});
