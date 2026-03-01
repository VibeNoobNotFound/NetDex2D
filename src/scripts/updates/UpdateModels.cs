using System.Collections.Generic;

namespace NetDex.Updates;

public enum UpdatePlatform
{
    MacOS = 0,
    Windows = 1,
    Linux = 2,
    Android = 3,
    IOS = 4,
    Unsupported = 5
}

public enum UpdateState
{
    Idle = 0,
    Checking = 1,
    UpdateAvailable = 2,
    Downloading = 3,
    ReadyToInstall = 4,
    Installing = 5,
    UpToDate = 6,
    Error = 7
}

public enum UpdateIssueCode
{
    NetworkError = 0,
    RateLimited = 1,
    InvalidPayload = 2,
    AssetMissing = 3,
    DigestMissing = 4,
    DigestMismatch = 5,
    DownloadFailed = 6,
    InstallFailed = 7,
    UnsupportedPlatform = 8
}

public sealed record UpdateAssetInfo(
    string Name,
    string DownloadUrl,
    long SizeBytes,
    string Digest
);

public sealed record UpdateReleaseInfo(
    string TagName,
    string Version,
    string HtmlUrl,
    IReadOnlyList<UpdateAssetInfo> Assets
);

public readonly record struct UpdateOperationResult(
    bool Success,
    string Message,
    UpdateIssueCode? IssueCode = null
);

public readonly record struct ReleaseQueryResult(
    bool Success,
    UpdateReleaseInfo? Release,
    string Message,
    UpdateIssueCode? IssueCode = null
);
