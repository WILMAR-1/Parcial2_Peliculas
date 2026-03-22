using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Parcial2_Peliculas.Helpers;
using Parcial2_Peliculas.Models;

namespace Parcial2_Peliculas.ViewModels
{
    public class PeliculasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly HttpClient httpClient;
        private static readonly string API_KEY = AppConfig.Get("TmdbApiKey");
        private const string BASE_URL = "https://api.themoviedb.org/3/search/movie";
        private const string POPULAR_URL = "https://api.themoviedb.org/3/movie/popular";

        // Colecciones
        public ObservableCollection<Movie> Peliculas { get; set; }

        // Texto de busqueda
        private string textoBusqueda;
        public string TextoBusqueda
        {
            get => textoBusqueda;
            set
            {
                textoBusqueda = value;
                OnPropertyChanged();
            }
        }

        // Indicador de carga
        private bool estaBuscando;
        public bool EstaBuscando
        {
            get => estaBuscando;
            set
            {
                estaBuscando = value;
                OnPropertyChanged();
            }
        }

        // Pelicula seleccionada
        private Movie peliculaSeleccionada;
        public Movie PeliculaSeleccionada
        {
            get => peliculaSeleccionada;
            set
            {
                peliculaSeleccionada = value;
                OnPropertyChanged();
                if (value != null)
                {
                    MostrarDetalles(value);
                }
            }
        }

        // Commands
        public ICommand BuscarCommand { get; set; }

        public PeliculasViewModel()
        {
            httpClient = new HttpClient();
            Peliculas = new ObservableCollection<Movie>();
            BuscarCommand = new RelayCommand(async () => await BuscarPeliculas());
            _ = CargarPopulares();
        }

        private async Task CargarPopulares()
        {
            EstaBuscando = true;

            try
            {
                string url = $"{POPULAR_URL}?api_key={API_KEY}&language=es-ES&page=1";
                var response = await httpClient.GetFromJsonAsync<MovieSearchResponse>(url);

                if (response?.Results != null)
                {
                    foreach (var pelicula in response.Results)
                    {
                        Peliculas.Add(pelicula);
                    }
                }
            }
            catch
            {
                // Silencioso al inicio, no molestar al usuario
            }
            finally
            {
                EstaBuscando = false;
            }
        }

        private async Task BuscarPeliculas()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Ingresa el nombre de una pelicula",
                    "OK");
                return;
            }

            EstaBuscando = true;
            Peliculas.Clear();

            try
            {
                string url = $"{BASE_URL}?api_key={API_KEY}&query={TextoBusqueda}&language=es-ES";

                var response = await httpClient.GetFromJsonAsync<MovieSearchResponse>(url);

                if (response?.Results != null && response.Results.Count > 0)
                {
                    foreach (var pelicula in response.Results)
                    {
                        Peliculas.Add(pelicula);
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Sin Resultados",
                        "No se encontraron peliculas con ese nombre",
                        "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error de Red",
                    $"No se pudo conectar a la API: {ex.Message}",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Ocurrio un error: {ex.Message}",
                    "OK");
            }
            finally
            {
                EstaBuscando = false;
            }
        }

        private async void MostrarDetalles(Movie pelicula)
        {
            await Application.Current.MainPage.DisplayAlert(
                pelicula.Title,
                $"Ano: {pelicula.ReleaseDate}\n" +
                $"Calificacion: {pelicula.VoteAverage}/10\n\n" +
                $"{pelicula.Overview}",
                "OK");

            PeliculaSeleccionada = null;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
