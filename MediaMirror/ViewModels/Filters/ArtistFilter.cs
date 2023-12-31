using System.Collections.Generic;
using System.Linq;

namespace MediaMirror {
    public class ArtistFilter : IMusicFilter {
        public List<Music> GetData(List<Music> allMusic, string query) {
            return allMusic.Where(m => m.artist != null && m.artist.Contains(query)).ToList();
        }
    }
}
