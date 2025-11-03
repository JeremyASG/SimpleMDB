# API Troubleshooting Guide

## Auth Endpoints Not Working

If `/api/v1/register`, `/api/v1/login`, or `/api/v1/logout` are not working, follow these steps:

### 1. Check if the server is running
```bash
# Make sure the server is started
cd /home/runner/work/SimpleMDB/SimpleMDB
dotnet run
```

The server should display:
```
Server listening on...http://127.0.0.1:8080/
```

### 2. Test with verbose output
Use the `-v` flag with curl to see detailed request/response information:

```bash
curl -v -X POST http://127.0.0.1:8080/api/v1/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass","cpassword":"testpass"}'
```

Look for:
- **HTTP status code** (should be 201 for success, 400 for errors)
- **Content-Type header** in the request (must be `application/json`)
- **Response body** (should contain JSON with error details if failed)

### 3. Common Issues and Solutions

#### Issue: "Request body is required" error
**Cause:** The JSON body is not being sent or received properly.

**Solutions:**
- Ensure you're using `-d` flag with curl
- Ensure Content-Type header is set to `application/json`
- Check that JSON is properly formatted (no extra spaces, valid JSON)

**Example:**
```bash
# ✅ Correct
curl -X POST http://127.0.0.1:8080/api/v1/register \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"pass123","cpassword":"pass123"}'

# ❌ Incorrect (missing Content-Type)
curl -X POST http://127.0.0.1:8080/api/v1/register \
  -d '{"username":"test","password":"pass123","cpassword":"pass123"}'
```

#### Issue: "Username and password are required" error
**Cause:** JSON properties are missing or misspelled.

**Solutions:**
- Check property names match exactly: `username`, `password`, `cpassword` (for register)
- Ensure values are not empty strings
- Check JSON structure is correct

**Example:**
```bash
# ✅ Correct property names
{"username":"test","password":"pass123","cpassword":"pass123"}

# ❌ Incorrect (wrong property names)
{"user":"test","pass":"pass123","confirmPass":"pass123"}
```

#### Issue: "Passwords do not match" error (Register only)
**Cause:** The `password` and `cpassword` fields have different values.

**Solution:** Ensure both password fields are identical:
```bash
curl -X POST http://127.0.0.1:8080/api/v1/register \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"samepass","cpassword":"samepass"}'
```

#### Issue: Connection refused or timeout
**Cause:** Server is not running or listening on wrong address.

**Solutions:**
1. Start the server: `dotnet run`
2. Check the URL matches server address (default: `http://127.0.0.1:8080`)
3. Ensure no firewall is blocking the connection

#### Issue: "Invalid JSON format" error
**Cause:** JSON is malformed.

**Solutions:**
- Use single quotes around the JSON string in bash: `'{"key":"value"}'`
- Escape quotes if using double quotes: `"{\"key\":\"value\"}"`
- Validate JSON using online tool like jsonlint.com

**Example:**
```bash
# ✅ Correct (single quotes outside, double quotes inside)
-d '{"username":"test","password":"pass"}'

# ❌ Incorrect (unescaped quotes)
-d "{"username":"test","password":"pass"}"
```

### 4. Test Individual Endpoints

#### Register
```bash
curl -v -X POST http://127.0.0.1:8080/api/v1/register \
  -H "Content-Type: application/json" \
  -d '{"username":"newuser","password":"newpass123","cpassword":"newpass123"}'
```

**Expected Success Response (201):**
```json
{
  "message": "User registered successfully!",
  "user": {
    "id": 1,
    "username": "newuser",
    ...
  }
}
```

**Expected Error Response (400):**
```json
{
  "error": "Passwords do not match."
}
```

#### Login
```bash
curl -v -X POST http://127.0.0.1:8080/api/v1/login \
  -H "Content-Type: application/json" \
  -d '{"username":"newuser","password":"newpass123"}'
```

**Expected Success Response (200):**
```json
{
  "message": "User logged in successfully!",
  "token": "eyJhbGc..."
}
```

**Expected Error Response (401):**
```json
{
  "error": "Invalid username or password"
}
```

#### Logout
```bash
curl -v -X POST http://127.0.0.1:8080/api/v1/logout
```

**Expected Success Response (200):**
```json
{
  "message": "User logged out successfully!"
}
```

### 5. Check Database Connection

If registration/login fail with database errors:

1. Ensure MySQL is running
2. Check connection string in `App.cs`:
   ```csharp
   var userRepository = new MySqlUserRepository("Server=localhost;Database=simplemdb;Uid=root;******;");
   ```
3. Verify database `simplemdb` exists
4. Run database setup scripts in `src/db/`

### 6. Enable Debug Mode

For more detailed error messages, set the environment variable:
```bash
export DEVELOPMENT_MODE=Development
dotnet run
```

### 7. Check Server Logs

The server console will show:
- Each request with method, URL, and status code
- Error messages if exceptions occur
- Response times and sizes

Look for patterns like:
```
Request    0: POST /api/v1/register from 127.0.0.1:xxxxx --> 400 (45 bytes) [application/json] in 5ms error:"..."
```

### 8. Test with Alternative Tools

If curl doesn't work, try:

**Using wget:**
```bash
wget --method=POST \
  --header="Content-Type: application/json" \
  --body-data='{"username":"test","password":"pass123","cpassword":"pass123"}' \
  http://127.0.0.1:8080/api/v1/register \
  -O -
```

**Using Postman or Insomnia:**
1. Set method to POST
2. Set URL to `http://127.0.0.1:8080/api/v1/register`
3. Set header `Content-Type: application/json`
4. Set body to raw JSON:
   ```json
   {
     "username": "test",
     "password": "pass123",
     "cpassword": "pass123"
   }
   ```

### 9. Verify Route Registration

Check that routes are registered in `src/App.cs`:
```csharp
router.AddPost("/api/v1/register", authApiController.RegisterPost);
router.AddPost("/api/v1/login", authApiController.LoginPost);
router.AddPost("/api/v1/logout", authApiController.LogoutPost);
```

### 10. Still Not Working?

If you've tried all the above and it still doesn't work:

1. **Check the build output** for any warnings
2. **Clear and rebuild:**
   ```bash
   dotnet clean
   dotnet build
   ```
3. **Check for port conflicts:**
   ```bash
   # On Linux/Mac
   lsof -i :8080
   
   # On Windows
   netstat -ano | findstr :8080
   ```
4. **Try a different port** by modifying `App.cs`:
   ```csharp
   string host = "http://127.0.0.1:8081/";  // Changed from 8080
   ```

## Quick Validation Test

Run this single command to test all three auth endpoints:
```bash
# Test register
curl -X POST http://127.0.0.1:8080/api/v1/register \
  -H "Content-Type: application/json" \
  -d '{"username":"quicktest","password":"test123","cpassword":"test123"}' && \
echo "" && \
# Test login  
curl -X POST http://127.0.0.1:8080/api/v1/login \
  -H "Content-Type: application/json" \
  -d '{"username":"quicktest","password":"test123"}' && \
echo "" && \
# Test logout
curl -X POST http://127.0.0.1:8080/api/v1/logout
```

If all three return JSON responses, the endpoints are working correctly!
