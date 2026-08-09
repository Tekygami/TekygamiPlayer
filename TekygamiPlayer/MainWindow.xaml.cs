using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        public MainWindow()

        {
           
            InitializeComponent();

            LbPlayList.MouseDoubleClick += LbPlayList_MouseDoubleClick;
            LbPlayList.SelectionChanged += LbPlayList_SelectionChanged;
            BtnPlay.Click += BtnPlay_Click;
            BtnPause.Click += BtnPause_Click;
            BtnStop.Click += BtnStop_Click;

          

        }

        private void BtnSelectFolder_Click(Object sender, RoutedEventArgs e)

        {

            var dialog = new Microsoft.Win32.OpenFolderDialog();



            //Показываем окно выбора папки

            bool? result = dialog.ShowDialog();





            if (result == true)

            {

                //Получаем путь к выбраной папке

                string folderPath = dialog.FolderName;

                this.Title = folderPath;





                //Чистим старый плейлист

                LbPlayList.Items.Clear();
                audioFilePaths.Clear();







                //Форматы мп3 и тд

                string[] extensions = { "*.mp3", "*.wav", "*.flac", "*.aac", "*.ogg", "*.wma" };

                var allFiles = new System.Collections.Generic.List<string>();



                //Проходимся по форматам игем файлы во всех ПОДПАПКАХ



                foreach (string ext in extensions)

                {

                    string[] foundFiles = System.IO.Directory.GetFiles(folderPath, ext, System.IO.SearchOption.AllDirectories);

                    allFiles.AddRange(foundFiles);

                }





                //тут добавляем каждый трек в лист бокс на екране

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
            }
        }



        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath)) return;

         
            if (selectedFilePath == currentlyPlayingPath)
            {
                mediaPlayer.Play();
            }
            else
            {
               
                mediaPlayer.Open(new Uri(selectedFilePath));
                mediaPlayer.Play();
                currentlyPlayingPath = selectedFilePath;
            }
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            currentlyPlayingPath = string.Empty;
        }

    }
}