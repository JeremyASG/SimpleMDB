# cURL Examples for SimpleMDB API

This document provides example cURL commands for testing all 24 API endpoints.

## Setup

Set the base URL and store your token in a variable:

```bash
BASE_URL="http://127.0.0.1:8080/api/v1"
TOKEN=""
```

## Authentication Endpoints

### 1. Register
```bash
curl -X POST "$BASE_URL/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass"}'
```

### 2. Login (save the token)
```bash
curl -X POST "$BASE_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass"}'

# Extract and save the token
TOKEN="your_token_here"
```

### 3. Logout
```bash
curl -X POST "$BASE_URL/auth/logout" \
  -H "Authorization: Bearer $TOKEN"
```

## Actor Endpoints

### 4. Get All Actors (public)
```bash
curl "$BASE_URL/actors?page=1&size=10"
```

### 5. Add Actor
```bash
curl -X POST "$BASE_URL/actors/add" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Tom","lastName":"Hanks","bio":"Award-winning actor","rating":9.5}'
```

### 6. View Actor
```bash
curl "$BASE_URL/actors/view?aid=1" \
  -H "Authorization: Bearer $TOKEN"
```

### 7. Edit Actor
```bash
curl -X POST "$BASE_URL/actors/edit?aid=1" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Thomas","lastName":"Hanks","bio":"Updated bio","rating":9.8}'
```

### 8. Remove Actor
```bash
curl -X POST "$BASE_URL/actors/remove?aid=1" \
  -H "Authorization: Bearer $TOKEN"
```

## Movie Endpoints

### 9. Get All Movies (public)
```bash
curl "$BASE_URL/movies?page=1&size=10"
```

### 10. Add Movie
```bash
curl -X POST "$BASE_URL/movies/add" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Forrest Gump","year":1994,"description":"Life story","rating":8.8}'
```

### 11. View Movie
```bash
curl "$BASE_URL/movies/view?mid=1" \
  -H "Authorization: Bearer $TOKEN"
```

### 12. Edit Movie
```bash
curl -X POST "$BASE_URL/movies/edit?mid=1" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Forrest Gump","year":1994,"description":"Updated description","rating":9.0}'
```

### 13. Remove Movie
```bash
curl -X POST "$BASE_URL/movies/remove?mid=1" \
  -H "Authorization: Bearer $TOKEN"
```

## Actor-Movie Relationship Endpoints

### 14. Get Movies by Actor
```bash
curl "$BASE_URL/actors/movies?aid=1&page=1&size=10" \
  -H "Authorization: Bearer $TOKEN"
```

### 15. Add Movie to Actor
```bash
curl -X POST "$BASE_URL/actors/movies/add" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"actorId":1,"movieId":1,"roleName":"Main Character"}'
```

### 16. Remove Movie from Actor
```bash
curl -X POST "$BASE_URL/actors/movies/remove?amid=1" \
  -H "Authorization: Bearer $TOKEN"
```

### 17. Get Actors by Movie
```bash
curl "$BASE_URL/movies/actors?mid=1&page=1&size=10" \
  -H "Authorization: Bearer $TOKEN"
```

### 18. Add Actor to Movie
```bash
curl -X POST "$BASE_URL/movies/actors/add" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"actorId":1,"movieId":1,"roleName":"Lead Role"}'
```

### 19. Remove Actor from Movie
```bash
curl -X POST "$BASE_URL/movies/actors/remove?amid=1" \
  -H "Authorization: Bearer $TOKEN"
```

## User Management Endpoints (Admin Only)

### 20. Get All Users
```bash
curl "$BASE_URL/users?page=1&size=10" \
  -H "Authorization: Bearer $TOKEN"
```

### 21. Add User
```bash
curl -X POST "$BASE_URL/users/add" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"username":"newuser","password":"newpass","role":"user"}'
```

### 22. View User
```bash
curl "$BASE_URL/users/view?uid=1" \
  -H "Authorization: Bearer $TOKEN"
```

### 23. Edit User
```bash
curl -X POST "$BASE_URL/users/edit?uid=1" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"username":"updateduser","password":"newpass","role":"admin"}'
```

### 24. Remove User
```bash
curl -X POST "$BASE_URL/users/remove?uid=1" \
  -H "Authorization: Bearer $TOKEN"
```

## Testing Workflow

Here's a complete workflow to test the API:

```bash
# 1. Register a user
curl -X POST "$BASE_URL/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass"}'

# 2. Login and save token
RESPONSE=$(curl -X POST "$BASE_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass"}')
TOKEN=$(echo $RESPONSE | grep -o '"token":"[^"]*' | sed 's/"token":"//')

# 3. Create an actor
curl -X POST "$BASE_URL/actors/add" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Tom","lastName":"Hanks","bio":"Actor","rating":9.0}'

# 4. Create a movie
curl -X POST "$BASE_URL/movies/add" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Forrest Gump","year":1994,"description":"Drama","rating":8.8}'

# 5. Link actor to movie
curl -X POST "$BASE_URL/actors/movies/add" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"actorId":1,"movieId":1,"roleName":"Forrest Gump"}'

# 6. Get all movies for the actor
curl "$BASE_URL/actors/movies?aid=1&page=1&size=10" \
  -H "Authorization: Bearer $TOKEN"

# 7. Logout
curl -X POST "$BASE_URL/auth/logout" \
  -H "Authorization: Bearer $TOKEN"
```

## Notes

- All POST requests require `Content-Type: application/json` header
- Authentication is required for most endpoints (see API_ENDPOINTS.md)
- Admin role is required for all user management endpoints
- Use the `-i` flag to see response headers: `curl -i ...`
- Use the `-v` flag for verbose output: `curl -v ...`
- Add `| jq` at the end to pretty-print JSON responses (requires jq): `curl ... | jq`
