#r "nuget: AngleSharp, 1.1.2"
#r "nuget: Lestaly, 0.68.0"
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

    Token = new
    {
        Name = "admin-script-token",
        User = "forgejo-admin",
        Scopes = new[]
        {
            new { Name = "activitypub",  Access = ScopeAccess.read, },
            new { Name = "admin",        Access = ScopeAccess.read, },
            new { Name = "issue",        Access = ScopeAccess.read, },
            new { Name = "misc",         Access = ScopeAccess.read, },
            new { Name = "notification", Access = ScopeAccess.read, },
            new { Name = "organization", Access = ScopeAccess.read, },
            new { Name = "package",      Access = ScopeAccess.read, },
            new { Name = "repository",   Access = ScopeAccess.read, },
            new { Name = "user",         Access = ScopeAccess.read, },
        },
    },

    TokenFile = ThisSource.RelativeFile("./.auth-token"),
};

enum ScopeAccess
{
    read,
    write,
}

await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    WriteLine("Generate token");
    var apiToken = await "docker".args(
        "compose", "--file", settings.ComposeFile.FullName, "exec", "-u", "1000", "app",
        "forgejo", "admin", "user", "generate-access-token",
        "--token-name", settings.Token.Name,
        "--username", settings.Token.User,
        "--scopes", settings.Token.Scopes.Select(s => $"{s.Access}:{s.Name}").JoinString(","),
        "--raw"
    ).echo().result().success().output();

    WriteLine("Save token");
    var scrambler = settings.TokenFile.CreateScrambler(context: settings.Token.Name);
    await scrambler.ScrambleTextAsync(apiToken);

    WriteLine("API token generation completed.");
});
