#r "nuget: Lestaly, 0.83.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

return await Paved.ProceedAsync(async () =>
{
    var composeFile = ThisSource.RelativeFile("../compose.yml");
    await "docker".args(
        "compose", "--file", composeFile, "exec", "ldap",
        "ldapadd", "-Q", "-Y", "EXTERNAL", "-H", "ldapi:///", "-f", "/ldifs/memberof.ldif"
    );
});
