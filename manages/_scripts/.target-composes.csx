#r "nuget: Lestaly.General, 0.102.0"
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
        new("Keycloak", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../keycloak/compose.yml"), UpArgs: ["--wait"], Volumes: ["keycloak-db-data"]),
            Endpoints: [new("Keycloak", "https://keycloak.myserver.home")]
        ),
        new("Vaultwarden", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../vaultwarden/compose.yml"), UpArgs: ["--wait"], Volumes: ["vaultwarden-app-data"]),
            Endpoints: [new("Vaultwarden", "https://vaultwarden.myserver.home")]
        ),
        new("it-tools", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../it-tools/compose.yml")),
            Endpoints: [new("it-tools", "https://it-tools.myserver.home")]
        ),
        new("planka", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../planka/compose.yml"), Volumes: ["planka-db-data", "planka-app-avatars", "planka-app-images", "planka-app-attachments"]),
            Endpoints: [new("Planka", "https://planka.myserver.home")]
        ),
        new("Dillinger", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../dillinger/compose.yml"), Volumes: ["dillinger-app-data"]),
            Endpoints: [new("Dillinger", "https://dillinger.myserver.home")]
        ),
        new("UpSnap", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../upsnap/compose.yml"), Volumes: ["upsnap-app-data"]),
            Endpoints: [new("UpSnap", "https://upsnap.myserver.home")]
        ),
        new("Forgejo", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../forgejo/compose.yml"), Volumes: ["forgejo-db-data", "forgejo-app-data"]),
            Endpoints: [new("Forgejo", "https://forgejo.myserver.home")]
        ),
        new("Forgejo-runner", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../forgejo-runner/compose.yml"), Volumes: ["forgejo-runner-data"])
        ),
        new("BookStack", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../bookstack/compose.yml"), UpArgs: ["--wait"], Volumes: ["bookstack-db-data", "bookstack-app-data"]),
            Endpoints: [new("BookStack", "https://bookstack.myserver.home")]
        ),
        new("Nextcloud", Enabled: true,
            Compose: new(ThisSource.RelativeFile("../nextcloud/compose.yml"), UpArgs: ["--wait"], Volumes: ["nextcloud-db-data", "nextcloud-app-data", "nextcloud-redis-data"]),
            Endpoints: [new("Nextcloud", "https://nextcloud.myserver.home")]
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
