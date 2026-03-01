using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace NetDex.Updates.Installers;

public sealed class WindowsUpdateInstaller : IUpdateInstaller
{
    private const string UpdateDir = "user://updates";
    private const string DownloadTempPath = "user://updates/netdex-windows-x64.zip.download";
    private const string DownloadFinalPath = "user://updates/netdex-windows-x64.zip";
    private const string ScriptPath = "user://updates/apply_update_windows.cmd";

    public UpdatePlatform Platform => UpdatePlatform.Windows;
    public string ActionLabel => "Download & Install";

    public async Task<UpdateOperationResult> ExecuteAsync(Node host, UpdateReleaseInfo release, UpdateAssetInfo? platformAsset)
    {
        if (platformAsset == null)
        {
            return new UpdateOperationResult(false, "No Windows update asset found in release.", UpdateIssueCode.AssetMissing);
        }

        if (!ChecksumVerifier.TryParseSha256Digest(platformAsset.Digest, out var expectedDigest))
        {
            return new UpdateOperationResult(false, "Release digest missing or invalid for Windows asset.", UpdateIssueCode.DigestMissing);
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
        var pid = OS.CreateProcess("cmd.exe", new[] { "/C", scriptGlobalPath });
        if (pid < 0)
        {
            return new UpdateOperationResult(false, "Failed to launch Windows updater helper process.", UpdateIssueCode.InstallFailed);
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

        var script = BuildScript(zipPath, appDir, executableName);

        try
        {
            File.WriteAllText(scriptGlobalPath, script, Encoding.ASCII);
            message = "Windows updater helper script ready.";
            return true;
        }
        catch (Exception ex)
        {
            message = $"Failed to write update script: {ex.Message}";
            return false;
        }
    }

    private static string BuildScript(string zipPath, string appDir, string executableName)
    {
        var escapedZip = EscapeBatchPath(zipPath);
        var escapedAppDir = EscapeBatchPath(appDir);
        var escapedExe = EscapeBatchPath(executableName);

        return "@echo off\r\n" +
               "setlocal enabledelayedexpansion\r\n" +
               $"set \"ZIP_PATH={escapedZip}\"\r\n" +
               $"set \"APP_DIR={escapedAppDir}\"\r\n" +
               $"set \"EXE_NAME={escapedExe}\"\r\n" +
               "set \"TEMP_DIR=%TEMP%\\NetDexUpdate_%RANDOM%\"\r\n" +
               "timeout /t 2 /nobreak >nul\r\n" +
               "if exist \"%TEMP_DIR%\" rmdir /s /q \"%TEMP_DIR%\"\r\n" +
               "mkdir \"%TEMP_DIR%\"\r\n" +
               "powershell -NoProfile -ExecutionPolicy Bypass -Command \"Expand-Archive -Path '%ZIP_PATH%' -DestinationPath '%TEMP_DIR%' -Force\"\r\n" +
               "if errorlevel 1 exit /b 1\r\n" +
               "set \"SRC_DIR=%TEMP_DIR%\"\r\n" +
               "for /d %%D in (\"%TEMP_DIR%\\*\") do (\r\n" +
               "  set \"SRC_DIR=%%~fD\"\r\n" +
               "  goto :copy\r\n" +
               ")\r\n" +
               ":copy\r\n" +
               "robocopy \"%SRC_DIR%\" \"%APP_DIR%\" /MIR >nul\r\n" +
               "if %ERRORLEVEL% GEQ 8 exit /b 1\r\n" +
               "start \"\" \"%APP_DIR%\\%EXE_NAME%\"\r\n" +
               "exit /b 0\r\n";
    }

    private static string EscapeBatchPath(string input)
    {
        return input.Replace("\"", "\"\"");
    }
}
