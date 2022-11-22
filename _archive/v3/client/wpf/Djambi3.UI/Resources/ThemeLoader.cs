using System.IO;
using System.Text.Json;

namespace Djambi.UI.Resources
{
    public delegate void Notify();

    public interface IThemeLoader
    {
        Theme Theme { get; }

        void LoadTheme(string name);

        event Notify ThemeLoaded;
    }

    class ThemeLoader : IThemeLoader
    {
        public Theme Theme { get; private set; }

        public void LoadTheme(string name)
        {
            var path = $"{Directory.GetCurrentDirectory()}/Resources/{name}/keys.json";
            var json = File.ReadAllText(path);
            Theme = JsonSerializer.Deserialize<Theme>(json);
            ThemeLoaded?.Invoke();
        }

        public event Notify ThemeLoaded;
    }
}
