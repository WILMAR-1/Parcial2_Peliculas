using System.Text.Json.Serialization;

namespace Parcial2_Peliculas.Models
{
    public class MovieSearchResponse
    {
        [JsonPropertyName("results")]
        public List<Movie> Results { get; set; }
    }

    public class Movie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        // Propiedad calculada para URL completa del poster
        public string PosterUrl => string.IsNullOrEmpty(PosterPath)
            ? "https://via.placeholder.com/500x750?text=Sin+Imagen"
            : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}
