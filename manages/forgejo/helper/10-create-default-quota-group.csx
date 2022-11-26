#r "nuget: ForgejoApiClient, 13.0.0-rev.1"
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly.General, 0.105.0"
#nullable enable
using System.Threading;
using ForgejoApiClient;
using ForgejoApiClient.Api;
using Kokuban;
using Lestaly;
using Lestaly.Cx;

var settings = new
{
    ServiceURL = new Uri("http://forgejo.myserver.home"),

    TokenFile = ThisSource.RelativeFile("./.auth-token"),

    Quota = new
    {
        GroupName = "default-quota",
        Rules = new CreateQuotaRuleOptions[]
        {
            new(name: "default-git-limit",          limit:  8 * 1024 * 1024 * 1024L, subjects: ["size:git:all"]),
            new(name: "default-repos-limit",        limit:  8 * 1024 * 1024 * 1024L, subjects: ["size:repos:all"]),
            new(name: "default-artifacts-limit",    limit:  2 * 1024 * 1024 * 1024L, subjects: ["size:assets:artifacts"]),
            new(name: "default-attachments-limit",  limit:  2 * 1024 * 1024 * 1024L, subjects: ["size:assets:attachments:all"]),
            new(name: "default-packages-limit",     limit: 16 * 1024 * 1024 * 1024L, subjects: ["size:assets:packages:all"]),
        },
    },
};

return await Paved.RunAsync(async () =>
{
    using var signal = new SignalCancellationPeriod();
    using var outenc = ConsoleWig.OutputEncodingPeriod(Encoding.UTF8);

    WriteLine("Service URL");
    WriteLine($"  {Poster.Link[settings.ServiceURL.AbsoluteUri]}");
    WriteLine();

    WriteLine("Load token");
    var scrambler = settings.TokenFile.CreateScrambler(context: settings.TokenFile.FullName);
    var apiToken = (await scrambler.DescrambleTextAsync(signal.Token))?.Trim();
    if (apiToken.IsWhite())
    {
        WriteLine($".. Not preserved.");
        WriteLine($"Enter API token");
        Write(">"); apiToken = ReadLine().CancelIfWhite();
        await scrambler.ScrambleTextAsync(apiToken, cancelToken: signal.Token);
    }

    WriteLine("Generate API client");
    using var forgejo = new ForgejoClient(settings.ServiceURL, apiToken);

    WriteLine("Check group exists");
    try
    {
        await forgejo.Admin.GetQuotaGroupAsync(settings.Quota.GroupName, signal.Token);
        WriteLine(".. Already exists");
        return;
    }
    catch { }

    WriteLine("Create quota group");
    var options = new CreateQuotaGroupOptions(name: settings.Quota.GroupName, rules: settings.Quota.Rules);
    await forgejo.Admin.CreateQuotaGroupAsync(options, signal.Token);

    WriteLine(Chalk.Green["Successful"]);
});
