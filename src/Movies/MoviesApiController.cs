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
    public async Task GetAll(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 10;

        Result<PagedResult<Movie>> result = await movieService.ReadAll(page, size);

        if (result.IsValid)
        {
            PagedResult<Movie> pagedResult = result.Value!;
            List<Movie> movies = pagedResult.Values;
            int movieCount = pagedResult.Totalcount;

            var moviesData = movies.Select(m => new { 
                id = m.Id, 
                title = m.Title,
                year = m.Year,
                description = m.Description,
                rating = m.Rating
            }).ToList();

            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    movies = moviesData,
                    totalCount = movieCount,
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

    // POST /api/v1/movies/add
    public async Task Add(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        string title = jsonData.ContainsKey("title") ? jsonData["title"].GetString() ?? "" : "";
        int year = jsonData.ContainsKey("year") && jsonData["year"].TryGetInt32(out int y) ? y : DateTime.Now.Year;
        string description = jsonData.ContainsKey("description") ? jsonData["description"].GetString() ?? "" : "";
        float rating = jsonData.ContainsKey("rating") && jsonData["rating"].TryGetSingle(out float r) ? r : 5.0f;

        Movie newMovie = new Movie(0, title, year, description, rating);

        Result<Movie> result = await movieService.Create(newMovie);

        if (result.IsValid)
        {
            var movie = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, 
                new { 
                    message = "Movie added successfully",
                    movie = new { 
                        id = movie.Id, 
                        title = movie.Title,
                        year = movie.Year,
                        description = movie.Description,
                        rating = movie.Rating
                    } 
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }

    // GET /api/v1/movies/view?mid=1
    public async Task View(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : 0;

        if (mid == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid movie ID" });
            return;
        }

        Result<Movie> result = await movieService.Read(mid);

        if (result.IsValid)
        {
            Movie movie = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    movie = new { 
                        id = movie.Id, 
                        title = movie.Title,
                        year = movie.Year,
                        description = movie.Description,
                        rating = movie.Rating
                    } 
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotFound, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/movies/edit?mid=1
    public async Task Edit(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var jsonData = (Dictionary<string, JsonElement>?)options["req.json"];
        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : 0;

        if (mid == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid movie ID" });
            return;
        }

        if (jsonData == null)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid JSON data" });
            return;
        }

        string title = jsonData.ContainsKey("title") ? jsonData["title"].GetString() ?? "" : "";
        int year = jsonData.ContainsKey("year") && jsonData["year"].TryGetInt32(out int y) ? y : DateTime.Now.Year;
        string description = jsonData.ContainsKey("description") ? jsonData["description"].GetString() ?? "" : "";
        float rating = jsonData.ContainsKey("rating") && jsonData["rating"].TryGetSingle(out float r) ? r : 5.0f;

        Movie newMovie = new Movie(0, title, year, description, rating);

        Result<Movie> result = await movieService.Update(mid, newMovie);

        if (result.IsValid)
        {
            var movie = result.Value!;
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { 
                    message = "Movie edited successfully",
                    movie = new { 
                        id = movie.Id, 
                        title = movie.Title,
                        year = movie.Year,
                        description = movie.Description,
                        rating = movie.Rating
                    } 
                });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }

    // POST /api/v1/movies/remove?mid=1
    public async Task Remove(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : 0;

        if (mid == 0)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = "Invalid movie ID" });
            return;
        }

        Result<Movie> result = await movieService.Delete(mid);

        if (result.IsValid)
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, 
                new { message = "Movie removed successfully" });
        }
        else
        {
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, 
                new { error = result.Error!.Message });
        }
    }
}
