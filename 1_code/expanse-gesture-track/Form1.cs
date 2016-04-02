using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using KinectLibrary;
using KinectLibrary.Contour;
using Microsoft.Kinect;
using KinectLibrary.Curves;
using KinectLibrary.Fingers;
using KinectLibrary.DTWGestureRecognition;
namespace kinect2
{
    public partial class Form1 : Form
    {

        
        Kinect k=new Kinect();
        short[] colorData = null;
        byte[] colorData1 = null;
        byte[] colorPixel = null;
        IntPtr depthPtr;
        IntPtr depthPtr1;
        Bitmap kinectBitmap = null;
        Bitmap kinectBitmap1 = null;
        IKinect kinect = new Kinect();
        IGestureRecognition j;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            //   while (true)
            //    {
            //   k = new Kinect();
            kinect.DepthDistanceUpdated += new DepthDistanceEventHandler(kinect_DepthDistanceUpdated);
            Main obj = new Main(kinect);
          
            IFingerRecognition Finger = obj.FingerRecognition;
            Finger.FingertipLocationsReady += new FingertipPoints(Finger_FingertipLocationsReady);
                k.Sensor.ColorFrameReady+= new EventHandler<ColorImageFrameReadyEventArgs>(ColorFrameReady);
            j = obj.GestureRecognition;
            
           // k.Sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(DepthFrameReady);
            //kinect_ColorFrameReady();
            //  kinect_DepthFrameReady();
            // }
        }

        private void Finger_FingertipLocationsReady(IEnumerable<Fingertip> points)
        {
            kinectBitmap = new Bitmap(640, 480, PixelFormat.Format24bppRgb);
            List<Fingertip> Points = points.ToList();
            for (int i = 0; i < Points.Count; i++)
            {
                colorPixel[(int)(Points[i].Position.Y * 640 + Points[i].Position.X) * 3 + 0] = 255;
                colorPixel[(int)(Points[i].Position.Y * 640 + Points[i].Position.X) * 3 + 1] = 0;
                colorPixel[(int)(Points[i].Position.Y * 640 + Points[i].Position.X) * 3 + 2] = 0;
            }
            BitmapData bmpData = kinectBitmap.LockBits(
                  new Rectangle(0, 0, kinectBitmap.Width, kinectBitmap.Height),
                ImageLockMode.WriteOnly, kinectBitmap.PixelFormat);
            Marshal.Copy(colorPixel, 0, bmpData.Scan0, colorPixel.Length);
            kinectBitmap.UnlockBits(bmpData);
            Bitmap newbitmap = new Bitmap(kinectBitmap, 320, 240);
            pictureBox1.Image = newbitmap;
        }

        private void kinect_DepthDistanceUpdated(short[] depthDistanceData, int width, int height)
        {
            int colorPixelIndex = 0;
            colorPixel = new byte[k.Sensor.DepthStream.FramePixelDataLength * 3];
       //     kinectBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            for (int i = 0; i < depthDistanceData.Length; i++)
            {

                byte intensity = (byte)(depthDistanceData[i] >= 800 && depthDistanceData[i] <= 4000 ? (depthDistanceData[i] / 8192.0) * 255.0 : 0);

                colorPixel[colorPixelIndex++] = intensity;
                colorPixel[colorPixelIndex++] = intensity;
                colorPixel[colorPixelIndex++] = intensity;
            }
       //     BitmapData bmpData = kinectBitmap.LockBits(
         //           new Rectangle(0, 0, kinectBitmap.Width, kinectBitmap.Height),
           //       ImageLockMode.WriteOnly, kinectBitmap.PixelFormat);
           // Marshal.Copy(colorPixel, 0, bmpData.Scan0, colorPixel.Length);
           // kinectBitmap.UnlockBits(bmpData);
           // Bitmap newbitmap = new Bitmap(kinectBitmap, 320, 240);
           // pictureBox1.Image = newbitmap;
            //pictureBox1.Refresh();
        }

