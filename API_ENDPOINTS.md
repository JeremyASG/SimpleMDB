# SimpleMDB API Endpoints Documentation

This document describes all the API endpoints available in SimpleMDB under the `/api/v1` path.

## Base URL
```
http://127.0.0.1:8080/api/v1
```

## Authentication

All authenticated endpoints require a Bearer token in the Authorization header:
```
Authorization: Bearer <token>
```

The token is obtained from the login endpoint.

## API Endpoints

### Authentication (3 endpoints)

#### 1. Register
- **POST** `/api/v1/auth/register`
- **Authentication:** None
- **Request Body:**
```json
{
  "username": "string",
  "password": "string"
}
```
- **Response:** 201 Created
```json
{
  "message": "User registered successfully",
  "user": {
    "id": 1,
    "username": "string",
    "role": "string"
  }
}
```

#### 2. Login
- **POST** `/api/v1/auth/login`
- **Authentication:** None
- **Request Body:**
```json
{
  "username": "string",
  "password": "string"
}
```
- **Response:** 200 OK
```json
{
  "message": "User logged in successfully",
  "token": "string"
}
```

#### 3. Logout
- **POST** `/api/v1/auth/logout`
- **Authentication:** Required
- **Request Body:** None
- **Response:** 200 OK
```json
{
  "message": "User logged out successfully"
}
```

### Users (5 endpoints)

All user endpoints require Admin authentication.

#### 4. Get All Users
- **GET** `/api/v1/users?page=1&size=10`
- **Authentication:** Admin only
- **Response:** 200 OK
```json
{
  "users": [
    {
      "id": 1,
      "username": "string",
      "role": "string"
    }
  ],
  "totalCount": 10,
  "page": 1,
  "size": 10
}
```

#### 5. Add User
- **POST** `/api/v1/users/add`
- **Authentication:** Admin only
- **Request Body:**
```json
{
  "username": "string",
  "password": "string",
  "role": "string"
}
```
- **Response:** 201 Created
```json
{
  "message": "User added successfully",
  "user": {
    "id": 1,
    "username": "string",
    "role": "string"
  }
}
```

#### 6. View User
- **GET** `/api/v1/users/view?uid=1`
- **Authentication:** Admin only
- **Response:** 200 OK
```json
{
  "user": {
    "id": 1,
    "username": "string",
    "role": "string"
  }
}
```

#### 7. Edit User
- **POST** `/api/v1/users/edit?uid=1`
- **Authentication:** Admin only
- **Request Body:**
```json
{
  "username": "string",
  "password": "string",
  "role": "string"
}
```
- **Response:** 200 OK
```json
{
  "message": "User edited successfully",
  "user": {
    "id": 1,
    "username": "string",
    "role": "string"
  }
}
```

#### 8. Remove User
- **POST** `/api/v1/users/remove?uid=1`
- **Authentication:** Admin only
- **Request Body:** None
- **Response:** 200 OK
```json
{
  "message": "User removed successfully"
}
```

### Actors (5 endpoints)

#### 9. Get All Actors
- **GET** `/api/v1/actors?page=1&size=10`
- **Authentication:** None
- **Response:** 200 OK
```json
{
  "actors": [
    {
      "id": 1,
      "firstName": "string",
      "lastName": "string",
      "bio": "string",
      "rating": 5.0
    }
  ],
  "totalCount": 10,
  "page": 1,
  "size": 10
}
```

#### 10. Add Actor
- **POST** `/api/v1/actors/add`
- **Authentication:** Required
- **Request Body:**
```json
{
  "firstName": "string",
  "lastName": "string",
  "bio": "string",
  "rating": 5.0
}
```
- **Response:** 201 Created
```json
{
  "message": "Actor added successfully",
  "actor": {
    "id": 1,
    "firstName": "string",
    "lastName": "string",
    "bio": "string",
    "rating": 5.0
  }
}
```

#### 11. View Actor
- **GET** `/api/v1/actors/view?aid=1`
- **Authentication:** Required
- **Response:** 200 OK
```json
{
  "actor": {
    "id": 1,
    "firstName": "string",
    "lastName": "string",
    "bio": "string",
    "rating": 5.0
  }
}
```

#### 12. Edit Actor
- **POST** `/api/v1/actors/edit?aid=1`
- **Authentication:** Required
- **Request Body:**
```json
{
  "firstName": "string",
  "lastName": "string",
  "bio": "string",
  "rating": 5.0
}
```
- **Response:** 200 OK
```json
{
  "message": "Actor edited successfully",
  "actor": {
    "id": 1,
    "firstName": "string",
    "lastName": "string",
    "bio": "string",
    "rating": 5.0
  }
}
```

#### 13. Remove Actor
- **POST** `/api/v1/actors/remove?aid=1`
- **Authentication:** Required
- **Request Body:** None
- **Response:** 200 OK
```json
{
  "message": "Actor removed successfully"
}
```

### Movies (5 endpoints)

