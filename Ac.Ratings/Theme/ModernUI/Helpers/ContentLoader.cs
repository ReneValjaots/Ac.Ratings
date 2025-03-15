using System.Windows.Controls;

namespace Ac.Ratings.Theme.ModernUI.Helpers;

public class ContentLoader : IContentLoader {
    private readonly string _namespacePrefix = "Ac.Ratings.Theme.Components";
    private readonly Func<string, string> _uriParser;

    public ContentLoader() {
        _uriParser = uri => uri.TrimStart('/').Replace("Theme/Components/", "").Replace(".xaml", "");
    }

    public Task<object?> LoadContentAsync(Uri uri, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(uri, nameof(uri));
        cancellationToken.ThrowIfCancellationRequested();

        string pageName = _uriParser(uri.OriginalString);
        string typeName = $"{_namespacePrefix}.{pageName}";

        var type = Type.GetType(typeName);
        if (type != null && typeof(UserControl).IsAssignableFrom(type)) {
            return Task.FromResult(Activator.CreateInstance(type));
        }
        return Task.FromResult<object?>(null);
    }
}