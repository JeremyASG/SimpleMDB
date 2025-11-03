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
        var actorMovieApiController = new ActorMovieApiController(actorMovieService, actorService, movieService);

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

        // API Routes - Auth (3 endpoints)
        router.AddPost("/api/v1/register", authApiController.RegisterPost);
        router.AddPost("/api/v1/login", authApiController.LoginPost);
        router.AddPost("/api/v1/logout", authApiController.LogoutPost);

        // API Routes - Users (5 endpoints)
        router.AddGet("/api/v1/users", authController.CheckAdmin, usersApiController.ViewAllUsersGet);
        router.AddPost("/api/v1/users/add", authController.CheckAdmin, usersApiController.AddUserPost);
        router.AddGet("/api/v1/users/view", authController.CheckAdmin, usersApiController.ViewUserGet);
        router.AddPost("/api/v1/users/edit", authController.CheckAdmin, usersApiController.EditUserPost);
        router.AddPost("/api/v1/users/remove", authController.CheckAdmin, usersApiController.RemoveUserPost);

        // API Routes - Actors (5 endpoints)
        router.AddGet("/api/v1/actors", actorsApiController.ViewAllActorsGet);
        router.AddPost("/api/v1/actors/add", authController.CheckAuth, actorsApiController.AddActorPost);
        router.AddGet("/api/v1/actors/view", authController.CheckAuth, actorsApiController.ViewActorGet);
        router.AddPost("/api/v1/actors/edit", authController.CheckAuth, actorsApiController.EditActorPost);
        router.AddPost("/api/v1/actors/remove", authController.CheckAuth, actorsApiController.RemoveActorPost);

        // API Routes - Movies (5 endpoints)
        router.AddGet("/api/v1/movies", moviesApiController.ViewAllMoviesGet);
        router.AddPost("/api/v1/movies/add", authController.CheckAuth, moviesApiController.AddMoviePost);
        router.AddGet("/api/v1/movies/view", authController.CheckAuth, moviesApiController.ViewMovieGet);
        router.AddPost("/api/v1/movies/edit", authController.CheckAuth, moviesApiController.EditMoviePost);
        router.AddPost("/api/v1/movies/remove", authController.CheckAuth, moviesApiController.RemoveMoviePost);

        // API Routes - Actor-Movies (6 endpoints)
        router.AddGet("/api/v1/actors/movies", authController.CheckAuth, actorMovieApiController.ViewAllMoviesByActorGet);
        router.AddPost("/api/v1/actors/movies/add", authController.CheckAuth, actorMovieApiController.AddMoviesByActorPost);
        router.AddPost("/api/v1/actors/movies/remove", authController.CheckAuth, actorMovieApiController.RemoveMoviesByActorPost);
        router.AddGet("/api/v1/movies/actors", authController.CheckAuth, actorMovieApiController.ViewAllActorsByMovieGet);
        router.AddPost("/api/v1/movies/actors/add", authController.CheckAuth, actorMovieApiController.AddActorsByMoviePost);
        router.AddPost("/api/v1/movies/actors/remove", authController.CheckAuth, actorMovieApiController.RemoveActorsByMoviePost);
       
        
        

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
