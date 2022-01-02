using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Toolkit.Uwp.Notifications;
using System.IO;

namespace ToastPresenter
{
    public static class Toasts
    {
        public static void ShowExampleToast(string ToShow)
        {
            string configFolder = AppDomain.CurrentDomain.BaseDirectory;
            string pathOfPhoto = Path.Combine(configFolder, WyprostujSieBackground.Kinect.PhotoFileName);

            DateTimeOffset dateTimeOffset = DateTime.Now.AddSeconds(15);

            new ToastContentBuilder()
            .SetToastScenario(ToastScenario.Default)
            .AddText(ToShow)
            .AddHeroImage(new Uri(pathOfPhoto))
            .Show(toast =>
            {
                toast.Tag = "WyprSie";
                toast.SuppressPopup = false;
                toast.ExpirationTime = dateTimeOffset;
                toast.ExpiresOnReboot = false;
            });
        }

        public static void ShowBadPostureToast()
        {
            string configFolder = AppDomain.CurrentDomain.BaseDirectory;
            string pathOfPhoto = Path.Combine(configFolder, WyprostujSieBackground.Kinect.PhotoFileName);

            DateTimeOffset dateTimeOffset = DateTime.Now.AddSeconds(15);

            new ToastContentBuilder()
            .SetToastScenario(ToastScenario.Default)
            .AddText("Wyprostuj się!")
            .AddHeroImage(new Uri(pathOfPhoto))
            .Show(toast =>
            {
                toast.Tag = "WyprSie";
                toast.SuppressPopup = false;
                toast.ExpirationTime = dateTimeOffset;
                toast.ExpiresOnReboot = false;
            });
        }

        public static void ShowTooManyPeopleToast()
        {
            DateTimeOffset dateTimeOffset = DateTime.Now.AddMinutes(10);

            new ToastContentBuilder()
            .SetToastScenario(ToastScenario.Default)
            .AddText("Nie wiem na kogo patrzeć.")
            .Show(toast =>
            {
                toast.Tag = "WyprSie";
                toast.SuppressPopup = true;
                toast.ExpirationTime = dateTimeOffset;
                toast.ExpiresOnReboot = false;
            });
        }

        public static void ShowNoPersonToast()
        {
            DateTimeOffset dateTimeOffset = DateTime.Now.AddMinutes(3);

            new ToastContentBuilder()
            .SetToastScenario(ToastScenario.Default)
            .AddText("Nie widzę cię.")
            .Show(toast =>
            {
                toast.Tag = "WyprSie";
                toast.SuppressPopup = false;
                toast.ExpirationTime = dateTimeOffset;
                toast.ExpiresOnReboot = false;
            });
        }
    }
}
