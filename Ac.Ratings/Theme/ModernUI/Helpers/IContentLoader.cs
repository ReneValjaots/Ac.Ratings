namespace Ac.Ratings.Theme.ModernUI.Helpers {
    public interface IContentLoader {
        Task<object> LoadContentAsync(Uri uri, CancellationToken cancellationToken);
    }
}
