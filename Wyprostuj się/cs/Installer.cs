using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;

[RunInstaller(true)]
public class MyProjectInstaller : Installer
{
    private ServiceInstaller serviceInstaller;

    public MyProjectInstaller(string args)
    {
        // Instantiate installers for process and services.
        serviceInstaller = new ServiceInstaller();
        ServiceProcessInstaller ProcesServiceInstaller = new ServiceProcessInstaller();

        InstallContext Context = new System.Configuration.Install.InstallContext();
        
        //String path = String.Format("/assemblypath={0}", @"..\..\..\WyprostujSieBackground\bin\Debug\WyprostujSieBackground.exe");
        String path = String.Format("/assemblypath={0}", @"C:\Users\arkad\source\repos\ArBom\Wyprostuj-si-\WyprostujSieBackground\bin\Debug\WyprostujSieBackground.exe");
        String[] cmdline = { path };

        serviceInstaller.Parent = ProcesServiceInstaller;
        Context = new System.Configuration.Install.InstallContext("", cmdline);

        serviceInstaller.Context = Context;
        // The services are started manually.
        serviceInstaller.StartType = ServiceStartMode.Automatic;

        // ServiceName must equal those on ServiceBase derived classes.
        serviceInstaller.ServiceName = "WyprostujSieBackground";
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
                serviceController.Start();
                break;
            }
            case "uninstall":
            {
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
}