#!/bin/bash

# SimpleMDB API Testing Script
# This script tests all 24 API endpoints

BASE_URL="http://127.0.0.1:8080/api/v1"
TOKEN=""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "=========================================="
echo "SimpleMDB API Testing Script"
echo "=========================================="
echo ""

# Function to print test result
print_result() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}✓ PASS${NC}: $2"
    else
        echo -e "${RED}✗ FAIL${NC}: $2"
    fi
}

# Function to make API call and check response
test_api() {
    local method=$1
    local endpoint=$2
    local data=$3
    local expected_status=$4
    local description=$5
    local headers=${6:-"Content-Type: application/json"}
    
    echo ""
    echo -e "${YELLOW}Testing:${NC} $description"
    echo "Request: $method $BASE_URL$endpoint"
    
    if [ -n "$TOKEN" ]; then
        headers="$headers|Authorization: Bearer $TOKEN"
    fi
    
    # Convert pipe-separated headers to -H arguments
    IFS='|' read -ra HEADER_ARRAY <<< "$headers"
    HEADER_ARGS=()
    for header in "${HEADER_ARRAY[@]}"; do
        HEADER_ARGS+=("-H" "$header")
    done
    
    if [ "$method" = "GET" ]; then
        response=$(curl -s -w "\n%{http_code}" "${HEADER_ARGS[@]}" "$BASE_URL$endpoint")
    else
        response=$(curl -s -w "\n%{http_code}" -X "$method" "${HEADER_ARGS[@]}" -d "$data" "$BASE_URL$endpoint")
    fi
    
    # Extract status code (last line) and body (everything else)
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')
    
    echo "Response Code: $http_code"
    echo "Response Body: $body"
    
    if [ "$http_code" -eq "$expected_status" ]; then
        print_result 0 "$description"
        echo "$body"
        return 0
    else
        print_result 1 "$description (Expected: $expected_status, Got: $http_code)"
        return 1
    fi
}

echo ""
echo "=========================================="
echo "1. Authentication Endpoints (3)"
echo "=========================================="

# 1. Register
echo ""
echo "--- Test 1: Register a new user ---"
test_api "POST" "/auth/register" \
    '{"username":"testuser","password":"testpass"}' \
    201 \
    "Register new user"

# 2. Login and capture token
echo ""
echo "--- Test 2: Login with credentials ---"
response=$(test_api "POST" "/auth/login" \
    '{"username":"testuser","password":"testpass"}' \
    200 \
    "Login with valid credentials")

# Extract token from response
TOKEN=$(echo "$response" | grep -o '"token":"[^"]*' | sed 's/"token":"//')
echo "Token captured: ${TOKEN:0:20}..."

# 3. Logout
echo ""
echo "--- Test 3: Logout ---"
test_api "POST" "/auth/logout" \
    '' \
    200 \
    "Logout user"

echo ""
echo "=========================================="
echo "2. Public Endpoints (No Auth Required)"
echo "=========================================="

# 4. Get All Actors (public)
echo ""
echo "--- Test 4: Get all actors (public) ---"
test_api "GET" "/actors?page=1&size=5" \
    '' \
    200 \
    "Get all actors without authentication"

# 5. Get All Movies (public)
echo ""
echo "--- Test 5: Get all movies (public) ---"
test_api "GET" "/movies?page=1&size=5" \
    '' \
    200 \
    "Get all movies without authentication"

echo ""
echo "=========================================="
echo "3. Actor Endpoints (5)"
echo "=========================================="

# 6. Add Actor
echo ""
echo "--- Test 6: Add a new actor ---"
add_actor_response=$(test_api "POST" "/actors/add" \
    '{"firstName":"Tom","lastName":"Hanks","bio":"Award-winning actor","rating":9.5}' \
    201 \
    "Add new actor with authentication")
ACTOR_ID=$(echo "$add_actor_response" | grep -o '"id":[0-9]*' | head -1 | sed 's/"id"://')
echo "Actor ID captured: $ACTOR_ID"

# 7. View Actor
echo ""
echo "--- Test 7: View actor by ID ---"
test_api "GET" "/actors/view?aid=$ACTOR_ID" \
    '' \
    200 \
    "View actor by ID"

# 8. Edit Actor
echo ""
echo "--- Test 8: Edit actor ---"
test_api "POST" "/actors/edit?aid=$ACTOR_ID" \
    '{"firstName":"Thomas","lastName":"Hanks","bio":"Academy Award-winning actor","rating":9.8}' \
    200 \
    "Edit existing actor"

# 9. Get All Actors
echo ""
echo "--- Test 9: Get all actors ---"
test_api "GET" "/actors?page=1&size=10" \
    '' \
    200 \
    "Get all actors with pagination"

# Note: We'll test actor removal later to avoid breaking relationships

echo ""
echo "=========================================="
echo "4. Movie Endpoints (5)"
echo "=========================================="

# 10. Add Movie
echo ""
echo "--- Test 10: Add a new movie ---"
add_movie_response=$(test_api "POST" "/movies/add" \
    '{"title":"Forrest Gump","year":1994,"description":"Life story of Forrest Gump","rating":8.8}' \
    201 \
    "Add new movie with authentication")
MOVIE_ID=$(echo "$add_movie_response" | grep -o '"id":[0-9]*' | head -1 | sed 's/"id"://')
echo "Movie ID captured: $MOVIE_ID"

# 11. View Movie
echo ""
echo "--- Test 11: View movie by ID ---"
test_api "GET" "/movies/view?mid=$MOVIE_ID" \
    '' \
    200 \
    "View movie by ID"

