using System.Diagnostics;
using System.Reflection;
using Aco228.Common.LocalStorage;

namespace Aco228.Common.Helpers;

public enum SelfUpdateState
{
    Idle,
    Building,
    Succeeded,
    Failed,
}

/// <summary>
/// Builds the app into the deploy slot it is not running from ({github}\deploy\{app}\a or \b) while it keeps running.
/// On success deploy\{app}\current.txt points to the new slot, so the run bat starts the new version after the app exits.
/// The app is the entry assembly (e.g. CK.WebPortal), built from ./src/{app}/{app}.csproj by build_next.bat.
/// Started by TaskManagerService.RequestShutdown(rebuild: true), which exits only after the build succeeded.
/// </summary>
public static class SelfUpdateService
{
    public static string App => Assembly.GetEntryAssembly()?.GetName().Name ?? "";
    public static string Csproj => $"./src/{App}/{App}.csproj";

    private static int _running;

    public static event Action? OnStateChanged;
    public static SelfUpdateState State { get; private set; } = SelfUpdateState.Idle;
    public static bool IsBuilding => State == SelfUpdateState.Building;
    public static DateTime? LastBuildFailedUtc { get; private set; }

    /// <summary>Starts the build in background. Does nothing when a build is already running.</summary>
    public static void StartRebuild()
    {
        if (Interlocked.Exchange(ref _running, 1) == 1)
        {
            Console.WriteLine("REBUILD: already running -- ignored");
            return;
        }

        LastBuildFailedUtc = null;
        SetState(SelfUpdateState.Building);
        Task.Run(Rebuild);
    }

    private static async Task Rebuild()
    {
        var app = App;
        var succeeded = false;
        try
        {
            var baseFolder = StorageManager.Instance.GetBaseGithubFolder();
            if (baseFolder == null)
            {
                Console.WriteLine("REBUILD FAILED: base github folder not found");
                return;
            }

            var deployFolder = Directory.CreateDirectory(Path.Combine(baseFolder.FullName, "deploy", app)).FullName;
            var currentSlot = new DirectoryInfo(AppContext.BaseDirectory).Name;
            var nextSlot = currentSlot.Equals("a", StringComparison.OrdinalIgnoreCase) ? "b" : "a";

            Console.WriteLine($"REBUILD: building {app} into slot '{nextSlot}' (running from '{currentSlot}')");
            var exitCode = await RunBuild(baseFolder.FullName, deployFolder, app, Csproj, nextSlot);
            if (exitCode != 0)
            {
                Console.WriteLine($"REBUILD FAILED (exit {exitCode})");
                return;
            }

            var targetPath = Path.Combine(deployFolder, "current.txt");
            var tempPath = Path.Combine(deployFolder, $".current.txt.{Guid.NewGuid():N}.tmp");
            await File.WriteAllTextAsync(tempPath, nextSlot);
            File.Move(tempPath, targetPath, overwrite: true);

            Console.WriteLine($"REBUILD: ok, next start runs slot '{nextSlot}'");
            succeeded = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("REBUILD FAILED: " + ex);
        }
        finally
        {
            if (!succeeded)
                LastBuildFailedUtc = DateTime.UtcNow;

            Interlocked.Exchange(ref _running, 0);
            SetState(succeeded ? SelfUpdateState.Succeeded : SelfUpdateState.Failed);
        }
    }

    private static async Task<int> RunBuild(string baseFolder, string deployFolder, string app, string csproj, string slot)
    {
        // Run a copy of build_next.bat: the pull inside it may rewrite the original while cmd is still reading it
        var script = Path.Combine(deployFolder, "build_next.run.bat");
        File.Copy(Path.Combine(baseFolder, "build_next.bat"), script, overwrite: true);

        // No output redirect: build servers can inherit the pipes and keep WaitForExit hanging. Output goes to the app console.
        var info = new ProcessStartInfo("cmd.exe", $"/c \"\"{script}\" {app} {csproj} {slot} \"{baseFolder}\"\"")
        {
            WorkingDirectory = baseFolder,
            UseShellExecute = false,
        };

        using var process = Process.Start(info)!;
        await process.WaitForExitAsync();
        return process.ExitCode;
    }

    private static void SetState(SelfUpdateState state)
    {
        State = state;
        OnStateChanged?.Invoke();
    }
}
