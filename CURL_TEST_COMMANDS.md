# cURL Test Commands for SimpleMDB API

This document contains all cURL commands to test the 24 API endpoints.

## Prerequisites
- Server running: `dotnet run` from the project directory
- Base URL: `http://127.0.0.1:8080`

## Quick Start
Run the automated test script:
```bash
./test-api-endpoints.sh
```

---

## 1. AUTH ENDPOINTS (3)

### 1.1 Register a New User
```bash
curl -X POST http://127.0.0.1:8080/api/v1/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "testpass123",
    "cpassword": "testpass123"
  }'
```

**Expected Response:**
```json
{
  "message": "User registered successfully!",
  "user": {
    "id": 1,
    "username": "testuser",
    "password": "hashed_password",
    "salt": "salt_value",
    "role": ""
  }
}
```

### 1.2 Login
```bash
curl -X POST http://127.0.0.1:8080/api/v1/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "testpass123"
  }'
```

**Expected Response:**
```json
{
  "message": "User logged in successfully!",
  "token": "your_jwt_token_here"
}
```

**Save the token for authenticated requests:**
```bash
TOKEN="paste_token_here"
```

### 1.3 Logout
```bash
curl -X POST http://127.0.0.1:8080/api/v1/logout
```

**Expected Response:**
```json
{
  "message": "User logged out successfully!"
}
```

---

## 2. USERS ENDPOINTS (5) - Requires Admin Role

### 2.1 List All Users
```bash
curl -X GET "http://127.0.0.1:8080/api/v1/users?page=1&size=10" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "users": [...],
  "totalCount": 5,
  "page": 1,
  "size": 10
}
```

### 2.2 Add User
```bash
curl -X POST http://127.0.0.1:8080/api/v1/users/add \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "username": "newuser",
    "password": "newpass123",
    "role": "user"
  }'
```

**Expected Response:**
```json
{
  "id": 2,
  "username": "newuser",
  "password": "hashed_password",
  "salt": "salt_value",
  "role": "user"
}
```

### 2.3 View User
```bash
curl -X GET "http://127.0.0.1:8080/api/v1/users/view?uid=1" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "id": 1,
  "username": "testuser",
  "password": "hashed_password",
  "salt": "salt_value",
  "role": "admin"
}
```

### 2.4 Edit User
```bash
curl -X POST "http://127.0.0.1:8080/api/v1/users/edit?uid=1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "username": "updateduser",
    "password": "newpass456",
    "role": "admin"
  }'
```

**Expected Response:**
```json
{
  "id": 1,
  "username": "updateduser",
  "password": "new_hashed_password",
  "salt": "new_salt",
  "role": "admin"
}
```

### 2.5 Remove User
```bash
curl -X POST "http://127.0.0.1:8080/api/v1/users/remove?uid=2" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "message": "User removed successfully",
  "user": {
    "id": 2,
    "username": "newuser",
    ...
  }
}
```

---

## 3. ACTORS ENDPOINTS (5)

### 3.1 List All Actors (No Auth Required)
```bash
curl -X GET "http://127.0.0.1:8080/api/v1/actors?page=1&size=10"
```

**Expected Response:**
```json
{
  "actors": [...],
  "totalCount": 15,
  "page": 1,
  "size": 10
}
```

### 3.2 Add Actor (Requires Auth)
```bash
curl -X POST http://127.0.0.1:8080/api/v1/actors/add \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "firstname": "Tom",
    "lastname": "Hanks",
    "bio": "Award-winning American actor and filmmaker",
    "rating": 9.5
  }'
```

**Expected Response:**
```json
{
  "id": 1,
  "firstName": "Tom",
  "lastName": "Hanks",
  "bio": "Award-winning American actor and filmmaker",
  "rating": 9.5
}
```

### 3.3 View Actor (Requires Auth)
```bash
curl -X GET "http://127.0.0.1:8080/api/v1/actors/view?aid=1" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "id": 1,
  "firstName": "Tom",
  "lastName": "Hanks",
  "bio": "Award-winning American actor and filmmaker",
  "rating": 9.5
}
```

### 3.4 Edit Actor (Requires Auth)
```bash
curl -X POST "http://127.0.0.1:8080/api/v1/actors/edit?aid=1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "firstname": "Thomas",
    "lastname": "Hanks",
    "bio": "Updated biography",
    "rating": 9.8
  }'
```

**Expected Response:**
```json
{
  "id": 1,
  "firstName": "Thomas",
  "lastName": "Hanks",
  "bio": "Updated biography",
  "rating": 9.8
}
```

### 3.5 Remove Actor (Requires Auth)
```bash
curl -X POST "http://127.0.0.1:8080/api/v1/actors/remove?aid=1" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "message": "Actor removed successfully",
  "actor": {
    "id": 1,
    "firstName": "Tom",
    "lastName": "Hanks",
    ...
  }
}
```

---

## 4. MOVIES ENDPOINTS (5)

### 4.1 List All Movies (No Auth Required)
```bash
curl -X GET "http://127.0.0.1:8080/api/v1/movies?page=1&size=10"
```

**Expected Response:**
```json
{
  "movies": [...],
  "totalCount": 20,
  "page": 1,
  "size": 10
}
```

### 4.2 Add Movie (Requires Auth)
```bash
curl -X POST http://127.0.0.1:8080/api/v1/movies/add \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "title": "The Matrix",
    "year": 1999,
    "description": "A computer hacker learns about the true nature of reality",
    "rating": 9.0
  }'
```

