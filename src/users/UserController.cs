using System.Collections;
using System.Collections.Specialized;
using System.Net;

namespace SimpleMDB;

public class UserController

{
  private IUSerService userService;

  public UserController(IUSerService userService)
  {
    this.userService = userService;
  }

  //GET /users?page=1&size=5
  public async Task ViewAllGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
  {
    string message = req.QueryString["message"] ?? "";
    int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
    int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

    Result<PagedResult<User>> result = await userService.ReadAll(page, size);

    if (result.IsValid)
    {
      PagedResult<User> pagedResult = result.Value!;

      List<User> users = pagedResult.Values;
      int userCount = pagedResult.Totalcount;
      int pageCount = (int)Math.Ceiling((double)userCount / size);

      string rows = "";

      foreach (var user in users)
      {
        rows += @$"
                <tr>
                   <td>{user.Id}</td>
                   <td>{user.Username}</td>
                   <td>{user.Password}</td>
                   <td>{user.Salt}</td>
                   <td>{user.Role}</td>
                   <td><a href=""/users/view?uid={user.Id}"">View</a></td>
                   <td><a href=""/users/edit?uid={user.Id}"">Edit</a></td>
                   <td><a href=""/users/remove?uid={user.Id}"">Remove</a></td>
                </tr>
                
                ";

      }

      string html = $@"
            <a href=""/users/add"">Add New User</a>
            <table border= ""1"">
             <thead>
               <th>Id</th>
               <th>Username</th>
               <th>Password</th>
               <th>Salt</th>
               <th>Role</th>
               <th>View</th>
               <th>Edit</th>
               <th>Remove</th>
             </thead>
             <tbody>
               {rows}
             </tbody>
            </table>
            <div>
              <a href=""?page=1&size={size}"">First</a>
              <a href=""?page={page - 1}&size={size}"">Prev</a>
              <span>{page} / {pageCount}</span>
              <a href=""?page={page + 1}&size={size}"">Next</a>
              <a href=""?page={pageCount}&size={size}"">Last</a>

            
            </div.
            <div>
              {message}
            </div>
            
            ";

      string content = HtmlTemplates.Base("SimpleMDB", "Users View All Page", html);
      await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);

    }
  }

  //GET /users/add
  public async Task AddGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
  {
    string roles = "";
    string message = req.QueryString["message"] ?? "";

    foreach (var role in Roles.ROLES)
    {
      roles += $@"<option value=""{role}"">{role}</option>";
    }

    string html = $@"
            <form action=""/users/add"" method=""POST"">
              <label for=""username"">Username</label>
              <input id=""username"" name=""username"" type= ""text"" placeholder=""Username"">
              <label for=""password"">Password</label>
              <input id=""password"" name=""password"" type= ""text"" placeholder=""Password"">
              <label for=""role"">Role</label>
              <select id= ""role"" name= ""role"">
                {roles}
              </select>
              <input type=""submit"" value=""Add"">
            </form>
            <div>
              {message}
            </div>
            
            ";
    string content = HtmlTemplates.Base("SimpleMDB", "Users Add Page", html);
    await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);


  }




  //POST /users/add

  public async Task AddPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
  {
    var formData = (NameValueCollection?)options["req.form"] ?? [];

    string username = formData["username"] ?? "";
    string password = formData["password"] ?? "";
    string role = formData["role"] ?? "";

    User newUser = new User(0, username, password, "", role);

    Result<User> result = await userService.Create(newUser);

    if (result.IsValid)
    {
      options["message"] = "User added succesfully!";
      await HttpUtils.Redirect(req, res, options, "/users");

    }
    else
    {
      options["message"] = result.Error!.Message;
      await HttpUtils.Redirect(req, res, options, "/users/add");
    }


  }


  // GET /users/view?uid=1

  public async Task ViewGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
  {
    string message = req.QueryString["message"] ?? "";
    int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;


    Result<User> result = await userService.Read(uid);

    if (result.IsValid)
    {
      User user = result.Value!;

      string html = $@"
            <table border= ""1"">
             <thead>
               <th>Id</th>
               <th>Username</th>
               <th>Password</th>
               <th>Salt</th>
               <th>Role</th>
             </thead>
             <tbody>
              <tr>
                <td>{user.Id}</td>
                <td>{user.Username}</td>
                <td>{user.Password}</td>
                <td>{user.Salt}</td>
                <td>{user.Role}</td>
                </tr>
             </tbody>
            </table>
            <div>
              {message}
            </div>
            
            ";

      string content = HtmlTemplates.Base("SimpleMDB", "Users View  Page", html);
      await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);

    }
  }


  //GET /users/edit?uid=1

  public async Task EditGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
  {
    string roles = "";

    string message = req.QueryString["message"] ?? "";
    int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;


    Result<User> result = await userService.Read(uid);

    if (result.IsValid)
    {
      User user = result.Value!;



      foreach (var role in Roles.ROLES)
      {
        string selected = (role == user.Role) ? " selected" : "";
        roles += $@"<option value=""{role}""{selected}>{role}</option>";
      }

      string html = $@"
            <form action=""/users/edit?uid={uid}"" method=""POST"">
              <label for=""username"">Username</label>
              <input id=""username"" name=""username"" type= ""text"" placeholder=""Username"" value=""{user.Username}"">
              <label for=""password"">Password</label>
              <input id=""password"" name=""password"" type= ""text"" placeholder=""Password"" value=""{user.Password}"">
              <label for=""role"">Role</label>
              <select id= ""role"" name= ""role"">
                {roles}
              </select>
              <input type=""submit"" value=""Edit"">
            </form>
            <div>
              {message}
            </div>
            
            ";
      string content = HtmlTemplates.Base("SimpleMDB", "Users Edit Page", html);
      await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
    }


  }




  //POST /users/edit?uid=1

  public async Task EditPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
  {
    var formData = (NameValueCollection?)options["req.form"] ?? [];
    int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;

    string username = formData["username"] ?? "";
    string password = formData["password"] ?? "";
    string role = formData["role"] ?? "";

    User newUser = new User(0, username, password, "", role);

    Result<User> result = await userService.Update(uid, newUser);

    if (result.IsValid)
    {
      options["message"] = "User edited succesfully!";
      await HttpUtils.Redirect(req, res, options, "/users");

    }
    else
    {
      options["message"] = result.Error!.Message;
      await HttpUtils.Redirect(req, res, options, "/users/edit");
    }


  }

  //GET /users/remove?uid=1
  
    public async Task RemoveGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
  {
    int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;


    Result<User> result = await userService.Delete(uid);

   if (result.IsValid)
    {
      options["message"] = "User removed succesfully!";
      await HttpUtils.Redirect(req, res, options, "/users");

    }
    else
    {
      options["message"] = result.Error!.Message;
      await HttpUtils.Redirect(req, res, options, "/users");
    }

  }

}

    


