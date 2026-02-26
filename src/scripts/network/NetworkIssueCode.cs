namespace NetDex.Networking;

public enum NetworkIssueCode
{
    None = 0,
    MissingInternetPermission = 1,
    SocketCreateFailed = 2,
    DiscoveryBindFailed = 3
}
