using api_cinema_challenge.DTOs.CustomerDTOs;
using api_cinema_challenge.DTOs.MovieDTOs;
using api_cinema_challenge.Models;
using api_cinema_challenge.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace api_cinema_challenge.Endpoints
{
    public static class MovieEndpoints
    {
        public static void ConfigureMovieEndpoint(this WebApplication app)
        {
            var movies = app.MapGroup("movies");

            movies.MapGet("/{id}", GetMovieById);
            movies.MapGet("/", GetMovies);
            movies.MapPost("/", AddMovie);
            movies.MapDelete("/{id}", DeleteMovie);

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetMovieById(int id, IRepository<Movie> repository)
        {
            var targetMovie = await repository.GetById(id);
            if (targetMovie == null) { return TypedResults.NotFound($"Movie with id {id} not found."); }

            var movieDto = new MovieDto
            {
                Id = targetMovie.Id,
                Title = targetMovie.Title,
                Rating = targetMovie.Rating,
                Description = targetMovie.Description,
                RuntimeMins = targetMovie.RuntimeMins
            };

            return TypedResults.Ok(movieDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetMovies(IRepository<Movie> repository)
        {
            var movies = await repository.GetAll();
            if (movies == null || !movies.Any()) { return Results.NotFound("No movies found."); }
            var movieDtos = movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                Rating = m.Rating,
                Description = m.Description,
                RuntimeMins = m.RuntimeMins
            }).ToList();

            return TypedResults.Ok(movieDtos);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static async Task<IResult> AddMovie(IRepository<Movie> repository, [FromBody] MoviePostDto model, HttpRequest request, IValidator<MoviePostDto> validator)
        {
            if (model == null) { return TypedResults.BadRequest("Invalid movie data"); }

            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return TypedResults.BadRequest(errors);
            }

            var newMovie = new Movie { Title = model.Title, Rating = model.Rating, Description = model.Description, RuntimeMins = model.RuntimeMins };
            var addedMovie = await repository.Add(newMovie);

            var movieDto = new MovieDto { Id = addedMovie.Id, Title = addedMovie.Title, Rating = addedMovie.Rating, Description = addedMovie.Description, RuntimeMins = addedMovie.RuntimeMins };

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var location = $"{baseUrl}/movies/{addedMovie.Id}";
            return TypedResults.Created(location, movieDto);

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> DeleteMovie(int id, IRepository<Movie> repository)
        {
            var targetMovie = await repository.GetById(id);
            if (targetMovie == null) { return TypedResults.NotFound($"Movie with id {id} not found."); }

            var deletedMovie = await repository.Delete(id);

            var movieDto = new MovieDto
            {
                Id = deletedMovie.Id,
                Title = deletedMovie.Title,
                Rating = deletedMovie.Rating,
                Description = deletedMovie.Description,
                RuntimeMins = deletedMovie.RuntimeMins,
            };

            return TypedResults.Ok(movieDto);
        }
    }
}
