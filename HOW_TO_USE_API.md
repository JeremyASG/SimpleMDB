# How to Use the SimpleMDB API

This guide explains how to use the newly implemented API endpoints in SimpleMDB.

## Overview

SimpleMDB now includes 24 RESTful API endpoints that return JSON data instead of HTML. These endpoints allow you to perform all CRUD (Create, Read, Update, Delete) operations on:

- **Authentication** (register, login, logout)
- **Users** (admin-only operations)
- **Actors** (create, read, update, delete)
- **Movies** (create, read, update, delete)
- **Actor-Movie relationships** (link actors to movies)

## Getting Started

### 1. Start the Server

First, build and run the SimpleMDB server:

```bash
cd /home/runner/work/SimpleMDB/SimpleMDB
dotnet build
dotnet run
```

The server will start on `http://127.0.0.1:8080/`

### 2. Test Basic Connectivity

Open a new terminal and test that the API is accessible:

```bash
curl http://127.0.0.1:8080/api/v1/actors
```

You should get a JSON response with a list of actors.

## API Architecture

### Request Format

All API endpoints:
- Accept requests at the base path `/api/v1`
- Use JSON for request bodies (POST operations)
- Require `Content-Type: application/json` header for POST requests
- Return JSON responses

### Response Format

All successful responses return JSON:
```json
{
  "message": "Operation successful",
  "data": { ... }
}
```

All error responses return:
```json
{
  "error": "Error message"
}
```

### Authentication

Most endpoints require authentication via Bearer token:

1. **Register** or **Login** to get a token
2. Include the token in subsequent requests:
   ```
   Authorization: Bearer <your-token>
   ```

### Authorization Levels

- **Public**: No authentication required (GET actors, GET movies)
- **Authenticated**: Requires valid token (add/edit/delete actors and movies)
- **Admin Only**: Requires admin role (all user management operations)

## Common Workflows

### Workflow 1: Register and Login

```bash
# Step 1: Register a new user
curl -X POST http://127.0.0.1:8080/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"myuser","password":"mypassword"}'

# Step 2: Login to get a token
curl -X POST http://127.0.0.1:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"myuser","password":"mypassword"}'

# Response will include:
# {
#   "message": "User logged in successfully",
#   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
# }

# Step 3: Save the token for subsequent requests
TOKEN="your_token_here"
```

### Workflow 2: Create and Manage Actors

```bash
# Create an actor
curl -X POST http://127.0.0.1:8080/api/v1/actors/add \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Leonardo",
    "lastName": "DiCaprio",
    "bio": "American actor and film producer",
    "rating": 9.2
  }'

# View the actor (assuming ID is 1)
curl http://127.0.0.1:8080/api/v1/actors/view?aid=1 \
  -H "Authorization: Bearer $TOKEN"

# Update the actor
curl -X POST http://127.0.0.1:8080/api/v1/actors/edit?aid=1 \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Leonardo",
    "lastName": "DiCaprio",
    "bio": "Academy Award-winning actor",
    "rating": 9.5
  }'

# Get all actors with pagination
curl http://127.0.0.1:8080/api/v1/actors?page=1&size=10
```

### Workflow 3: Create and Manage Movies

```bash
# Create a movie
curl -X POST http://127.0.0.1:8080/api/v1/movies/add \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Inception",
    "year": 2010,
    "description": "A thief who steals corporate secrets",
    "rating": 8.8
  }'

# View the movie (assuming ID is 1)
curl http://127.0.0.1:8080/api/v1/movies/view?mid=1 \
  -H "Authorization: Bearer $TOKEN"

# Update the movie
curl -X POST http://127.0.0.1:8080/api/v1/movies/edit?mid=1 \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Inception",
    "year": 2010,
    "description": "A mind-bending thriller about dream invasion",
    "rating": 9.0
  }'

# Get all movies
curl http://127.0.0.1:8080/api/v1/movies?page=1&size=10
```

### Workflow 4: Link Actors to Movies

```bash
# Add an actor to a movie
curl -X POST http://127.0.0.1:8080/api/v1/actors/movies/add \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "actorId": 1,
    "movieId": 1,
    "roleName": "Dom Cobb"
  }'

# Get all movies for an actor
curl http://127.0.0.1:8080/api/v1/actors/movies?aid=1&page=1&size=10 \
  -H "Authorization: Bearer $TOKEN"

# Get all actors for a movie
curl http://127.0.0.1:8080/api/v1/movies/actors?mid=1&page=1&size=10 \
  -H "Authorization: Bearer $TOKEN"

# Remove the relationship (assuming actor-movie ID is 1)
curl -X POST http://127.0.0.1:8080/api/v1/actors/movies/remove?amid=1 \
  -H "Authorization: Bearer $TOKEN"
```

