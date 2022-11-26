#r "nuget: KallitheaApiClient, 0.7.0-lib.23.private.1"
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly, 0.68.0"
#nullable enable
using KallitheaApiClient;
using KallitheaApiClient.Utils;
using Kokuban;
using Lestaly;

var settings = new
{
    // Service URL for Kallithea.
    ServiceUrl = new Uri("https://kallithea.myserver.home"),

    // API key save file.
    ApiKeyFile = ThisSource.RelativeFile(".api-key"),
};

// main processing
await Paved.RunAsync(config: o => o.AnyPause(), action: async () =>
{
    // Prepare console
    using var outenc = ConsoleWig.OutputEncodingPeriod(Encoding.UTF8);
    using var signal = ConsoleWig.CreateCancelKeyHandlePeriod();

    // Show access address
    Console.WriteLine($"Service URL : {settings.ServiceUrl}");

    // Attempt to recover saved API key information.
    var apiEp = new Uri(settings.ServiceUrl, "_admin/api");
    var apiKeyScrambler = settings.ApiKeyFile.CreateScrambler();
    var apiKey = await apiKeyScrambler.DescrambleTextAsync(signal.Token);
    if (apiKey == null)
    {
        WriteLine("Enter Kallithea API key"); Write(">");
        apiKey = ReadLine();
        if (apiKey.IsWhite()) throw new PavedMessageException("no key", PavedMessageKind.Cancelled);
    }

    // Create client
    using var client = new SimpleKallitheaClient(apiEp, apiKey);

    // If API access is successful, scramble and save the API key.
    var me = await client.GetUserAsync(new(default!), signal.Token);
    await apiKeyScrambler.ScrambleTextAsync(apiKey, cancelToken: signal.Token);

    // Show user info
    WriteLine($"API user: {me.user.username}");
    WriteLine();

    // Enter repo name
    WriteLine("Enter create repository name(path)"); Write(">");
    var repoName = ReadLine();
    if (repoName.IsWhite()) throw new PavedMessageException("no name", PavedMessageKind.Cancelled);

    // Create repository
    ConsoleWig.WriteLine($"Create a repository");
    var repoInfo = await client.CreateRepoAsync(new(repoName, repo_type: RepoType.git, enable_downloads: true, enable_statistics: false));

    // Create extra field
    ConsoleWig.WriteLine($"Create extra field");
    var fieldInfo = await client.CreateRepoExtraFieldAsync(new(repoName, "field-key", "field-label", "field-desc", "field-value"));
    ConsoleWig.WriteLine($"  Result: added={fieldInfo.extra_field.key}, msg={fieldInfo.msg}");
});
