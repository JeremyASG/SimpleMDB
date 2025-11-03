using System.Collections;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SimpleMDB;

public class App
{
    private HttpListener server;
    private HttpRouter router;
    private int requestId;
    public App()
    {
        string host = "http://127.0.0.1:8080/";
        server = new HttpListener();
        server.Prefixes.Add(host);
        requestId = 0;

        Console.WriteLine("Server listening on..." + host);
        //var userRepository = new MockUserRepository();
        var userRepository = new MySqlUserRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=hero;");
        var userService = new MockUserService(userRepository);
        var userController = new UserController(userService);
        var authController = new AuthController(userService);

        //var actorRepository = new MockActorRepository();
        var actorRepository = new MySqlActorRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=hero;");
        var actorService = new MockActorService(actorRepository);
        var actorController = new ActorController(actorService);

        //var movieRepository = new MockMovieRepository();
        var movieRepository = new MySqlMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=hero;");
        var movieService = new MockMovieService(movieRepository);
        var movieController = new MovieController(movieService);

        //var actorMovieRepository = new MockActorMovieRepository(actorRepository, movieRepository);
        var actorMovieRepository = new MySqlActorMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=hero;");
        var actorMovieService = new MocklActorMovieService(actorMovieRepository);
        var actorMovieController = new ActorMovieController(actorMovieService, actorService, movieService);

        // API Controllers
        var authApiController = new AuthApiController(userService);
        var usersApiController = new UsersApiController(userService);
        var actorsApiController = new ActorsApiController(actorService);
        var moviesApiController = new MoviesApiController(movieService);
        var actorMovieApiController = new ActorMovieApiController(actorMovieService);

        router = new HttpRouter();
        router.Use(HttpUtils.ServeStaticFile);
        router.Use(HttpUtils.ReadRequestFormData);


        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/register", authController.RegisterGet);
        router.AddPost("/register", authController.RegisterPost);
        router.AddGet("/login", authController.LoginGet);
        router.AddPost("/login", authController.LoginPost);
        router.AddPost("/logout", authController.LogoutPost);


        router.AddGet("/users", authController.CheckAdmin, userController.ViewAllUsersGet);
        router.AddGet("/users/add", authController.CheckAdmin, userController.AddUserGet);
        router.AddPost("/users/add", authController.CheckAdmin, userController.AddUserPost);
        router.AddGet("/users/view", authController.CheckAdmin, userController.ViewUserGet);
        router.AddGet("/users/edit", authController.CheckAdmin, userController.EditUserGet);
        router.AddPost("/users/edit", authController.CheckAdmin, userController.EditUserPost);
        router.AddPost("/users/remove", authController.CheckAdmin, userController.RemoveUserPost);


        router.AddGet("/actors", actorController.ViewAllActorsGet);
        router.AddGet("/actors/add", authController.CheckAuth, actorController.AddActorGet);
        router.AddPost("/actors/add", authController.CheckAuth, actorController.AddActorPost);
        router.AddGet("/actors/view", authController.CheckAuth, actorController.ViewActorGet);
        router.AddGet("/actors/edit", authController.CheckAuth, actorController.EditActorGet);
        router.AddPost("/actors/edit", authController.CheckAuth, actorController.EditActorPost);
        router.AddPost("/actors/remove", authController.CheckAuth, actorController.RemoveActorPost);


        router.AddGet("/movies", movieController.ViewAllMoviesGet);
        router.AddGet("/movies/add", authController.CheckAuth,  movieController.AddMovieGet);
        router.AddPost("/movies/add", authController.CheckAuth,  movieController.AddMoviePost);
        router.AddGet("/movies/view", authController.CheckAuth,  movieController.ViewMovieGet);
        router.AddGet("/movies/edit", authController.CheckAuth,  movieController.EditMovieGet);
        router.AddPost("/movies/edit", authController.CheckAuth,  movieController.EditMoviePost);
        router.AddPost("/movies/remove", authController.CheckAuth,  movieController.RemoveMoviePost);

        router.AddGet("/actors/movies", authController.CheckAuth,  actorMovieController.ViewAllMoviesByActor);
        router.AddGet("/actors/movies/add", authController.CheckAuth,  actorMovieController.AddMoviesByActorGet);
        router.AddPost("/actors/movies/add", authController.CheckAuth,  actorMovieController.AddMoviesByActorPost);
        router.AddPost("/actors/movies/remove", authController.CheckAuth,  actorMovieController.RemoveMoviesByActorPost);

        router.AddGet("/movies/actors", authController.CheckAuth,  actorMovieController.ViewAllActorsByMovie);
        router.AddGet("/movies/actors/add", authController.CheckAuth,  actorMovieController.AddActorsByMovieGet);
        router.AddPost("/movies/actors/add", authController.CheckAuth,  actorMovieController.AddActorsByMoviePost);
        router.AddPost("/movies/actors/remove", authController.CheckAuth,  actorMovieController.RemoveActorsByMoviePost);
       
        // API Routes - /api/v1
        
        // Auth API endpoints
        router.AddPost("/api/v1/auth/register", authApiController.RegisterPost);
        router.AddPost("/api/v1/auth/login", authApiController.LoginPost);
        router.AddPost("/api/v1/auth/logout", authApiController.LogoutPost);

        // Users API endpoints
        router.AddGet("/api/v1/users", authApiController.CheckAdmin, usersApiController.GetAll);
        router.AddPost("/api/v1/users/add", authApiController.CheckAdmin, usersApiController.Add);
        router.AddGet("/api/v1/users/view", authApiController.CheckAdmin, usersApiController.View);
        router.AddPost("/api/v1/users/edit", authApiController.CheckAdmin, usersApiController.Edit);
        router.AddPost("/api/v1/users/remove", authApiController.CheckAdmin, usersApiController.Remove);

        // Actors API endpoints
        router.AddGet("/api/v1/actors", actorsApiController.GetAll);
        router.AddPost("/api/v1/actors/add", authApiController.CheckAuth, actorsApiController.Add);
        router.AddGet("/api/v1/actors/view", authApiController.CheckAuth, actorsApiController.View);
        router.AddPost("/api/v1/actors/edit", authApiController.CheckAuth, actorsApiController.Edit);
        router.AddPost("/api/v1/actors/remove", authApiController.CheckAuth, actorsApiController.Remove);

        // Movies API endpoints
        router.AddGet("/api/v1/movies", moviesApiController.GetAll);
        router.AddPost("/api/v1/movies/add", authApiController.CheckAuth, moviesApiController.Add);
        router.AddGet("/api/v1/movies/view", authApiController.CheckAuth, moviesApiController.View);
        router.AddPost("/api/v1/movies/edit", authApiController.CheckAuth, moviesApiController.Edit);
        router.AddPost("/api/v1/movies/remove", authApiController.CheckAuth, moviesApiController.Remove);

        // Actor-Movie API endpoints
        router.AddGet("/api/v1/actors/movies", authApiController.CheckAuth, actorMovieApiController.GetMoviesByActor);
        router.AddPost("/api/v1/actors/movies/add", authApiController.CheckAuth, actorMovieApiController.AddMovieToActor);
        router.AddPost("/api/v1/actors/movies/remove", authApiController.CheckAuth, actorMovieApiController.RemoveMovieFromActor);

        router.AddGet("/api/v1/movies/actors", authApiController.CheckAuth, actorMovieApiController.GetActorsByMovie);
        router.AddPost("/api/v1/movies/actors/add", authApiController.CheckAuth, actorMovieApiController.AddActorToMovie);
        router.AddPost("/api/v1/movies/actors/remove", authApiController.CheckAuth, actorMovieApiController.RemoveActorFromMovie);

    }

