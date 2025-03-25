namespace Ac.Ratings.Theme.ModernUI.Helpers {
    public static class NavigationHelper {
        public static Uri RemoveFragment(Uri uri) {
            string fragment;
            return RemoveFragment(uri, out fragment);
        }

        public static Uri RemoveFragment(Uri uri, out string fragment) {
            fragment = null;

            if (uri != null) {
                var value = uri.OriginalString;

                var i = value.IndexOf('#');
                if (i != -1) {
                    fragment = value.Substring(i + 1);
                    uri = new Uri(value.Substring(0, i), uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
                }
            }

            return uri;
        }
    }
}
