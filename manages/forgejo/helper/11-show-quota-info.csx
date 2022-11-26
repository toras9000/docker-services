#r "nuget: ForgejoApiClient, 9.0.0-rev.2"
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly, 0.73.0"
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

    Targets = new[]
    {
        "forgejo-admin",
    },
};

return await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
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

    WriteLine("Get quota info");
    foreach (var name in settings.Targets)
    {
        try
        {
            WriteLine($"User={name}");
            var info = await forgejo.Admin.GetUserQuotaRuleAsync(name, signal.Token);
            WriteLine($"  Used:");
            WriteLine($"    git.LFS={info.used?.size?.git?.LFS}");
            WriteLine($"    repo.public={info.used?.size?.repos?.@public}");
            WriteLine($"    repo.private={info.used?.size?.repos?.@private}");
            WriteLine($"    assets.artifacts={info.used?.size?.assets?.artifacts}");
            WriteLine($"    assets.attachments.issues={info.used?.size?.assets?.attachments?.issues}");
            WriteLine($"    assets.attachments.releases={info.used?.size?.assets?.attachments?.releases}");
            WriteLine($"    assets.packages.all={info.used?.size?.assets?.packages?.all}");
            WriteLine($"  Quota:");
            if (0 < info.groups?.Length)
            {
                foreach (var group in info.groups)
                {
                    WriteLine($"    Group={group.name}");
                    foreach (var rule in group.rules ?? [])
                    {
                        WriteLine($"      Rule={rule.name}, Limit={rule.limit}, Subjects={rule.subjects?.JoinString(";")}");
                    }
                }
            }
            else
            {
                WriteLine($"    No quota rules");
            }
        }
        catch (Exception ex)
        {
            WriteLine(Chalk.Red[$".. {ex.Message}"]);
        }
        WriteLine();
    }
});
