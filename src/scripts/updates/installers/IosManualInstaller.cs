using System.Threading.Tasks;
using Godot;

namespace NetDex.Updates.Installers;

public sealed class IosManualInstaller : IUpdateInstaller
{
    public UpdatePlatform Platform => UpdatePlatform.IOS;
    public string ActionLabel => "Open Update Page";

    public Task<UpdateOperationResult> ExecuteAsync(Node host, UpdateReleaseInfo release, UpdateAssetInfo? platformAsset)
    {
        if (!string.IsNullOrWhiteSpace(release.HtmlUrl) && OS.ShellOpen(release.HtmlUrl) == Error.Ok)
        {
            return Task.FromResult(new UpdateOperationResult(true, "Opened update page. iOS requires manual update."));
        }

        return Task.FromResult(new UpdateOperationResult(false, "Unable to open update page.", UpdateIssueCode.InstallFailed));
    }
}