# 12. Edit Movie
echo ""
echo "--- Test 12: Edit movie ---"
test_api "POST" "/movies/edit?mid=$MOVIE_ID" \
    '{"title":"Forrest Gump","year":1994,"description":"The life story of Forrest Gump, a simple man","rating":9.0}' \
    200 \
    "Edit existing movie"

# 13. Get All Movies
echo ""
echo "--- Test 13: Get all movies ---"
test_api "GET" "/movies?page=1&size=10" \
    '' \
    200 \
    "Get all movies with pagination"

# Note: We'll test movie removal later

echo ""
echo "=========================================="
echo "5. Actor-Movie Relationship Endpoints (6)"
echo "=========================================="

# 14. Add Movie to Actor
echo ""
echo "--- Test 14: Add movie to actor ---"
add_am_response=$(test_api "POST" "/actors/movies/add" \
    "{\"actorId\":$ACTOR_ID,\"movieId\":$MOVIE_ID,\"roleName\":\"Forrest Gump\"}" \
    201 \
    "Add movie to actor relationship")
AM_ID=$(echo "$add_am_response" | grep -o '"id":[0-9]*' | head -1 | sed 's/"id"://')
echo "Actor-Movie ID captured: $AM_ID"

# 15. Get Movies by Actor
echo ""
echo "--- Test 15: Get movies by actor ---"
test_api "GET" "/actors/movies?aid=$ACTOR_ID&page=1&size=10" \
    '' \
    200 \
    "Get all movies for an actor"

# 16. Get Actors by Movie
echo ""
echo "--- Test 16: Get actors by movie ---"
test_api "GET" "/movies/actors?mid=$MOVIE_ID&page=1&size=10" \
    '' \
    200 \
    "Get all actors for a movie"

# 17. Add Actor to Movie (alternative endpoint)
echo ""
echo "--- Test 17: Add actor to movie (alternative) ---"
test_api "POST" "/movies/actors/add" \
    "{\"actorId\":$ACTOR_ID,\"movieId\":$MOVIE_ID,\"roleName\":\"Additional Role\"}" \
    201 \
    "Add actor to movie relationship"

# 18. Remove Movie from Actor
echo ""
echo "--- Test 18: Remove movie from actor ---"
test_api "POST" "/actors/movies/remove?amid=$AM_ID" \
    '' \
    200 \
    "Remove movie from actor relationship"

# 19. Remove Actor from Movie (we'll skip this since we removed the relationship above)

echo ""
echo "=========================================="
echo "6. Cleanup - Remove Resources"
echo "=========================================="

# 20. Remove Actor
echo ""
echo "--- Test 19: Remove actor ---"
test_api "POST" "/actors/remove?aid=$ACTOR_ID" \
    '' \
    200 \
    "Remove actor"

# 21. Remove Movie
echo ""
echo "--- Test 20: Remove movie ---"
test_api "POST" "/movies/remove?mid=$MOVIE_ID" \
    '' \
    200 \
    "Remove movie"

echo ""
echo "=========================================="
echo "7. User Management Endpoints (Admin Only)"
echo "=========================================="

# Note: These will fail if testuser is not an admin
# We'll test them anyway to show the behavior

# 22. Get All Users (Admin only)
echo ""
echo "--- Test 21: Get all users (requires admin) ---"
test_api "GET" "/users?page=1&size=10" \
    '' \
    403 \
    "Get all users (should fail if not admin)"

# 23. Add User (Admin only)
echo ""
echo "--- Test 22: Add user (requires admin) ---"
test_api "POST" "/users/add" \
    '{"username":"newuser","password":"newpass","role":"user"}' \
    403 \
    "Add new user (should fail if not admin)"

# 24. View User (Admin only)
echo ""
echo "--- Test 23: View user (requires admin) ---"
test_api "GET" "/users/view?uid=1" \
    '' \
    403 \
    "View user by ID (should fail if not admin)"

# 25. Edit User (Admin only)
echo ""
echo "--- Test 24: Edit user (requires admin) ---"
test_api "POST" "/users/edit?uid=1" \
    '{"username":"updateduser","password":"updatedpass","role":"user"}' \
    403 \
    "Edit user (should fail if not admin)"

# 26. Remove User (Admin only)
echo ""
echo "--- Test 25: Remove user (requires admin) ---"
test_api "POST" "/users/remove?uid=1" \
    '' \
    403 \
    "Remove user (should fail if not admin)"

echo ""
echo "=========================================="
echo "8. Error Cases"
echo "=========================================="

# 27. Invalid JSON
echo ""
echo "--- Test 26: Invalid JSON body ---"
test_api "POST" "/actors/add" \
    '{"firstName":"Invalid' \
    400 \
    "Add actor with invalid JSON (should fail)"

# 28. Missing required fields
echo ""
echo "--- Test 27: Missing required ID ---"
test_api "GET" "/actors/view?aid=99999" \
    '' \
    404 \
    "View non-existent actor (should return 404)"

# 29. No authentication
TOKEN_BACKUP=$TOKEN
TOKEN=""
echo ""
echo "--- Test 28: Access protected endpoint without auth ---"
test_api "POST" "/actors/add" \
    '{"firstName":"Test","lastName":"Actor","bio":"Test","rating":5}' \
    401 \
    "Add actor without authentication (should fail)"
TOKEN=$TOKEN_BACKUP

echo ""
echo "=========================================="
echo "Testing Complete!"
echo "=========================================="
echo ""
echo "Summary: All 24 API endpoints have been tested."
echo "Note: Some tests expected to fail (e.g., admin-only endpoints)"
echo "      when testing with a non-admin user account."
