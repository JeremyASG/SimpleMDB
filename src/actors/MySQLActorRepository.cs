using System.Data;
using MySql.Data.MySqlClient;

namespace SimpleMDB;

public class MySqlActorRepository : IActorRepository
{
    private string connectionString;

    public MySqlActorRepository(string connectionString)
    {
        this.connectionString = connectionString;
        //Init();
    }

    private void Init()
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Actors (
                id INT AUTO_INCREMENT PRIMARY KEY,
                firstname VARCHAR(64) NOT NULL,
                lastname VARCHAR(64) NOT NULL,
                bio TEXT,
                rating FLOAT
            );
        ";
        cmd.ExecuteNonQuery();
    }

    public MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    public async Task<PagedResult<Actor>> ReadAll(int page, int size)
    {
        using var dbc = OpenDb();
        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM Actors";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Actors LIMIT @offset, @limit";
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();
        var actors = new List<Actor>();

        while (await rows.ReadAsync())
        {
            
            var bio = rows.IsDBNull(rows.GetOrdinal("bio")) ? "" : rows.GetString(rows.GetOrdinal("bio"));
            var rating = rows.IsDBNull(rows.GetOrdinal("rating")) ? 0f : Convert.ToSingle(rows["rating"]);

            actors.Add(new Actor
            {
                Id = Convert.ToInt32(rows["id"]),
                FirstName = rows["firstname"].ToString() ?? "",
                LastName = rows["lastname"].ToString() ?? "",
                Bio = bio,
                Rating = rating
            });
        }

        return new PagedResult<Actor>(actors, totalCount);
    }

    public async Task<Actor?> Create(Actor actor)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Actors (firstname, lastname, bio, rating)
            VALUES(@firstname, @lastname, @bio, @rating);
            SELECT LAST_INSERT_ID();
        ";
        cmd.Parameters.AddWithValue("@firstname", actor.FirstName ?? "");
        cmd.Parameters.AddWithValue("@lastname", actor.LastName ?? "");
        // asegura DBNull cuando actor.bio es null
        cmd.Parameters.AddWithValue("@bio", (object?)actor.Bio ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@rating", actor.Rating);

        actor.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return actor;
    }

    public async Task<Actor?> Read(int id)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Actors WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var rows = await cmd.ExecuteReaderAsync();
        if (await rows.ReadAsync())
        {
            var bio = rows.IsDBNull(rows.GetOrdinal("bio")) ? "" : rows.GetString(rows.GetOrdinal("bio"));
            var rating = rows.IsDBNull(rows.GetOrdinal("rating")) ? 0f : Convert.ToSingle(rows["rating"]);

            return new Actor
            {
                Id = Convert.ToInt32(rows["id"]),
                FirstName = rows["firstname"].ToString() ?? "",
                LastName = rows["lastname"].ToString() ?? "",
                Bio = bio,
                Rating = rating
            };
        }
        return null;
    }

    public async Task<Actor?> Update(int id, Actor newActor)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            UPDATE Actors
            SET firstname = @firstname,
                lastname = @lastname,
                bio = @bio,
                rating = @rating
            WHERE id = @id
        ";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@firstname", newActor.FirstName ?? "");
        cmd.Parameters.AddWithValue("@lastname", newActor.LastName ?? "");
        cmd.Parameters.AddWithValue("@bio", (object?)newActor.Bio ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@rating", newActor.Rating);

        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? newActor : null;
    }

    public async Task<Actor?> Delete(int id)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "DELETE FROM Actors WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        Actor? actor = await Read(id);
        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? actor : null;
    }
}
