#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly, 0.73.0"
#load ".target-composes.csx"
#nullable enable
using Kokuban;
using Lestaly;
using Lestaly.Cx;

return await Paved.RunAsync(async () =>
{
    var containers = ManageServices.GetContainers().ToArray();
    var namewidth = containers.Max(c => c.Name.Length);

    var services = new List<string>();
    for (var i = 0; i < containers.Length; i++)
    {
        var container = containers[i];
        var padding = new string('.', 2 + namewidth - container.Name.Length);
        var endpoints = container.Endpoints?.Length switch
        {
            1 => $" {padding} {Poster.Link[container.Endpoints[0].Url]}",
            >= 2 => container.Endpoints.Select((ep, i) => $"[{Poster.Link[ep.Url, "Address"]}]").JoinString(" ").DecoratePrefix($" {padding} "),
            _ => default,
        };
        services.Add($"  {i + 1,2}: {container.Name}{endpoints}");
    }

    void WriteServices(string caption)
    {
        WriteLine(caption);
        foreach (var svc in services) WriteLine(svc);
        WriteLine();
    }

    WriteLine("Reset (restart & delete volumes) specify service");
    WriteServices("Service list:");

    while (true)
    {
        Write(">");
        var input = ReadLine();
        if (input.IsWhite()) continue;

        var number = input.TryParseNumber<int>();
        if (!number.HasValue) { WriteLine(Chalk.Yellow["Illegal number"]); continue; }
        var target = containers.ElementAtOrDefault(number.Value - 1);
        if (target == null) { WriteServices(Chalk.Yellow["Specify service number"]); continue; }

        try
        {
            WriteLine($"Target: {Chalk.Green[target.Name]}");
            WriteLine("Down service");
            await "docker".args("compose", "--file", target.Compose.File.FullName, "down", "--volumes", "--remove-orphans").silent().result().success();

            if (0 < target.Compose.Volumes?.Length)
            {
                WriteLine("Recreate volumes");
                foreach (var vol in target.Compose.Volumes)
                {
                    await "docker".args(["volume", "rm", vol]).silent();
                    await "docker".args(["volume", "create", "--driver", "local", vol]).silent().result().success();
                }
            }

            WriteLine("Up service");
            await "docker".args(["compose", "--file", target.Compose.File.FullName, "up", "-d"]);
        }
        catch (Exception ex)
        {
            WriteLine(Chalk.Red[$"Error: {ex.Message}"]);
        }
        WriteLine();

    }
});
