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

    // POST /api/v1/register
    public async Task RegisterPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string jsonBody = (string?)options["req.json"] ?? "{}";

        try
        {
            if (string.IsNullOrWhiteSpace(jsonBody) || jsonBody == "{}")
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Request body is required");
                return;
            }

            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            string username = data.TryGetProperty("username", out var u) ? u.GetString() ?? "" : "";
            string password = data.TryGetProperty("password", out var p) ? p.GetString() ?? "" : "";
            string cpassword = data.TryGetProperty("cpassword", out var cp) ? cp.GetString() ?? "" : "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Username and password are required");
                return;
            }

            if (password != cpassword)
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Passwords do not match.");
                return;
            }

            User newUser = new User(0, username, password, "", "");
            var result = await userService.Create(newUser);

            if (result.IsValid)
            {
                var response = new { message = "User registered successfully!", user = result.Value! };
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, response);
            }
            else
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, result.Error!.Message);
            }
        }
        catch (JsonException ex)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, $"Invalid JSON format: {ex.Message}");
        }
        catch (Exception ex)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.InternalServerError, $"Server error: {ex.Message}");
        }
    }

    // POST /api/v1/login
    public async Task LoginPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string jsonBody = (string?)options["req.json"] ?? "{}";

        try
        {
            if (string.IsNullOrWhiteSpace(jsonBody) || jsonBody == "{}")
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Request body is required");
                return;
            }

            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            string username = data.TryGetProperty("username", out var u) ? u.GetString() ?? "" : "";
            string password = data.TryGetProperty("password", out var p) ? p.GetString() ?? "" : "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Username and password are required");
                return;
            }

            var result = await userService.GetToken(username, password);

            if (result.IsValid)
            {
                string token = result.Value!;
                var response = new { message = "User logged in successfully!", token = token };
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
            }
            else
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.Unauthorized, result.Error!.Message);
            }
        }
        catch (JsonException ex)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, $"Invalid JSON format: {ex.Message}");
        }
        catch (Exception ex)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.InternalServerError, $"Server error: {ex.Message}");
        }
    }

    // POST /api/v1/logout
    public async Task LogoutPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var response = new { message = "User logged out successfully!" };
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
    }
}
