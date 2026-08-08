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

        public MainWindow()

        {

            InitializeComponent();

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

                    string fileName = System.IO.Path.GetFileName(file);

                    LbPlayList.Items.Add(fileName);

                }

            }

        }

    }
}