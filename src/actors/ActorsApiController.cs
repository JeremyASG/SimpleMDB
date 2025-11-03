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
    public async Task GetAll(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        Result<PagedResult<Actor>> result = await actorService.ReadAll(page, size);

        if (result.IsValid)
        {
            PagedResult<Actor> pagedResult = result.Value!;
            List<Actor> actors = pagedResult.Values;
            int actorCount = pagedResult.Totalcount;

            var actorsData = actors.Select(a => new { 
                id = a.Id, 
                firstName = a.FirstName,
                lastName = a.LastName,
                bio = a.Bio,
                rating = a.Rating
            }).ToList();

            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    actors = actorsData,
                    totalCount = actorCount,
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

    // POST /api/v1/actors/add
    public async Task Add(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        string firstName = jsonData.ContainsKey("firstName") ? jsonData["firstName"].GetString() ?? "" : "";
        string lastName = jsonData.ContainsKey("lastName") ? jsonData["lastName"].GetString() ?? "" : "";
        string bio = jsonData.ContainsKey("bio") ? jsonData["bio"].GetString() ?? "" : "";
        float rating = jsonData.ContainsKey("rating") && jsonData["rating"].TryGetSingle(out float r) ? r : 5.0f;

        Actor newActor = new Actor(0, firstName, lastName, bio, rating);

        Result<Actor> result = await actorService.Create(newActor);

        if (result.IsValid)
        {
            var actor = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, 
                new { 
                    message = "Actor added successfully",
                    actor = new { 
                        id = actor.Id, 
                        firstName = actor.FirstName,
                        lastName = actor.LastName,
                        bio = actor.Bio,
                        rating = actor.Rating
                    } 
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }

    // GET /api/v1/actors/view?aid=1
    public async Task View(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : 0;

        if (aid == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor ID" });
            return;
        }

        Result<Actor> result = await actorService.Read(aid);

        if (result.IsValid)
        {
            Actor actor = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    actor = new { 
                        id = actor.Id, 
                        firstName = actor.FirstName,
                        lastName = actor.LastName,
                        bio = actor.Bio,
                        rating = actor.Rating
                    } 
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotFound, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/actors/edit?aid=1
    public async Task Edit(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];
        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : 0;

        if (aid == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor ID" });
            return;
        }

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        string firstName = jsonData.ContainsKey("firstName") ? jsonData["firstName"].GetString() ?? "" : "";
        string lastName = jsonData.ContainsKey("lastName") ? jsonData["lastName"].GetString() ?? "" : "";
        string bio = jsonData.ContainsKey("bio") ? jsonData["bio"].GetString() ?? "" : "";
        float rating = jsonData.ContainsKey("rating") && jsonData["rating"].TryGetSingle(out float r) ? r : 5.0f;

        Actor newActor = new Actor(0, firstName, lastName, bio, rating);

        Result<Actor> result = await actorService.Update(aid, newActor);

        if (result.IsValid)
        {
            var actor = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    message = "Actor edited successfully",
                    actor = new { 
                        id = actor.Id, 
                        firstName = actor.FirstName,
                        lastName = actor.LastName,
                        bio = actor.Bio,
                        rating = actor.Rating
                    } 
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/actors/remove?aid=1
    public async Task Remove(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : 0;

        if (aid == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor ID" });
            return;
        }

        Result<Actor> result = await actorService.Delete(aid);

        if (result.IsValid)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { message = "Actor removed successfully" });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }
}
