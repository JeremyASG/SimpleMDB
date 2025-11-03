using System.Collections;
using System.Net;
using System.Text.Json;

namespace SimpleMDB;

public class AuthApiController
{
    private IUSerService userService;

    public AuthApiController(IUSerService userService)
    {
        this.userService = userService;
    }

    // POST /api/v1/auth/register
    public async Task RegisterPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        string username = jsonData.ContainsKey("username") ? jsonData["username"].GetString() ?? "" : "";
        string password = jsonData.ContainsKey("password") ? jsonData["password"].GetString() ?? "" : "";

        User newUser = new User(0, username, password, "", "");
        var result = await userService.Create(newUser);

        if (result.IsValid)
        {
            var user = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, 
                new { 
                    message = "User registered successfully",
                    user = new { 
                        id = user.Id, 
                        username = user.Username, 
                        role = user.Role 
                    } 
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/auth/login
    public async Task LoginPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        string username = jsonData.ContainsKey("username") ? jsonData["username"].GetString() ?? "" : "";
        string password = jsonData.ContainsKey("password") ? jsonData["password"].GetString() ?? "" : "";

        var result = await userService.GetToken(username, password);

        if (result.IsValid)
        {
            string token = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    message = "User logged in successfully",
                    token = token 
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Unauthorized, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/auth/logout
    public async Task LogoutPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
            new { message = "User logged out successfully" });
    }

    // Middleware for checking authentication
    public async Task CheckAuth(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string token = req.Headers["Authorization"]?.StartsWith("Bearer ") == true 
            ? req.Headers["Authorization"]!.Substring(7) 
            : "";
        
        var result = await userService.ValidateToken(token);

        if (result.IsValid)
        {
            options["claims"] = result.Value!;
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Unauthorized, 
                new { error = result.Error!.Message });
        }
    }

    // Middleware for checking admin authorization
    public async Task CheckAdmin(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string token = req.Headers["Authorization"]?.StartsWith("Bearer ") == true 
            ? req.Headers["Authorization"]!.Substring(7) 
            : "";
        
        var result = await userService.ValidateToken(token);

        if (result.IsValid)
        {
            if (result.Value!["role"] == Roles.ADMIN)
            {
                options["claims"] = result.Value!;
            }
            else
            {
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Forbidden, 
                    new { error = "Authenticated but not authorized. You must be an administrator to access this resource." });
            }
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Unauthorized, 
                new { error = result.Error!.Message });
        }
    }
}
