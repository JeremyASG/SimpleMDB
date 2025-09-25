using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleMDB;

public class MockMovieRepository : IMovieRepository
{
    private List<Movie> movies;
    private int idCount;

    public MockMovieRepository()
    {
        movies = new List<Movie>();
        idCount = 0;

        var titles = new[]
        {
            "The Shawshank Redemption", "The Godfather", "The Dark Knight", "Pulp Fiction",
            "Forrest Gump", "Inception", "Fight Club", "The Matrix", "Goodfellas", "Se7en",
            "The Silence of the Lambs", "Saving Private Ryan", "The Green Mile", "Interstellar", "Gladiator",
            "The Departed", "Whiplash", "The Prestige", "The Lion King", "Braveheart",
            "Titanic", "Avatar", "The Avengers", "Jurassic Park", "Star Wars: A New Hope",
            "The Empire Strikes Back", "Return of the Jedi", "The Fellowship of the Ring", "The Two Towers", "The Return of the King",
            "Harry Potter and the Sorcerer's Stone", "Harry Potter and the Chamber of Secrets", "Harry Potter and the Prisoner of Azkaban",
            "The Hunger Games", "Catching Fire", "Mockingjay Part 1", "Mockingjay Part 2",
            "Frozen", "Coco", "Toy Story", "Toy Story 2", "Toy Story 3", "Toy Story 4",
            "Finding Nemo", "Finding Dory", "Inside Out", "Soul", "Up",
            "Monsters Inc.", "Monsters University", "Cars", "Cars 2", "Cars 3",
            "Ratatouille", "Wall-E", "Onward", "Turning Red", "Lightyear",
            "Doctor Strange", "Iron Man", "Iron Man 2", "Iron Man 3",
            "Captain America: The First Avenger", "The Winter Soldier", "Civil War",
            "Thor", "Thor: The Dark World", "Thor: Ragnarok", "Thor: Love and Thunder",
            "Black Panther", "Black Panther: Wakanda Forever",
            "Guardians of the Galaxy", "Guardians of the Galaxy Vol. 2", "Guardians of the Galaxy Vol. 3",
            "Spider-Man: Homecoming", "Far From Home", "No Way Home",
            "Ant-Man", "Ant-Man and the Wasp", "Quantumania",
            "Doctor Strange in the Multiverse of Madness", "Avengers: Infinity War", "Avengers: Endgame",
            "Eternals", "Shang-Chi and the Legend of the Ten Rings", "Captain Marvel",
            "Man of Steel", "Batman v Superman", "Justice League", "Zack Snyder's Justice League",
            "Aquaman", "Wonder Woman", "Wonder Woman 1984", "The Flash", "Shazam!", "Shazam! Fury of the Gods",
            "Black Adam", "Suicide Squad", "The Suicide Squad", "Birds of Prey"
        };

        var descriptions = new[]
        {
            "A groundbreaking classic that defined a generation of cinema.",
            "A powerful story of love, loss, and redemption.",
            "An action-packed adventure full of unforgettable moments.",
            "A thrilling journey through the human spirit.",
            "A dramatic tale of courage, loyalty, and sacrifice.",
            "A heartwarming story that touches audiences worldwide.",
            "A dark and twisted narrative that redefined its genre.",
            "A visually stunning and emotionally gripping masterpiece.",
            "A film that continues to inspire new generations.",
            "An iconic story that set new standards in filmmaking."
        };

            var years = new[]
            {
                1994, 1972, 2008, 1974, 1957, 1993, 2003, 1994, 1966, 1999,
                1994, 2010, 2001, 1980, 2002, 1999, 1990, 1975, 1995, 1954,
                2002, 1991, 1946, 1997, 1995, 1994, 1998, 2001, 1998, 2014,
                2019, 1999, 1942, 1994, 2006, 2006, 2014, 2000, 2000, 1979,
                1940, 1988, 2006, 1988, 1957, 2012, 2008, 1980, 2018, 2003,
                1986, 1968, 1957, 2018, 2012, 2019, 2019, 1999, 1995, 2010,
                2017, 2009, 2017, 2013, 2015, 1997, 1998, 2010, 2017, 2016,
                2019, 2013, 2019, 2004, 2002, 2011, 2006, 2011, 1997, 1954,
                1952, 1963, 1927, 1949, 1959, 1950, 1958, 1974, 1944, 1957,
                1948, 1925, 1948, 1936, 1992
            };

        var random = new Random();

        for (int i = 0; i < titles.Length; i++)
        {
            int year = random.Next(1970, 2023); // películas entre 1970 y 2022
            float rating = (float)Math.Round(random.NextDouble() * 10, 1);
            string description = $"{titles[i]} - {descriptions[i % descriptions.Length]} Released in {year}, it remains a fan favorite.";

            movies.Add(new Movie(idCount++, titles[i], year, description, rating));
        }
    }

    public async Task<PagedResult<Movie>> ReadAll(int page, int size)
    {
        int totalCount = movies.Count;
        int start = Math.Clamp((page - 1) * size, 0, totalCount);
        int length = Math.Clamp(size, 0, totalCount - start);

        List<Movie> values = movies.GetRange(start, length);
        var pagedResult = new PagedResult<Movie>(values, totalCount);

        return await Task.FromResult(pagedResult);
    }

    public async Task<Movie?> Create(Movie newMovie)
    {
        newMovie.Id = idCount++;
        movies.Add(newMovie);

        return await Task.FromResult(newMovie);
    }

    public async Task<Movie?> Read(int id)
    {
        Movie? movie = movies.FirstOrDefault(u => u.Id == id);
        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Update(int id, Movie newMovie)
    {
        Movie? movie = movies.FirstOrDefault(u => u.Id == id);

        if (movie != null)
        {
            movie.Title = newMovie.Title;
            movie.Year = newMovie.Year;
            movie.Description = newMovie.Description;
            movie.Rating = newMovie.Rating;
        }

        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Delete(int id)
    {
        Movie? movie = movies.FirstOrDefault(u => u.Id == id);

        if (movie != null)
        {
            movies.Remove(movie);
        }

        return await Task.FromResult(movie);
    }
}



