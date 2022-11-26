#r "nuget: Lestaly, 0.58.0"
#nullable enable
using Lestaly;

record ComposeInfo(string Name, FileInfo File, string[]? UpArgs = default, string? Service = default);

var manageComposes = new ComposeInfo[]
{
//    new("dnsmasq", ThisSource.RelativeFile("../dnsmasq/compose.yml"), UpArgs: ["--wait"]),
    new("OpenLDAP",      ThisSource.RelativeFile("../ldap/compose.yml"), UpArgs: ["--wait"]),

    new("ImageRegistry", ThisSource.RelativeFile("../registry/compose.yml")),
    new("BaGetter",      ThisSource.RelativeFile("../bagetter/compose.yml"), Service: "https://bagetter.myserver.home"),
    new("Forgejo",       ThisSource.RelativeFile("../forgejo/compose.yml"), Service: "https://forgejo.myserver.home"),
    new("BookStack",     ThisSource.RelativeFile("../bookstack/compose.yml"), UpArgs: ["--wait"], Service: "https://bookstack.myserver.home"),
    new("Keycloak",      ThisSource.RelativeFile("../keycloak/compose.yml"), Service: "https://keycloak.myserver.home"),

    new("nginx proxy",   ThisSource.RelativeFile("../proxy/compose.yml")),
};
