using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Parcial2_Peliculas.Helpers;
using Parcial2_Peliculas.Models;

namespace Parcial2_Peliculas.ViewModels
{
    public class BuscadorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly HttpClient httpClient;
        private static readonly string API_KEY = AppConfig.Get("TmdbApiKey");
        private const string SEARCH_URL = "https://api.themoviedb.org/3/search/movie";
        private const string DISCOVER_URL = "https://api.themoviedb.org/3/discover/movie";

        public ObservableCollection<Movie> Peliculas { get; set; }

        private string textoBusqueda;
        public string TextoBusqueda
        {
            get => textoBusqueda;
            set { textoBusqueda = value; OnPropertyChanged(); }
        }

        private bool estaBuscando;
        public bool EstaBuscando
        {
            get => estaBuscando;
            set { estaBuscando = value; OnPropertyChanged(); }
        }

        private Movie peliculaSeleccionada;
        public Movie PeliculaSeleccionada
        {
            get => peliculaSeleccionada;
            set
            {
                peliculaSeleccionada = value;
                OnPropertyChanged();
                if (value != null) MostrarDetalles(value);
            }
        }

        public ICommand BuscarCommand { get; set; }
        public ICommand FiltrarCommand { get; set; }

        public BuscadorViewModel()
        {
            httpClient = new HttpClient();
            Peliculas = new ObservableCollection<Movie>();
            BuscarCommand = new RelayCommand(async () => await BuscarPeliculas());
            FiltrarCommand = new RelayCommand<string>(async (genreId) => await FiltrarPorGenero(genreId));
        }

        private async Task BuscarPeliculas()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Ingresa el nombre de una pelicula", "OK");
                return;
            }

            EstaBuscando = true;
            Peliculas.Clear();

            try
            {
                string url = $"{SEARCH_URL}?api_key={API_KEY}&query={TextoBusqueda}&language=es-ES";
                var response = await httpClient.GetFromJsonAsync<MovieSearchResponse>(url);

                if (response?.Results != null && response.Results.Count > 0)
                {
                    foreach (var pelicula in response.Results)
                        Peliculas.Add(pelicula);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Sin Resultados", "No se encontraron peliculas", "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error de Red", $"No se pudo conectar: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrio un error: {ex.Message}", "OK");
            }
            finally
            {
                EstaBuscando = false;
            }
        }

        private async Task FiltrarPorGenero(string genreId)
        {
            EstaBuscando = true;
            Peliculas.Clear();

            try
            {
                string url = $"{DISCOVER_URL}?api_key={API_KEY}&with_genres={genreId}&language=es-ES&sort_by=popularity.desc";
                var response = await httpClient.GetFromJsonAsync<MovieSearchResponse>(url);

                if (response?.Results != null)
                {
                    foreach (var pelicula in response.Results)
                        Peliculas.Add(pelicula);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrio un error: {ex.Message}", "OK");
            }
            finally
            {
                EstaBuscando = false;
            }
        }

        private async void MostrarDetalles(Movie pelicula)
        {
            var action = await Application.Current.MainPage.DisplayActionSheet(
                pelicula.Title,
                "Cerrar",
                null,
                "Ver detalles",
                "Agregar a favoritos");

            if (action == "Ver detalles")
            {
                await Application.Current.MainPage.DisplayAlert(
                    pelicula.Title,
                    $"Ano: {pelicula.ReleaseDate}\nCalificacion: {pelicula.VoteAverage}/10\n\n{pelicula.Overview}",
                    "OK");
            }
            else if (action == "Agregar a favoritos")
            {
                if (FavoritosService.EsFavorito(pelicula.Id))
                {
                    await Application.Current.MainPage.DisplayAlert("Info", "Ya esta en tus favoritos", "OK");
                }
                else
                {
                    FavoritosService.Agregar(pelicula);
                    await Application.Current.MainPage.DisplayAlert("Listo", $"{pelicula.Title} agregada a favoritos", "OK");
                }
            }

            PeliculaSeleccionada = null;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
