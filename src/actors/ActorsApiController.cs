using System.Collections;
using System.Net;
using System.Text.Json;

namespace SimpleMDB;

public class ActorsApiController
{
    private IActorService actorService;

    public ActorsApiController(IActorService actorService)
    {
        this.actorService = actorService;
    }

    // GET /api/v1/actors?page=1&size=10
    public async Task ViewAllActorsGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        Result<PagedResult<Actor>> result = await actorService.ReadAll(page, size);

        if (result.IsValid)
        {
            PagedResult<Actor> pagedResult = result.Value!;
            var response = new
            {
                actors = pagedResult.Values,
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

    // POST /api/v1/actors/add
    public async Task AddActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string jsonBody = (string?)options["req.json"] ?? "{}";

        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            string firstname = data.TryGetProperty("firstname", out var fn) ? fn.GetString() ?? "" : "";
            string lastname = data.TryGetProperty("lastname", out var ln) ? ln.GetString() ?? "" : "";
            string bio = data.TryGetProperty("bio", out var b) ? b.GetString() ?? "" : "";
            float rating = data.TryGetProperty("rating", out var r) && r.TryGetSingle(out float rat) ? rat : 5F;

            Actor newActor = new Actor(0, firstname, lastname, bio, rating);
            Result<Actor> result = await actorService.Create(newActor);

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

    // GET /api/v1/actors/view?aid=1
    public async Task ViewActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : -1;

        if (aid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid actor ID");
            return;
        }

        Result<Actor> result = await actorService.Read(aid);

        if (result.IsValid)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, result.Value!);
        }
        else
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, result.Error!.Message);
        }
    }

    // POST /api/v1/actors/edit?aid=1
    public async Task EditActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : -1;

        if (aid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid actor ID");
            return;
        }

        string jsonBody = (string?)options["req.json"] ?? "{}";

        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            string firstname = data.TryGetProperty("firstname", out var fn) ? fn.GetString() ?? "" : "";
            string lastname = data.TryGetProperty("lastname", out var ln) ? ln.GetString() ?? "" : "";
            string bio = data.TryGetProperty("bio", out var b) ? b.GetString() ?? "" : "";
            float rating = data.TryGetProperty("rating", out var r) && r.TryGetSingle(out float rat) ? rat : 5F;

            Actor newActor = new Actor(0, firstname, lastname, bio, rating);
            Result<Actor> result = await actorService.Update(aid, newActor);

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

    // POST /api/v1/actors/remove?aid=1
    public async Task RemoveActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : -1;

        if (aid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid actor ID");
            return;
        }

        Result<Actor> result = await actorService.Delete(aid);

        if (result.IsValid)
        {
            var response = new { message = "Actor removed successfully", actor = result.Value! };
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
        }
        else
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, result.Error!.Message);
        }
    }
}
