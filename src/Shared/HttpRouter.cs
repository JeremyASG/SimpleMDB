using System.Collections;
using System.Net;
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
    public async Task Handle(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        

        foreach (var middleware in middlewares)
        {
            await middleware(req, res, options);
            if (res.StatusCode != RESPONSE_NOT_SENT_YET) { return; }
        }

        foreach (var (method, route, middlewares) in endpoints)
        {
            if (req.HttpMethod == method && req.Url!.AbsolutePath == route)
            {
                foreach (var middleware in middlewares)
                {
                    await middleware(req, res, options);

                    if (res.StatusCode != RESPONSE_NOT_SENT_YET) { return; }
                }
            }
        }


    }


    
}