## Testing All Endpoints

We've provided a comprehensive test script that tests all 24 endpoints:

```bash
# Make the script executable (if not already)
chmod +x test_api.sh

# Run the test script
./test_api.sh
```

The script will:
1. Register a test user
2. Login and capture the authentication token
3. Test all CRUD operations on actors, movies, and relationships
4. Test error cases
5. Clean up test data

## API Endpoint Summary

Here's a quick reference of all 24 endpoints:

### Authentication (3)
- POST `/api/v1/auth/register` - Register new user
- POST `/api/v1/auth/login` - Login and get token
- POST `/api/v1/auth/logout` - Logout

### Users (5) - Admin Only
- GET `/api/v1/users?page=1&size=10` - Get all users
- POST `/api/v1/users/add` - Add user
- GET `/api/v1/users/view?uid=1` - View user
- POST `/api/v1/users/edit?uid=1` - Edit user
- POST `/api/v1/users/remove?uid=1` - Remove user

### Actors (5)
- GET `/api/v1/actors?page=1&size=10` - Get all actors (public)
- POST `/api/v1/actors/add` - Add actor (auth required)
- GET `/api/v1/actors/view?aid=1` - View actor (auth required)
- POST `/api/v1/actors/edit?aid=1` - Edit actor (auth required)
- POST `/api/v1/actors/remove?aid=1` - Remove actor (auth required)

### Movies (5)
- GET `/api/v1/movies?page=1&size=10` - Get all movies (public)
- POST `/api/v1/movies/add` - Add movie (auth required)
- GET `/api/v1/movies/view?mid=1` - View movie (auth required)
- POST `/api/v1/movies/edit?mid=1` - Edit movie (auth required)
- POST `/api/v1/movies/remove?mid=1` - Remove movie (auth required)

### Actor-Movie Relationships (6)
- GET `/api/v1/actors/movies?aid=1&page=1&size=10` - Get movies by actor
- POST `/api/v1/actors/movies/add` - Add movie to actor
- POST `/api/v1/actors/movies/remove?amid=1` - Remove movie from actor
- GET `/api/v1/movies/actors?mid=1&page=1&size=10` - Get actors by movie
- POST `/api/v1/movies/actors/add` - Add actor to movie
- POST `/api/v1/movies/actors/remove?amid=1` - Remove actor from movie

## Additional Resources

- **API_ENDPOINTS.md** - Detailed documentation of all endpoints with request/response examples
- **CURL_EXAMPLES.md** - Ready-to-use cURL commands for each endpoint
- **test_api.sh** - Automated test script for all endpoints

## Tips and Best Practices

1. **Token Management**: Store your authentication token securely. Tokens are required for most operations.

2. **Pagination**: Use the `page` and `size` query parameters to paginate large result sets.

3. **Error Handling**: Always check the HTTP status code. Common codes:
   - 200: Success
   - 201: Created
   - 400: Bad Request (invalid data)
   - 401: Unauthorized (missing/invalid token)
   - 403: Forbidden (not authorized)
   - 404: Not Found
   - 500: Internal Server Error

4. **JSON Formatting**: Use tools like `jq` to format JSON responses:
   ```bash
   curl http://127.0.0.1:8080/api/v1/actors | jq
   ```

5. **Testing**: Use tools like Postman, Insomnia, or the provided test script for comprehensive testing.

## Troubleshooting

### "Invalid JSON data" Error
- Ensure you're including the `Content-Type: application/json` header
- Verify your JSON is valid (use a JSON validator)

### "Unauthorized" Error
- Make sure you've included the `Authorization: Bearer <token>` header
- Verify your token hasn't expired

### "Forbidden" Error
- Check that your user has the required role (e.g., admin for user management)

### Connection Refused
- Ensure the server is running on port 8080
- Check that no firewall is blocking the connection

## Next Steps

Now that you have working API endpoints, you can:

1. Build a frontend application (React, Vue, Angular) that consumes these APIs
2. Create a mobile app that uses the APIs
3. Integrate with other services or systems
4. Add more features like search, filtering, and sorting

The existing HTML endpoints continue to work, so you can gradually migrate to the API-based approach.
