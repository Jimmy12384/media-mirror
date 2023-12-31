using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MediaMirror {
    public abstract class AbsState {
        public MediaMirrorContext ctx { get; set; }
        public AbsState parent {get; set;}

        public AbsState(MediaMirrorContext ctx, AbsState parent) {
            this.ctx = ctx;
            this.parent = parent;
        }
        public virtual void Enter() { return; }
        public virtual void MusicSwitchButtonClick(object sender, RoutedEventArgs e) { return; }
        public virtual void AddMusicButtonClick(object sender, RoutedEventArgs e) { return; }
        public virtual void SongListViewDoubleClick(object sender, MouseButtonEventArgs e) { return; }
        public virtual void MusicSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { return; }
        public virtual void MusicSliderDragEnded(object sender, DragCompletedEventArgs e) { return; }
        public virtual void MusicSliderDragStarted(object sender, DragStartedEventArgs e) { return; }
        public virtual void KeyDown(object sender, KeyEventArgs e) { return; }
        public virtual void Shutdown() { return; }
        public virtual void ConnectButtonClick(object sender, RoutedEventArgs e) { return; }

        public void UpdateHost() {
            ctx.currentSong.current_elapsed_time = ctx.Gui.mediaElement.Position.TotalMilliseconds;
            ctx.Gui.musicSlider.Value = ctx.currentSong.current_elapsed_time / ctx.currentSong.duration * 1000;
            ctx.Gui.songNameLabel.Content = ctx.currentSong.song;
            ctx.Gui.songInfoLabel.Content = ctx.currentSong.album != null ? ctx.currentSong.artist + "-" + ctx.currentSong.album : ctx.currentSong.artist;

            ctx.Gui.timeElapsed.Content = Convert.ToInt32(TimeSpan.FromMilliseconds(ctx.currentSong.current_elapsed_time).Minutes) + ":" + ((TimeSpan.FromMilliseconds(ctx.currentSong.current_elapsed_time).Seconds) < 10 ? ("0" + Convert.ToInt32(TimeSpan.FromMilliseconds(ctx.currentSong.current_elapsed_time).Seconds)) : Convert.ToInt32(TimeSpan.FromMilliseconds(ctx.currentSong.current_elapsed_time).Seconds) + "");
            double time_left = ctx.currentSong.duration - ctx.currentSong.current_elapsed_time;
            if (ctx.Gui.mediaElement.NaturalDuration.HasTimeSpan)
                time_left = ctx.Gui.mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds - ctx.currentSong.current_elapsed_time;
            if (!ctx.isDraggingSlider) {
                string str_ = TimeSpan.FromMilliseconds(ctx.currentSong.duration).Seconds < 10 ? ("0" + Convert.ToInt32(TimeSpan.FromMilliseconds(ctx.currentSong.duration).Seconds)) : Convert.ToInt32(TimeSpan.FromMilliseconds(ctx.currentSong.duration).Seconds) + "";
                ctx.Gui.timeLeft.Content = Convert.ToInt32(TimeSpan.FromMilliseconds(ctx.currentSong.duration).Minutes) + ":" + str_;
            } else {
                string str = TimeSpan.FromMilliseconds(time_left).Seconds < 10 ? ("0" + Convert.ToInt32(TimeSpan.FromMilliseconds(time_left).Seconds)) : Convert.ToInt32(TimeSpan.FromMilliseconds(time_left).Seconds) + "";
                ctx.Gui.timeLeft.Content = Convert.ToInt32(TimeSpan.FromMilliseconds(time_left).Minutes) + ":" + str;
            }
            ctx.Gui.ipLabel.Content = ctx.serverIP;
        }
    }
}
