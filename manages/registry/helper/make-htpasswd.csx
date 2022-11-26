#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.65.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    var userName = ConsoleWig.WriteLine("Input Username").Write(">").ReadLine().CancelIfWhite();
    await "docker".args("run", "-i", "--rm", "httpd:2", "htpasswd", "-nB", userName).input(Console.In);
});
