using System.Text.Json;
using Parcial2_Peliculas.Models;

namespace Parcial2_Peliculas.Helpers
{
    public static class FavoritosService
    {
        private static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "favoritos.json");

        public static event Action OnFavoritosChanged;

        public static List<Movie> Cargar()
        {
            if (!File.Exists(FilePath))
                return new List<Movie>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Movie>>(json) ?? new List<Movie>();
        }

        public static void Guardar(List<Movie> favoritos)
        {
            var json = JsonSerializer.Serialize(favoritos);
            File.WriteAllText(FilePath, json);
            OnFavoritosChanged?.Invoke();
        }

        public static bool EsFavorito(int movieId)
        {
            var favoritos = Cargar();
            return favoritos.Any(f => f.Id == movieId);
        }

        public static void Agregar(Movie movie)
        {
            var favoritos = Cargar();
            if (!favoritos.Any(f => f.Id == movie.Id))
            {
                favoritos.Add(movie);
                Guardar(favoritos);
            }
        }

        public static void Eliminar(int movieId)
        {
            var favoritos = Cargar();
            favoritos.RemoveAll(f => f.Id == movieId);
            Guardar(favoritos);
        }
    }
}
