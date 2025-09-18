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
    public App()
    {
        string host = "http://127.0.0.1:8080/";
        server = new HttpListener();
        server.Prefixes.Add(host);

        Console.WriteLine("Server listening on..." + host);
        var userRepository = new MockUserRepository();
        var userService = new MockUserService(userRepository);
        var userController = new UserController(userService);
        var authController = new AuthController(userService);

        router = new HttpRouter();
        router.Use(HttpUtils.ReadRequestFormData);

        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/users", userController.ViewAllGet);
        router.AddGet("/users/add", userController.AddGet);
        router.AddPost("/users/add", userController.AddPost);
        router.AddGet("/users/view", userController.ViewGet);
        router.AddGet("/users/edit", userController.EditGet);
        router.AddPost("/users/edit", userController.EditPost);
        router.AddGet("/users/remove", userController.RemoveGet);
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
        DateTime startTime = DateTime.UtcNow;



        try
        {
            res.StatusCode = HttpRouter.RESPONSE_NOT_SENT_YET;

            await router.Handle(req, res, options);
        }

        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
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

            string rid = req.Headers["X-Request-ID"] ?? "0";

            TimeSpan elapsedTime = DateTime.UtcNow - startTime;

            Console.WriteLine($"Request {rid}: {req.HttpMethod} {req.RawUrl} from {req.RemoteEndPoint} --> {res.StatusCode} ({res.ContentLength64} bytes) in {elapsedTime.TotalMilliseconds}ms");

        }

    }
}
