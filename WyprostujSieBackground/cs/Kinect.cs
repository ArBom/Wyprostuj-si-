using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if DEBUG
using System.Diagnostics;
#endif

using Microsoft.Kinect;

namespace WyprostujSieBackground
{
    public enum OnTick { Always, Waiting, Going};

    public class Kinect
    {
        private KinectSensor kinectSensor = null;
        private CoordinateMapper coordinateMapper = null;

        private readonly int displayWidth;
        private readonly int displayHeight;

        private const double JointThickness = 3;
        private const double ClipBoundsThickness = 10;
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        public OnTick onTick;
        private readonly bool draw;
        public bool takePic = false;

        public MultiSourceFrameReader multiSourceFrameReader;
        public ColorFrameReader colorFrameReader;
        public WriteableBitmap colorBitmap = null;

        public delegate void PersonAtPhoto(int howMany);
        public delegate void TakenPic(Uri uri);
        public Action newData = null;
        public TakenPic takenPic = null;
        public PersonAtPhoto personAtPhoto = null;

        private int PersonAtLastPhoto = 0;

        public double SpineAn { get; private set; } = 0;
        public double BokAn { get; private set; } = 0;
        public double NeckAn { get; private set; } = 0;

        readonly double SpineAnTreshold;
        readonly double BokAnTreshold;
        readonly double NeckAnTreshold;

        readonly bool SpineAnWatched;
        readonly bool BokAnWatched;
        readonly bool NeckAnWatched;

        private const float InferredZPositionClamp = 0.1f;

        public string StatusText;
        private DrawingGroup drawingGroup;
        public DrawingImage ImageSource { get; private set; }

        public BodyFrameReader bodyFrameReader = null;
        private Body[] bodies = null;
        private List<Tuple<JointType, JointType>> bones;

        private List<Pen> bodyColors;

        public const string PhotoFileName = "photo.jpg";

        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e) //TODO
        {
            // on failure, set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? "RunningStatusText"
                                                            : "SensorNotAvailableStatusText";
        }

        private void Angles (ref Body body)
        {
            Joint head = body.Joints[JointType.Head];
            Joint neck = body.Joints[JointType.Neck];
            Joint spineBase = body.Joints[JointType.SpineBase];

            if (neck.TrackingState == TrackingState.Tracked && spineBase.TrackingState == TrackingState.Tracked)
            {
                SpineAn = Math.Atan2(neck.Position.Y - spineBase.Position.Y, neck.Position.X - spineBase.Position.X);
                BokAn = Math.Atan2(neck.Position.Y - spineBase.Position.Y, neck.Position.Z - spineBase.Position.Z);

                if (head.TrackingState == TrackingState.Tracked)
                    NeckAn = Math.Atan2(head.Position.Y - neck.Position.Y, head.Position.X - neck.Position.X) - SpineAn;
            }

            if (SpineAnWatched)
            {
                if (SpineAn > SpineAnTreshold)
                    takePic = true;
            }

            if (BokAnWatched)
            {
                if (BokAn > BokAnTreshold)
                    takePic = true;
            }

            if (NeckAnWatched)
            {
                if (NeckAn > NeckAnTreshold)
                    takePic = true;
            }
        }

        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            if((int)joint0.JointType >= 4 && (int)joint1.JointType >= 4)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {             
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }

