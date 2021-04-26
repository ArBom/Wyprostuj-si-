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

using Microsoft.Win32;
using Wyprostuj_sie.cs;

namespace Wyprostuj_sie
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Kinect kinect;
        Data data;

        public MainWindow()
        {
            kinect = new Kinect(true);
            data = new Data();
            
            InitializeComponent();
            setValuaes();

        }

        private void setValuaes()
        {
            this.spineChB.IsChecked = data.SpineAnB;
            this.spineSlider.Value = data.SpineAnD;

            this.headChB.IsChecked = data.NeckAnB;
            this.headSlider.Value = data.NeckAnD;

            this.bokChB.IsChecked = data.BokAnB;
            this.bokSlider.Value = data.BokAnD;
        }

        public ImageSource ImageSource
        {
            get
            {
                return kinect.colorBitmap;
            }
        }

        private void UpdateScreen()
        {
            this.kinColour.Source = kinect.colorBitmap;
            this.kinSpindlelegs.Source = kinect.ImageSource;

            SpineLabel.Content = kinect.SpineAn;
            headLabel.Content = kinect.NeckAn;
            bokLabel.Content = kinect.BokAn;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinect.newData += UpdateScreen;
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
    }
}
