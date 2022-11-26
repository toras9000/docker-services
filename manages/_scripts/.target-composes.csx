#r "nuget: Lestaly, 0.69.0"
#nullable enable
using Lestaly;

record NetworkInfo(string Name, bool Enabled, string Subnet);

record ComposeInfo(FileInfo File, string[]? UpArgs = default, string[]? Volumes = default);
record ServiceEndpoint(string Name, string Url);
record ServiceInfo(string Name, bool Enabled, ComposeInfo Compose, ServiceEndpoint[]? Endpoints = default);

static class ManageServices
{
    private static IReadOnlyList<NetworkInfo> networks { get; } = new NetworkInfo[]
    {
        new("composes-frontend", Enabled: true, "172.31.0.0/16"),
    };

    private static IReadOnlyList<ServiceInfo> containers { get; } = new ServiceInfo[]
    {
        new("dnsmasq", Enabled: false,
            Compose: new(ThisSource.RelativeFile("../dnsmasq/compose.yml"), UpArgs: ["--wait"])
        ),
        new("OpenLDAP", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../ldap/compose.yml"), UpArgs: ["--wait"], Volumes: ["openldap-app-data"])
        ),
        new("it-tools", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../it-tools/compose.yml")),
            Endpoints: [new("it-tools", "https://it-tools.myserver.home")]
        ),
        new("Traggo", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../traggo/compose.yml"), Volumes: ["traggo-app-data"]),
            Endpoints: [new("Traggo", "https://traggo.myserver.home")]
        ),
        new("Dillinger", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../dillinger/compose.yml"), Volumes: ["dillinger-app-data"]),
            Endpoints: [new("Dillinger", "https://dillinger.myserver.home")]
        ),
        new("UpSnap", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../upsnap/compose.yml"), Volumes: ["upsnap-app-data"]),
            Endpoints: [new("UpSnap", "https://upsnap.myserver.home")]
        ),
        new("snipe-it", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../snipe-it/compose.yml"), Volumes: ["snipe-it-db-data", "snipe-it-app-data"]),
            Endpoints: [new("snipe-it", "https://snipe-it.myserver.home")]
        ),
        new("Forgejo", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../forgejo/compose.yml"), Volumes: ["forgejo-db-data", "forgejo-app-data"]),
            Endpoints: [new("Forgejo", "https://forgejo.myserver.home")]
        ),
        new("Forgejo-runner", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../forgejo-runner/compose.yml"), Volumes: ["forgejo-runner-data"])
        ),
        new("Nextcloud", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../nextcloud/compose.yml"), UpArgs: ["--wait"], Volumes: ["nextcloud-db-data", "nextcloud-app-data"]),
            Endpoints: [new("Nextcloud", "https://nextcloud.myserver.home")]
        ),
        new("BookStack", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../bookstack/compose.yml"), UpArgs: ["--wait"], Volumes: ["bookstack-db-data", "bookstack-app-data"]),
            Endpoints: [new("BookStack", "https://bookstack.myserver.home")]
        ),
        new("Keycloak", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../keycloak/compose.yml"), UpArgs: ["--wait"], Volumes: ["keycloak-db-data"]),
            Endpoints: [new("Keycloak", "https://keycloak.myserver.home")]
        ),
        new("ReverseProxy", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../proxy/compose.yml"), Volumes: ["proxy-logs", "proxy-reports"]),
            Endpoints: [new("GoAccess", "https://goaccess.myserver.home")]
        ),
    };

    public static IEnumerable<NetworkInfo> GetNetworks() => networks
        .Where(i => i.Enabled)
        ;

    public static IEnumerable<ServiceInfo> GetContainers() => containers
        .Where(i => i.Enabled)
        ;

    public static IEnumerable<string> GetDockerVolumeNames() => containers
        .Where(i => i.Enabled)
        .SelectMany(i => i.Compose.Volumes ?? [])
        ;

}
