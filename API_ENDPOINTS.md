# SimpleMDB API Documentation

This document describes the JSON API endpoints available in SimpleMDB.

## Base URL
All API endpoints are prefixed with `/api/v1`

Example: `http://127.0.0.1:8080/api/v1/users`

## Authentication

Most endpoints require authentication via Bearer token in the Authorization header:
```
Authorization: Bearer YOUR_TOKEN_HERE
```

To obtain a token, use the `/api/v1/login` endpoint.

## Response Format

All responses are in JSON format with appropriate HTTP status codes:
- **200 OK** - Successful request
- **201 Created** - Resource created successfully
- **400 Bad Request** - Invalid input or malformed JSON
- **401 Unauthorized** - Authentication required or failed
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Server error

Error responses follow this format:
```json
{
  "error": "Error message description"
}
```

## Endpoints

### Authentication (No auth required)

#### Register
- **POST** `/api/v1/register`
- **Body**: `{"username": "string", "password": "string", "cpassword": "string"}`
- **Response**: `{"message": "string", "user": {...}}`

#### Login
- **POST** `/api/v1/login`
- **Body**: `{"username": "string", "password": "string"}`
- **Response**: `{"message": "string", "token": "string"}`

#### Logout
- **POST** `/api/v1/logout`
- **Response**: `{"message": "string"}`

---

### Users (Requires Admin role)

#### List Users
- **GET** `/api/v1/users?page=1&size=10`
- **Response**: `{"users": [...], "totalCount": 0, "page": 1, "size": 10}`

#### Add User
- **POST** `/api/v1/users/add`
- **Body**: `{"username": "string", "password": "string", "role": "string"}`
- **Response**: User object

#### View User
- **GET** `/api/v1/users/view?uid=1`
- **Response**: User object

#### Edit User
- **POST** `/api/v1/users/edit?uid=1`
- **Body**: `{"username": "string", "password": "string", "role": "string"}`
- **Response**: User object

#### Remove User
- **POST** `/api/v1/users/remove?uid=1`
- **Response**: `{"message": "string", "user": {...}}`

---

### Actors

#### List Actors (No auth required)
- **GET** `/api/v1/actors?page=1&size=10`
- **Response**: `{"actors": [...], "totalCount": 0, "page": 1, "size": 10}`

#### Add Actor (Requires auth)
- **POST** `/api/v1/actors/add`
- **Body**: `{"firstname": "string", "lastname": "string", "bio": "string", "rating": 0.0}`
- **Response**: Actor object

#### View Actor (Requires auth)
- **GET** `/api/v1/actors/view?aid=1`
- **Response**: Actor object

#### Edit Actor (Requires auth)
- **POST** `/api/v1/actors/edit?aid=1`
- **Body**: `{"firstname": "string", "lastname": "string", "bio": "string", "rating": 0.0}`
- **Response**: Actor object

#### Remove Actor (Requires auth)
- **POST** `/api/v1/actors/remove?aid=1`
- **Response**: `{"message": "string", "actor": {...}}`

---

### Movies

#### List Movies (No auth required)
- **GET** `/api/v1/movies?page=1&size=10`
- **Response**: `{"movies": [...], "totalCount": 0, "page": 1, "size": 10}`

#### Add Movie (Requires auth)
- **POST** `/api/v1/movies/add`
- **Body**: `{"title": "string", "year": 2025, "description": "string", "rating": 0.0}`
- **Response**: Movie object

#### View Movie (Requires auth)
- **GET** `/api/v1/movies/view?mid=1`
- **Response**: Movie object

#### Edit Movie (Requires auth)
- **POST** `/api/v1/movies/edit?mid=1`
- **Body**: `{"title": "string", "year": 2025, "description": "string", "rating": 0.0}`
- **Response**: Movie object

#### Remove Movie (Requires auth)
- **POST** `/api/v1/movies/remove?mid=1`
- **Response**: `{"message": "string", "movie": {...}}`

---

### Actor-Movies (All require auth)

#### List Movies by Actor
- **GET** `/api/v1/actors/movies?aid=1&page=1&size=10`
- **Response**: 
```json
{
  "actor": {...},
  "movies": [
    {"actorMovieId": 0, "roleName": "string", "movie": {...}}
  ],
  "totalCount": 0,
  "page": 1,
  "size": 10
}
```

#### Add Movie to Actor
- **POST** `/api/v1/actors/movies/add`
- **Body**: `{"aid": 1, "mid": 1, "rolename": "string"}`
- **Response**: ActorMovie object

#### Remove Movie from Actor
- **POST** `/api/v1/actors/movies/remove?amid=1`
- **Response**: `{"message": "string", "actorMovie": {...}}`

#### List Actors by Movie
- **GET** `/api/v1/movies/actors?mid=1&page=1&size=10`
- **Response**: 
```json
{
  "movie": {...},
  "actors": [
    {"actorMovieId": 0, "roleName": "string", "actor": {...}}
  ],
  "totalCount": 0,
  "page": 1,
  "size": 10
}
```

#### Add Actor to Movie
- **POST** `/api/v1/movies/actors/add`
- **Body**: `{"mid": 1, "aid": 1, "rolename": "string"}`
- **Response**: ActorMovie object

#### Remove Actor from Movie
- **POST** `/api/v1/movies/actors/remove?amid=1`
- **Response**: `{"message": "string", "actorMovie": {...}}`

---

## Data Models

### User
```json
{
  "id": 0,
  "username": "string",
  "password": "string",
  "salt": "string",
  "role": "string"
}
```

### Actor
```json
{
  "id": 0,
  "firstName": "string",
  "lastName": "string",
  "bio": "string",
  "rating": 0.0
}
```

### Movie
```json
{
  "id": 0,
  "title": "string",
  "year": 0,
  "description": "string",
  "rating": 0.0
}
```

### ActorMovie
```json
{
  "id": 0,
  "actorId": 0,
  "movieId": 0,
  "roleName": "string"
}
```

## Pagination

List endpoints support pagination via query parameters:
- `page` - Page number (default: 1)
- `size` - Items per page (default: 10)

Response includes:
- Array of items
- `totalCount` - Total number of items
- `page` - Current page number
- `size` - Items per page

## Example Usage

### Register and Login
```bash
# Register
curl -X POST http://127.0.0.1:8080/api/v1/register \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"demo123","cpassword":"demo123"}'

# Login
curl -X POST http://127.0.0.1:8080/api/v1/login \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"demo123"}'
```

### Create an Actor
```bash
curl -X POST http://127.0.0.1:8080/api/v1/actors/add \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"firstname":"Tom","lastname":"Hanks","bio":"Award-winning actor","rating":9.5}'
```

### List Movies
```bash
curl -X GET "http://127.0.0.1:8080/api/v1/movies?page=1&size=5"
```
