#r "nuget: AngleSharp, 1.1.2"
#r "nuget: Lestaly, 0.69.0"
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

    Auth = new
    {
        Name = "myserver-ldap",
        Host = "ldap.myserver.home",
        Port = 636,
        Security = SecurityProtocol.LDAPS,
        SearchBaseDn = "ou=persons,ou=accounts,dc=myserver,o=home",
        UserFilter = "(&(objectClass=inetOrgPerson)(|(uid=%[1]s)(mail=%[1]s)))",
        AttrUser = "uid",
        AttrGivenname = "givenName",
        AttrSurname = "sn",
        AttrMailaddr = "mail",

    },
};

enum SecurityProtocol
{
    Unencrypted,
    LDAPS,
    StartTLS,
}

await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    await "docker".args(
        "compose", "--file", settings.ComposeFile.FullName, "exec", "-u", "1000", "app",
        "forgejo", "admin", "auth", "add-ldap",
        "--active",
        "--name", settings.Auth.Name,
        "--host", settings.Auth.Host,
        "--port", $"{settings.Auth.Port}",
        "--security-protocol", $"{settings.Auth.Security}",
        "--user-search-base", settings.Auth.SearchBaseDn,
        "--user-filter", settings.Auth.UserFilter,
        "--username-attribute", settings.Auth.AttrUser,
        "--firstname-attribute", settings.Auth.AttrGivenname,
        "--surname-attribute", settings.Auth.AttrSurname,
        "--email-attribute", settings.Auth.AttrMailaddr,
        "--synchronize-users"
    ).echo().result().success().output();

    WriteLine("LDAP auth addtion completed.");
});
