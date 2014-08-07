﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Kinect;

using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        KinectSensor sensor;

        public Form1()
        {
            InitializeComponent();
            sensor = KinectSensor.KinectSensors[0];

            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution1280x960Fps12);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensor.SkeletonStream.Enable();

            if (this.WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }

            sensor.AllFramesReady += FramesReady;
            sensor.Start();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        void FramesReady(object sender, AllFramesReadyEventArgs e)
        {
            ColorImageFrame VFrame = e.OpenColorImageFrame();
            if (VFrame == null) return;
            byte[] pixelS = new byte[VFrame.PixelDataLength];
            Bitmap bmap = ImageToBitmap(VFrame);


            SkeletonFrame SFrame = e.OpenSkeletonFrame();
            if (SFrame == null) return;

            Graphics g = Graphics.FromImage(bmap);
            Skeleton[] Skeletons = new Skeleton[SFrame.SkeletonArrayLength];
            SFrame.CopySkeletonDataTo(Skeletons);

            foreach (Skeleton S in Skeletons)
            {
                if (S.TrackingState == SkeletonTrackingState.Tracked)
                {

                    //body
                    DrawBone(JointType.Head, JointType.ShoulderCenter, S, g);
                    DrawBone(JointType.ShoulderCenter, JointType.Spine, S, g);
                    DrawBone(JointType.Spine, JointType.HipCenter, S, g);
                    //left leg
                    DrawBone(JointType.HipCenter, JointType.HipLeft, S, g);
                    DrawBone(JointType.HipLeft, JointType.KneeLeft, S, g);
                    DrawBone(JointType.KneeLeft, JointType.AnkleLeft, S, g);
                    DrawBone(JointType.AnkleLeft, JointType.FootLeft, S, g);
                    //Right Leg
                    DrawBone(JointType.HipCenter, JointType.HipRight, S, g);
                    DrawBone(JointType.HipRight, JointType.KneeRight, S, g);
                    DrawBone(JointType.KneeRight, JointType.AnkleRight, S, g);
                    DrawBone(JointType.AnkleRight, JointType.FootRight, S, g);
                    //Left Arm
                    DrawBone(JointType.ShoulderCenter, JointType.ShoulderLeft, S, g);
                    DrawBone(JointType.ShoulderLeft, JointType.ElbowLeft, S, g);
                    DrawBone(JointType.ElbowLeft, JointType.WristLeft, S, g);
                    DrawBone(JointType.WristLeft, JointType.HandLeft, S, g);
                    //Right Arm
                    DrawBone(JointType.ShoulderCenter, JointType.ShoulderRight, S, g);
                    DrawBone(JointType.ShoulderRight, JointType.ElbowRight, S, g);
                    DrawBone(JointType.ElbowRight, JointType.WristRight, S, g);
                    DrawBone(JointType.WristRight, JointType.HandRight, S, g);


                    //Check that hand is higher than head
                    label1.Text = (S.Joints[JointType.HandRight].Position.Y > S.Joints[JointType.Head].Position.Y).ToString();
                }



            }
            pictureBox1.Image = bmap;

        }

        void DrawBone(JointType j1, JointType j2, Skeleton S, Graphics g)
        {
            Point p1 = GetJoint(j1, S);
            Point p2 = GetJoint(j2, S);
            g.DrawLine(Pens.Red, p1, p2);
        }

        Point GetJoint(JointType j, Skeleton S)
        {
            SkeletonPoint Sloc = S.Joints[j].Position;
            ColorImagePoint Cloc = sensor.MapSkeletonPointToColor(Sloc, ColorImageFormat.RgbResolution1280x960Fps12);

            return new Point(Cloc.X, Cloc.Y);
        }



        Bitmap ImageToBitmap(
                           ColorImageFrame Image)
        {
            byte[] pixelData =
                     new byte[Image.PixelDataLength];
            Image.CopyPixelDataTo(pixelData);
            Bitmap bmap = new Bitmap(
                   Image.Width,
                   Image.Height,
                   PixelFormat.Format32bppRgb);
            BitmapData bmapS = bmap.LockBits(
              new Rectangle(0, 0,
                         Image.Width, Image.Height),
              ImageLockMode.WriteOnly,
              bmap.PixelFormat);
            IntPtr ptr = bmapS.Scan0;
            Marshal.Copy(pixelData, 0, ptr,
                       Image.PixelDataLength);
            bmap.UnlockBits(bmapS);
            return bmap;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sensor.Stop();
        }




    }
}
