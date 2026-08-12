using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using TagLib;
using System.Windows;
using System.Windows.Controls;
using WpfAnimatedGif;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace TekygamiPlayer
 
{

    /// <summary>

    /// Interaction logic for MainWindow.xaml

    /// </summary>

    public partial class MainWindow : Window

    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private List<string> audioFilePaths = new List<string>();
        private string selectedFilePath = string.Empty;
        private string currentlyPlayingPath = string.Empty;
        private System.Windows.Threading.DispatcherTimer timer;
        private bool isDragging = false;
        public MainWindow()

        {
           
            InitializeComponent();

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;

            LbPlayList.MouseDoubleClick += LbPlayList_MouseDoubleClick;
            LbPlayList.SelectionChanged += LbPlayList_SelectionChanged;
            BtnPlay.Click += BtnPlay_Click;
            BtnPause.Click += BtnPause_Click;
            BtnStop.Click += BtnStop_Click;
            BtnNext.Click += BtnNext_Click;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;


            ProgressSlider.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ProgressSlider_MouseLeftButtonDown), true);
            ProgressSlider.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ProgressSlider_MouseLeftButtonUp), true);


        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            if(!isDragging && mediaPlayer.Source != null && mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan currentPosition = mediaPlayer.Position;
                TimeSpan totalDuration = mediaPlayer.NaturalDuration.TimeSpan;

                ProgressSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressSlider.Value = mediaPlayer.Position.TotalSeconds;

                TxtCurrentTime.Text = currentPosition.ToString(@"mm\:ss");
                TxtTotalTime.Text = totalDuration.ToString(@"mm\:ss");
             
            }
        }
        private void ProgressSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
        }
        private void ProgressSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            if(mediaPlayer.Source != null && mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
            }
        }


        private void BtnSelectFolder_Click(Object sender, RoutedEventArgs e)

        {

            var dialog = new Microsoft.Win32.OpenFolderDialog();


            bool? result = dialog.ShowDialog();


            if (result == true)

            {

                string folderPath = dialog.FolderName;

                this.Title = "TekygamiPlayer";

                LbPlayList.Items.Clear();
                audioFilePaths.Clear();

                string[] extensions = { "*.mp3", "*.wav", "*.flac", "*.aac", "*.ogg", "*.wma" };

                var allFiles = new System.Collections.Generic.List<string>();


                foreach (string ext in extensions)

                {

                    string[] foundFiles = System.IO.Directory.GetFiles(folderPath, ext, System.IO.SearchOption.AllDirectories);

                    allFiles.AddRange(foundFiles);

                }

                foreach (string file in allFiles)

                {
                    audioFilePaths.Add(file);

                    string fileName = System.IO.Path.GetFileName(file);

                    LbPlayList.Items.Add(fileName);

                }

            }

        }

        private void LbPlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            int index = LbPlayList.SelectedIndex;
            if (index >= 0 && index < audioFilePaths.Count)
            {

                selectedFilePath = audioFilePaths[index];
                LoadAlbumArt(selectedFilePath);
            }
        }


        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            BtnNext_Click(null, null);
        }
  



        private void LoadAlbumArt(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                {
                    AlbumArtImage.Source = null;
                    return;
                }
                using (var file = TagLib.File.Create(filePath))
                {
                    var pictures = file.Tag.Pictures;
                    if (pictures.Length > 0)
                    {
                        var bin = pictures[0].Data.Data;
                        using (var ms = new MemoryStream(bin))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();
                            image.Freeze();

                            AlbumArtImage.Source = image;
                            return;
                        }
                    }        
                }
                string directoryPath = System.IO.Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    string[] possibleCoverDirs =
                    {
                        System.IO.Path.Combine(directoryPath, "Cover"),
                        System.IO.Path.Combine(directoryPath, "covers"),
                        System.IO.Path.Combine(directoryPath, "Artwork"),
                        directoryPath
                    };

                    foreach(string dir in possibleCoverDirs)
                    {
                        if (System.IO.Directory.Exists(dir))
                        {
                            var imageFiles = System.IO.Directory.GetFiles(dir, "*.jpg")
                            .Concat(System.IO.Directory.GetFiles(dir, "*.jpeg"))
                            .Concat(System.IO.Directory.GetFiles(dir, "*.png"))
                            .ToArray();
                            if(imageFiles.Length > 0)
                            {
                                var image = new BitmapImage();
                                image.BeginInit();
                                image.UriSource = new Uri(imageFiles[0], UriKind.Absolute);
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.EndInit();
                                image.Freeze();

                                AlbumArtImage.Source = image;
                                return;
                            }

                        }
                    }
                     
                }
            }
            catch (Exception)
            {
                AlbumArtImage.Source = null;
            }
        }

        private void LbPlayList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = LbPlayList.SelectedIndex;
            if (index >= 0 && index < audioFilePaths.Count)
            {
                string fullPath = audioFilePaths[index];
                mediaPlayer.Open(new Uri(fullPath));
                mediaPlayer.Play();
                timer.Start();
                ProgressSlider.IsEnabled = true;

                var controller = ImageBehavior.GetAnimationController(CatGifImage);
                if (controller != null) controller.Play();
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath)) return;


            if (selectedFilePath == currentlyPlayingPath)
            {
                mediaPlayer.Play();
                ProgressSlider.IsEnabled = true;
                timer.Start();
                
            }
            else
            {

                mediaPlayer.Open(new Uri(selectedFilePath));
                mediaPlayer.Play();
                ProgressSlider.IsEnabled = true;
                timer.Start();
                currentlyPlayingPath = selectedFilePath;
            }
            var controller = ImageBehavior.GetAnimationController(CatGifImage);
            if (controller != null) controller.Play();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            timer.Stop();

            var controller = ImageBehavior.GetAnimationController(CatGifImage);
            if (controller != null) controller.Pause();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            timer.Stop();
            ProgressSlider.IsEnabled = false;
            ProgressSlider.Value = 0;
            TxtCurrentTime.Text = "00:00";
            TxtTotalTime.Text = "00:00";

            var controller = ImageBehavior.GetAnimationController(CatGifImage);
            if (controller != null)
            {
                controller.Pause();
                controller.GotoFrame(0);
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = VolumeSlider.Value;
            if(TxtVolumeValue != null)
            {
                int volPercent = (int)(VolumeSlider.Value * 100);
                TxtVolumeValue.Text = $"{volPercent}%";
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = LbPlayList.SelectedIndex;

            if (currentIndex == -1) currentIndex = 0;

            if(currentIndex + 1 < audioFilePaths.Count)
            {

                LbPlayList.SelectedIndex = currentIndex + 1;

                string nextPath = audioFilePaths[LbPlayList.SelectedIndex];
                mediaPlayer.Open(new Uri(nextPath));
                mediaPlayer.Play();
                timer.Start();
                ProgressSlider.IsEnabled = true;
                currentlyPlayingPath = nextPath;

                var controller = ImageBehavior.GetAnimationController(CatGifImage);
                if (controller != null) controller.Play();
            }
           
        }
    }
}