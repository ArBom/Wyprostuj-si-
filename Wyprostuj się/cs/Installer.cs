using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;
using System.IO;
using System.Linq;

[RunInstaller(true)]
public class MyProjectInstaller : Installer
{
    private ServiceInstaller serviceInstaller;
    private const string ServiceName = "WyprostujSieBackground";

    public MyProjectInstaller(string args)
    {
        // Instantiate installers for process and services.
        serviceInstaller = new ServiceInstaller();
        ServiceProcessInstaller ProcesServiceInstaller = new ServiceProcessInstaller();

        InstallContext Context = new InstallContext();

        String RelatPath = @"..\..\..\WyprostujSieBackground\bin\Debug\WyprostujSieBackground.exe";
        String AbsolPath = Path.GetFullPath(RelatPath);
        AbsolPath = String.Format("/assemblypath={0}", AbsolPath);
        String[] cmdline = { AbsolPath };

        serviceInstaller.Parent = ProcesServiceInstaller;
        Context = new InstallContext("", cmdline);

        serviceInstaller.Context = Context;
        // The services are started manually.
        serviceInstaller.StartType = ServiceStartMode.Automatic;

        // ServiceName must equal those on ServiceBase derived classes.
        serviceInstaller.ServiceName = ServiceName;// "WyprostujSieBackground";
        serviceInstaller.DisplayName = "Wyprostuj się";
        serviceInstaller.Description = "Wykorzystuje sensor Kinect do analizowania postawy twojego ciała; informuje o jej wadach";

        ServiceController serviceController = new ServiceController(serviceInstaller.ServiceName);

        System.Collections.Specialized.ListDictionary stateSaver = new System.Collections.Specialized.ListDictionary();
        //args = "uninstall";

        switch (args)
        {
            case "install":
            {
                serviceInstaller.Install(stateSaver);
                if (installed())
                    serviceController.Start();
                break;
            }
            case "uninstall":
            {
                if (installed())
                    serviceController.Stop();
                serviceInstaller.Uninstall(null);
                break;
            }
            case "reset":
            {
                serviceController.Refresh();
                break;
            }
        }
    }

    public static bool installed()
    {
        return ServiceController.GetServices().Any(s => s.ServiceName == ServiceName); //.FirstOrDefault(s => s.ServiceName == ServiceName);
    }
}