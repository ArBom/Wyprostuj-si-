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
using System.Xml;

using System.ServiceProcess;
using Microsoft.Toolkit.Uwp.Notifications;
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

        Kinect kinect;
        Data data;
        KalmanFilter[] kalmanFilters;
        Notifications notifications;
        DispatcherTimer dispatcherTimer;

        readonly bool IsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public MainWindow()
        {
            using (MyProjectInstaller mpi = new MyProjectInstaller(ExpectedState.Stop))

            kinect = new Kinect(true);
            data = new Data(true);
            kalmanFilters = new KalmanFilter[3];
            notifications = new Notifications();

            InitializeComponent();
            this.autorunChB.IsChecked = MyProjectInstaller.installed();
            this.autorunChB.IsEnabled = IsAdmin;
            if (!IsAdmin)
            {
                notifications.addNotif("Uruchom aplikację jako administrator, aby móc korzystać ze wszystkich funkcji.", Brushes.Yellow, "admin");
            }

            setValuaes();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Tick += setStatus;
            dispatcherTimer.Start();
        }

        public void setStatus(object sender, EventArgs e)
        {
            Tuple<string, Brush> newStatus = notifications.ChNoti();

            Status.Content = newStatus.Item1;
            Status.Background = newStatus.Item2;
        }

        private void setValuaes()
        {
            this.spineChB.IsChecked = data.SpineAnB;
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

        private void NumOfPeoleChanged(int howMamyPeolple)
        {
            if (howMamyPeolple == 0 && noPersonChB.IsChecked.Value)
            {
                new ToastContentBuilder()
                    .SetToastScenario(ToastScenario.Default)
                    .AddArgument("howManyP")
                    .AddText("Wyprostuj się")
                    .AddText("Nie widać cię")
                    .Show(toast =>
                    {
                        toast.Tag = "howManyP";
                        toast.Group = "WyprostujSieGrop";
                    });
            }
            else if (howMamyPeolple > 1 && toManyPersonChB.IsChecked.Value)
            {
                new ToastContentBuilder()
                    .SetToastScenario(ToastScenario.Default)
                    .AddText("Wyprostuj się")
                    .AddText("Więcej niż jedna osoba.")
                    .Show(toast =>
                    {
                        toast.Tag = "howManyP";
                        toast.Group = "WyprostujSieGrop";
                        toast.SuppressPopup = true;
                    });
            }
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
            Task.Run(() =>
            {
                kinect.newData += UpdateScreen;
                kinect.takenPic += ShowNot;
                kinect.personAtPhoto += NumOfPeoleChanged;
            });

            Brush backgroundColor;
            String content;

            switch (kinect.StatusText)
            {
                case "SensorNotAvailableStatusText":
                    {
                        backgroundColor = Brushes.Red;
                        content = "Nie można połączyć się z sensorem Kinect.";
                        break;
                    }
                default:
                    {
                        backgroundColor = Brushes.Transparent;
                        content = "";
                        Tag = "";
                        break;
                    }
            }

            notifications.addNotif(content, backgroundColor, "kinect");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            kinect.newData -= UpdateScreen;
            kinect = null;
            using (MyProjectInstaller mpi = new MyProjectInstaller(ExpectedState.Start));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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

        private void Slider_Changed(object sender, RoutedEventArgs e)
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

        private void AutorunChB_Click(object sender, RoutedEventArgs e)
        {
            autorunChB.IsEnabled = false;
            UpdateLayout();

            MyProjectInstaller myProjectInstaller;

            if (autorunChB.IsChecked.Value)
            {
                myProjectInstaller = new MyProjectInstaller(ExpectedState.Install);
            }
            else
            {
                myProjectInstaller = new MyProjectInstaller(ExpectedState.Uninstall);
            }

            autorunChB.IsEnabled = true;
        }
    }
}
