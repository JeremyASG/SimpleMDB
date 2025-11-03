using System.Collections;
using System.Net;
using System.Text.Json;

namespace SimpleMDB;

public class MoviesApiController
{
    private IMovieService movieService;

    public MoviesApiController(IMovieService movieService)
    {
        this.movieService = movieService;
    }

    // GET /api/v1/movies?page=1&size=10
    public async Task ViewAllMoviesGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        Result<PagedResult<Movie>> result = await movieService.ReadAll(page, size);

        if (result.IsValid)
        {
            PagedResult<Movie> pagedResult = result.Value!;
            var response = new
            {
                movies = pagedResult.Values,
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

    // POST /api/v1/movies/add
    public async Task AddMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string jsonBody = (string?)options["req.json"] ?? "{}";

        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            string title = data.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
            int year = data.TryGetProperty("year", out var y) && y.TryGetInt32(out int yr) ? yr : DateTime.Now.Year;
            string description = data.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "";
            float rating = data.TryGetProperty("rating", out var r) && r.TryGetSingle(out float rat) ? rat : 5F;

            Movie newMovie = new Movie(0, title, year, description, rating);
            Result<Movie> result = await movieService.Create(newMovie);

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

    // GET /api/v1/movies/view?mid=1
    public async Task ViewMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;

        if (mid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid movie ID");
            return;
        }

        Result<Movie> result = await movieService.Read(mid);

        if (result.IsValid)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, result.Value!);
        }
        else
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, result.Error!.Message);
        }
    }

    // POST /api/v1/movies/edit?mid=1
    public async Task EditMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;

        if (mid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid movie ID");
            return;
        }

        string jsonBody = (string?)options["req.json"] ?? "{}";

        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(jsonBody);
            string title = data.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
            int year = data.TryGetProperty("year", out var y) && y.TryGetInt32(out int yr) ? yr : DateTime.Now.Year;
            string description = data.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "";
            float rating = data.TryGetProperty("rating", out var r) && r.TryGetSingle(out float rat) ? rat : 5F;

            Movie newMovie = new Movie(0, title, year, description, rating);
            Result<Movie> result = await movieService.Update(mid, newMovie);

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

    // POST /api/v1/movies/remove?mid=1
    public async Task RemoveMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;

        if (mid <= 0)
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.BadRequest, "Invalid movie ID");
            return;
        }

        Result<Movie> result = await movieService.Delete(mid);

        if (result.IsValid)
        {
            var response = new { message = "Movie removed successfully", movie = result.Value! };
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, response);
        }
        else
        {
            await HttpUtils.RespondJsonError(req, res, options, (int)HttpStatusCode.NotFound, result.Error!.Message);
        }
    }
}
