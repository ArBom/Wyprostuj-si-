﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace WyprostujSieBackground
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void ServiceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(serviceInstaller.ServiceName).Start();
        }

        private void ServiceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
