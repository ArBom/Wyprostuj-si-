using System;
using System.IO;
using System.Globalization;

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

            string[] Files = new string[8];
            Files[0] = TP;
            Files[1] = WSB;
            Files[2] = "Microsoft.Toolkit.Uwp.Notifications.dll";
            Files[3] = "WyprostujSieBackground.exe.config";
            Files[4] = "WyprostujSieBackground.pdb";
            Files[5] = "Microsoft.Kinect.dll";
            Files[6] = "Newtonsoft.Json.dll";
            Files[7] = "ToastPresenter.resources.dll";

            string language = CultureInfo.CurrentCulture.IetfLanguageTag;
            string DirectoryPath = Path.Combine(configFolder, language);

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            Files[7] = Path.Combine(language, Files[7]);

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
