using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ToastPresenter
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string[] args = e.Args;

            if (args.Length != 0)
            {
                if (args[0] == WyprostujSieBackground.Data.BadPostureKey)
                {
                    Toasts.ShowBadPostureToast();
                }
                else if (args[0] == WyprostujSieBackground.Data.NoPersBKey)
                {
                    Toasts.ShowNoPersonToast();
                }
                else if (args[0] == WyprostujSieBackground.Data.TMPersBKey)
                {
                    Toasts.ShowTooManyPeopleToast();
                }
                else if (args[0] == WyprostujSieBackground.Toast.RemoveToastKey)
                {
                    Toasts.RemoveToast();
                }
                else
                {
                    Toasts.ShowExampleToast(args[0]);
                }
            }
            else
            {
                Toasts.ShowExampleToast(null);
            }
        }
    }
}
