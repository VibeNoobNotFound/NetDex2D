using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace NetDex.Updates.Installers;

public sealed class LinuxUpdateInstaller : IUpdateInstaller
{
    private const string UpdateDir = "user://updates";
    private const string DownloadTempPath = "user://updates/netdex-linux-x64.zip.download";
    private const string DownloadFinalPath = "user://updates/netdex-linux-x64.zip";
    private const string ScriptPath = "user://updates/apply_update_linux.sh";

    public UpdatePlatform Platform => UpdatePlatform.Linux;
    public string ActionLabel => "Download & Install";

    public async Task<UpdateOperationResult> ExecuteAsync(Node host, UpdateReleaseInfo release, UpdateAssetInfo? platformAsset)
    {
        if (platformAsset == null)
        {
            return new UpdateOperationResult(false, "No Linux update asset found in release.", UpdateIssueCode.AssetMissing);
        }

        if (!ChecksumVerifier.TryParseSha256Digest(platformAsset.Digest, out var expectedDigest))
        {
            return new UpdateOperationResult(false, "Release digest missing or invalid for Linux asset.", UpdateIssueCode.DigestMissing);
        }

        var ensureDirError = EnsureUpdateDirectory();
        if (ensureDirError != null)
        {
            return ensureDirError.Value;
        }

        var downloadResult = await DownloadFileAsync(host, platformAsset.DownloadUrl, DownloadTempPath);
        if (!downloadResult.Success)
        {
            return downloadResult;
        }

        if (!ChecksumVerifier.VerifyFileSha256(DownloadTempPath, expectedDigest, out var actualDigest))
        {
            return new UpdateOperationResult(false,
                $"Downloaded file checksum mismatch. Expected {expectedDigest}, got {actualDigest}.",
                UpdateIssueCode.DigestMismatch);
        }

        if (!MoveTempDownloadToFinalPath(out var moveMessage))
        {
            return new UpdateOperationResult(false, moveMessage, UpdateIssueCode.DownloadFailed);
        }

        var executablePath = OS.GetExecutablePath();
        if (string.IsNullOrWhiteSpace(executablePath))
        {
            return new UpdateOperationResult(false, "Unable to resolve current executable path.", UpdateIssueCode.InstallFailed);
        }

        var appDir = Path.GetDirectoryName(executablePath);
        var exeName = Path.GetFileName(executablePath);
        if (string.IsNullOrWhiteSpace(appDir) || string.IsNullOrWhiteSpace(exeName))
        {
            return new UpdateOperationResult(false, "Unable to resolve app directory or executable name.", UpdateIssueCode.InstallFailed);
        }

        if (!WriteUpdateScript(appDir, exeName, out var scriptMessage))
        {
            return new UpdateOperationResult(false, scriptMessage, UpdateIssueCode.InstallFailed);
        }

        var scriptGlobalPath = ProjectSettings.GlobalizePath(ScriptPath);
        var pid = OS.CreateProcess("/bin/sh", new[] { scriptGlobalPath });
        if (pid < 0)
        {
            return new UpdateOperationResult(false, "Failed to launch Linux updater helper process.", UpdateIssueCode.InstallFailed);
        }

        return new UpdateOperationResult(true, "Update prepared. Relaunching to apply update.");
    }

    private static UpdateOperationResult? EnsureUpdateDirectory()
    {
        try
        {
            var globalPath = ProjectSettings.GlobalizePath(UpdateDir);
            Directory.CreateDirectory(globalPath);
            return null;
        }
        catch (Exception ex)
        {
            return new UpdateOperationResult(false, $"Failed to prepare update directory: {ex.Message}", UpdateIssueCode.DownloadFailed);
        }
    }

