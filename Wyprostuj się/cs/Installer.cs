﻿using System;
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

    public MyProjectInstaller(ExpectedState expectedState, string RelatPath)
    {
        // Instantiate installers for process and services.
        serviceInstaller = new ServiceInstaller();
        ServiceProcessInstaller ProcesServiceInstaller = new ServiceProcessInstaller();

        InstallContext Context = new InstallContext();

        String AbsolPath = Path.GetFullPath(RelatPath);
        AbsolPath = Path.Combine(AbsolPath, WyprostujSie.Files.WSB);
        AbsolPath = String.Format("/assemblypath={0}", AbsolPath);
        String[] cmdline = { AbsolPath };

        serviceInstaller.Parent = ProcesServiceInstaller;
        Context = new InstallContext("", cmdline);

        serviceInstaller.Context = Context;

        // The services are started automaticly.
        serviceInstaller.StartType = ServiceStartMode.Automatic;

        //Service can start with delay
        serviceInstaller.DelayedAutoStart = true;

        //This service need "KinectMonitor" the service to run
        serviceInstaller.ServicesDependedOn = new string[] {"KinectMonitor"};

        serviceInstaller.ServiceName = ServiceName;

        // Info for users
        serviceInstaller.DisplayName = "Wyprostuj się";
        serviceInstaller.Description = WyprostujSie.Properties.Resources.ServiceInstallerDescr.ToString();
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