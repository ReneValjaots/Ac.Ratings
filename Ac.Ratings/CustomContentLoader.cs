using System.Windows.Controls;
using Ac.Ratings.Theme.ModernUI.Helpers;

namespace Ac.Ratings;

public class CustomContentLoader : IContentLoader {
    public Task<object> LoadContentAsync(Uri uri, CancellationToken cancellationToken) {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        // Extract the page name from the URI (e.g., "/Pages/General.xaml" -> "General")
        string pageName = uri.OriginalString.TrimStart('/').Replace("Theme/Components/", "").Replace(".xaml", "");
        string namespacePart = "Ac.Ratings.Theme.Components";
        string typeName = $"{namespacePart}.{pageName}";

        // Debug logging to verify URI and type resolution
        System.Diagnostics.Debug.WriteLine($"Loading URI: {uri.OriginalString}, Type: {typeName}");

        var type = Type.GetType(typeName);
        if (type != null && typeof(UserControl).IsAssignableFrom(type)) {
            System.Diagnostics.Debug.WriteLine($"Successfully resolved type: {typeName}");
            return Task.FromResult(Activator.CreateInstance(type));
        }

        System.Diagnostics.Debug.WriteLine($"Failed to resolve type: {typeName}");
        return Task.FromResult<object>(null);
    }
}