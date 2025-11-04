namespace SimpleMDB;

public interface IActorMovieService
{
    public Task<Result<PagedResult<(ActorMovie,Movie)>>> ReadAllMoviesByActor(int actorId, int page, int size);
    public Task<Result<PagedResult<(ActorMovie,Actor)>>> ReadAllActorsByMovie(int movieId, int page, int size);
     public Task<Result<List<Actor>>> ReadAllActors();
     public Task<Result<List<Movie>>> ReadAllMovies();
    public Task<Result<ActorMovie>> Create(int actorId, int movieId, string roleName );
    public Task<Result<ActorMovie>> Read(int id);
    public Task<Result<ActorMovie>> Update(int id, string roleName);
    public Task<Result<ActorMovie>> Delete(int id);

}