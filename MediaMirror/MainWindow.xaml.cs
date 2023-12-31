using System.Windows;

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MediaMirror {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        MediaMirrorContext ctx;
        public MainWindow() {
            InitializeComponent();
            ctx = new MediaMirrorContext(this);
            AbsState initial_state = new IdleState(ctx, null);
            ctx.CurrentState = initial_state;
            ctx.CurrentState.Enter();
        }
       
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e) {
            if(searchBox.Text == "Search...") 
                searchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e) {
            if (searchBox.Text == "")
                searchBox.Text = "Search...";
        }

        private void AddMusicButton_Click(object sender, RoutedEventArgs e) {
            ctx.CurrentState.AddMusicButtonClick(sender, e);
        }

        private void MusicSwitchButton_Click(object sender, RoutedEventArgs e) {
            ctx.CurrentState.MusicSwitchButtonClick(sender, e);
        }

        private void SongListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            ctx.CurrentState.SongListViewDoubleClick(sender, e);
        }

        private void MusicSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ctx.CurrentState.MusicSliderValueChanged(sender, e);
        }

        private void MusicSlider_DragEnded(object sender, DragCompletedEventArgs e) {
            ctx.CurrentState.MusicSliderDragEnded(sender, e);
        }

        private void MusicSlider_DragStarted(object sender, DragStartedEventArgs e) {
            ctx.CurrentState.MusicSliderDragStarted(sender, e);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e) {
            ctx.CurrentState.ConnectButtonClick(sender, e);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            ctx.CurrentState.Shutdown();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e) {
            ctx.CurrentState.KeyDown(sender, e);
        }

        private void SongListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }
    }
}
