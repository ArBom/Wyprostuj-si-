﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace WyprostujSieBackground
{
    public partial class Wyprostuj_sie : ServiceBase
    {
        Kinect kinect;
        Data data;
        Timer timer;
        int counter = 0;

        public Wyprostuj_sie()
        {
            timer = new Timer
            {
                Interval = 10000, //1 min.
                AutoReset = true,     
            };
            timer.Elapsed += onTick;

            data = new Data(true, false);
            kinect = new Kinect(data.SpineAnB, data.BokAnB, data.NeckAnB, data.SpineAnD, data.BokAnD, data.NeckAnD);

            //kinect.takenPic += Toast.ShowNot;

            InitializeComponent();
        }

        protected void onTick (Object source, ElapsedEventArgs e)
        {
            Toast.ShowForDebug(Data.BadPostureKey);

            counter++;

            if (counter>10)
            {
                timer.Stop();
            }
            //kinect.onTick = OnTick.Going;
            //kinect.MultisourceFrameArrived(null, null);
            int a = 50;
        }

        protected override void OnStart(string[] args)
        {
            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStart.txt");
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Elapsed -= onTick;
            kinect = null;

            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStop.txt");
        }
    }
}
