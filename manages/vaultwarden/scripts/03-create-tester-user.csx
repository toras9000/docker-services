#!/usr/bin/env dotnet-script
#r "nuget: VwConnector, 1.34.3-rev.1"
#r "nuget: Lestaly.General, 0.108.0"
#r "nuget: Kokuban, 0.2.0"
#nullable enable
using Kokuban;
using Lestaly;
using Lestaly.Cx;
using VwConnector;
using VwConnector.Agent;

return await Paved.ProceedAsync(noPause: Args.RoughContains("--no-pause"), async () =>
{
    using var signal = new SignalCancellationPeriod();

    var service = new Uri("https://vaultwarden.myserver.home");
    var mail = "tester@myserver.home";
    var pass = "tester-password";

    WriteLine("Register test user");
    using var vaultwarden = new VaultwardenConnector(service);
    await vaultwarden.Account.RegisterUserNoSmtpAsync(new(mail, pass));
    WriteLine(".. Completed");
});