        public void MultisourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if (this.onTick == OnTick.Always || this.onTick == OnTick.Going || takePic)
            {
                if (this.onTick == OnTick.Going)
                    this.onTick = OnTick.Waiting;

                MultiSourceFrame reference = e.FrameReference.AcquireFrame();

            if (draw || takePic)
            {
                using (var colorFrame = reference.ColorFrameReference.AcquireFrame())
                {
                    if (colorFrame != null)
                    {
                        FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                        using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                        {
                            if (this.colorBitmap == null)
                            {
                                this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96, 96, PixelFormats.Bgr32, null);
                            }

                            this.colorBitmap.Lock();

                            // verify data and write the new color frame data to the display bitmap
                            if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                            {
                                colorFrame.CopyConvertedFrameDataToIntPtr(
                                    this.colorBitmap.BackBuffer,
                                    (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                    ColorImageFormat.Bgra);

                                this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                            }
                            this.colorBitmap.Unlock();
                        }
                    }
                }

                if (takePic)
                {
                    string Path = SaveColorBitmap();
                    takePic = false;
                    Uri uriOfPic = new Uri(Path, UriKind.Absolute);
                    takenPic?.Invoke(uriOfPic);
                }
            }


            bool dataReceived = false;
            using (BodyFrame bodyFrame = reference.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }
            if (dataReceived)
            {
                if (draw)
                {
                    using (DrawingContext dc = this.drawingGroup.Open())
                    {
                        // Draw a transparent background to set the render size
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                        int penIndex = 0;
                        foreach (Body body in this.bodies)
                        {
                            Pen drawPen = this.bodyColors[penIndex++];

                            if (body.IsTracked)
                            {
                                this.DrawClippedEdges(body, dc);

                                IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                                // convert the joint points to depth (display) space
                                Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                                foreach (JointType jointType in joints.Keys)
                                {
                                    // sometimes the depth(Z) of an inferred joint may show as negative
                                    // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                                    CameraSpacePoint position = joints[jointType].Position;
                                    if (position.Z < 0)
                                    {
                                        position.Z = InferredZPositionClamp;
                                    }

                                    DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                                    jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                                }
                                this.DrawBody(joints, jointPoints, dc, drawPen);
                            }
                        }

                        // prevent drawing outside of our render area
                        this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                    }
                }

                int trB = bodies.Where(b => b.IsTracked).Count();

                if (PersonAtLastPhoto != trB)
                    personAtPhoto?.Invoke(trB);
                PersonAtLastPhoto = trB;

                if (trB == 1)
                {
                    Body b = bodies.Where(bc => bc.IsTracked).First();
                    {
                        Angles(ref b);
                    }
                }
            }

            newData?.Invoke();
        }
        }

        string SaveColorBitmap()
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configFolder = Path.Combine(commonAppData, "WyprostujSie");
            var PicPath = Path.Combine(configFolder, PhotoFileName);

            PicPath = "C://Users//arkad//AppData//Roaming//WyprostujSie//photo.jpg"; //TODO
            //using (
            File.Delete(PicPath);
            FileStream stream = new FileStream(PicPath, FileMode.CreateNew);
               // )
           // {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(colorBitmap));
                encoder.Save(stream);

            stream.Close();
            //}

            return PicPath;
        }

        public Kinect(bool SpineAnWatched, bool BokAnWatched, bool NeckAnWatched, double SpineAnTreshold, double BokAnTreshold, double NeckAnTreshold)
        {
            this.SpineAnTreshold = SpineAnTreshold;
            this.BokAnTreshold = BokAnTreshold;
            this.NeckAnTreshold = NeckAnTreshold;

            this.SpineAnWatched = SpineAnWatched;
            this.BokAnWatched = BokAnWatched;
            this.NeckAnWatched = NeckAnWatched;
            this.draw = false;

            this.kinectSensor = KinectSensor.GetDefault();
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            onTick = OnTick.Waiting;

            StartWork();
        }

        public Kinect(bool draw)
        {
            this.draw = draw;

            this.SpineAnWatched = false;
            this.BokAnWatched = false;
            this.NeckAnWatched = false;

            this.kinectSensor = KinectSensor.GetDefault();
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            onTick = OnTick.Always;
            StartWork();
        }

        private void StartWork()
        {
            multiSourceFrameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);

            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>
            {

                // Torso
                new Tuple<JointType, JointType>(JointType.Head, JointType.Neck),
                new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder),
                new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid),
                new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase),
                new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight),
                new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft),
                new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight),
                new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft),

                // Right Arm
                new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight),
                new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight),
                new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight),
                new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight),
                new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight),

                // Left Arm
                new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft),
                new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft),
                new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft),
                new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft),
                new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft),

                // Right Leg
                new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight),
                new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight),
                new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight),

                // Left Leg
                new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft),
                new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft),
                new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft)
            };

            // populate body colors, one for each BodyIndex
            this.bodyColors = new List<Pen>
            {
                new Pen(Brushes.Red, 6),
                new Pen(Brushes.Orange, 6),
                new Pen(Brushes.Green, 6),
                new Pen(Brushes.Blue, 6),
                new Pen(Brushes.Indigo, 6),
                new Pen(Brushes.Violet, 6)
            };

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? "Kinect podłączony"
                                                            : "Nie znaleziono Kinect'a";

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.ImageSource = new DrawingImage(this.drawingGroup);

            if (multiSourceFrameReader != null)
            {
                multiSourceFrameReader.MultiSourceFrameArrived += MultisourceFrameArrived;
            }
        }

        ~Kinect()
        {
            if (this.kinectSensor != null)
            {
                try
                {
                    if (this.kinectSensor.IsOpen)
                        this.kinectSensor.Close();
                }
                catch(Exception e)
                {
#if DEBUG
                    Debug.WriteLine(e.ToString());
#endif
                }
                this.kinectSensor = null;
            }
        }

    }
}
