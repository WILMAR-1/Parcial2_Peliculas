using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Parcial2_Peliculas.Helpers;
using Parcial2_Peliculas.Models;

namespace Parcial2_Peliculas.ViewModels
{
    public class FavoritosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Movie> Favoritos { get; set; }

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

        public ICommand EliminarCommand { get; set; }

        public FavoritosViewModel()
        {
            Favoritos = new ObservableCollection<Movie>();
            EliminarCommand = new RelayCommand<Movie>(Eliminar);
            FavoritosService.OnFavoritosChanged += CargarFavoritos;
            CargarFavoritos();
        }

        public void CargarFavoritos()
        {
            Favoritos.Clear();
            var lista = FavoritosService.Cargar();
            foreach (var movie in lista)
                Favoritos.Add(movie);
        }

        private void Eliminar(Movie movie)
        {
            if (movie == null) return;
            FavoritosService.Eliminar(movie.Id);
        }

        private async void MostrarDetalles(Movie pelicula)
        {
            await Application.Current.MainPage.DisplayAlert(
                pelicula.Title,
                $"Ano: {pelicula.ReleaseDate}\nCalificacion: {pelicula.VoteAverage}/10\n\n{pelicula.Overview}",
                "OK");
            PeliculaSeleccionada = null;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