    public async Task Start()
    {
        server.Start();

        while (server.IsListening)
        {
            var ctx = await server.GetContextAsync();
            _ = HandleContextAsync(ctx);
        }
    }

    public void Stop()
    {
        server.Stop();
        server.Close();
    }

    private async Task HandleContextAsync(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var res = ctx.Response;
        var options = new Hashtable();
        var rid = req.Headers["X-Request-ID"] ?? requestId.ToString().PadLeft(6, ' ');
        var method = req.HttpMethod;
        var RawUrl = req.RawUrl;
        var removeEndPoint = req.RemoteEndPoint;


        res.StatusCode = HttpRouter.RESPONSE_NOT_SENT_YET;
        DateTime startTime = DateTime.UtcNow;
        requestId++;
        string error = "";


        try
        {
            await router.Handle(req, res, options);
        }

        catch (Exception ex)
        {
            error = ex.ToString();
            if (res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                res.Close();

                if (Environment.GetEnvironmentVariable("DEVELOMENT_MOVE") != "Production")
                {
                    string html = HtmlTemplates.Base("SimpleMDB", "Error Page", ex.ToString());
                    await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError, html);
                }
                else
                {
                    string html = HtmlTemplates.Base("SimpleMDB", "Error Page", "An error occurred.");
                    await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError, html);
                }
            }

        }
        finally
        {
            if (res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
            {
                string html = HtmlTemplates.Base("SimpleMDB", "Not Found Page ", "Resource was not found.");
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.NotFound, html);
            }

            

            TimeSpan elapsedTime = DateTime.UtcNow - startTime;

            Console.WriteLine($"Request {rid}: {method} {RawUrl} from {removeEndPoint} --> {res.StatusCode} ({res.ContentLength64} bytes) [{res.ContentType}] in {elapsedTime.TotalMilliseconds}ms error:\"{error}\"");

        }

    }
}
