using System;
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
        KalmanFilter[] KalFils;
        string TPComLinArg = "";

        public Wyprostuj_sie()
        {
            timer = new Timer
            {
                Interval = 1000, //1Hz
                AutoReset = true,
            };
            timer.Elapsed += onTick;

            data = new Data(true, false);

            KalmanFilter Spin = new KalmanFilter(1, 1, 0.18, 1, 0.095, data.SpineAnD);
            KalmanFilter Neck = new KalmanFilter(1, 1, 0.175, 1, 0.09, data.NeckAnD);
            KalmanFilter Side = new KalmanFilter(1, 1, 0.175, 1, 0.09, data.BokAnD);

            KalFils = new KalmanFilter[] { Spin, Neck, Side };

            kinect = new Kinect();

            if (data.SpineAnB || data.NeckAnB || data.BokAnB)
            {
                kinect.newData += NewAngles;
            }

            if (data.NoPersB || data.TMPersB)
            {
                kinect.personAtPhoto += MenInPhotoChanged;
            }

            kinect.takenPic += ShowToast;

            InitializeComponent();
        }

        protected void NewAngles()
        {
            if (false)
            {
                TPComLinArg = Data.BadPostureKey;
                kinect.takePic = true;
            }
        }

        protected void MenInPhotoChanged (int howMany)
        {
            if (howMany == 0 && data.NoPersB)
            {
                Toast.ShowForDebug(Data.NoPersBKey);
            }

            else if (howMany == 1)
                Toast.ShowForDebug(Toast.RemoveToastKey);

            else if (howMany > 1 && data.TMPersB)
            {
                Toast.ShowForDebug(Data.NoPersBKey);
                TPComLinArg = Data.TMPersBKey;
            }
        }

        protected void ShowToast()
        {
            Toast.ShowForDebug(TPComLinArg);
        }

        protected void onTick (Object source, ElapsedEventArgs e)
        {
            kinect.onTick = OnTick.Going;
        }

        protected override void OnStart(string[] args)
        {
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Elapsed -= onTick;
            timer.Stop();
            timer = null;
            kinect = null;
        }
    }
}
