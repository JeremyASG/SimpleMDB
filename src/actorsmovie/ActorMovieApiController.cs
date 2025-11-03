using System.Collections;
using System.Net;
using System.Text.Json;

namespace SimpleMDB;

public class ActorMovieApiController
{
    private IActorMovieService actorMovieService;
    private IActorService actorService;
    private IMovieService movieService;

    public ActorMovieApiController(IActorMovieService actorMovieService, IActorService actorService, IMovieService movieService)
    {
        this.actorMovieService = actorMovieService;
        this.actorService = actorService;
        this.movieService = movieService;
    }

    // GET /api/v1/actors/movies?aid=1&page=1&size=10
    public async Task ViewAllMoviesByActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : -1;
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        if (aid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid actor ID");
            return;
        }

        var result1 = await actorService.Read(aid);
        var result2 = await actorMovieService.ReadAllMoviesByActor(aid, page, size);

        if (result1.IsValid && result2.IsValid)
        {
            var actor = result1.Value!;
            var pagedResult = result2.Value!;
            var moviesWithRoles = pagedResult.Values.Select(tuple => new
            {
                actorMovieId = tuple.Item1.Id,
                roleName = tuple.Item1.RoleName,
                movie = tuple.Item2
            }).ToList();

            var response = new
            {
                actor = actor,
                movies = moviesWithRoles,
                totalCount = pagedResult.Totalcount,
                page = page,
                size = size
            };
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : " " + result2.Error!.Message;
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, error.Trim());
        }
    }

    // POST /api/v1/actors/movies/add
    public async Task AddMoviesByActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string jsonBody = (string?)options["req.json"] ?? "{}";

        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            int aid = data.TryGetProperty("aid", out var a) && a.TryGetInt32(out int actorId) ? actorId : -1;
            int mid = data.TryGetProperty("mid", out var m) && m.TryGetInt32(out int movieId) ? movieId : -1;
            string rolename = data.TryGetProperty("rolename", out var r) ? r.GetString() ?? "" : "";

            if (aid <= 0 || mid <= 0)
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid actor ID or movie ID");
                return;
            }

            var result = await actorMovieService.Create(aid, mid, rolename);
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

    // POST /api/v1/actors/movies/remove?amid=1
    public async Task RemoveMoviesByActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : -1;

        if (amid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid actor-movie ID");
            return;
        }

        Result<ActorMovie> result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            var response = new { message = "Actor-movie relationship removed successfully", actorMovie = result.Value! };
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
        }
        else
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, result.Error!.Message);
        }
    }

    // GET /api/v1/movies/actors?mid=1&page=1&size=10
    public async Task ViewAllActorsByMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        if (mid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid movie ID");
            return;
        }

        var result1 = await movieService.Read(mid);
        var result2 = await actorMovieService.ReadAllActorsByMovie(mid, page, size);

        if (result1.IsValid && result2.IsValid)
        {
            var movie = result1.Value!;
            var pagedResult = result2.Value!;
            var actorsWithRoles = pagedResult.Values.Select(tuple => new
            {
                actorMovieId = tuple.Item1.Id,
                roleName = tuple.Item1.RoleName,
                actor = tuple.Item2
            }).ToList();

            var response = new
            {
                movie = movie,
                actors = actorsWithRoles,
                totalCount = pagedResult.Totalcount,
                page = page,
                size = size
            };
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : " " + result2.Error!.Message;
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, error.Trim());
        }
    }

    // POST /api/v1/movies/actors/add
    public async Task AddActorsByMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string jsonBody = (string?)options["req.json"] ?? "{}";

        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            int mid = data.TryGetProperty("mid", out var m) && m.TryGetInt32(out int movieId) ? movieId : -1;
            int aid = data.TryGetProperty("aid", out var a) && a.TryGetInt32(out int actorId) ? actorId : -1;
            string rolename = data.TryGetProperty("rolename", out var r) ? r.GetString() ?? "" : "";

            if (aid <= 0 || mid <= 0)
            {
                await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid actor ID or movie ID");
                return;
            }

            var result = await actorMovieService.Create(aid, mid, rolename);
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

    // POST /api/v1/movies/actors/remove?amid=1
    public async Task RemoveActorsByMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : -1;

        if (amid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid actor-movie ID");
            return;
        }

        Result<ActorMovie> result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            var response = new { message = "Actor-movie relationship removed successfully", actorMovie = result.Value! };
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
        }
        else
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, result.Error!.Message);
        }
    }
}