#### 14. Get All Movies
- **GET** `/api/v1/movies?page=1&size=10`
- **Authentication:** None
- **Response:** 200 OK
```json
{
  "movies": [
    {
      "id": 1,
      "title": "string",
      "year": 2025,
      "description": "string",
      "rating": 5.0
    }
  ],
  "totalCount": 10,
  "page": 1,
  "size": 10
}
```

#### 15. Add Movie
- **POST** `/api/v1/movies/add`
- **Authentication:** Required
- **Request Body:**
```json
{
  "title": "string",
  "year": 2025,
  "description": "string",
  "rating": 5.0
}
```
- **Response:** 201 Created
```json
{
  "message": "Movie added successfully",
  "movie": {
    "id": 1,
    "title": "string",
    "year": 2025,
    "description": "string",
    "rating": 5.0
  }
}
```

#### 16. View Movie
- **GET** `/api/v1/movies/view?mid=1`
- **Authentication:** Required
- **Response:** 200 OK
```json
{
  "movie": {
    "id": 1,
    "title": "string",
    "year": 2025,
    "description": "string",
    "rating": 5.0
  }
}
```

#### 17. Edit Movie
- **POST** `/api/v1/movies/edit?mid=1`
- **Authentication:** Required
- **Request Body:**
```json
{
  "title": "string",
  "year": 2025,
  "description": "string",
  "rating": 5.0
}
```
- **Response:** 200 OK
```json
{
  "message": "Movie edited successfully",
  "movie": {
    "id": 1,
    "title": "string",
    "year": 2025,
    "description": "string",
    "rating": 5.0
  }
}
```

#### 18. Remove Movie
- **POST** `/api/v1/movies/remove?mid=1`
- **Authentication:** Required
- **Request Body:** None
- **Response:** 200 OK
```json
{
  "message": "Movie removed successfully"
}
```

### Actor-Movie Relationships (6 endpoints)

#### 19. Get Movies by Actor
- **GET** `/api/v1/actors/movies?aid=1&page=1&size=10`
- **Authentication:** Required
- **Response:** 200 OK
```json
{
  "actorMovies": [
    {
      "actorMovieId": 1,
      "actorId": 1,
      "movieId": 1,
      "roleName": "string",
      "movie": {
        "id": 1,
        "title": "string",
        "year": 2025,
        "description": "string",
        "rating": 5.0
      }
    }
  ],
  "totalCount": 10,
  "page": 1,
  "size": 10
}
```

#### 20. Add Movie to Actor
- **POST** `/api/v1/actors/movies/add`
- **Authentication:** Required
- **Request Body:**
```json
{
  "actorId": 1,
  "movieId": 1,
  "roleName": "string"
}
```
- **Response:** 201 Created
```json
{
  "message": "Movie added to actor successfully",
  "actorMovie": {
    "id": 1,
    "actorId": 1,
    "movieId": 1,
    "roleName": "string"
  }
}
```

#### 21. Remove Movie from Actor
- **POST** `/api/v1/actors/movies/remove?amid=1`
- **Authentication:** Required
- **Request Body:** None
- **Response:** 200 OK
```json
{
  "message": "Movie removed from actor successfully"
}
```

#### 22. Get Actors by Movie
- **GET** `/api/v1/movies/actors?mid=1&page=1&size=10`
- **Authentication:** Required
- **Response:** 200 OK
```json
{
  "actorMovies": [
    {
      "actorMovieId": 1,
      "actorId": 1,
      "movieId": 1,
      "roleName": "string",
      "actor": {
        "id": 1,
        "firstName": "string",
        "lastName": "string",
        "bio": "string",
        "rating": 5.0
      }
    }
  ],
  "totalCount": 10,
  "page": 1,
  "size": 10
}
```

#### 23. Add Actor to Movie
- **POST** `/api/v1/movies/actors/add`
- **Authentication:** Required
- **Request Body:**
```json
{
  "actorId": 1,
  "movieId": 1,
  "roleName": "string"
}
```
- **Response:** 201 Created
```json
{
  "message": "Actor added to movie successfully",
  "actorMovie": {
    "id": 1,
    "actorId": 1,
    "movieId": 1,
    "roleName": "string"
  }
}
```

#### 24. Remove Actor from Movie
- **POST** `/api/v1/movies/actors/remove?amid=1`
- **Authentication:** Required
- **Request Body:** None
- **Response:** 200 OK
```json
{
  "message": "Actor removed from movie successfully"
}
```

## Error Responses

All endpoints return error responses in the following format:

```json
{
  "error": "Error message describing what went wrong"
}
```

Common HTTP status codes:
- `400 Bad Request` - Invalid input data
- `401 Unauthorized` - Missing or invalid authentication token
- `403 Forbidden` - Authenticated but not authorized (e.g., non-admin trying to access admin endpoints)
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Testing with cURL

See `test_api.sh` for comprehensive cURL test examples for all endpoints.
