#r "nuget: Lestaly, 0.65.0"
#nullable enable
using Lestaly;

record ComposeInfo(string Name, FileInfo File, string[]? UpArgs = default);

var manageComposes = new ComposeInfo[]
{
//    new("dnsmasq", ThisSource.RelativeFile("../dnsmasq/compose.yml"), UpArgs: ["--wait"]),
    new("OpenLDAP",      ThisSource.RelativeFile("../ldap/compose.yml"), UpArgs: ["--wait"]),

    new("ImageRegistry", ThisSource.RelativeFile("../registry/compose.yml")),
    new("BaGetter",      ThisSource.RelativeFile("../bagetter/compose.yml")),
    new("Forgejo",       ThisSource.RelativeFile("../forgejo/compose.yml")),
    new("Kallithea",     ThisSource.RelativeFile("../kallithea/compose.yml")),
    new("BookStack",     ThisSource.RelativeFile("../bookstack/compose.yml"), UpArgs: ["--wait"]),
    new("Keycloak",      ThisSource.RelativeFile("../keycloak/compose.yml")),

    new("nginx proxy",   ThisSource.RelativeFile("../proxy/compose.yml")),
};

record ServiceEndpoint(string Name, string Url);

var serviceEps = new ServiceEndpoint[]
{
    new("BaGetter",  "https://bagetter.myserver.home"),
    new("Forgejo",   "https://forgejo.myserver.home"),
    new("Kallithea", "https://kallithea.myserver.home"),
    new("BookStack", "https://bookstack.myserver.home"),
    new("Keycloak",  "https://keycloak.myserver.home"),
    new("GoAccess",  "https://goaccess.myserver.home"),
};

