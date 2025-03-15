using System.Windows.Controls;

namespace Ac.Ratings.Theme.ModernUI.Helpers {
    public class DefaultContentLoader : IContentLoader {
        private readonly string _namespacePrefix;
        private readonly Func<string, string> _uriParser;

        public DefaultContentLoader(string namespacePrefix = "Ac.Ratings.Theme.Components",
            Func<string, string>? uriParser = null) {
            _namespacePrefix = namespacePrefix ?? throw new ArgumentNullException(nameof(namespacePrefix));
            _uriParser = uriParser ?? (uri => uri.TrimStart('/').Replace("Theme/Components/", "").Replace(".xaml", ""));
        }

        public Task<object> LoadContentAsync(Uri uri, CancellationToken cancellationToken) {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            cancellationToken.ThrowIfCancellationRequested();

            string pageName = _uriParser(uri.OriginalString);
            string typeName = $"{_namespacePrefix}.{pageName}";
            var type = Type.GetType(typeName);

            if (type != null && typeof(UserControl).IsAssignableFrom(type)) {
                System.Diagnostics.Debug.WriteLine($"Loaded content for URI: {uri.OriginalString}, Type: {typeName}");
                return Task.FromResult(Activator.CreateInstance(type));
            }

            System.Diagnostics.Debug.WriteLine($"Failed to load content for URI: {uri.OriginalString}, Type: {typeName}");
            return Task.FromResult<object>(null);
        }
    }
}
