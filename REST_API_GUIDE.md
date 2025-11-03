# RESTful API Guide - SimpleMDB

## Overview

SimpleMDB now supports RESTful API endpoints with parametrized routing. You can use either the new REST-style URLs or the legacy query string format - both work!

## What's New

### Parametrized Routing
Routes now support parameters in the path using `{param}` syntax:
- `/api/v1/users/{id}` instead of `/api/v1/users/view?uid={id}`
- `/api/v1/actors/{id}/movies` instead of `/api/v1/actors/movies?aid={id}`

### Proper HTTP Verbs
RESTful endpoints use appropriate HTTP methods:
- **GET** - Retrieve resources
- **POST** - Create new resources
- **PUT** - Update existing resources
- **DELETE** - Remove resources

## RESTful Endpoints

### Users (Admin Only)

```bash
# Get a specific user
GET /api/v1/users/45
curl -H "Authorization: ******" \
  http://127.0.0.1:8080/api/v1/users/45

# Update a user
PUT /api/v1/users/45
curl -X PUT http://127.0.0.1:8080/api/v1/users/45 \
  -H "Authorization: ******" \
  -H "Content-Type: application/json" \
  -d '{"username":"updated","password":"newpass","role":"user"}'

# Delete a user
DELETE /api/v1/users/45
curl -X DELETE http://127.0.0.1:8080/api/v1/users/45 \
  -H "Authorization: ******"
```

### Actors

```bash
# Get a specific actor
GET /api/v1/actors/10
curl -H "Authorization: ******" \
  http://127.0.0.1:8080/api/v1/actors/10

# Update an actor
PUT /api/v1/actors/10
curl -X PUT http://127.0.0.1:8080/api/v1/actors/10 \
  -H "Authorization: ******" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Tom","lastName":"Hanks","bio":"Updated bio","rating":9.5}'

# Delete an actor
DELETE /api/v1/actors/10
curl -X DELETE http://127.0.0.1:8080/api/v1/actors/10 \
  -H "Authorization: ******"

# Get all movies for an actor (nested resource)
GET /api/v1/actors/10/movies?page=1&size=10
curl -H "Authorization: ******" \
  "http://127.0.0.1:8080/api/v1/actors/10/movies?page=1&size=10"
```

### Movies

```bash
# Get a specific movie
GET /api/v1/movies/25
curl -H "Authorization: ******" \
  http://127.0.0.1:8080/api/v1/movies/25

# Update a movie
PUT /api/v1/movies/25
curl -X PUT http://127.0.0.1:8080/api/v1/movies/25 \
  -H "Authorization: ******" \
  -H "Content-Type: application/json" \
  -d '{"title":"Fight Club","year":1999,"description":"Updated","rating":9.0}'

# Delete a movie
DELETE /api/v1/movies/25
curl -X DELETE http://127.0.0.1:8080/api/v1/movies/25 \
  -H "Authorization: ******"

# Get all actors for a movie (nested resource)
GET /api/v1/movies/25/actors?page=1&size=10
curl -H "Authorization: ******" \
  "http://127.0.0.1:8080/api/v1/movies/25/actors?page=1&size=10"
```

### Actor-Movie Relationships

```bash
# Create a relationship (actor-movie as its own resource)
POST /api/v1/actor-movies
curl -X POST http://127.0.0.1:8080/api/v1/actor-movies \
  -H "Authorization: ******" \
  -H "Content-Type: application/json" \
  -d '{"actorId":10,"movieId":25,"roleName":"Lead Role"}'

# Delete a relationship
DELETE /api/v1/actor-movies/15
curl -X DELETE http://127.0.0.1:8080/api/v1/actor-movies/15 \
  -H "Authorization: ******"
```

## Backward Compatibility

All legacy query string endpoints still work! Choose whichever style you prefer:

### Legacy Format (still supported)
```bash
# Get user
GET /api/v1/users/view?uid=45

# Update user
POST /api/v1/users/edit?uid=45

# Delete user
POST /api/v1/users/remove?uid=45
```

### RESTful Format (new)
```bash
# Get user
GET /api/v1/users/45

# Update user
PUT /api/v1/users/45

# Delete user
DELETE /api/v1/users/45
```

## Complete RESTful Route Map

