CREATE TABLE IF NOT EXISTS Actors 
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    firstname VARCHAR(64) NOT NULL,
    lastname VARCHAR(64) NOT NULL,
    bio TEXT,
    rating FLOAT
);

CREATE TABLE IF NOT EXISTS Movies
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(256) NOT NULL,
    year INT NOT NULL,
    description TEXT,
    rating FLOAT
);


CREATE TABLE IF NOT EXISTS ActorsMovies 
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    actorId INT NOT NULL,
    movieId INT NOT NULL,
    rolename TEXT,
    FOREIGN KEY (actorId) REFERENCES Actors(id) ON DELETE CASCADE,
    FOREIGN KEY (movieId) REFERENCES Movies(id) ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS Users 
(
    id int AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(64) NOT NULL UNIQUE,
    password VARCHAR(64) NOT NULL,
    salt VARCHAR(64) NOT NULL,
    role ENUM('Admin', 'User') NOT NULL
);








