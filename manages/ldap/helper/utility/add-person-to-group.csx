#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.73.0"
#r "nuget: Kokuban, 0.2.0"
#nullable enable
using System.DirectoryServices.Protocols;
using System.Net;
using Kokuban;
using Lestaly;
using static Lestaly.ConsoleWig;

var settings = new
{
    // LDAP server settings
    Server = new
    {
        // Host name or ip
        Host = "myserver.home",

        // Port number
        Port = 636,

        // Use SSL
        Ssl = true,

        // LDAP protocol version
        ProtocolVersion = 3,
    },

    Directory = new
    {
        // Bind user credential, null is anonymous
        BindCredential = new NetworkCredential("uid=configurator,ou=operators,dc=myserver,o=home", "configurator-pass"),

        // Person manage unit DN
        PersonUnitDn = "ou=persons,ou=accounts,dc=myserver,o=home",

        // Group manage unit DN
        GroupUnitDn = "ou=groups,dc=myserver,o=home",
    },

};

return await Paved.RunAsync(config: o => o.AnyPause(), action: async () =>
{
    // Bind to LDAP server
    WriteLine("Bind to LDAP server");
    var server = new LdapDirectoryIdentifier(settings.Server.Host, settings.Server.Port);
    using var ldap = new LdapConnection(server);
    ldap.SessionOptions.SecureSocketLayer = settings.Server.Ssl;
    ldap.SessionOptions.ProtocolVersion = settings.Server.ProtocolVersion;
    ldap.AuthType = AuthType.Basic;
    ldap.Credential = settings.Directory.BindCredential;
    ldap.Bind();
    WriteLine(Chalk.Green[$".. OK"]);

    // Check group unit entry.
    while (true)
    {
        WriteLine("Enter a group name.");
        Write(">");
        var group = ReadLine();
        if (group.IsWhite()) break;

        var groupDn = $"cn={group},{settings.Directory.GroupUnitDn}";
        var groupEntry = await ldap.GetEntryOrDefaultAsync(groupDn);
        if (groupEntry == null)
        {
            WriteLine(Chalk.Yellow["no group"]);
            continue;
        }

        while (true)
        {
            try
            {
                var uid = Write("uid=").ReadLine();
                if (uid.IsWhite()) break;

                var personDn = $"uid={uid},{settings.Directory.PersonUnitDn}";
                await ldap.AddAttributeAsync(groupDn, "member", [personDn]);
                WriteLine(Chalk.Green[$"Added: {uid} to {group}"]);
            }
            catch (Exception ex)
            {
                WriteLine(Chalk.Red[$"Error: {ex.Message}"]);
            }
        }
        WriteLine();


    }
});
