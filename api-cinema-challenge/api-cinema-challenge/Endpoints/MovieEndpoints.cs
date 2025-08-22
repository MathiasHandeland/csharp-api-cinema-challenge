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
            movies.MapPut("/{id}", UpdateMovie);

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

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> UpdateMovie(int id, IRepository<Movie> repository, [FromBody] MoviePutDto model, IValidator<MoviePutDto> validator, HttpRequest request)
        {
            // check if the movie we want to update exists
            var existingMovie = await repository.GetById(id);
            if (existingMovie == null) { return TypedResults.NotFound($"The movie you want to update with ID {id} does not exist"); }

            if (model == null) { return TypedResults.BadRequest("Invalid movie data"); }

            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return TypedResults.BadRequest(errors);
            }

            // check if the new title already exists for another movie
            var allMovies = await repository.GetAll();
            var duplicateTitleMovie = allMovies.FirstOrDefault(
                m => m.Title == model.Title && m.Id != id);
            if (duplicateTitleMovie != null) { return TypedResults.BadRequest($"A movie with the title '{model.Title}' already exists."); }

            // update the movie
            if (model.Title is not null) existingMovie.Title = model.Title;
            if (model.Rating is not null) existingMovie.Rating = model.Rating;
            if (model.Description is not null) existingMovie.Description = model.Description;
            if (model.RuntimeMins is not null) existingMovie.RuntimeMins = model.RuntimeMins.Value;

            var updatedMovie = await repository.Update(id, existingMovie);

            // generate respone dto
            var movieDto = new MovieDto { Id = updatedMovie.Id, Title = updatedMovie.Title, Rating = updatedMovie.Rating, Description = updatedMovie.Description, RuntimeMins = updatedMovie.RuntimeMins };

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var location = $"{baseUrl}/movies/{updatedMovie.Id}";
            return TypedResults.Created(location, movieDto);

        }
    }
}
