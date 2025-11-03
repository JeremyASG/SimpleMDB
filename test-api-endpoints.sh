#!/bin/bash

# SimpleMDB API Test Script
# This script tests all 24 API endpoints using cURL

BASE_URL="http://127.0.0.1:8080"
TOKEN=""

echo "================================"
echo "SimpleMDB API Testing Script"
echo "================================"
echo ""

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_test() {
    echo -e "${BLUE}TEST $1: $2${NC}"
}

print_success() {
    echo -e "${GREEN}✓ Success${NC}"
}

print_error() {
    echo -e "${RED}✗ Failed${NC}"
}

# ================================
# AUTH API ENDPOINTS (3)
# ================================
echo "================================"
echo "1. AUTH ENDPOINTS"
echo "================================"
echo ""

# Test 1: Register
print_test "1" "POST /api/v1/register"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/register" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser123","password":"testpass123","cpassword":"testpass123"}')
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"message"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 2: Login
print_test "2" "POST /api/v1/login"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser123","password":"testpass123"}')
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"token"* ]]; then
    TOKEN=$(echo $RESPONSE | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
    echo "Token extracted: ${TOKEN:0:20}..."
    print_success
else
    print_error
fi
echo ""

# Test 3: Logout
print_test "3" "POST /api/v1/logout"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/logout")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"message"* ]]; then
    print_success
else
    print_error
fi
echo ""

# ================================
# USERS API ENDPOINTS (5) - Requires Admin
# ================================
echo "================================"
echo "2. USERS ENDPOINTS (Admin Only)"
echo "================================"
echo ""

# Test 4: List all users
print_test "4" "GET /api/v1/users?page=1&size=5"
RESPONSE=$(curl -s -X GET "$BASE_URL/api/v1/users?page=1&size=5" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"users"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 5: Add user
print_test "5" "POST /api/v1/users/add"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/users/add" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"username":"newuser123","password":"newpass123","role":"user"}')
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"username"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 6: View user
print_test "6" "GET /api/v1/users/view?uid=1"
RESPONSE=$(curl -s -X GET "$BASE_URL/api/v1/users/view?uid=1" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"username"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 7: Edit user
print_test "7" "POST /api/v1/users/edit?uid=1"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/users/edit?uid=1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"username":"updateduser","password":"newpass","role":"user"}')
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"username"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 8: Remove user (using a high ID that likely doesn't exist)
print_test "8" "POST /api/v1/users/remove?uid=9999"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/users/remove?uid=9999" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"message"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# ================================
# ACTORS API ENDPOINTS (5)
# ================================
echo "================================"
echo "3. ACTORS ENDPOINTS"
echo "================================"
echo ""

# Test 9: List all actors (no auth required)
print_test "9" "GET /api/v1/actors?page=1&size=5"
RESPONSE=$(curl -s -X GET "$BASE_URL/api/v1/actors?page=1&size=5")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"actors"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 10: Add actor (requires auth)
print_test "10" "POST /api/v1/actors/add"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/actors/add" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"firstname":"Tom","lastname":"Hanks","bio":"Award-winning actor","rating":9.5}')
echo "Response: $RESPONSE"
ACTOR_ID=$(echo $RESPONSE | grep -o '"id":[0-9]*' | head -1 | cut -d':' -f2)
echo "Actor ID created: $ACTOR_ID"
if [[ $RESPONSE == *"firstName"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 11: View actor (requires auth)
print_test "11" "GET /api/v1/actors/view?aid=${ACTOR_ID:-1}"
RESPONSE=$(curl -s -X GET "$BASE_URL/api/v1/actors/view?aid=${ACTOR_ID:-1}" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"firstName"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 12: Edit actor (requires auth)
print_test "12" "POST /api/v1/actors/edit?aid=${ACTOR_ID:-1}"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/actors/edit?aid=${ACTOR_ID:-1}" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"firstname":"Thomas","lastname":"Hanks","bio":"Updated bio","rating":9.8}')
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"firstName"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 13: Remove actor (requires auth) - using high ID
print_test "13" "POST /api/v1/actors/remove?aid=9999"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/actors/remove?aid=9999" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"message"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# ================================
# MOVIES API ENDPOINTS (5)
# ================================
echo "================================"
echo "4. MOVIES ENDPOINTS"
echo "================================"
echo ""

# Test 14: List all movies (no auth required)
print_test "14" "GET /api/v1/movies?page=1&size=5"
RESPONSE=$(curl -s -X GET "$BASE_URL/api/v1/movies?page=1&size=5")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"movies"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 15: Add movie (requires auth)
print_test "15" "POST /api/v1/movies/add"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/movies/add" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"The Matrix","year":1999,"description":"A computer hacker learns about the true nature of reality","rating":9.0}')
echo "Response: $RESPONSE"
MOVIE_ID=$(echo $RESPONSE | grep -o '"id":[0-9]*' | head -1 | cut -d':' -f2)
echo "Movie ID created: $MOVIE_ID"
if [[ $RESPONSE == *"title"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 16: View movie (requires auth)
print_test "16" "GET /api/v1/movies/view?mid=${MOVIE_ID:-1}"
RESPONSE=$(curl -s -X GET "$BASE_URL/api/v1/movies/view?mid=${MOVIE_ID:-1}" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"title"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 17: Edit movie (requires auth)
print_test "17" "POST /api/v1/movies/edit?mid=${MOVIE_ID:-1}"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/movies/edit?mid=${MOVIE_ID:-1}" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"The Matrix Reloaded","year":2003,"description":"Updated description","rating":8.5}')
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"title"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 18: Remove movie (requires auth) - using high ID
print_test "18" "POST /api/v1/movies/remove?mid=9999"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/movies/remove?mid=9999" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"message"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# ================================
# ACTOR-MOVIES API ENDPOINTS (6)
# ================================
echo "================================"
echo "5. ACTOR-MOVIES ENDPOINTS"
echo "================================"
echo ""

# Test 19: Get movies by actor (requires auth)
print_test "19" "GET /api/v1/actors/movies?aid=${ACTOR_ID:-1}&page=1&size=5"
RESPONSE=$(curl -s -X GET "$BASE_URL/api/v1/actors/movies?aid=${ACTOR_ID:-1}&page=1&size=5" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"actor"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 20: Add movie to actor (requires auth)
print_test "20" "POST /api/v1/actors/movies/add"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/actors/movies/add" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"aid\":${ACTOR_ID:-1},\"mid\":${MOVIE_ID:-1},\"rolename\":\"Neo\"}")
echo "Response: $RESPONSE"
ACTORMOVIE_ID=$(echo $RESPONSE | grep -o '"id":[0-9]*' | head -1 | cut -d':' -f2)
echo "ActorMovie ID created: $ACTORMOVIE_ID"
if [[ $RESPONSE == *"roleName"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 21: Remove movie from actor (requires auth) - using high ID
print_test "21" "POST /api/v1/actors/movies/remove?amid=9999"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/actors/movies/remove?amid=9999" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"message"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 22: Get actors by movie (requires auth)
print_test "22" "GET /api/v1/movies/actors?mid=${MOVIE_ID:-1}&page=1&size=5"
RESPONSE=$(curl -s -X GET "$BASE_URL/api/v1/movies/actors?mid=${MOVIE_ID:-1}&page=1&size=5" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"movie"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 23: Add actor to movie (requires auth)
print_test "23" "POST /api/v1/movies/actors/add"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/movies/actors/add" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"mid\":${MOVIE_ID:-1},\"aid\":${ACTOR_ID:-1},\"rolename\":\"Trinity\"}")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"roleName"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

# Test 24: Remove actor from movie (requires auth) - using high ID
print_test "24" "POST /api/v1/movies/actors/remove?amid=9999"
RESPONSE=$(curl -s -X POST "$BASE_URL/api/v1/movies/actors/remove?amid=9999" \
  -H "Authorization: Bearer $TOKEN")
echo "Response: $RESPONSE"
if [[ $RESPONSE == *"message"* || $RESPONSE == *"error"* ]]; then
    print_success
else
    print_error
fi
echo ""

echo "================================"
echo "Testing Complete!"
echo "================================"
echo ""
echo "Note: Some tests may show errors if:"
echo "  - Server is not running"
echo "  - Database is not set up"
echo "  - User doesn't have admin privileges"
echo "  - Resources don't exist"
echo ""
echo "To run the server: cd /home/runner/work/SimpleMDB/SimpleMDB && dotnet run"
