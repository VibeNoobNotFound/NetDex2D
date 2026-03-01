using System.Threading.Tasks;
using Godot;

namespace NetDex.Updates.Installers;

public interface IUpdateInstaller
{
    UpdatePlatform Platform { get; }
    string ActionLabel { get; }
    Task<UpdateOperationResult> ExecuteAsync(Node host, UpdateReleaseInfo release, UpdateAssetInfo? platformAsset);
}
