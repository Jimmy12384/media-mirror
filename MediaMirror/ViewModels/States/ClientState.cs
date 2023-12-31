using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MediaMirror {

    public class ClientState : AbsState {
        DispatcherTimer timer;
        Music prev;
        Thread clientThread;
        byte[] entireSong = null;
        public ClientState(MediaMirrorContext ctx, AbsState parent) : base(ctx, parent) { }

        public override void ConnectButtonClick(object sender, RoutedEventArgs e) {
            Enter();
        }

        private void TimerTick(object sender, EventArgs e) {
            if (ctx.currentSong != null && ctx.songUpdated && ctx.currentSong.packet != null) {
                if (prev != null && !prev.song.Equals(ctx.currentSong.song)) {
                    entireSong = new byte[Convert.ToInt32(ctx.currentSong.duration * ctx.currentSong.bitrate)];
                    for (int i = 0; i < entireSong.Length; i++) {
                        entireSong[i] = 0x20;
                    }
                    string new_uri = "client.mp3";
                    Console.WriteLine("new uri: " + new_uri);
                    File.WriteAllBytes(new_uri, entireSong);
                    ctx.Gui.mediaElement.Source = new Uri(ctx.currentSong.uri);
                }

                //ctx.gui.mediaElement.Position = TimeSpan.FromMilliseconds(ctx.currentSong.current_elapsed_time);
                ctx.Gui.mediaElement.LoadedBehavior = MediaState.Manual;
                ctx.Gui.mediaElement.Play();
                if (ctx.currentSong.isDraggingSlider)
                    ctx.Gui.mediaElement.Position = TimeSpan.FromMilliseconds(ctx.currentSong.current_elapsed_time);
                ctx.Gui.mediaElement.LoadedBehavior = MediaState.Manual;
                if (ctx.currentSong.isPaused) {
                    ctx.Gui.mediaElement.Pause();
                    ctx.Gui.mediaElement.Position = TimeSpan.FromMilliseconds(ctx.currentSong.current_elapsed_time);
                } else
                    ctx.Gui.mediaElement.Play();
                ctx.songUpdated = false;


            }
            if (ctx.Gui.mediaElement.Source != null && ctx.Gui.mediaElement.NaturalDuration.HasTimeSpan && !ctx.isDraggingSlider) {
                UpdateHost();
            }
            prev = ctx.currentSong;
        }
        public override void Enter() {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ip = ipHostInfo.AddressList[0];
            new MusicFetcher(this.ctx, this.clientThread).fetchMusic();

            Console.WriteLine("Continuing to search for music");
            while (ctx.currentSong == null) ;
            Console.WriteLine("Song found and loaded into context");
            entireSong = Enumerable.Repeat<byte>(0x20, Convert.ToInt32(this.ctx.currentSong.duration * ctx.currentSong.bitrate)).ToArray();

            for (int i = 0; i < ctx.currentSong.packet.Length; i++) {
                entireSong[Convert.ToInt32(ctx.currentSong.current_elapsed_time / 1000 * ctx.currentSong.bitrate) + i] = ctx.currentSong.packet[i];
            }

            string new_uri = "client.mp3";
            Console.WriteLine("new uri: " + new_uri);
            File.WriteAllBytes(new_uri, entireSong);
            ctx.Gui.mediaElement.Source = new Uri(ctx.currentSong.uri);
            ctx.Gui.mediaElement.LoadedBehavior = MediaState.Manual;
            ctx.songUpdated = true;
            ctx.Gui.mediaElement.Position = TimeSpan.FromMilliseconds(ctx.currentSong.current_elapsed_time);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.03);
            timer.Tick += TimerTick;
            timer.Start();
        }

        public override void KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                ctx.FilteredLibrary = LibraryFilterFactory.GetLibraryFilter(ctx.Gui.searchTypeCombo.Text).GetData(ctx.MusicLibrary, ctx.Gui.searchBox.Text);
            }
        }
    }
}
