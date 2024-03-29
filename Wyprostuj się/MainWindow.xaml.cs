﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using System.Security.Principal;
using System.Windows.Threading;

namespace WyprostujSie
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const float headAv = 0f;
        const float spineAv = 1.57f;
        const float flankAv = 1.57f;

        WyprostujSieBackground.Kinect kinect;
        WyprostujSieBackground.Data data;
        WyprostujSieBackground.KalmanFilter[] kalmanFilters;
        Notifications notifications;
        DispatcherTimer dispatcherTimer;

        readonly bool IsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public MainWindow()
        {
            data = new WyprostujSieBackground.Data(true, true);
            using (MyProjectInstaller mpi = new MyProjectInstaller(ExpectedState.Stop, data.configFolder))

            kinect = new WyprostujSieBackground.Kinect(true);
            kalmanFilters = new WyprostujSieBackground.KalmanFilter[3];
            notifications = new Notifications();

            InitializeComponent();
            this.autorunChB.IsChecked = MyProjectInstaller.Installed();
            this.autorunChB.IsEnabled = IsAdmin;
            if (!IsAdmin)
            {
                notifications.AddNotif( Properties.Resources.adminNotif.ToString(), Brushes.Yellow, "admin");
            }

            SetValuaes();

            if (!Files.CopyFiles(data.configFolder))
            {
                this.notifications.AddNotif(Properties.Resources.errorFilesCopy, Brushes.Yellow, "files");
            }

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Tick += SetStatus;
            dispatcherTimer.Start();
        }

        public void SetStatus(object sender, EventArgs e)
        {
            Tuple<string, Brush> newStatus = notifications.ChNoti();

            Status.Content = newStatus.Item1;
            Status.Background = newStatus.Item2;
        }

        private void SetValuaes()
        {
            this.spineChB.IsChecked = data.SpineAnB;
            this.spineSlider.IsEnabled = this.spineChB.IsChecked.Value;
            this.spineSlider.Value = data.SpineAnD;
            this.kalmanFilters[0] = new WyprostujSieBackground.KalmanFilter(1, 1, 0.18, 1, 0.095, data.SpineAnD);

            this.headChB.IsChecked = data.NeckAnB;
            this.headSlider.IsEnabled = this.headChB.IsChecked.Value;
            this.headSlider.Value = data.NeckAnD;
            this.kalmanFilters[1] = new WyprostujSieBackground.KalmanFilter(1, 1, 0.175, 1, 0.09, data.NeckAnD);

            this.bokChB.IsChecked = data.BokAnB;
            this.bokSlider.IsEnabled = this.bokChB.IsChecked.Value;
            this.bokSlider.Value = data.BokAnD;
            this.kalmanFilters[2] = new WyprostujSieBackground.KalmanFilter(1, 1, 0.175, 1, 0.09, data.BokAnD);

            this.noPersonChB.IsChecked = data.NoPersB;
            this.toManyPersonChB.IsChecked = data.TMPersB;
        }

        private void UpdateScreen()
        {
            this.kinColour.Source = kinect.colorBitmap;
            this.kinSpindlelegs.Source = kinect.ImageSource;

            float SpineEst = (float)kalmanFilters[0].Output(kinect.SpineAn);
            SpineLabel.Content = SpineEst;
            if (Math.Abs(SpineEst - spineAv) > (float)spineSlider.Value / 10)
                SpineLabel.Foreground = Brushes.DarkRed;
            else
                SpineLabel.Foreground = Brushes.Green;

            float headEst = (float)kalmanFilters[1].Output(kinect.NeckAn);
            headLabel.Content = headEst;
            if (Math.Abs(headEst - headAv) > (float)headSlider.Value / 10)
                headLabel.Foreground = Brushes.DarkRed;
            else
                headLabel.Foreground = Brushes.Green;

            float flankEst = (float)kalmanFilters[2].Output(kinect.BokAn);
            bokLabel.Content = flankEst;
            if (Math.Abs(flankEst - flankAv) > (float)bokSlider.Value / 10)
                bokLabel.Foreground = Brushes.DarkRed;
            else
                bokLabel.Foreground = Brushes.Green;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinect.newData += UpdateScreen;
            kinect.kinectStatusUptate += UpdateKinectStatus;
            if (kinect != null)
                this.notifications.AddNotif(Properties.Resources.KinectUnknown, Brushes.Red, WyprostujSieBackground.Kinect.KinectTag);
        }

        private void UpdateKinectStatus()
        { 
            Brush backgroundColor;
            String content;

            if (kinect.KinectAvaible)
            {
                backgroundColor = Brushes.LimeGreen;
                content = Properties.Resources.KinectOK;
            }
            else
            {
                backgroundColor = Brushes.Red;
                content = Properties.Resources.errorKinectConnNotif;
            }

            notifications.UpdNotif(content, backgroundColor, WyprostujSieBackground.Kinect.KinectTag);
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            kinect.newData -= UpdateScreen;
            kinect = null;
            using (MyProjectInstaller mpi = new MyProjectInstaller(ExpectedState.Start, data.configFolder)) { };
        }

        private async void SpineChB_Click(object sender, RoutedEventArgs e)
        {
            if (spineChB.IsChecked == true)
                spineSlider.IsEnabled = true;
            else
                spineSlider.IsEnabled = false;

            data.SpineAnB = spineChB.IsChecked.Value;
            await data.Save();
        }

        private async void HeadCbBClicked(object sender, RoutedEventArgs e)
        {
            if (headChB.IsChecked == true)
                headSlider.IsEnabled = true;
            else
                headSlider.IsEnabled = false;

            data.NeckAnB = headChB.IsChecked.Value;
            await data.Save();
        }

        private async void BokChB_Click(object sender, RoutedEventArgs e)
        {
            if (bokChB.IsChecked == true)
                bokSlider.IsEnabled = true;
            else
                bokSlider.IsEnabled = false;

            data.BokAnB = bokChB.IsChecked.Value;
            await data.Save();
        }

        private async void Slider_Changed(object sender, RoutedEventArgs e)
        {
            var s = (Slider)sender;

            switch(s.Name)
            {
                case "headSlider":
                    {
                        data.NeckAnD = headSlider.Value;
                        break;
                    }

                case "bokSlider":
                    {
                        data.BokAnD = bokSlider.Value;
                        break;
                    }

                case "spineSlider":
                    {
                        data.SpineAnD = spineSlider.Value;
                        break;
                    }
            }

            await data.Save();
        }

        private async void NoPersonChB_Click(object sender, RoutedEventArgs e)
        {
            data.NoPersB = (sender as CheckBox).IsChecked.Value;
            await data.Save();
        }

        private async void ToManyPersonChB_Click(object sender, RoutedEventArgs e)
        {
            data.TMPersB = (sender as CheckBox).IsChecked.Value;
            await data.Save();
        }

        private void AutorunChB_Click(object sender, RoutedEventArgs e)
        {
            autorunChB.IsEnabled = false;
            UpdateLayout();

            Action AutorunChAction;
            MyProjectInstaller myProjectInstaller;


            if (autorunChB.IsChecked.Value)
            {
                AutorunChAction = () => {
                    myProjectInstaller = new MyProjectInstaller(ExpectedState.Install, data.configFolder);
                };
            }
            else
            {
                AutorunChAction = () => {
                    myProjectInstaller = new MyProjectInstaller(ExpectedState.Uninstall, data.configFolder);
                };
            }

            Task AutorunChTask = new Task(AutorunChAction);
            AutorunChTask.Start();

            Task UITask = AutorunChTask.ContinueWith(a =>
            {
                autorunChB.IsEnabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
