using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Toolkit.Uwp.Notifications;
using System.IO;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;

namespace ToastPresenter
{
    public static class Toasts
    { 
        private const string Tag = "WyprSie";

        public static void ShowExampleToast(string ToShow)
        {
            string configFolder = AppDomain.CurrentDomain.BaseDirectory;
            string pathOfPhoto = Path.Combine(configFolder, WyprostujSieBackground.Kinect.PhotoFileName);

            if (ToShow == null)
                ToShow = Properties.Resources.ResourceManager.GetString("Example");

            DateTimeOffset dateTimeOffset = DateTime.Now.AddSeconds(15);

            new ToastContentBuilder()
            .SetToastScenario(ToastScenario.Default)
            .AddText(ToShow)
            .AddHeroImage(new Uri(pathOfPhoto))
            .Show(toast =>
            {
                toast.Tag = Tag;
                toast.SuppressPopup = false;
                toast.ExpirationTime = dateTimeOffset;
                toast.ExpiresOnReboot = false;
            });
        }

        public static void ShowBadPostureToast()
        {
            string configFolder = AppDomain.CurrentDomain.BaseDirectory;
            string pathOfPhoto = Path.Combine(configFolder, WyprostujSieBackground.Kinect.PhotoFileName);
            string TextToShow = Properties.Resources.ResourceManager.GetString("BadPosture");

            DateTimeOffset dateTimeOffset = DateTime.Now.AddMinutes(3);

            new ToastContentBuilder()
            .SetToastScenario(ToastScenario.Default)
            .AddText(TextToShow)
            .AddHeroImage(new Uri(pathOfPhoto))
            .Show(toast =>
            {
                toast.Tag = Tag;
                toast.SuppressPopup = false;
                toast.ExpirationTime = dateTimeOffset;
                toast.ExpiresOnReboot = false;
            });
        }

        public static void ShowTooManyPeopleToast()
        {
            DateTimeOffset dateTimeOffset = DateTime.Now.AddMinutes(10);
            string TextToShow = Properties.Resources.ResourceManager.GetString("TMPersB");

            new ToastContentBuilder()
            .SetToastScenario(ToastScenario.Default)
            .AddText(TextToShow)
            .Show(toast =>
            {
                toast.Tag = Tag;
                toast.SuppressPopup = true;
                toast.ExpirationTime = dateTimeOffset;
                toast.ExpiresOnReboot = false;
            });
        }

        public static void ShowNoPersonToast()
        {
            DateTimeOffset dateTimeOffset = DateTime.Now.AddMinutes(5);
            string TextToShow = Properties.Resources.ResourceManager.GetString("NoPersB");

            new ToastContentBuilder()
            .SetToastScenario(ToastScenario.Default)
            .AddText(TextToShow)
            .Show(toast =>
            {
                toast.Tag = Tag;
                toast.SuppressPopup = false;
                toast.ExpirationTime = dateTimeOffset;
                toast.ExpiresOnReboot = false;
            });
        }

        public static void RemoveToast()
        {
            ToastNotificationManagerCompat.History.Remove(Tag);
        }
    }
}