        private void DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            DepthImage dImg = k.DepthImageData;
            colorPixel = new byte[k.Sensor.DepthStream.FramePixelDataLength * 3];
            kinectBitmap = new Bitmap(dImg.Width, dImg.Height, PixelFormat.Format24bppRgb);
            colorData = dImg.DepthData;
             int colorPixelIndex = 0;
            int mindepth =800;
               int maxdepth = 4000;
        //    MessageBox.Show("sstucj");
            RangeFinder range = new RangeFinder() ;
            Pixel[] pixArray = range.PixelsInRange(dImg.DepthData,800,4000);
            List<Pixel> inRangePix=new List<Pixel>();
             for (int count = 0; count < pixArray.Count(); count++)
             {
                if (pixArray[count].ToString() == "InRange")
                    inRangePix.Add(pixArray[count]);

            }
            Pixel[] InRangePix = inRangePix.ToArray();
            ContourTracking contourTrack = new ContourTracking();
            IEnumerable<Vector> vecPoints = contourTrack.StartTracking(pixArray, dImg.Width,dImg.Height);
            List<Vector> vecPoints2 = vecPoints.ToList();
            //MessageBox.Show(vecPoints2.Count.ToString());
           // CurveDetection curves = new CurveDetection();
           // IEnumerable<CurvePoint> curvePoints = curves.FindCurves(vecPoints);
           // FingerRecognition fingures = new FingerRecognition(range);
           // IEnumerable<Fingertip> fingTips = fingures.FindFingertipLocations(curvePoints,pixArray,dImg.Width,dImg.Height);
          //  foreach (Fingertip f in fingTips)
           // {
             //   label1.Text = curvePoints.Count().ToString();
           // }
            for (int i = 0; i < colorData.Length; i++)
            {
               
                byte intensity = (byte)(colorData[i] >= mindepth && colorData[i] <= maxdepth ? (colorData[i]/8192.0)*255.0:0);

            colorPixel[colorPixelIndex++] = intensity;
            colorPixel[colorPixelIndex++] = intensity;                     
            colorPixel[colorPixelIndex++] = intensity;
             }
            for (int i = 0; i < vecPoints2.Count; i++)
            {
                colorPixel[(int)(vecPoints2[i].Y * 640 + vecPoints2[i].X) * 3 + 0] = 0;
                colorPixel[(int)(vecPoints2[i].Y * 640 + vecPoints2[i].X) * 3 + 1] = 0;
                colorPixel[(int)(vecPoints2[i].Y * 640 + vecPoints2[i].X) * 3 + 2] = 255;
            }
               BitmapData bmpData = kinectBitmap.LockBits(
                    new Rectangle(0, 0, kinectBitmap.Width, kinectBitmap.Height),
                  ImageLockMode.WriteOnly, kinectBitmap.PixelFormat);
             Marshal.Copy(colorPixel, 0, bmpData.Scan0, colorPixel.Length);
            kinectBitmap.UnlockBits(bmpData);
            Bitmap newbitmap = new Bitmap(kinectBitmap, 320, 240);
            pictureBox1.Image = newbitmap;
            pictureBox1.Refresh();
          //  kinectBitmap.Dispose();
        }

        private void ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImage cImg = k.ColorImage;
            Marshal.FreeHGlobal(depthPtr);
            //MessageBox.Show(cImg.PixelData.Length.ToString());
            depthPtr = Marshal.AllocHGlobal(cImg.PixelData.Length);

            Marshal.Copy(cImg.PixelData, 0, depthPtr, cImg.PixelData.Length);

            kinectBitmap1 = new Bitmap(cImg.Width, cImg.Height, cImg.Width * 4, PixelFormat.Format32bppRgb, depthPtr);
            Bitmap newbitmap = new Bitmap(kinectBitmap1, 320, 240);
            pictureBox2.Image = newbitmap;
            pictureBox2.Refresh();
        }

      

        private void kinect_ColorFrameReady()
        {

            //       if (depthFrame == null) return;
            //       if (colorData1 == null)
            //       colorData1 = new byte[depthFrame.PixelDataLength];
            //MessageBox.Show(depthFrame.BytesPerPixel.ToString());
            // depthFrame.CopyPixelDataTo(colorData1);
//k = new Kinect() ;

            ColorImage cImg = k.ColorImage;
            Marshal.FreeHGlobal(depthPtr);
            //MessageBox.Show(cImg.PixelData.Length.ToString());
            depthPtr = Marshal.AllocHGlobal(cImg.PixelData.Length);

            Marshal.Copy(cImg.PixelData, 0, depthPtr, cImg.PixelData.Length);

            kinectBitmap1 = new Bitmap(cImg.Width, cImg.Height, cImg.Width * 4, PixelFormat.Format32bppRgb, depthPtr);
            Bitmap newbitmap = new Bitmap(kinectBitmap1, 320, 240);
            pictureBox2.Image = newbitmap;
        //    k.Dispose();

        }
        private void kinect_DepthFrameReady()
        {
            //  colorPixel = new byte[kinect.DepthStream.FramePixelDataLength * sizeof(int)];

            //  using (DepthImageFrame depthFrame =e.OpenDepthImageFrame())
            // {
            //     kinectBitmap = new Bitmap(depthFrame.Width, depthFrame.Height, PixelFormat.Format32bppRgb);
            //    int mindepth = depthFrame.MinDepth;
            //   int maxdepth = depthFrame.MaxDepth;

            // if (depthFrame == null) return;
            // if (colorData == null)
            //   colorData = new short[depthFrame.PixelDataLength];
            // MessageBox.Show(mindepth.ToString()+":"+maxdepth.ToString());
            //depthFrame.CopyPixelDataTo(colorData);
            // int colorPixelIndex = 0;
            // MessageBox.Show(colorData[0].ToString());
            //  for (int i = 0; i < colorData.Length; i++)
            //{

            //   byte intensity = (byte)(colorData[i] >= mindepth && colorData[i] <= maxdepth ? colorData[i]:0);
            //  byte intensity = (byte)((colorData[i]/255));
            // Write out blue byte
            //colorPixel[colorPixelIndex++] = intensity;

            // Write out green byte
            //colorPixel[colorPixelIndex++] = intensity;

            // Write out red byte                        
            //colorPixel[colorPixelIndex++] = intensity;
            //++colorPixelIndex;
            // }
          //  DepthImage dImg = k.DepthImageData;
          
            Marshal.FreeHGlobal(depthPtr1);
            //depthPtr1 = Marshal.AllocHGlobal(k.DepthImageData.DepthData.Length);
            //   BitmapData bmpData = kinectBitmap.LockBits(
            //        new Rectangle(0, 0, kinectBitmap.Width, kinectBitmap.Height),
            //      ImageLockMode.WriteOnly, kinectBitmap.PixelFormat);
           // Marshal.Copy(k.DepthImageData.DepthData, 0, depthPtr1, k.DepthImageData.DepthData.Length);
            //kinectBitmap = new Bitmap(k.DepthImageData.Width, k.DepthImageData.Height, k.DepthImageData.Width * 2, PixelFormat.Format32bppRgb, depthPtr1);
            //    kinectBitmap.UnlockBits(bmpData);
            //Bitmap newbitmap = new Bitmap(kinectBitmap, 320, 240);
            //pictureBox1.Image = newbitmap;
            //kinectBitmap.Dispose();
            //    }
            //  throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (button1.Text == "Start Recording Gesture")
            {
                
                button1.Text = "Stop Recording Gesture";
                j.StartRecognizer();
                   j.GestureRecorded += new GestureRecorded(j_GestureRecorded);
                MessageBox.Show(j.Recording.ToString());

            }
            else
            {
                button1.Text = "Start Recording Gesture";
               
               
                j.StopRecognizer();
            }
        }

        private void j_GestureRecognized(Gesture recognizedGesture)
        {
            
        }

        private void j_GestureRecorded(Gesture recordedGesture)
        {
            string path = @"C: \Users\Marc\Documents\Visual Studio 2012\Projects\"; //change to user's location
            MessageBox.Show(j.SaveGesturesToFile(path).ToString());
        }
    }
}
