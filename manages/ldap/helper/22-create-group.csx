#!/usr/bin/env dotnet-script
#r "nuget: Lestaly, 0.100.0"
#r "nuget: Kokuban, 0.2.0"
#nullable enable
using System.DirectoryServices.Protocols;
using System.Net;
using Kokuban;
using Lestaly;

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

        // Group manage unit DN
        GroupUnitDn = "ou=groups,dc=myserver,o=home",

        // Default user in group
        DefaultMemberDn = "uid=authenticator,ou=operators,dc=myserver,o=home",
    },

};

return await Paved.ProceedAsync(async () =>
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
        WriteLine("Enter the name of the group to be created.");
        Write(">");
        var name = ReadLine();
        if (name.IsWhite()) break;

        try
        {
            var groupDn = $"cn={name},{settings.Directory.GroupUnitDn}";
            await ldap.CreateEntryAsync(groupDn,
            [
                new("objectClass", "groupOfNames"),
                new("cn", name),
                new("member", settings.Directory.DefaultMemberDn),
            ]);
            WriteLine(Chalk.Green[$"Created: {groupDn}"]);
        }
        catch (Exception ex)
        {
            WriteLine(Chalk.Red[$"Error: {ex.Message}"]);
        }
        WriteLine();
    }
});
