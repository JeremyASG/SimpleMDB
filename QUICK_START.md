# Quick Start Guide - SimpleMDB API

Get up and running with the SimpleMDB API in under 5 minutes!

## Prerequisites

- .NET 9.0 SDK installed
- MySQL server running (for full functionality) or use the Mock repositories
- Terminal/Command line access

## Step 1: Start the Server

```bash
cd /home/runner/work/SimpleMDB/SimpleMDB
dotnet run
```

You should see:
```
Server listening on...http://127.0.0.1:8080/
```

## Step 2: Test Public Endpoints (No Auth Required)

Open a new terminal and try these commands:

### Get All Actors
```bash
curl http://127.0.0.1:8080/api/v1/actors?page=1&size=5
```

Expected response:
```json
{
  "actors": [
    {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "bio": "Sample actor",
      "rating": 7.5
    }
  ],
  "totalCount": 10,
  "page": 1,
  "size": 5
}
```

### Get All Movies
```bash
curl http://127.0.0.1:8080/api/v1/movies?page=1&size=5
```

## Step 3: Register and Login

### Register a New User
```bash
curl -X POST http://127.0.0.1:8080/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"quickstart","password":"password123"}'
```

Expected response:
```json
{
  "message": "User registered successfully",
  "user": {
    "id": 5,
    "username": "quickstart",
    "role": "user"
  }
}
```

### Login to Get Token
```bash
curl -X POST http://127.0.0.1:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"quickstart","password":"password123"}'
```

Expected response:
```json
{
  "message": "User logged in successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiI1IiwidXNlcm5hbWUiOiJxdWlja3N0YXJ0Iiwicm9sZSI6InVzZXIifQ..."
}
```

**SAVE THIS TOKEN!** You'll need it for authenticated requests.

```bash
# Save the token in a variable (replace with your actual token)
export TOKEN="your_token_here"
```

## Step 4: Create an Actor (Authenticated)

```bash
curl -X POST http://127.0.0.1:8080/api/v1/actors/add \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Brad",
    "lastName": "Pitt",
    "bio": "Award-winning American actor",
    "rating": 8.7
  }'
```

Expected response:
```json
{
  "message": "Actor added successfully",
  "actor": {
    "id": 15,
    "firstName": "Brad",
    "lastName": "Pitt",
    "bio": "Award-winning American actor",
    "rating": 8.7
  }
}
```

**SAVE THE ACTOR ID** for the next step!

## Step 5: Create a Movie (Authenticated)

```bash
curl -X POST http://127.0.0.1:8080/api/v1/movies/add \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Fight Club",
    "year": 1999,
    "description": "An insomniac office worker forms an underground fight club",
    "rating": 8.8
  }'
```

Expected response:
```json
{
  "message": "Movie added successfully",
  "movie": {
    "id": 20,
    "title": "Fight Club",
    "year": 1999,
    "description": "An insomniac office worker forms an underground fight club",
    "rating": 8.8
  }
}
```

**SAVE THE MOVIE ID** for the next step!

## Step 6: Link Actor to Movie

```bash
# Replace 15 and 20 with your actual actor and movie IDs
curl -X POST http://127.0.0.1:8080/api/v1/actors/movies/add \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "actorId": 15,
    "movieId": 20,
    "roleName": "Tyler Durden"
  }'
```

Expected response:
```json
{
  "message": "Movie added to actor successfully",
  "actorMovie": {
    "id": 8,
    "actorId": 15,
    "movieId": 20,
    "roleName": "Tyler Durden"
  }
}
```

## Step 7: Query the Relationship

### Get All Movies for the Actor
```bash
curl http://127.0.0.1:8080/api/v1/actors/movies?aid=15 \
  -H "Authorization: Bearer $TOKEN"
```

