using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;
using System.IO;
using System.Linq;

public enum ExpectedState { Install, Uninstall, Stop, Start };

[RunInstaller(true)]
public class MyProjectInstaller : Installer
{
    private ServiceInstaller serviceInstaller;
    private const string ServiceName = "WyprostujSieBackground";

    public MyProjectInstaller(ExpectedState expectedState)
    {
        // Instantiate installers for process and services.
        serviceInstaller = new ServiceInstaller();
        ServiceProcessInstaller ProcesServiceInstaller = new ServiceProcessInstaller();

        InstallContext Context = new InstallContext();

        String RelatPath = @"..\..\..\WyprostujSieBackground\bin\Debug\WyprostujSieBackground.exe";
        String AbsolPath = Path.GetFullPath(RelatPath);
        AbsolPath = String.Format("/assemblypath={0}", AbsolPath);
        String[] cmdline = { AbsolPath };


        /*ServiceProcessInstaller spi = new ServiceProcessInstaller();
        spi.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
        spi.Password = null;
        spi.Username = null;

        this.Installers.Add(spi);*/

        ProcesServiceInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem; //ServiceAccount.User;
        ProcesServiceInstaller.Username = null;
        ProcesServiceInstaller.Password = null;

        serviceInstaller.Parent = ProcesServiceInstaller;
        Context = new InstallContext("", cmdline);

        serviceInstaller.Context = Context;

        // The services are started automaticly.
        serviceInstaller.StartType = ServiceStartMode.Automatic;

        serviceInstaller.ServiceName = ServiceName;

        // Info for users
        serviceInstaller.DisplayName = "Wyprostuj się";
        serviceInstaller.Description = "Wykorzystuje sensor Kinect do analizowania postawy twojego ciała; informuje o jej wadach";

        ServiceController serviceController = new ServiceController(serviceInstaller.ServiceName);

        System.Collections.Specialized.ListDictionary stateSaver = new System.Collections.Specialized.ListDictionary();

        switch (expectedState)
        {
            case ExpectedState.Install:
                {
                    if (!Installed())
                    {
                        try
                        {
                            serviceInstaller.Install(null);
                        }
                        catch { }


                    }
                    break;
                }
            case ExpectedState.Uninstall:
                {
                    if (Installed())
                    {
                        try
                        {
                            if (serviceController.Status == ServiceControllerStatus.Running || serviceController.Status == ServiceControllerStatus.StartPending)
                            {
                                serviceController.Stop();
                                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(3));
                            }
                            serviceInstaller.Uninstall(null);
                        }
                        catch { }
                    }
                    break;
                }
            case ExpectedState.Stop:
                {
                    if (Installed())
                    {
                        if (serviceController.Status == ServiceControllerStatus.Running || serviceController.Status == ServiceControllerStatus.StartPending)
                        {
                            try
                            {
                                serviceController.Stop();
                                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                            }
                            catch { }
                        }
                    }
                    break;
                }
            case ExpectedState.Start:
                {
                    if (Installed())
                    {
                        if (serviceController.Status == ServiceControllerStatus.Stopped || serviceController.Status == ServiceControllerStatus.StopPending)
                        {
                            try
                            {
                                serviceController.Start();
                                serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
                            }
                            catch { }
                        }
                    }
                    break;
                }
        }
    }

    public static bool Installed()
    {
        return ServiceController.GetServices().Any(s => s.ServiceName == ServiceName);
    }
}