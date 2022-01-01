using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WyprostujSie
{
    public static class Files
    {
        public const string TP = "ToastPresenter.exe";
        public const string WSB = "WyprostujSieBackground.exe";

        public static string PathOfThisApp()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static bool CopyFiles(string configFolder)
        {
            bool success = true;

            string[] Files = new string[7];
            Files[0] = TP;
            Files[1] = WSB;
            Files[2] = "Microsoft.Toolkit.Uwp.Notifications.dll";
            Files[3] = "WyprostujSieBackground.exe.config";
            Files[4] = "WyprostujSieBackground.pdb";
            Files[5] = "Microsoft.Kinect.dll";
            Files[6] = "Newtonsoft.Json.dll";


            foreach (string f in Files)
            {
                try
                {
                    System.IO.File.Copy(Path.Combine(PathOfThisApp(), f), Path.Combine(configFolder, f), true);
                }
                catch
                {
                    success = false;
                }
            }

            return success;
        }
    }
}