**Expected Response:**
```json
{
  "id": 1,
  "title": "The Matrix",
  "year": 1999,
  "description": "A computer hacker learns about the true nature of reality",
  "rating": 9.0
}
```

### 4.3 View Movie (Requires Auth)
```bash
curl -X GET "http://127.0.0.1:8080/api/v1/movies/view?mid=1" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "id": 1,
  "title": "The Matrix",
  "year": 1999,
  "description": "A computer hacker learns about the true nature of reality",
  "rating": 9.0
}
```

### 4.4 Edit Movie (Requires Auth)
```bash
curl -X POST "http://127.0.0.1:8080/api/v1/movies/edit?mid=1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "title": "The Matrix Reloaded",
    "year": 2003,
    "description": "Neo and the rebel leaders fight back against the machines",
    "rating": 8.5
  }'
```

**Expected Response:**
```json
{
  "id": 1,
  "title": "The Matrix Reloaded",
  "year": 2003,
  "description": "Neo and the rebel leaders fight back against the machines",
  "rating": 8.5
}
```

### 4.5 Remove Movie (Requires Auth)
```bash
curl -X POST "http://127.0.0.1:8080/api/v1/movies/remove?mid=1" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "message": "Movie removed successfully",
  "movie": {
    "id": 1,
    "title": "The Matrix",
    ...
  }
}
```

---

## 5. ACTOR-MOVIES ENDPOINTS (6) - All Require Auth

### 5.1 Get Movies by Actor
```bash
curl -X GET "http://127.0.0.1:8080/api/v1/actors/movies?aid=1&page=1&size=10" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "actor": {
    "id": 1,
    "firstName": "Tom",
    "lastName": "Hanks",
    ...
  },
  "movies": [
    {
      "actorMovieId": 1,
      "roleName": "Forrest Gump",
      "movie": {
        "id": 1,
        "title": "Forrest Gump",
        ...
      }
    }
  ],
  "totalCount": 5,
  "page": 1,
  "size": 10
}
```

### 5.2 Add Movie to Actor
```bash
curl -X POST http://127.0.0.1:8080/api/v1/actors/movies/add \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "aid": 1,
    "mid": 1,
    "rolename": "Neo"
  }'
```

**Expected Response:**
```json
{
  "id": 1,
  "actorId": 1,
  "movieId": 1,
  "roleName": "Neo"
}
```

### 5.3 Remove Movie from Actor
```bash
curl -X POST "http://127.0.0.1:8080/api/v1/actors/movies/remove?amid=1" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "message": "Actor-movie relationship removed successfully",
  "actorMovie": {
    "id": 1,
    "actorId": 1,
    "movieId": 1,
    "roleName": "Neo"
  }
}
```

### 5.4 Get Actors by Movie
```bash
curl -X GET "http://127.0.0.1:8080/api/v1/movies/actors?mid=1&page=1&size=10" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "movie": {
    "id": 1,
    "title": "The Matrix",
    ...
  },
  "actors": [
    {
      "actorMovieId": 1,
      "roleName": "Neo",
      "actor": {
        "id": 1,
        "firstName": "Keanu",
        "lastName": "Reeves",
        ...
      }
    }
  ],
  "totalCount": 3,
  "page": 1,
  "size": 10
}
```

### 5.5 Add Actor to Movie
```bash
curl -X POST http://127.0.0.1:8080/api/v1/movies/actors/add \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "mid": 1,
    "aid": 2,
    "rolename": "Trinity"
  }'
```

**Expected Response:**
```json
{
  "id": 2,
  "actorId": 2,
  "movieId": 1,
  "roleName": "Trinity"
}
```

### 5.6 Remove Actor from Movie
```bash
curl -X POST "http://127.0.0.1:8080/api/v1/movies/actors/remove?amid=2" \
  -H "Authorization: Bearer $TOKEN"
```

**Expected Response:**
```json
{
  "message": "Actor-movie relationship removed successfully",
  "actorMovie": {
    "id": 2,
    "actorId": 2,
    "movieId": 1,
    "roleName": "Trinity"
  }
}
```

---

## Error Responses

All endpoints return consistent error responses:

### 400 Bad Request
```json
{
  "error": "Invalid JSON format"
}
```

### 401 Unauthorized
```json
{
  "error": "Authentication required"
}
```

### 404 Not Found
```json
{
  "error": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "error": "Internal server error message"
}
```

---

## Testing Tips

1. **Start the server first:**
   ```bash
   cd /home/runner/work/SimpleMDB/SimpleMDB
   dotnet run
   ```

2. **Use the automated test script:**
   ```bash
   ./test-api-endpoints.sh
   ```

3. **Save your token after login:**
   ```bash
   TOKEN=$(curl -s -X POST http://127.0.0.1:8080/api/v1/login \
     -H "Content-Type: application/json" \
     -d '{"username":"testuser","password":"testpass123"}' \
     | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
   echo $TOKEN
   ```

4. **Pretty print JSON responses:**
   ```bash
   curl ... | jq '.'
   ```
   (requires `jq` to be installed)

5. **Test with verbose output:**
   ```bash
   curl -v ...
   ```

---

## Summary

- **Total Endpoints:** 24
- **Auth Endpoints:** 3 (no authentication required)
- **User Endpoints:** 5 (admin role required)
- **Actor Endpoints:** 5 (list is public, others require auth)
- **Movie Endpoints:** 5 (list is public, others require auth)
- **Actor-Movie Endpoints:** 6 (all require authentication)

All endpoints accept and return JSON with camelCase property names.
