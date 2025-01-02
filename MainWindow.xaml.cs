using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using NAudio.Wave;

namespace Ariadna
{
    public partial class MainWindow : Window
    {
        private WaveOutEvent waveOut = new WaveOutEvent();
        private AudioFileReader currentTrack;
        private const string CacheFileName = "music_cache.json";
        private List<string> cachedMusicList = new List<string>();
        private string musicFolder = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            LoadCache();  // Cargar la caché al iniciar la aplicación
            Volumen.ValueChanged += Volumen_ValueChanged;  // Agregar el manejador del volumen
        }

        private void LoadCache()
        {
            if (File.Exists(CacheFileName))
            {
                try
                {
                    string json = File.ReadAllText(CacheFileName);
                    cachedMusicList = JsonSerializer.Deserialize<List<string>>(json);
                    UpdateMusicList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar la caché: {ex.Message}");
                }
            }
        }

        private void SaveCache()
        {
            try
            {
                string json = JsonSerializer.Serialize(cachedMusicList);
                File.WriteAllText(CacheFileName, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la caché: {ex.Message}");
            }
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                musicFolder = dialog.SelectedPath;
                cachedMusicList = new List<string>(Directory.GetFiles(musicFolder, "*.mp3"));
                SaveCache();  // Guardar la nueva lista de música en caché
                UpdateMusicList();  // Actualizar la lista de canciones en la interfaz
            }
        }

        private void UpdateMusicList()
        {
            MusicList.Items.Clear();
            foreach (var file in cachedMusicList)
            {
                MusicList.Items.Add(file);
            }
        }

        private void MusicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MusicList.SelectedItem == null) return;

            string selectedTrack = MusicList.SelectedItem.ToString();
            PlayTrack(selectedTrack);
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

        private void Volumen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (currentTrack != null)
            {
                // Ajustar el volumen según el valor del slider
                currentTrack.Volume = (float)Volumen.Value / 100f;
            }
        }

        // Función para guardar la lista de música en la caché
        private void SaveCacheButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCache();
        }
    }
}
