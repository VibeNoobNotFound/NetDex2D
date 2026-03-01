using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace NetDex.Updates;

public sealed class GitHubReleaseProvider
{
    public async Task<ReleaseQueryResult> FetchLatestReleaseAsync(Node host, string owner, string repo)
    {
        var url = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

        var request = new HttpRequest();
        host.AddChild(request);

        var headers = new[]
        {
            "Accept: application/vnd.github+json",
            "X-GitHub-Api-Version: 2022-11-28",
            "User-Agent: NetDex-Updater"
        };

        var requestError = request.Request(url, headers, HttpClient.Method.Get);
        if (requestError != Error.Ok)
        {
            request.QueueFree();
            return new ReleaseQueryResult(false, null, $"Update check failed: {requestError}", UpdateIssueCode.NetworkError);
        }

        var completed = await host.ToSignal(request, HttpRequest.SignalName.RequestCompleted);
        request.QueueFree();

        var requestResult = (HttpRequest.Result)completed[0].AsInt32();
        var responseCode = completed[1].AsInt32();
        var bodyBytes = completed[3].AsByteArray();

        if (requestResult != HttpRequest.Result.Success)
        {
            return new ReleaseQueryResult(false, null, $"Update check network error: {requestResult}", UpdateIssueCode.NetworkError);
        }

        if (responseCode == 403)
        {
            return new ReleaseQueryResult(false, null, "GitHub API rate limit reached. Try again later.", UpdateIssueCode.RateLimited);
        }

        if (responseCode < 200 || responseCode >= 300)
        {
            return new ReleaseQueryResult(false, null, $"GitHub API returned {responseCode}.", UpdateIssueCode.NetworkError);
        }

        var jsonText = Encoding.UTF8.GetString(bodyBytes);
        var parsed = Json.ParseString(jsonText);
        if (parsed.VariantType != Variant.Type.Dictionary)
        {
            return new ReleaseQueryResult(false, null, "Invalid release payload from GitHub.", UpdateIssueCode.InvalidPayload);
        }

        var payload = parsed.AsGodotDictionary();
        if (!payload.TryGetValue("tag_name", out var tagVariant) || !payload.TryGetValue("html_url", out var htmlVariant))
        {
            return new ReleaseQueryResult(false, null, "Release payload missing required fields.", UpdateIssueCode.InvalidPayload);
        }

        var tagName = tagVariant.AsString();
        var htmlUrl = htmlVariant.AsString();
        var normalizedVersion = VersionComparer.NormalizeTagToVersion(tagName);

        var assets = new List<UpdateAssetInfo>();
        if (payload.TryGetValue("assets", out var assetsVariant) && assetsVariant.VariantType == Variant.Type.Array)
        {
            foreach (var item in assetsVariant.AsGodotArray())
            {
                if (item.VariantType != Variant.Type.Dictionary)
                {
                    continue;
                }

                var assetDict = item.AsGodotDictionary();
                var name = assetDict.TryGetValue("name", out var nameVariant) ? nameVariant.AsString() : string.Empty;
                var downloadUrl = assetDict.TryGetValue("browser_download_url", out var downloadVariant) ? downloadVariant.AsString() : string.Empty;
                var size = assetDict.TryGetValue("size", out var sizeVariant) ? sizeVariant.AsInt64() : 0L;
                var digest = assetDict.TryGetValue("digest", out var digestVariant) ? digestVariant.AsString() : string.Empty;

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(downloadUrl))
                {
                    continue;
                }

                assets.Add(new UpdateAssetInfo(name, downloadUrl, size, digest));
            }
        }

        var release = new UpdateReleaseInfo(tagName, normalizedVersion, htmlUrl, assets);
        return new ReleaseQueryResult(true, release, "Release metadata loaded.");
    }
}
