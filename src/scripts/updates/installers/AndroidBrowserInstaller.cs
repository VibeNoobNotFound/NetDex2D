using System.Threading.Tasks;
using Godot;

namespace NetDex.Updates.Installers;

public sealed class AndroidBrowserInstaller : IUpdateInstaller
{
    public UpdatePlatform Platform => UpdatePlatform.Android;
    public string ActionLabel => "Update in Browser";

    public Task<UpdateOperationResult> ExecuteAsync(Node host, UpdateReleaseInfo release, UpdateAssetInfo? platformAsset)
    {
        var preferredUrl = platformAsset?.DownloadUrl ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(preferredUrl) && OS.ShellOpen(preferredUrl) == Error.Ok)
        {
            return Task.FromResult(new UpdateOperationResult(true, "Opened APK download in browser."));
        }

        if (!string.IsNullOrWhiteSpace(release.HtmlUrl) && OS.ShellOpen(release.HtmlUrl) == Error.Ok)
        {
            return Task.FromResult(new UpdateOperationResult(true, "Opened release page in browser."));
        }

        return Task.FromResult(new UpdateOperationResult(false, "Unable to open browser for update.", UpdateIssueCode.InstallFailed));
    }
}
