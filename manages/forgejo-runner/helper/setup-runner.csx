#!/usr/bin/env dotnet-script
#r "nuget: AngleSharp, 1.2.0"
#r "nuget: Lestaly, 0.73.0"
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
        new { Name = "ubuntu",             URI = "docker://ubuntu:24.04",                      },
        new { Name = "ubuntu-24.04",       URI = "docker://ubuntu:24.04",                      },
        new { Name = "ubuntu-22.04",       URI = "docker://ubuntu:22.04",                      },
        new { Name = "debian",             URI = "docker://debian:12",                         },
        new { Name = "debian-12",          URI = "docker://debian:12",                         },
        new { Name = "debian-11",          URI = "docker://debian:11",                         },
        new { Name = "node-debian",        URI = "docker://node:20-bookworm",                  },
        new { Name = "node-20-debian-12",  URI = "docker://node:20-bookworm",                  },
        new { Name = "node-20-debian-11",  URI = "docker://node:20-bullseye",                  },
        new { Name = "dotnet-sdk",         URI = "docker://mcr.microsoft.com/dotnet/sdk:9.0",  },
        new { Name = "dotnet-sdk-9",       URI = "docker://mcr.microsoft.com/dotnet/sdk:9.0",  },
    },
};

await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    using var signal = new SignalCancellationPeriod();
    using var outenc = ConsoleWig.OutputEncodingPeriod(Encoding.UTF8);

    WriteLine($"Enter service URL(empty default: {settings.ForgejoURL})");
    Write(">");
    var service = ReadLine().CancelIfWhite().Trim();

    WriteLine("Enter runner token");
    Write(">");
    var token = ReadLine().CancelIfWhite().Trim();

    WriteLine("Enter runner name");
    Write(">");
    var name = ReadLine().CancelIfWhite().Trim();

    WriteLine("Register runner");
    var labels = settings.Labels.Select(l => $"{l.Name}:{l.URI}").JoinString(",");
    await "docker".args("compose", "--file", settings.ComposeFile.FullName,
       "run", "--rm", settings.RunnerContainer,
       "forgejo-runner", "register", "--no-interactive",
       "--name", name,
       "--instance", service,
       "--token", token,
       "--labels", labels
    ).silent().cancelby(signal.Token).result().success();
    WriteLine(".. Completed");
});
