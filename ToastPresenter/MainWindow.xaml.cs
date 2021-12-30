using Microsoft.Toolkit.Uwp.Notifications;
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
using System.IO;

namespace ToastPresenter
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFolder = Path.Combine(commonAppData, "WyprostujSie");
            string pathOfPhoto = (configFolder + @"\photo.jpg");

            DateTimeOffset dateTimeOffset = DateTime.Now.AddSeconds(15);

            new ToastContentBuilder()
            .SetToastScenario(ToastScenario.Default)
            .AddText("Wyprostuj się")
            .AddHeroImage(new Uri(pathOfPhoto))
            .Show( toast =>
            {
                toast.Tag = "WyprSie";
                toast.SuppressPopup = false;
                toast.ExpirationTime = dateTimeOffset;
                toast.ExpiresOnReboot = false;
            });

            this.Close();
        }
    }
}
