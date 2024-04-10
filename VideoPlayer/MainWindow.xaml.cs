using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VideoPlayer
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
            meVideo.MediaOpened += Player_MediaOpened;
            meVideo.MediaEnded += Player_MediaEnded;
        }

        DispatcherTimer timer = new DispatcherTimer();
        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLine.Value++;

            TimeSpan curTime = new TimeSpan(0, 0, (int)timeLine.Value);

            lbCurrentTime.Content = string.Format("{0:hh}:{0:mm}:{0:ss}", curTime);
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            //получение длительности медиа файла (свойство доступно только после события открытия)
            Duration d = meVideo.NaturalDuration;
            //получение длительности медиа файла в секундах
            timeLine.Maximum = d.TimeSpan.TotalSeconds;

            //Отформатированная длительность трека
            lbMaxTime.Content = string.Format("{0:hh}:{0:mm}:{0:ss}", d.TimeSpan);

            SldrVolume.Value = 0;
            meVideo.Volume = SldrVolume.Value;

            meVideo.Stop();
            timer.Stop();
        }
        private void Player_MediaEnded(object sender, EventArgs e)
        {
            timer.Stop();
            timeLine.Value = 0;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            //выбор медиа файла, например, в формате .mp4
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.ShowDialog();

            //установка источника
            meVideo.Source = new Uri(dlg.FileName);

            meVideo.Play();
        }

        private void SldrVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            meVideo.Volume = SldrVolume.Value;
        }
        
        //timeLine
        private void timeLine_DragStarted(object sender, DragStartedEventArgs e)
        {
            timer.Stop();

        }
        private void timeLine_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            meVideo.Position = new TimeSpan(0, 0, (int)timeLine.Value);
            timer.Start();
        }

        //Buttons
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            meVideo.Play();
            timer.Start();
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            meVideo.Pause();
            timer.Stop();
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            meVideo.Stop();
            timer.Stop();

            timeLine.Value = 0;
        }
    }
}
