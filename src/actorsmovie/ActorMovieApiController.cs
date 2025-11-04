using System.Collections;
using System.Net;
using System.Text.Json;

namespace SimpleMDB;

public class ActorMovieApiController
{
    private IActorMovieService actorMovieService;

    public ActorMovieApiController(IActorMovieService actorMovieService)
    {
        this.actorMovieService = actorMovieService;
    }

    // GET /api/v1/actors/movies?aid=1&page=1&size=10 (legacy) or GET /api/v1/actors/{id}/movies
    public async Task GetMoviesByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        // Try to get from route parameter first (REST style), then fall back to query string
        int actorId = HttpUtils.GetRouteParamAsInt(options, "id");
        if (actorId == 0)
        {
            actorId = int.TryParse(req.QueryString["aid"], out int aid) ? aid : 0;
        }
        
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        if (actorId == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor ID" });
            return;
        }

        Result<PagedResult<(ActorMovie, Movie)>> result = await actorMovieService.ReadAllMoviesByActor(actorId, page, size);

        if (result.IsValid)
        {
            PagedResult<(ActorMovie, Movie)> pagedResult = result.Value!;
            var moviesByActor = pagedResult.Values.Select(tuple => new {
                actorMovieId = tuple.Item1.Id,
                actorId = tuple.Item1.ActorId,
                movieId = tuple.Item1.MovieId,
                roleName = tuple.Item1.RoleName,
                movie = new {
                    id = tuple.Item2.Id,
                    title = tuple.Item2.Title,
                    year = tuple.Item2.Year,
                    description = tuple.Item2.Description,
                    rating = tuple.Item2.Rating
                }
            }).ToList();

            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    actorMovies = moviesByActor,
                    totalCount = pagedResult.Totalcount,
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

    // GET /api/v1/movies/actors?mid=1&page=1&size=10 (legacy) or GET /api/v1/movies/{id}/actors
    public async Task GetActorsByMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        // Try to get from route parameter first (REST style), then fall back to query string
        int movieId = HttpUtils.GetRouteParamAsInt(options, "id");
        if (movieId == 0)
        {
            movieId = int.TryParse(req.QueryString["mid"], out int mid) ? mid : 0;
        }
        
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        if (movieId == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid movie ID" });
            return;
        }

        Result<PagedResult<(ActorMovie, Actor)>> result = await actorMovieService.ReadAllActorsByMovie(movieId, page, size);

        if (result.IsValid)
        {
            PagedResult<(ActorMovie, Actor)> pagedResult = result.Value!;
            var actorsByMovie = pagedResult.Values.Select(tuple => new {
                actorMovieId = tuple.Item1.Id,
                actorId = tuple.Item1.ActorId,
                movieId = tuple.Item1.MovieId,
                roleName = tuple.Item1.RoleName,
                actor = new {
                    id = tuple.Item2.Id,
                    firstName = tuple.Item2.FirstName,
                    lastName = tuple.Item2.LastName,
                    bio = tuple.Item2.Bio,
                    rating = tuple.Item2.Rating
                }
            }).ToList();

            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    actorMovies = actorsByMovie,
                    totalCount = pagedResult.Totalcount,
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

    // POST /api/v1/actors/movies/add
    public async Task AddMovieToActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        int actorId = jsonData.ContainsKey("actorId") && jsonData["actorId"].TryGetInt32(out int aid) ? aid : 0;
        int movieId = jsonData.ContainsKey("movieId") && jsonData["movieId"].TryGetInt32(out int mid) ? mid : 0;
        string roleName = jsonData.ContainsKey("roleName") ? jsonData["roleName"].GetString() ?? "" : "";

        if (actorId == 0 || movieId == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor ID or movie ID" });
            return;
        }

        Result<ActorMovie> result = await actorMovieService.Create(actorId, movieId, roleName);

        if (result.IsValid)
        {
            var actorMovie = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, 
                new { 
                    message = "Movie added to actor successfully",
                    actorMovie = new {
                        id = actorMovie.Id,
                        actorId = actorMovie.ActorId,
                        movieId = actorMovie.MovieId,
                        roleName = actorMovie.RoleName
                    }
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/movies/actors/add
    public async Task AddActorToMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        int actorId = jsonData.ContainsKey("actorId") && jsonData["actorId"].TryGetInt32(out int aid) ? aid : 0;
        int movieId = jsonData.ContainsKey("movieId") && jsonData["movieId"].TryGetInt32(out int mid) ? mid : 0;
        string roleName = jsonData.ContainsKey("roleName") ? jsonData["roleName"].GetString() ?? "" : "";

        if (actorId == 0 || movieId == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor ID or movie ID" });
            return;
        }

        Result<ActorMovie> result = await actorMovieService.Create(actorId, movieId, roleName);

        if (result.IsValid)
        {
            var actorMovie = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, 
                new { 
                    message = "Actor added to movie successfully",
                    actorMovie = new {
                        id = actorMovie.Id,
                        actorId = actorMovie.ActorId,
                        movieId = actorMovie.MovieId,
                        roleName = actorMovie.RoleName
                    }
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/actors/movies/remove?amid=1 (legacy) or DELETE /api/v1/actor-movies/{id}
    public async Task RemoveMovieFromActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        // Try to get from route parameter first (REST style), then fall back to query string
        int amId = HttpUtils.GetRouteParamAsInt(options, "id");
        if (amId == 0)
        {
            amId = int.TryParse(req.QueryString["amid"], out int amid) ? amid : 0;
        }

        if (amId == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor-movie ID" });
            return;
        }

        Result<ActorMovie> result = await actorMovieService.Delete(amId);

        if (result.IsValid)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { message = "Movie removed from actor successfully" });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/movies/actors/remove?amid=1 (legacy) or DELETE /api/v1/actor-movies/{id}
    public async Task RemoveActorFromMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        // Try to get from route parameter first (REST style), then fall back to query string
        int amId = HttpUtils.GetRouteParamAsInt(options, "id");
        if (amId == 0)
        {
            amId = int.TryParse(req.QueryString["amid"], out int amid) ? amid : 0;
        }

        if (amId == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor-movie ID" });
            return;
        }

        Result<ActorMovie> result = await actorMovieService.Delete(amId);

        if (result.IsValid)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { message = "Actor removed from movie successfully" });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }

    // GET /api/v1/actors-movies/{id} - View specific actor-movie relationship
    public async Task View(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        // Try to get from route parameter first (REST style), then fall back to query string
        int amId = HttpUtils.GetRouteParamAsInt(options, "id");
        if (amId == 0)
        {
            amId = int.TryParse(req.QueryString["id"], out int amid) ? amid : 0;
        }

        if (amId == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor-movie ID" });
            return;
        }

        Result<ActorMovie> result = await actorMovieService.Read(amId);

        if (result.IsValid)
        {
            ActorMovie actorMovie = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    actorMovie = new {
                        id = actorMovie.Id,
                        actorId = actorMovie.ActorId,
                        movieId = actorMovie.MovieId,
                        roleName = actorMovie.RoleName
                    }
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotFound, 
                new { error = result.Error!.Message });
        }
    }

    // PUT /api/v1/actors-movies/{id} - Update actor-movie relationship
    public async Task Edit(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];
        
        // Try to get from route parameter first (REST style), then fall back to query string
        int amId = HttpUtils.GetRouteParamAsInt(options, "id");
        if (amId == 0)
        {
            amId = int.TryParse(req.QueryString["id"], out int amid) ? amid : 0;
        }

        if (amId == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid actor-movie ID" });
            return;
        }

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        string roleName = jsonData.ContainsKey("roleName") ? jsonData["roleName"].GetString() ?? "" : "";

        Result<ActorMovie> result = await actorMovieService.Update(amId, roleName);

        if (result.IsValid)
        {
            var actorMovie = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    message = "Actor-movie relationship updated successfully",
                    actorMovie = new {
                        id = actorMovie.Id,
                        actorId = actorMovie.ActorId,
                        movieId = actorMovie.MovieId,
                        roleName = actorMovie.RoleName
                    }
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }
}
