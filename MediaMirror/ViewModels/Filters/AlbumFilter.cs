using System.Collections.Generic;
using System.Linq;

namespace MediaMirror {
    public class AlbumFilter : IMusicFilter {
        public List<Music> GetData(List<Music> allMusic, string query) {
            return allMusic.Where(m => m.album != null && m.album.Contains(query)).ToList();
        }
    }
}
