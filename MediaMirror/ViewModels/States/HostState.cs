using Microsoft.Win32;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace MediaMirror {
    public class HostState : AbsState {
        DispatcherTimer timer;
        Thread server_thread;
        public HostState(MediaMirrorContext ctx, AbsState parent) : base(ctx, parent) { }
        public override void Shutdown() {
            server_thread.Abort();
        }
        public override void ConnectButtonClick(object sender, RoutedEventArgs e) {
            Console.WriteLine("Restart program to change into client mode(we prefer just having two instances open for testing");
        }
        public override void AddMusicButtonClick(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Music Files | *.mp3; *.wma | MP3s | *.mp3 | WMAs | *.wma";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            MusicFile musicFile = new MusicFile();
            Music newMusic = musicFile.DispatchMedia();

            ctx.MusicLibrary.Add(newMusic);
            ctx.DrawMusic(ctx.MusicLibrary);
        }

        public override void Enter() {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ip = ipHostInfo.AddressList[0];
            ASyncSocketListener server = new ASyncSocketListener(ctx, ip);
            server_thread = new Thread(new ThreadStart(server.StartListening));
            server_thread.Start();
            ctx.Gui.mediaElement.LoadedBehavior = MediaState.Manual;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.03);
            timer.Tick += TimerTick;
            timer.Start();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Music Files | *.mp3; *.wma | MP3s | *.mp3 | WMAs | *.wma";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            MusicFile musicFile = new MusicFile();
            Music newMusic = musicFile.DispatchMedia();

            ctx.MusicLibrary.Add(newMusic);
            ctx.DrawMusic(ctx.MusicLibrary);
        }
        private void TimerTick(object sender, EventArgs e) {
            if (ctx.Gui.mediaElement.Source != null && ctx.Gui.mediaElement.NaturalDuration.HasTimeSpan && !ctx.isDraggingSlider) {
                UpdateHost();
            }
        }
        public override void MusicSliderDragEnded(object sender, DragCompletedEventArgs e) {
            ctx.currentSong.isDraggingSlider = false;
            if (ctx.Gui.musicSwitchButton.Content.Equals("Pause"))
                ctx.Gui.mediaElement.Play();
        }

        public override void MusicSliderDragStarted(object sender, DragStartedEventArgs e) {
            ctx.currentSong.isDraggingSlider = true;
        }

        public override void MusicSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (ctx.currentSong != null) {
                ctx.currentSong.current_elapsed_time = ctx.currentSong.duration * e.NewValue / 1000;
                ctx.Gui.mediaElement.Pause();
                ctx.Gui.mediaElement.Position = TimeSpan.FromMilliseconds(ctx.currentSong.current_elapsed_time);
                if (ctx.Gui.musicSwitchButton.Content.Equals("Pause") && !ctx.currentSong.isDraggingSlider) // is playing
                    ctx.Gui.mediaElement.Play();
                UpdateHost();
            }
        }

        public override void MusicSwitchButtonClick(object sender, RoutedEventArgs e) {
            if (ctx.Gui.musicSwitchButton.Content.Equals("Play")) {
                ctx.Gui.musicSwitchButton.Content = "Pause";
                if(ctx.currentSong != null) {
                    ctx.currentSong.isPaused = false;
                    ctx.Gui.mediaElement.Play();
                }
            } else {
                ctx.Gui.musicSwitchButton.Content = "Play";
                ctx.currentSong.isPaused = true;
                ctx.Gui.mediaElement.Pause();
            }
        }

        public override void SongListViewDoubleClick(object sender, MouseButtonEventArgs e) {
            this.ctx.currentSong = (Music)((ListViewItem)ctx.Gui.songListView.SelectedItem).Tag;
            Console.WriteLine(ctx.currentSong);
            ctx.Gui.musicSlider.Value = 0;
            ctx.Gui.mediaElement.Source = new Uri(ctx.currentSong.filename);
            UpdateHost();
        }

        public override void KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                ctx.FilteredLibrary = LibraryFilterFactory.GetLibraryFilter(ctx.Gui.searchTypeCombo.Text).GetData(ctx.MusicLibrary, ctx.Gui.searchBox.Text);
                if (ctx.FilteredLibrary.Count == 0) {
                    Console.WriteLine("No songs search again.");
                } else {
                    Console.WriteLine(ctx.FilteredLibrary.ElementAt(0).artist);
                }
                ctx.DrawMusic(ctx.FilteredLibrary);
            }
        }
    }
}
