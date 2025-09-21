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
        var userRepository = new MockUserRepository();
        var userService = new MockUserService(userRepository);
        var userController = new UserController(userService);
        var authController = new AuthController(userService);

        var actorRepository = new MockActorRepository();
        var actorService = new MockActorService(actorRepository);
        var actorController = new ActorController(actorService);

        router = new HttpRouter();
        router.Use(HttpUtils.ServeStaticFile);
        router.Use(HttpUtils.ReadRequestFormData);


        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/users", userController.ViewAllUsersGet);
        router.AddGet("/users/add", userController.AddUserGet);
        router.AddPost("/users/add", userController.AddUserPost);
        router.AddGet("/users/view", userController.ViewUserGet);
        router.AddGet("/users/edit", userController.EditUserGet);
        router.AddPost("/users/edit", userController.EditUserPost);
        router.AddPost("/users/remove", userController.RemoveUserPost);
        
        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/actors", actorController.ViewAllActorsGet);
        router.AddGet("/actors/add", actorController.AddActorGet);
        router.AddPost("/actors/add", actorController.AddActorPost);
        router.AddGet("/actors/view", actorController.ViewActorGet);
        router.AddGet("/actors/edit", actorController.EditActorGet);
        router.AddPost("/actors/edit", actorController.EditActorPost);
        router.AddPost("/actors/remove", actorController.RemoveActorPost);
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
