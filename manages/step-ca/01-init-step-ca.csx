#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly.General, 0.108.0"
#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using Lestaly;
using Lestaly.Cx;

return await Paved.ProceedAsync(noPause: Args.RoughContains("--no-pause"), async () =>
{
    WriteLine("Check state");
    if (ThisSource.RelativeDirectory("assets/step").Exists)
    {
        WriteLine("Already initialized");
        return;
    }

    WriteLine("Init step-ca");
    var initFile = ThisSource.RelativeFile("compose-init.yml");
    await "docker".args(["compose", "--file", initFile, "up"]).result().success();
    await "docker".args(["compose", "--file", initFile, "down", "--volumes", "--remove-orphans"]).result().success();

    WriteLine("Edit CA config");
    var configFile = ThisSource.RelativeFile("assets/step/config/ca.json");
    var configJson = await configFile.ReadJsonAsync<JsonNode>() ?? throw new Exception();
    var authorityElem = configJson["authority"] ?? throw new Exception();
    authorityElem["claims"] = new JsonObject([
        new("defaultTLSCertDuration", JsonValue.Create("1080h")),
        new("maxTLSCertDuration", JsonValue.Create("9552h")),
    ]);
    await configFile.WriteJsonAsync(configJson, new JsonSerializerOptions
    {
        WriteIndented = true,
        IndentCharacter = '\t',
        IndentSize = 1,
        NewLine = "\n",
    });
});
