using System.Collections;
using System.Net;
using System.Text.Json;

namespace SimpleMDB;

public class UsersApiController
{
    private IUSerService userService;

    public UsersApiController(IUSerService userService)
    {
        this.userService = userService;
    }

    // GET /api/v1/users?page=1&size=10
    public async Task ViewAllUsersGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        Result<PagedResult<User>> result = await userService.ReadAll(page, size);

        if (result.IsValid)
        {
            PagedResult<User> pagedResult = result.Value!;
            var response = new
            {
                users = pagedResult.Values,
                totalCount = pagedResult.Totalcount,
                page = page,
                size = size
            };
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
        }
        else
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.InternalServerError, result.Error!.Message);
        }
    }

    // POST /api/v1/users/add
    public async Task AddUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string jsonBody = (string?)options["req.json"] ?? "{}";
        
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            string username = data.TryGetProperty("username", out var u) ? u.GetString() ?? "" : "";
            string password = data.TryGetProperty("password", out var p) ? p.GetString() ?? "" : "";
            string role = data.TryGetProperty("role", out var r) ? r.GetString() ?? "" : "";

            User newUser = new User(0, username, password, "", role);
            Result<User> result = await userService.Create(newUser);

            if (result.IsValid)
            {
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, result.Value!);
            }
            else
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, result.Error!.Message);
            }
        }
        catch (JsonException)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid JSON format");
        }
    }

    // GET /api/v1/users/view?uid=1
    public async Task ViewUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

        if (uid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid user ID");
            return;
        }

        Result<User> result = await userService.Read(uid);

        if (result.IsValid)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, result.Value!);
        }
        else
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, result.Error!.Message);
        }
    }

    // POST /api/v1/users/edit?uid=1
    public async Task EditUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

        if (uid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid user ID");
            return;
        }

        string jsonBody = (string?)options["req.json"] ?? "{}";

        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            string username = data.TryGetProperty("username", out var un) ? un.GetString() ?? "" : "";
            string password = data.TryGetProperty("password", out var p) ? p.GetString() ?? "" : "";
            string role = data.TryGetProperty("role", out var r) ? r.GetString() ?? "" : "";

            User newUser = new User(0, username, password, "", role);
            Result<User> result = await userService.Update(uid, newUser);

            if (result.IsValid)
            {
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, result.Value!);
            }
            else
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, result.Error!.Message);
            }
        }
        catch (JsonException)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid JSON format");
        }
    }

    // POST /api/v1/users/remove?uid=1
    public async Task RemoveUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

        if (uid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid user ID");
            return;
        }

        Result<User> result = await userService.Delete(uid);

        if (result.IsValid)
        {
            var response = new { message = "User removed successfully", user = result.Value! };
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
        }
        else
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, result.Error!.Message);
        }
    }
}
