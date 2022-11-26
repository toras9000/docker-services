#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.58.0"
#load ".target-composes.csx"
#nullable enable
using Lestaly;
using Lestaly.Cx;

await "dotnet".args("script", ThisSource.RelativeFile("ready-network.csx").FullName);

foreach (var compose in manageComposes)
{
    WriteLine($">{compose.File.Directory?.Name}");
    await "docker".args(["compose", "--file", compose.File.FullName, "up", "-d", .. compose.UpArgs ?? []]);
}

WriteLine();
WriteLine("Service addresses");
var webServices = manageComposes.Where(c => c.Service != null).Select(c => (c.Name, c.Service)).ToArray();
var nameWidth = webServices.Max(s => s.Name.Length);
foreach (var (name, url) in webServices)
{
    ConsoleWig.Write(" ").WriteLink(url, $"{name.PadRight(nameWidth)} - {url}").NewLine();
}
if (!IsInputRedirected)
{
    WriteLine("(press any key to exit)");
    ReadKey(intercept: false);
}
