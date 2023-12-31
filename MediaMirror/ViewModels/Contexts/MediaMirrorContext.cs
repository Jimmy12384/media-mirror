using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MediaMirror {
    public class MediaMirrorContext {
        public bool isDraggingSlider = false;
        public bool songUpdated = false;
        public string serverIP;

        public MainWindow Gui { get; set; }
        public List<Music> MusicLibrary { get; set; }
        public List<Music> FilteredLibrary{ get; set; }
        public Queue<Music> MusicQueue { get; set; }
        public AbsState CurrentState { get; set; }
        public Music currentSong = null;

        public MediaMirrorContext(MainWindow gui) {
            this.Gui = gui;
            MusicQueue = new Queue<Music>();
            MusicLibrary = new List<Music>();
        }

        public void AddMusic(Music m) { MusicQueue.Enqueue(m); }

        public Music NextSong() {
            return MusicQueue.Dequeue();
        }

        public Music PeekNext() {
            return MusicQueue.Peek();
        }

        public void DrawMusic(List<Music> music_library) {
            Gui.songListView.Items.Clear();
            foreach (Music song in music_library) {
                ListViewItem container = new ListViewItem();
                StackPanel horizontalStack = new StackPanel();
                Image album_cover = new Image();
                StackPanel verticalStack = new StackPanel();
                TextBlock song_name_tb = new TextBlock();
                TextBlock artist_tb = new TextBlock();
                Button add_button = new Button();
                //Create ListViewItem and style accordingly

                song_name_tb.Text = song.song;
                song_name_tb.Padding = new Thickness(10, 0, 0, 0);
                artist_tb.Padding = new Thickness(10, 0, 0, 0);
                artist_tb.Text = song.artist;

                //album_cover.Source = song.album_cover;
                song_name_tb.Height = 19;
                song_name_tb.Width = 217;
                album_cover.Height = 35;
                album_cover.Width = 35;

                verticalStack.Orientation = Orientation.Vertical;
                horizontalStack.Orientation = Orientation.Horizontal;

                verticalStack.Children.Add(song_name_tb);
                verticalStack.Children.Add(artist_tb);
                horizontalStack.Children.Add(album_cover);
                horizontalStack.Children.Add(verticalStack);
                horizontalStack.Children.Add(add_button);

                add_button.HorizontalAlignment = HorizontalAlignment.Right;
                add_button.VerticalAlignment = VerticalAlignment.Top;
                add_button.BorderBrush = null;
                add_button.FontSize = 24;
                add_button.Height = 36;
                add_button.Width = 26;
                add_button.VerticalContentAlignment = VerticalAlignment.Center;
                add_button.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                add_button.Background = null;
                add_button.Content = "+";
                container.Content = horizontalStack;
                container.Background = new SolidColorBrush(Color.FromArgb(255, 46, 46, 46));
                container.Height = 48;
                container.Tag = song;
                Gui.songListView.Items.Add(container);
            }
        }
    }
}
