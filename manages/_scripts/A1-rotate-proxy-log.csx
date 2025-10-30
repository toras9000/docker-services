#!/usr/bin/env dotnet-script
#r "nuget: Kokuban, 0.2.0"
#r "nuget: Lestaly.General, 0.108.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

try
{
    var volumeName = "proxy-logs";
    var logName = "access.log";
    var keepDays = TimeSpan.FromDays(2 * 7);
    var workDir = ThisSource.RelativeDirectory("../../work");

    WriteLine("Check volume usage");
    var uses = await "docker".args("ps", "-a", "--filter", $"volume={volumeName}", "--format", "{{.ID}}").silent().result().success().output();
    if (uses.IsNotWhite()) throw new Exception($"volume '{volumeName}' is in use");

    WriteLine("Copy the log file to work dir");
    await "docker".args(
        "run", "--rm", "-i",
        "--mount", $"type=bind,source={workDir.FullName},target=/work/host",
        "--mount", $"type=volume,source=proxy-logs,target=/work/volume",
        "alpine",
        "cp", $"/work/volume/{logName}", $"/work/host/{logName}"
    ).silent().result().success();

    var nowTime = DateTime.Now;
    var keepBase = nowTime.Date.Subtract(keepDays.Duration()).ToUniversalTime();
    var logFile = workDir.RelativeFile(logName);
    var refreshFile = workDir.RelativeFile($"{logFile.BaseName()}-refresh{logFile.Extension}");
    var backupFile = workDir.RelativeFile($"{logFile.BaseName()}-{nowTime:yyyyMMdd-HHmmss}{logFile.Extension}");
    var backupExists = false;

    WriteLine("Log file maintenance");
    using (var logReader = logFile.CreateTextReader())
    {
        using var refreshWriter = refreshFile.CreateTextWriter();
        using var backupWriter = backupFile.CreateTextWriter();

        // Move old logs to backup.
        while (true)
        {
            var line = logReader.ReadLine();
            if (line == null) break;

            var time = line.SkipToken('[').TakeToken(']').TryParseDateTimeExact("dd/MMM/yyyy:HH:mm:ss K")?.ToUniversalTime();
            if (time.HasValue && keepBase <= time.Value) break;

            backupWriter.WriteLine(line);
            backupExists = true;
        }

        // The rest is moved to a new log file.
        while (true)
        {
            var line = logReader.ReadLine();
            if (line == null) break;

            refreshWriter.WriteLine(line);
        }
    }

    WriteLine("Refresh log file");
    logFile.Delete();
    refreshFile.MoveTo(logFile.FullName);
    if (!backupExists) backupFile.Delete();

    WriteLine("Move the log file to volume");
    await "docker".args(
        "run", "--rm", "-i",
        "--mount", $"type=bind,source={workDir.FullName},target=/work/host",
        "--mount", $"type=volume,source=proxy-logs,target=/work/volume",
        "alpine",
        "mv", $"/work/host/{logName}", $"/work/volume/{logName}"
    ).silent().result().success();

}
catch (Exception ex)
{
    WriteLine($"Error: {ex.Message}");
}

