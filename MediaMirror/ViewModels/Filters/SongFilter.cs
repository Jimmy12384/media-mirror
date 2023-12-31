using System.Collections.Generic;
using System.Linq;

namespace MediaMirror {
    public class SongFilter : IMusicFilter {
        public List<Music> GetData(List<Music> allMusic, string query) {
            return allMusic.Where(m => m.song != null && m.song.Contains(query)).ToList();
        }
    }
}
