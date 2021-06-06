﻿using System;
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
using System.Xml;

using Windows.UI.Notifications; //TODO upewnić się że zbędne i usunąć
using Microsoft.Toolkit.Uwp.Notifications;


namespace Wyprostuj_sie
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const float headAv = 0f;
        const float spineAv = 1.57f;
        const float flankAv = 1.57f;

        Kinect kinect;
        Data data;
        KalmanFilter[] kalmanFilters;

        public MainWindow()
        {
            kinect = new Kinect(true);
            data = new Data(true);
            kalmanFilters = new KalmanFilter[3];
            
            InitializeComponent();
            setValuaes();
        }

        private void setValuaes()
        {
            this.spineChB.IsChecked = data.SpineAnB;
            if (data.SpineAnD >= spineSlider.Minimum && data.SpineAnD <= spineSlider.Maximum)
                this.spineSlider.Value = data.SpineAnD;
            this.kalmanFilters[0] = new KalmanFilter(1, 1, 0.18, 1, 0.095, data.SpineAnD);

            this.headChB.IsChecked = data.NeckAnB;
            this.headSlider.Value = data.NeckAnD;
            this.kalmanFilters[1] = new KalmanFilter(1, 1, 0.175, 1, 0.09, data.NeckAnD);

            this.bokChB.IsChecked = data.BokAnB;
            this.bokSlider.Value = data.BokAnD;
            this.kalmanFilters[2] = new KalmanFilter(1, 1, 0.175, 1, 0.09, data.BokAnD);

            this.noPersonChB.IsChecked = data.NoPersB;
            this.toManyPersonChB.IsChecked = data.TMPersB;
        }

        private async void UpdateScreen()
        {
            this.kinColour.Source = kinect.colorBitmap;
            this.kinSpindlelegs.Source = kinect.ImageSource;

            float SpineEst = (float)kalmanFilters[0].Output(kinect.SpineAn);
            SpineLabel.Content = SpineEst;
            if (Math.Abs(SpineEst - spineAv) > (float)spineSlider.Value/10)
                SpineLabel.Foreground = Brushes.DarkRed;
            else
                SpineLabel.Foreground = Brushes.Green;
            
            float headEst = (float)kalmanFilters[1].Output(kinect.NeckAn);
            headLabel.Content = headEst;
            if (Math.Abs(headEst - headAv) > (float)headSlider.Value/10)
                headLabel.Foreground = Brushes.DarkRed;
            else
                headLabel.Foreground = Brushes.Green;

            float flankEst = (float)kalmanFilters[2].Output(kinect.BokAn);
            bokLabel.Content = flankEst;
            if (Math.Abs(flankEst - flankAv) > (float)bokSlider.Value/10)
                bokLabel.Foreground = Brushes.DarkRed;
            else
                bokLabel.Foreground = Brushes.Green;
        }

        private void ShowNot(Uri uriOfPic)
        {
            new ToastContentBuilder()
                .SetToastScenario(ToastScenario.Default)
                .AddArgument("eventId", 1983)
                .AddText("Wyprostuj się")
                .AddText("Tak będzie wyglądać przykładowe powiadomienie")
                .AddInlineImage(uriOfPic)
                .Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinect.newData += UpdateScreen;
            kinect.takenPic += ShowNot;
            Status.Content = kinect.StatusText;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            kinect.newData -= UpdateScreen;
            kinect = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            System.Reflection.Assembly curAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            regKey.SetValue(curAssembly.GetName().Name, curAssembly.Location);*/
            kinect.takePic = true;
        }

        private void SpineChB_Click(object sender, RoutedEventArgs e)
        {
            if (spineChB.IsChecked == true)
                spineSlider.IsEnabled = true;
            else
                spineSlider.IsEnabled = false;

            data.SpineAnB = spineChB.IsChecked.Value;
            data.Save();
        }

        private void headCbBClicked(object sender, RoutedEventArgs e)
        {
            if (headChB.IsChecked == true)
                headSlider.IsEnabled = true;
            else
                headSlider.IsEnabled = false;

            data.NeckAnB = headChB.IsChecked.Value;
            data.Save();
        }

        private void BokChB_Click(object sender, RoutedEventArgs e)
        {
            if (bokChB.IsChecked == true)
                bokSlider.IsEnabled = true;
            else
                bokSlider.IsEnabled = false;

            data.BokAnB = bokChB.IsChecked.Value;
            data.Save();
        }

        private void Slider_Changed (object sender, RoutedEventArgs e)
        {
            data.BokAnD = bokSlider.Value;
            data.NeckAnD = headSlider.Value;
            data.SpineAnD = spineSlider.Value;

            data.Save();
        }

        private void noPersonChB_Click(object sender, RoutedEventArgs e)
        {
            data.NoPersB = (sender as CheckBox).IsChecked.Value;
            data.Save();
        }

        private void toManyPersonChB_Click(object sender, RoutedEventArgs e)
        {
            data.TMPersB = (sender as CheckBox).IsChecked.Value;
            data.Save();
        }
    }
}
