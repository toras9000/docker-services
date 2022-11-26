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
        WriteLine("Enter the uid of the person to be created.");
        Write(">");
        var uid = ReadLine();
        if (uid.IsWhite()) break;

        try
        {
            var cn = Write("cn=").ReadLine();
            var sn = Write("sn=").ReadLine();
            var mail = Write("mail=").ReadLine();
            var passwd = Write("password(no echo)=").ReadLineIntercepted();
            var hash = LdapExtensions.MakePasswordHash.SSHA256(passwd);
            WriteLine();

            var personDn = $"uid={uid},{settings.Directory.PersonUnitDn}";
            await ldap.CreateEntryAsync(personDn,
            [
                new("objectClass", ["inetOrgPerson", "extensibleObject"]),
                new("cn", cn),
                new("sn", sn),
                new("mail", mail),
                new("userPassword", hash),
            ]);
            WriteLine(Chalk.Green[$"Created: {personDn}"]);
        }
        catch (Exception ex)
        {
            WriteLine(Chalk.Red[$"Error: {ex.Message}"]);
        }
        WriteLine();
    }
});
