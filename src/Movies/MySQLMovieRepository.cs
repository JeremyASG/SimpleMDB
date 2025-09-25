using System.Data;
using MySql.Data.MySqlClient;

namespace SimpleMDB;

public class MySqlMovieRepository : IMovieRepository
{
    private string connectionString;

    public MySqlMovieRepository(string connectionString)
    {
        this.connectionString = connectionString;
        //Init();
    }

    private void Init()
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Movies
             (
                id INT AUTO_INCREMENT PRIMARY KEY,
                title VARCHAR(256) NOT NULL,
                year int NOT NULL,
                description TEXT,
                rating FLOAT
             )
        ";
        cmd.ExecuteNonQuery();
    }

    public MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    public async Task<PagedResult<Movie>> ReadAll(int page, int size)
    {
        using var dbc = OpenDb();
        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM Movies";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Movies LIMIT @offset, @limit";
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();
        var movies = new List<Movie>();

        while (await rows.ReadAsync())
        {
            // usa IsDBNull para evitar excepciones o valores raros
            var description = rows.IsDBNull(rows.GetOrdinal("description")) ? "" : rows.GetString(rows.GetOrdinal("description"));
            var rating = rows.IsDBNull(rows.GetOrdinal("rating")) ? 0f : Convert.ToSingle(rows["rating"]);

            movies.Add(new Movie
            {
                Id = Convert.ToInt32(rows["id"]),
                Title = rows["title"].ToString() ?? "",
                Year = Convert.ToInt32(rows["year"]),
                Description = description,
                Rating = rating
            });
        }

        return new PagedResult<Movie>(movies, totalCount);
    }

    public async Task<Movie?> Create(Movie movie)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Movies (title, year, description, rating)
            VALUES(@title, @year, @description, @rating);
            SELECT LAST_INSERT_ID();
        ";
        cmd.Parameters.AddWithValue("@title", movie.Title ?? "");
        cmd.Parameters.AddWithValue("@year", movie.Year);
        // asegura DBNull cuando movie.description es null
        cmd.Parameters.AddWithValue("@description", (object?)movie.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@rating", movie.Rating);

        movie.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return movie;
    }

    public async Task<Movie?> Read(int id)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Movies WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var rows = await cmd.ExecuteReaderAsync();
        if (await rows.ReadAsync())
        {
            var description = rows.IsDBNull(rows.GetOrdinal("description")) ? "" : rows.GetString(rows.GetOrdinal("description"));
            var rating = rows.IsDBNull(rows.GetOrdinal("rating")) ? 0f : Convert.ToSingle(rows["rating"]);

            return new Movie
            {
                Id = Convert.ToInt32(rows["id"]),
                Title = rows["title"].ToString() ?? "",
                Year = Convert.ToInt32(rows["year"]),
                Description = description,
                Rating = rating
            };
        }
        return null;
    }

    public async Task<Movie?> Update(int id, Movie newMovie)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            UPDATE Movies
            SET title = @title,
                year = @year,
                description = @description,
                rating = @rating
            WHERE id = @id
        ";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", newMovie.Title ?? "");
        cmd.Parameters.AddWithValue("@year", newMovie.Year);
        cmd.Parameters.AddWithValue("@description", (object?)newMovie.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@rating", newMovie.Rating);

        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? newMovie : null;
    }

    public async Task<Movie?> Delete(int id)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "DELETE FROM Movies WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        Movie? movie = await Read(id);
        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? movie : null;
    }
}
