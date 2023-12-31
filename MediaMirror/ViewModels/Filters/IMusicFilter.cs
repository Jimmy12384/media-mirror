using System.Collections.Generic;

namespace MediaMirror {
    public interface IMusicFilter {
        List<Music> GetData(List<Music> allMusic, string query);
    }
}
