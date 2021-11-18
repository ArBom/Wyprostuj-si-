using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Notification.Wpf;

namespace WyprostujSieBackground
{
    public partial class Wyprostuj_sie : ServiceBase
    {
        Kinect kinect;
        Data data;

        public Wyprostuj_sie()
        {
            kinect = new Kinect(false);
            data = new Data(true, false);


            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStart.txt");
        }

        protected override void OnStop()
        {
            kinect = null;

            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStop.txt");
        }
    }
}