Expected response:
```json
{
  "actorMovies": [
    {
      "actorMovieId": 8,
      "actorId": 15,
      "movieId": 20,
      "roleName": "Tyler Durden",
      "movie": {
        "id": 20,
        "title": "Fight Club",
        "year": 1999,
        "description": "An insomniac office worker forms an underground fight club",
        "rating": 8.8
      }
    }
  ],
  "totalCount": 1,
  "page": 1,
  "size": 10
}
```

### Get All Actors for the Movie
```bash
curl http://127.0.0.1:8080/api/v1/movies/actors?mid=20 \
  -H "Authorization: Bearer $TOKEN"
```

## Step 8: Update Data

### Update the Actor
```bash
curl -X POST http://127.0.0.1:8080/api/v1/actors/edit?aid=15 \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Brad",
    "lastName": "Pitt",
    "bio": "Academy Award-winning American actor and film producer",
    "rating": 9.0
  }'
```

### View the Updated Actor
```bash
curl http://127.0.0.1:8080/api/v1/actors/view?aid=15 \
  -H "Authorization: Bearer $TOKEN"
```

## Step 9: Test Error Handling

### Try to Access Admin Endpoint (Should Fail)
```bash
curl http://127.0.0.1:8080/api/v1/users?page=1&size=10 \
  -H "Authorization: Bearer $TOKEN"
```

Expected response:
```json
{
  "error": "Authenticated but not authorized. You must be an administrator to access this resource."
}
```

### Try to Access Endpoint Without Auth (Should Fail)
```bash
curl -X POST http://127.0.0.1:8080/api/v1/actors/add \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Test","lastName":"Actor","bio":"Test","rating":5}'
```

Expected response:
```json
{
  "error": "Invalid or expired token"
}
```

## Step 10: Run Comprehensive Tests

For a complete test of all 24 endpoints:

```bash
./test_api.sh
```

This will:
- Test all CRUD operations
- Test authentication and authorization
- Test error cases
- Clean up test data
- Report results

## What's Next?

Now that you've tested the API manually, you can:

1. **Build a Frontend App**: Create a React, Vue, or Angular app that consumes these APIs
2. **Explore More Endpoints**: Check out `API_ENDPOINTS.md` for all 24 endpoints
3. **Use Postman/Insomnia**: Import the API and test with a GUI tool
4. **Integrate with Mobile**: Build iOS/Android apps using the API
5. **Add More Features**: Extend the API with search, filtering, sorting

## Troubleshooting

### Server Not Starting
- Check if port 8080 is already in use
- Verify MySQL is running (or switch to Mock repositories in App.cs)

### Authentication Errors
- Make sure you're using the correct token
- Token format: `Authorization: Bearer <token>`
- Tokens may expire - login again to get a new token

### JSON Parsing Errors
- Always include `Content-Type: application/json` header for POST requests
- Verify your JSON is valid

## API Cheat Sheet

| Operation | Method | Endpoint | Auth Required |
|-----------|--------|----------|---------------|
| Register | POST | `/api/v1/auth/register` | No |
| Login | POST | `/api/v1/auth/login` | No |
| List Actors | GET | `/api/v1/actors` | No |
| List Movies | GET | `/api/v1/movies` | No |
| Add Actor | POST | `/api/v1/actors/add` | Yes |
| Add Movie | POST | `/api/v1/movies/add` | Yes |
| Link Actor-Movie | POST | `/api/v1/actors/movies/add` | Yes |
| View Actor | GET | `/api/v1/actors/view?aid=X` | Yes |
| Edit Actor | POST | `/api/v1/actors/edit?aid=X` | Yes |
| Delete Actor | POST | `/api/v1/actors/remove?aid=X` | Yes |

For the complete list of all 24 endpoints, see `API_ENDPOINTS.md`.

## Resources

- **API_ENDPOINTS.md** - Complete API documentation
- **CURL_EXAMPLES.md** - All curl commands
- **HOW_TO_USE_API.md** - Detailed usage guide
- **test_api.sh** - Automated testing script
- **IMPLEMENTATION_SUMMARY.md** - Technical details of implementation

Happy coding! 🚀
