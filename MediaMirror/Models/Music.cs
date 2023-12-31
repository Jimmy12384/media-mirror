using System;

namespace MediaMirror {
    [Serializable]
    public class Music {
        public string album { get; set; }
        public int bitrate;
        public string artist { get; set; }
        public string song { get; set; }
        public byte[] packet { get; set; }
        public double current_elapsed_time;
        public double duration;
        public bool isDraggingSlider = false;

        public DateTime system_time { get; set; }
        public string uri { get; set; }
        public string filename { get; set; }
        public bool isPaused = true;
    }
    
}
