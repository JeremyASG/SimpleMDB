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
    public async Task GetAll(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        Result<PagedResult<User>> result = await userService.ReadAll(page, size);

        if (result.IsValid)
        {
            PagedResult<User> pagedResult = result.Value!;
            List<User> users = pagedResult.Values;
            int userCount = pagedResult.Totalcount;

            var usersData = users.Select(u => new { 
                id = u.Id, 
                username = u.Username, 
                role = u.Role 
            }).ToList();

            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    users = usersData,
                    totalCount = userCount,
                    page = page,
                    size = size
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.InternalServerError, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/users/add
    public async Task Add(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
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
        string role = jsonData.ContainsKey("role") ? jsonData["role"].GetString() ?? "" : "";

        User newUser = new User(0, username, password, "", role);

        Result<User> result = await userService.Create(newUser);

        if (result.IsValid)
        {
            var user = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, 
                new { 
                    message = "User added successfully",
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

    // GET /api/v1/users/view?uid=1
    public async Task View(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 0;

        if (uid == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid user ID" });
            return;
        }

        Result<User> result = await userService.Read(uid);

        if (result.IsValid)
        {
            User user = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    user = new { 
                        id = user.Id, 
                        username = user.Username, 
                        role = user.Role 
                    } 
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotFound, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/users/edit?uid=1
    public async Task Edit(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 0;

        if (uid == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid user ID" });
            return;
        }

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        string username = jsonData.ContainsKey("username") ? jsonData["username"].GetString() ?? "" : "";
        string password = jsonData.ContainsKey("password") ? jsonData["password"].GetString() ?? "" : "";
        string role = jsonData.ContainsKey("role") ? jsonData["role"].GetString() ?? "" : "";

        User newUser = new User(0, username, password, "", role);

        Result<User> result = await userService.Update(uid, newUser);

        if (result.IsValid)
        {
            var user = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    message = "User edited successfully",
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

    // POST /api/v1/users/remove?uid=1
    public async Task Remove(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 0;

        if (uid == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid user ID" });
            return;
        }

        Result<User> result = await userService.Delete(uid);

        if (result.IsValid)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { message = "User removed successfully" });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }
}
