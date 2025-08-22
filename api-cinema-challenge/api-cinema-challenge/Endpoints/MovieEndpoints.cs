namespace api_cinema_challenge.Endpoints
{
    public static class MovieEndpoints
    {
        public static void ConfigureMovieEndpoint(this WebApplication app)
        {
            var movies = app.MapGroup("movies");

            //movies.MapGet("/{id}", GetMovieById);
           
        }
    }
}
