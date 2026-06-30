using UnityEngine;

public static class LaravelRequest
{
    private static string domainName = "bayanichronicles.com";

    public static string GetLink(string uri)
    {
        if (!uri.StartsWith("/"))
            uri = "/" + uri;

        return domainName + uri;
    }
}
