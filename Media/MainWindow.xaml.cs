using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//подключение пространства имён
using System.Media;
using System.Windows.Threading;

namespace Media
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;

            //назначение обработчика события на открытие медиа файла
            player.MediaOpened += Player_MediaOpened;
            player.MediaEnded += Player_MediaEnded;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLine.Value++;
        }

        //создание объекта, обычно глобального
        MediaPlayer player = new MediaPlayer();

        // 1 - name, 2 - path to file
        Dictionary<string, string> playlist = new Dictionary<string, string>();

        DispatcherTimer timer = new DispatcherTimer();

        

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            ////получение длительности медиа файла (свойство доступно только после события открытия)
            Duration d = player.NaturalDuration;
            ////получение половины длительности медиа файла в секундах
            int half = Convert.ToInt32(d.TimeSpan.TotalSeconds / 2);
            ////установка проигрывания со середины медиа файла
            //player.Position = new TimeSpan(0, 0, half);
            timeLine.Maximum= d.TimeSpan.TotalSeconds;

            player.Play();
            player.Volume = Voluem.Value;

            timer.Start();
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            if (ListOfTracks.SelectedIndex + 1 >= ListOfTracks.SelectedItems.Count) return;

            timer.Stop();

            ListOfTracks.SelectedIndex++;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            //выбор медиа файла, например, в формате .mp3
            OpenFileDialog dlg = new OpenFileDialog();
            
            dlg.Multiselect= true;

            dlg.ShowDialog();

            foreach(string pathName in dlg.FileNames)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(pathName);
                playlist.Add(name, pathName);
                ListOfTracks.Items.Add(name);
            }
            /*//загрузка выбранного файла
            player.Open(new Uri(dlg.FileName, UriKind.Relative));
            //воспроизведение
            player.Play();*/
        }

        private void ListOfTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListOfTracks.SelectedIndex < 0) return;

            player.Open(new Uri(playlist[ListOfTracks.SelectedItem.ToString()], UriKind.Relative));
        }

        private void Voluem_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = Voluem.Value;

        }

        private void timeLine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Position = new TimeSpan(0, 0, (int)timeLine.Value);
        }
        private void timeLine_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {

        }

        private void timeLine_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

        }

        
    }
}
