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
    public partial class Service1 : ServiceBase
    {
        Kinect kinect;
        Data data;

        public Service1()
        {
            kinect = new Kinect(false);
            data = new Data(true);

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

        }

        protected override void OnStop()
        {

        }
    }
}
