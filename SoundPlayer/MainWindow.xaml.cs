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
using System.Windows.Controls.Primitives;

namespace SoundPlayer
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

            TimeSpan curTime = new TimeSpan(0, 0, (int)timeLine.Value);

            lbCurrentTime.Content = string.Format("{0:mm}:{0:ss}", curTime);
        }

        //создание объекта, обычно глобального
        MediaPlayer player = new MediaPlayer();

        // 1 - name, 2 - path to file
        Dictionary<string, string> playlist = new Dictionary<string, string>();

        DispatcherTimer timer = new DispatcherTimer();

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            //получение длительности медиа файла (свойство доступно только после события открытия)
            Duration d = player.NaturalDuration;
            //получение длительности медиа файла в секундах
            timeLine.Maximum = d.TimeSpan.TotalSeconds;

            //Отформатированная длительность трека
            lbMaxTime.Content = string.Format("{0:mm}:{0:ss}", d.TimeSpan);

            timeLine.Value = 0;
            player.Play();
            player.Volume = SBvoluem.Value;

            timer.Start();
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            timer.Stop();

            if (ListOfTracks.SelectedIndex + 1 == ListOfTracks.Items.Count)
            {
                ListOfTracks.SelectedIndex = 0;
                return;
            }

            if (rnd == true)
            {
                Random rand = new Random();
                int value = rand.Next(0, ListOfTracks.Items.Count - 1);
                ListOfTracks.SelectedItem = ListOfTracks.Items[value];
            }

            if (repeat == true)
                ListOfTracks.SelectedIndex++;
            
        }


        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            //выбор медиа файла, например, в формате .mp3
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Multiselect = true;

            dlg.ShowDialog();

            foreach (string pathName in dlg.FileNames)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(pathName);
                playlist.Add(name, pathName);
                ListOfTracks.Items.Add(name);
            }
        }

        private void ListOfTracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StopBtn.IsEnabled = true;

            if (ListOfTracks.SelectedIndex < 0) return;

            player.Open(new Uri(playlist[ListOfTracks.SelectedItem.ToString()], UriKind.Relative));

            TrackName.Content = ListOfTracks.SelectedItem.ToString();

            player.IsMuted = false;

            player.Play();

            if(Click == 0)
            {
                PlayOrPauseBtn.Content = "⏸︎";
                Click = 1;
            }
        }

        private void SBvoluem_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = SBvoluem.Value;
        }

        //timeLine
        private void timeLine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(timeLine.Value == 100) ListOfTracks.SelectedIndex++;
        }
        private void timeLine_DragStarted(object sender, DragStartedEventArgs e)
        {
            timer.Stop();
        }
        private void timeLine_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            player.Position = new TimeSpan(0, 0, (int)timeLine.Value);
            timer.Start();
        }



        //Buttons
        int Click = 0;
        private void PlayOrPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            StopBtn.IsEnabled = true;

            //Play
            if (Click == 0)
            {
                if (player.IsMuted)
                {
                    player.Open(new Uri(playlist[ListOfTracks.SelectedItem.ToString()], UriKind.Relative));

                    player.IsMuted = false;
                }

                player.Play();
                timer.Start();

                PlayOrPauseBtn.Content = "⏸︎";
                Click = 1;


                return;
            }
            //Pause
            if (Click == 1)
            {
                player.Pause();

                timer.Stop();

                PlayOrPauseBtn.Content = "▶️";
                Click = 0;
                return;
            }
        }
        
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            StopBtn.IsEnabled = false;

            player.Stop();
            player.Close();

            timer.Stop();

            timeLine.Value = 0;
            lbCurrentTime.Content = "00:00";

            player.IsMuted = true;

            if (Click == 1)
            {
                PlayOrPauseBtn.Content = "▶️";
                Click = 0;
            }
        }

        bool rnd = false;
        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            rnd = true;
            repeat = false;

            Shuffle.IsEnabled = false;
            Repeat.IsEnabled = true;
        }

        bool repeat = true;
        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            repeat = true;
            rnd = false;

            Repeat.IsEnabled = false;
            Shuffle.IsEnabled = true;
        }

        
    }
}
