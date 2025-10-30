#r "nuget: AngleSharp, 1.3.0"
#r "nuget: Lestaly.General, 0.108.0"
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
            new { Name = "activitypub",  Access = ScopeAccess.write, },
            new { Name = "admin",        Access = ScopeAccess.write, },
            new { Name = "issue",        Access = ScopeAccess.write, },
            new { Name = "misc",         Access = ScopeAccess.write, },
            new { Name = "notification", Access = ScopeAccess.write, },
            new { Name = "organization", Access = ScopeAccess.write, },
            new { Name = "package",      Access = ScopeAccess.write, },
            new { Name = "repository",   Access = ScopeAccess.write, },
            new { Name = "user",         Access = ScopeAccess.write, },
        },
    },

    TokenFile = ThisSource.RelativeFile("./.auth-token"),
};

enum ScopeAccess
{
    read,
    write,
}

return await Paved.ProceedAsync(async () =>
{
    WriteLine("Generate token");
    var apiToken = await "docker".args(
        "compose", "--file", settings.ComposeFile, "exec", "-u", "1000", "app",
        "forgejo", "admin", "user", "generate-access-token",
        "--token-name", settings.Token.Name,
        "--username", settings.Token.User,
        "--scopes", settings.Token.Scopes.Select(s => $"{s.Access}:{s.Name}").JoinString(","),
        "--raw"
    ).silent().result().success().output(trim: true);

    WriteLine("Save token");
    var scrambler = settings.TokenFile.CreateScrambler(context: settings.TokenFile.FullName);
    await scrambler.ScrambleTextAsync(apiToken);

    WriteLine("API token generation completed.");
});
