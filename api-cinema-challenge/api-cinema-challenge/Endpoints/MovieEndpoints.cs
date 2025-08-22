using api_cinema_challenge.DTOs.MovieDTOs;
using api_cinema_challenge.Models;
using api_cinema_challenge.Repository;

namespace api_cinema_challenge.Endpoints
{
    public static class MovieEndpoints
    {
        public static void ConfigureMovieEndpoint(this WebApplication app)
        {
            var movies = app.MapGroup("movies");

            movies.MapGet("/{id}", GetMovieById);
           
        }

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
    }
}