### Authentication (No changes - POST only)
- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/logout`

### Users (Admin Only)
| Action | Legacy | RESTful |
|--------|--------|---------|
| List | `GET /api/v1/users` | Same |
| Create | `POST /api/v1/users/add` | Same |
| Read | `GET /api/v1/users/view?uid={id}` | `GET /api/v1/users/{id}` |
| Update | `POST /api/v1/users/edit?uid={id}` | `PUT /api/v1/users/{id}` |
| Delete | `POST /api/v1/users/remove?uid={id}` | `DELETE /api/v1/users/{id}` |

### Actors
| Action | Legacy | RESTful |
|--------|--------|---------|
| List | `GET /api/v1/actors` | Same |
| Create | `POST /api/v1/actors/add` | Same |
| Read | `GET /api/v1/actors/view?aid={id}` | `GET /api/v1/actors/{id}` |
| Update | `POST /api/v1/actors/edit?aid={id}` | `PUT /api/v1/actors/{id}` |
| Delete | `POST /api/v1/actors/remove?aid={id}` | `DELETE /api/v1/actors/{id}` |
| Movies | `GET /api/v1/actors/movies?aid={id}` | `GET /api/v1/actors/{id}/movies` |

### Movies
| Action | Legacy | RESTful |
|--------|--------|---------|
| List | `GET /api/v1/movies` | Same |
| Create | `POST /api/v1/movies/add` | Same |
| Read | `GET /api/v1/movies/view?mid={id}` | `GET /api/v1/movies/{id}` |
| Update | `POST /api/v1/movies/edit?mid={id}` | `PUT /api/v1/movies/{id}` |
| Delete | `POST /api/v1/movies/remove?mid={id}` | `DELETE /api/v1/movies/{id}` |
| Actors | `GET /api/v1/movies/actors?mid={id}` | `GET /api/v1/movies/{id}/actors` |

### Actor-Movie Relationships
| Action | Legacy | RESTful |
|--------|--------|---------|
| Create | `POST /api/v1/actors/movies/add` | `POST /api/v1/actor-movies` |
| Delete | `POST /api/v1/actors/movies/remove?amid={id}` | `DELETE /api/v1/actor-movies/{id}` |

## REST Principles Applied

### 1. Resource-Based URLs
Resources are identified by nouns in the URL:
- `/users/45` (user resource with ID 45)
- `/movies/25` (movie resource with ID 25)
- `/actors/10/movies` (movies related to actor 10)

### 2. HTTP Verbs for Actions
Actions are expressed through HTTP methods:
- `GET` - Retrieve (safe, idempotent)
- `POST` - Create (not idempotent)
- `PUT` - Update (idempotent)
- `DELETE` - Remove (idempotent)

### 3. Nested Resources
Related resources use nested URLs:
- `/actors/{id}/movies` - Movies for a specific actor
- `/movies/{id}/actors` - Actors for a specific movie

### 4. Status Codes
Appropriate HTTP status codes:
- `200 OK` - Successful GET, PUT, DELETE
- `201 Created` - Successful POST
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Missing/invalid token
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource doesn't exist

### 5. Stateless
Each request contains all necessary information (token in header).

## Implementation Details

### HttpRouter Enhancement
The `HttpRouter` now supports route patterns with `{param}` syntax:
```csharp
router.AddGet("/api/v1/users/{id}", authController.CheckAdmin, usersController.View);
```

### Parameter Extraction
Controllers can extract route parameters using helper methods:
```csharp
int userId = HttpUtils.GetRouteParamAsInt(options, "id");
```

### Backward Compatibility
Controllers check for route parameters first, then fall back to query strings:
```csharp
int uid = HttpUtils.GetRouteParamAsInt(options, "id");
if (uid == 0)
{
    uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 0;
}
```

## Testing RESTful Endpoints

### Example: Complete Movie Workflow

```bash
# 1. Login and get token
TOKEN=$(curl -s -X POST http://127.0.0.1:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass"}' \
  | grep -o '"token":"[^"]*' | sed 's/"token":"//')

# 2. Create a movie (POST still used for creation)
RESPONSE=$(curl -s -X POST http://127.0.0.1:8080/api/v1/movies/add \
  -H "Authorization: ******" \
  -H "Content-Type: application/json" \
  -d '{"title":"Inception","year":2010,"description":"Dream heist","rating":8.8}')

# Extract movie ID
MOVIE_ID=$(echo $RESPONSE | grep -o '"id":[0-9]*' | head -1 | sed 's/"id"://')

# 3. Get the movie (RESTful)
curl -H "Authorization: ******" \
  http://127.0.0.1:8080/api/v1/movies/$MOVIE_ID

# 4. Update the movie (RESTful with PUT)
curl -X PUT http://127.0.0.1:8080/api/v1/movies/$MOVIE_ID \
  -H "Authorization: ******" \
  -H "Content-Type: application/json" \
  -d '{"title":"Inception","year":2010,"description":"Mind-bending thriller","rating":9.0}'

# 5. Delete the movie (RESTful with DELETE)
curl -X DELETE http://127.0.0.1:8080/api/v1/movies/$MOVIE_ID \
  -H "Authorization: ******"
```

## Benefits of RESTful Implementation

✅ **Cleaner URLs**: `/movies/45` is clearer than `/movies/view?mid=45`  
✅ **Standard HTTP Methods**: Use PUT for updates, DELETE for removal  
✅ **Better Semantics**: URLs represent resources, verbs represent actions  
✅ **Framework Compatible**: Works with most HTTP clients and frameworks  
✅ **Backward Compatible**: Legacy endpoints still work  
✅ **Easier to Document**: Standard REST conventions are well-understood  

## Migration Guide

If you're using the legacy API, here's how to migrate:

### Before (Legacy)
```bash
curl -X POST http://127.0.0.1:8080/api/v1/movies/edit?mid=25 \
  -H "Authorization: ******" \
  -H "Content-Type: application/json" \
  -d '{"title":"Updated",...}'
```

### After (RESTful)
```bash
curl -X PUT http://127.0.0.1:8080/api/v1/movies/25 \
  -H "Authorization: ******" \
  -H "Content-Type: application/json" \
  -d '{"title":"Updated",...}'
```

Changes:
1. Replace `POST` with `PUT` for updates, `DELETE` for removal
2. Move ID from query string to path: `/movies/{id}` not `/movies/edit?mid={id}`
3. Keep everything else the same (headers, body, authentication)

## Summary

SimpleMDB now supports true RESTful API design while maintaining full backward compatibility with existing integrations. Choose the style that works best for your use case!

For more examples, see:
- `API_ENDPOINTS.md` - Complete endpoint reference
- `CURL_EXAMPLES.md` - Ready-to-use curl commands
- `QUICK_START.md` - Getting started guide
