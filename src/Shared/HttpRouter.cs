using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
namespace SimpleMDB;

public class HttpRouter
{
    
    public static readonly int RESPONSE_NOT_SENT_YET = 795;
    private List<HttpMiddleware> middlewares;
    private List<(string, string, HttpMiddleware[] middlewares)> endpoints;

    public HttpRouter()
    {

        middlewares = [];
        endpoints = [];
    }
    public void Use(params HttpMiddleware[] middlewares)
    {
        this.middlewares.AddRange(middlewares);
    }

    public void AddEndPoint(string method, string route, params HttpMiddleware[] middlewares)
    {
        this.endpoints.Add((method, route, middlewares));
    }
    public void AddGet(string route, params HttpMiddleware[] middlewares)
    {
        AddEndPoint("GET", route, middlewares);
    }

    public void AddPost(string route, params HttpMiddleware[] middlewares)
    {
        AddEndPoint("POST", route, middlewares);
    }

    public void AddPut(string route, params HttpMiddleware[] middlewares)
    {
        AddEndPoint("PUT", route, middlewares);
    }



    public void AddDelete(string route, params HttpMiddleware[] middlewares)
    {
        AddEndPoint("DELETE", route, middlewares);
    }

    // Check if a route pattern matches the request path and extract parameters
    private bool MatchRoute(string pattern, string path, out Dictionary<string, string> parameters)
    {
        parameters = new Dictionary<string, string>();

        // Replace {param} with regex capture groups
        string regexPattern = "^" + Regex.Replace(pattern, @"\{([^}]+)\}", @"(?<$1>[^/]+)") + "$";
        
        var match = Regex.Match(path, regexPattern);
        
        if (match.Success)
        {
            // Extract all captured groups as parameters
            foreach (Group group in match.Groups)
            {
                if (!int.TryParse(group.Name, out _) && group.Name != "0")
                {
                    parameters[group.Name] = group.Value;
                }
            }
            return true;
        }
        
        return false;
    }

    public async Task Handle(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        

        foreach (var middleware in middlewares)
        {
            await middleware(req, res, options);
            if (res.StatusCode != RESPONSE_NOT_SENT_YET) { return; }
        }

        foreach (var (method, route, middlewares) in endpoints)
        {
            // Check for exact match first (backward compatibility)
            if (req.HttpMethod == method && req.Url!.AbsolutePath == route)
            {
                foreach (var middleware in middlewares)
                {
                    await middleware(req, res, options);

                    if (res.StatusCode != RESPONSE_NOT_SENT_YET) { return; }
                }
            }
            // Check for parametrized route match
            else if (req.HttpMethod == method && MatchRoute(route, req.Url!.AbsolutePath, out var parameters))
            {
                // Store route parameters in options for middleware access
                options["route.params"] = parameters;
                
                foreach (var middleware in middlewares)
                {
                    await middleware(req, res, options);

                    if (res.StatusCode != RESPONSE_NOT_SENT_YET) { return; }
                }
            }
        }


    }


    
}