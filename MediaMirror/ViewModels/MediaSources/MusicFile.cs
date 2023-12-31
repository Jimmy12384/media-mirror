using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace MediaMirror {
    public class MusicFile {
        public Music DispatchMedia() {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Multiselect = true,
                Filter = "Music Files | *.mp3; *.wma | MP3s | *.mp3 | WMAs | *.wma",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
            };
            if (openFileDialog.ShowDialog() == true) {
                foreach (string filename in openFileDialog.FileNames) {
                    Music newMusic = new Music();

                    var file = TagLib.File.Create(filename);
                    newMusic.bitrate = file.Properties.AudioBitrate;
                    if (file.Tag.Pictures.Length > 0) {
                        TagLib.IPicture pic = file.Tag.Pictures[0];
                        MemoryStream stream = new MemoryStream(pic.Data.Data);
                        BitmapFrame bmp = BitmapFrame.Create(stream);
                    }
                    newMusic.album = file.Tag.Album;
                    if (newMusic.album == null)
                        newMusic.album = "n/a";
                    newMusic.packet = null;
                    //Create ListViewItem and style accordingly
                    string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    newMusic.uri = path + "\\" + filename.Substring(filename.LastIndexOf("\\"));
                    newMusic.filename = filename;
                    newMusic.song = file.Tag.Title;
                    newMusic.system_time = DateTime.Now;
                    newMusic.current_elapsed_time = 0;
                    newMusic.duration = file.Properties.Duration.TotalMilliseconds;
                    newMusic.artist = file.Tag.Performers[0];

                    if (newMusic.song == null) {
                        newMusic.song = "n/a";
                    }
                    if (newMusic.artist == null) {
                        newMusic.artist = "n/a";
                    }

                    byte[] song = { };
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                        song = File.ReadAllBytes(filename);
                        Console.WriteLine("packet len: " + song.Length);
                        fs.Read(song, 0, song.Length);
                        fs.Close();
                        File.WriteAllBytes(newMusic.uri, song);
                        Console.WriteLine("Finishing Serializing music to byte array");
                    }

                    return newMusic;
                }
            }
            return new Music();
        }
    }
}
