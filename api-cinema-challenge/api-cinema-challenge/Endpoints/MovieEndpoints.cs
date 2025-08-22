using api_cinema_challenge.DTOs.MovieDTOs;
using api_cinema_challenge.DTOs.ScreeningDTOs;
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

            // screening endpoints
            movies.MapGet("/{id}/screenings", GetScreeningsForMovie);
            movies.MapPost("/{id}/screenings", AddScreeningForMovie);
            movies.MapGet("/{movieId}/screenings/{screeningId}", GetScreeningForMovie);

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
                RuntimeMins = targetMovie.RuntimeMins,
                CreatedAt = targetMovie.CreatedAt,
                UpdatedAt = targetMovie.UpdatedAt
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
                RuntimeMins = m.RuntimeMins,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
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

            var movieDto = new MovieDto { Id = addedMovie.Id, Title = addedMovie.Title, Rating = addedMovie.Rating, Description = addedMovie.Description, RuntimeMins = addedMovie.RuntimeMins, CreatedAt = addedMovie.CreatedAt, UpdatedAt = addedMovie.UpdatedAt };

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
                CreatedAt = deletedMovie.CreatedAt,
                UpdatedAt = deletedMovie.UpdatedAt
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
            if (!string.IsNullOrWhiteSpace(model.Title)) existingMovie.Title = model.Title;
            if (!string.IsNullOrWhiteSpace(model.Rating)) existingMovie.Rating = model.Rating;
            if (!string.IsNullOrWhiteSpace(model.Description)) existingMovie.Description = model.Description;
            if (model.RuntimeMins is not null) existingMovie.RuntimeMins = model.RuntimeMins.Value;

            // set UpdatedAt to now
            existingMovie.UpdatedAt = DateTime.UtcNow;

            var updatedMovie = await repository.Update(id, existingMovie);

            // generate respone dto
            var movieDto = new MovieDto { Id = updatedMovie.Id, Title = updatedMovie.Title, Rating = updatedMovie.Rating, Description = updatedMovie.Description, RuntimeMins = updatedMovie.RuntimeMins, CreatedAt = updatedMovie.CreatedAt, UpdatedAt = updatedMovie.UpdatedAt };

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var location = $"{baseUrl}/movies/{updatedMovie.Id}";
            return TypedResults.Created(location, movieDto);

        }

        // screening endpoints
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetScreeningsForMovie(int id, IRepository<Screening> screeningRepository)
        {
            var screenings = await screeningRepository.GetAll();
            var filteredScreenings = screenings.Where(s => s.MovieId == id).ToList();

            if (!filteredScreenings.Any())
                return TypedResults.NotFound($"No screenings found for movie with id {id}.");

            var screeningDtos = filteredScreenings.Select(s => new ScreeningDto
            {
                Id = s.Id,
                ScreenNumber = s.ScreenNumber,
                Capacity = s.Capacity,
                StartsAt = s.StartsAt,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return TypedResults.Ok(screeningDtos);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static async Task<IResult> AddScreeningForMovie(int id, IRepository<Screening> screeningRepository, [FromBody] ScreeningPostDto model, IValidator<ScreeningPostDto> validator, HttpRequest request)
        {
            if (model == null) { return TypedResults.BadRequest("Invalid screening data"); }
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return TypedResults.BadRequest(errors);
            }

            var newScreening = new Screening
            {
                ScreenNumber = model.ScreenNumber,
                Capacity = model.Capacity,
                StartsAt = model.StartsAt,
                MovieId = id
            };

            var addedScreening = await screeningRepository.Add(newScreening);

            var screeningDto = new ScreeningDto
            {
                Id = addedScreening.Id,
                ScreenNumber = addedScreening.ScreenNumber,
                Capacity = addedScreening.Capacity,
                StartsAt = addedScreening.StartsAt,
                CreatedAt = addedScreening.CreatedAt,
                UpdatedAt = addedScreening.UpdatedAt
            };

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var location = $"{baseUrl}/movies/{id}/screenings/{addedScreening.Id}";
            return TypedResults.Created(location, screeningDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetScreeningForMovie(int movieId, int screeningId, IRepository<Screening> screeningRepository)
        {
            var screening = await screeningRepository.GetById(screeningId);
            if (screening == null || screening.MovieId != movieId)
                return TypedResults.NotFound($"Screening with id {screeningId} for movie {movieId} not found.");

            var screeningDto = new ScreeningDto
            {
                Id = screening.Id,
                ScreenNumber = screening.ScreenNumber,
                Capacity = screening.Capacity,
                StartsAt = screening.StartsAt,
                CreatedAt = screening.CreatedAt,
                UpdatedAt = screening.UpdatedAt
            };

            return TypedResults.Ok(screeningDto);
        }
    }
}
