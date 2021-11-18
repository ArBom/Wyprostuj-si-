using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Toolkit.Uwp.Notifications;

namespace WyprostujSieBackground
{
    public static class Toast
    {
        public static void ShowNot(Uri uriOfPic)
        {
            new ToastContentBuilder()
                .SetToastScenario(ToastScenario.Default)
                .AddArgument("eventId", 1983)
                .AddText("Wyprostuj się")
                .AddText("Tak będzie wyglądać przykładowe powiadomienie")
                .AddInlineImage(uriOfPic)
                .Show();
        }

        static void NumOfPeoleChanged(int howMamyPeolple)
        {
            if (howMamyPeolple == 0 && true)
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
            else if (howMamyPeolple > 1 && true)
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
    }
}
