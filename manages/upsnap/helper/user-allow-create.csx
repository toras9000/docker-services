#r "nuget: Lestaly.General, 0.105.0"
#load ".pocketbase-client.csx"
#nullable enable
using System.Threading;
using Lestaly;

var settings = new
{
    Url = new Uri("http://upsnap.myserver.home/"),

    Admin = new
    {
        ID = "admin@example.com",
        Pass = "admin-secret-pass",
    },
};

public class PermissionItem : RecordsItem
{
    public string user { get; set; } = default!;
    public bool create { get; set; }
}

return await Paved.ProceedAsync(async () =>
{
    using var client = new PocketBaseClient(settings.Url);

    var admin = await client.Admins.AuthWithPsswordAsync(new(settings.Admin.ID, settings.Admin.Pass));
    client.SetAccessToken(admin.token);

    var users = await client.Records.ListAsync<UserItem>("users", new(perPage: 100));
    var permissions = await client.Records.ListAsync<PermissionItem>("permissions", new(perPage: 100));

    foreach (var user in users.items)
    {
        var perm = permissions.items.FirstOrDefault(p => p.user == user.id);
        if (perm == null)
        {
            WriteLine($"User={user.username}, No perm");
            await client.Records.CreateAsync<PermissionItem>("permissions", new { user = user.id, create = true, });
        }
        else
        {
            if (perm.create)
            {
                WriteLine($"User={user.username}, Already allowed");
            }
            else
            {
                WriteLine($"User={user.username}, Not allowed");
                await client.Records.UpdateAsync<PermissionItem>("permissions", perm.id, new { user = user.id, create = true, });
            }
        }
    }

});
