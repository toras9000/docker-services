#!/usr/bin/env dotnet-script
#r "nuget: Lestaly.General, 0.102.0"
#r "nuget: Lestaly.Ldap, 0.100.0"
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

        // Bind user credential, null is anonymous
        BindCredential = new NetworkCredential("cn=config-admin,cn=config", "config-admin-pass"),

        // Configuration Base DN
        ConfigDn = "olcDatabase={2}mdb,cn=config",

        // Access definitions to be added
        AccessDefineFile = ThisSource.RelativeFile("02-config-access-data.txt"),
    },

};

return await Paved.ProceedAsync(async () =>
{
    // Read the access definition to be added
    var accessDefines = settings.Server.AccessDefineFile.EnumerateTextBlocks().ToArray();

    // Bind to LDAP server
    WriteLine("Bind to LDAP server");
    var server = new LdapDirectoryIdentifier(settings.Server.Host, settings.Server.Port);
    using var ldap = new LdapConnection(server);
    ldap.SessionOptions.SecureSocketLayer = settings.Server.Ssl;
    ldap.SessionOptions.ProtocolVersion = settings.Server.ProtocolVersion;
    ldap.AuthType = AuthType.Basic;
    ldap.Credential = settings.Server.BindCredential;
    ldap.Bind();

    // Create a search request.
    WriteLine("Request a search");
    var searchReq = new SearchRequest();
    searchReq.DistinguishedName = settings.Server.ConfigDn;
    searchReq.Scope = SearchScope.Base;

    // Request a search.
    var searchRsp = await ldap.SendRequestAsync(searchReq);
    if (searchRsp.ResultCode != 0) throw new PavedMessageException($"failed to search: {searchRsp.ErrorMessage}");
    var searchResult = searchRsp as SearchResponse ?? throw new PavedMessageException("unexpected result");

    // Read the existing access definition.
    var configEntry = searchResult.Entries[0];
    var accessExists = configEntry.EnumerateAttributeValues("olcAccess").ToArray();

    // Remove all existing access.
    if (0 < accessExists.Length)
    {
        WriteLine("Delete all access");
        var deleteRsp = await ldap.DeleteAttributeAsync(settings.Server.ConfigDn, "olcAccess", accessDefines);
        if (deleteRsp.ResultCode != 0) throw new PavedMessageException($"failed to modify: {deleteRsp.ErrorMessage}");
    }

    // Add defined access.
    WriteLine("Request to add access.");
    {
        var modifyRsp = await ldap.AddAttributeAsync(settings.Server.ConfigDn, "olcAccess", accessDefines);
        if (modifyRsp.ResultCode != 0) throw new PavedMessageException($"failed to modify: {modifyRsp.ErrorMessage}");
    }

    WriteLine("Completed.");
});

public static IEnumerable<string> EnumerateTextBlocks(this FileInfo self)
{
    var buffer = new StringBuilder();
    foreach (var line in self.ReadLines())
    {
        if (line.IsWhite())
        {
            if (0 < buffer.Length)
            {
                yield return buffer.ToString();
                buffer.Clear();
            }
            continue;
        }

        buffer.AppendLine(line);
    }

    if (0 < buffer.Length)
    {
        yield return buffer.ToString();
    }
}
