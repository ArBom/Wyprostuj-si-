using System;
using System.ServiceProcess;
using System.Timers;

namespace WyprostujSieBackground
{
    public partial class Wyprostuj_sie : ServiceBase
    {

        Kinect kinect;
        Data data;
        Timer timer;
        KalmanFilter[] KalFils;
        bool IsUserQuirk;
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
                IsUserQuirk = false;
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
            if ((data.BokAnB && Math.Abs((float)KalFils[2].Output(kinect.BokAn) - 1.57f) > data.BokAnD / 10) 
             || (data.NeckAnB && Math.Abs((float)KalFils[1].Output(kinect.NeckAn)) > data.NeckAnD / 10)
             || (data.SpineAnB && Math.Abs((float)KalFils[0].Output(kinect.SpineAn) - 1.57f) > data.SpineAnD / 10))
            {
                if (!IsUserQuirk)
                {
                    IsUserQuirk = true;
                    TPComLinArg = Data.BadPostureKey;
                    kinect.takePic = true;
                    timer.Interval = 100;
                }
            }
            else
            {
                if (IsUserQuirk)
                {
                    IsUserQuirk = false;
                    Toast.ShowForDebug(Toast.RemoveToastKey);
                    timer.Interval = 1000;
                }
            }
        }

        protected void MenInPhotoChanged (int howMany)
        {
            if (howMany == 0 && data.NoPersB)
            {
                timer.Interval = 250;
                Toast.ShowForDebug(Data.NoPersBKey);
            }

            else if (howMany == 1)
            {
                timer.Interval = 1000;
                IsUserQuirk = false;
                Toast.ShowForDebug(Toast.RemoveToastKey);
            }

            else if (howMany > 1 && data.TMPersB)
            {
                timer.Interval = 250;
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
            kinect.studyData = StudyData.KeepGoing;
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
