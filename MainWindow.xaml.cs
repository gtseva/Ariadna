using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using NAudio.Wave;

namespace Ariadna
{
    
    /// Interaccion Logica de xaml a cs
  
    public partial class MainWindow : Window
    {
        private WaveOutEvent waveOut = new WaveOutEvent();
        private AudioFileReader currentTrack;
        private string musicFolder = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                musicFolder = dialog.SelectedPath;
                LoadMusicList();
            }
        }

        private void LoadMusicList()
        {
            if (string.IsNullOrEmpty(musicFolder)) return;

            MusicList.Items.Clear();
            var files = Directory.GetFiles(musicFolder, "*.mp3");
            foreach (var file in files)
            {
                MusicList.Items.Add(file);
            }
        }

        private void MusicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MusicList.SelectedItem == null) return;

            PlayTrack(MusicList.SelectedItem.ToString());
        }

        private void PlayTrack(string filePath)
        {
            waveOut.Stop();
            currentTrack?.Dispose();

            currentTrack = new AudioFileReader(filePath);
            waveOut.Init(currentTrack);
            waveOut.Play();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (waveOut.PlaybackState != PlaybackState.Playing && currentTrack != null)
            {
                waveOut.Play();
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (waveOut.PlaybackState == PlaybackState.Playing)
            {
                waveOut.Pause();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrack != null)
            {
                currentTrack.CurrentTime = currentTrack.CurrentTime.Add(TimeSpan.FromSeconds(10));
            }
        }

        private void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrack != null)
            {
                currentTrack.CurrentTime = currentTrack.CurrentTime.Subtract(TimeSpan.FromSeconds(10));
            }
        }
    }
}