    private static async Task<UpdateOperationResult> DownloadFileAsync(Node host, string url, string targetGodotPath)
    {
        var request = new HttpRequest();
        host.AddChild(request);
        request.DownloadFile = ProjectSettings.GlobalizePath(targetGodotPath);

        var headers = new[]
        {
            "User-Agent: NetDex-Updater",
            "Accept: application/octet-stream"
        };

        var requestError = request.Request(url, headers, HttpClient.Method.Get);
        if (requestError != Error.Ok)
        {
            request.QueueFree();
            return new UpdateOperationResult(false, $"Download start failed: {requestError}", UpdateIssueCode.DownloadFailed);
        }

        var completed = await host.ToSignal(request, HttpRequest.SignalName.RequestCompleted);
        request.QueueFree();

        var result = (HttpRequest.Result)completed[0].AsInt32();
        var responseCode = completed[1].AsInt32();

        if (result != HttpRequest.Result.Success)
        {
            return new UpdateOperationResult(false, $"Download failed: {result}", UpdateIssueCode.DownloadFailed);
        }

        if (responseCode < 200 || responseCode >= 300)
        {
            return new UpdateOperationResult(false, $"Download returned HTTP {responseCode}.", UpdateIssueCode.DownloadFailed);
        }

        return new UpdateOperationResult(true, "Update package downloaded.");
    }

    private static bool MoveTempDownloadToFinalPath(out string message)
    {
        var tempGlobal = ProjectSettings.GlobalizePath(DownloadTempPath);
        var finalGlobal = ProjectSettings.GlobalizePath(DownloadFinalPath);

        try
        {
            if (File.Exists(finalGlobal))
            {
                File.Delete(finalGlobal);
            }

            File.Move(tempGlobal, finalGlobal);
            message = "Update package prepared.";
            return true;
        }
        catch (Exception ex)
        {
            message = $"Failed to finalize downloaded package: {ex.Message}";
            return false;
        }
    }

    private static bool WriteUpdateScript(string appDir, string executableName, out string message)
    {
        var scriptGlobalPath = ProjectSettings.GlobalizePath(ScriptPath);
        var zipPath = ProjectSettings.GlobalizePath(DownloadFinalPath);

        var script = BuildScript(appDir, executableName, zipPath);

        try
        {
            File.WriteAllText(scriptGlobalPath, script, Encoding.UTF8);
            message = "Linux updater helper script ready.";
            return true;
        }
        catch (Exception ex)
        {
            message = $"Failed to write update script: {ex.Message}";
            return false;
        }
    }

    private static string BuildScript(string appDir, string executableName, string zipPath)
    {
        var escapedAppDir = ShellEscape(appDir);
        var escapedExeName = ShellEscape(executableName);
        var escapedZip = ShellEscape(zipPath);

        return "#!/bin/bash\n" +
               "set -euo pipefail\n" +
               $"APP_DIR='{escapedAppDir}'\n" +
               $"EXE_NAME='{escapedExeName}'\n" +
               $"ZIP_PATH='{escapedZip}'\n" +
               "TEMP_DIR=\"$(mktemp -d)\"\n" +
               "cleanup() { rm -rf \"$TEMP_DIR\"; }\n" +
               "trap cleanup EXIT\n" +
               "sleep 1\n" +
               "unzip -o \"$ZIP_PATH\" -d \"$TEMP_DIR\"\n" +
               "SRC_DIR=\"$TEMP_DIR\"\n" +
               "FIRST_DIR=\"$(find \"$TEMP_DIR\" -mindepth 1 -maxdepth 1 -type d | head -n 1 || true)\"\n" +
               "if [ -n \"$FIRST_DIR\" ]; then\n" +
               "  SRC_DIR=\"$FIRST_DIR\"\n" +
               "fi\n" +
               "cp -a \"$SRC_DIR\"/. \"$APP_DIR\"/\n" +
               "chmod +x \"$APP_DIR/$EXE_NAME\" || true\n" +
               "(\"$APP_DIR/$EXE_NAME\" >/dev/null 2>&1 &)\n";
    }

    private static string ShellEscape(string input)
    {
        return input.Replace("'", "'\"'\"'");
    }
}
