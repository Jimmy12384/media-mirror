namespace MediaMirror {
    public class LibraryFilterFactory {
        public static IMusicFilter GetLibraryFilter(string filterValue) {
            if (filterValue.Equals("Artist")) {
                return new ArtistFilter();
            } else if (filterValue.Equals("Album")) {
                return new AlbumFilter();
            }
            return new SongFilter();
        }
    }
}
