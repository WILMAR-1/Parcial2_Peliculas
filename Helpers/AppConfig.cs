using System.Reflection;
using System.Text.Json;

namespace Parcial2_Peliculas.Helpers
{
    public static class AppConfig
    {
        private static Dictionary<string, string> _settings;

        public static string Get(string key)
        {
            if (_settings == null)
                Load();

            return _settings.TryGetValue(key, out var value) ? value : string.Empty;
        }

        private static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Parcial2_Peliculas.appsettings.json");

            if (stream == null)
            {
                _settings = new Dictionary<string, string>();
                return;
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            _settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                        ?? new Dictionary<string, string>();
        }
    }
}
