using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fotobudka.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    /// 
   
    public partial class Home : UserControl
    {
        VideoCaptureDevice LocalWebCam;
        public FilterInfoCollection LoaclWebCamsCollection;
        public Home()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            LocalWebCam.Stop();
        }

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                System.Drawing.Image img = (Bitmap)eventArgs.Frame.Clone();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    frameHolder.Source = bi;
                }));

                
            }
            catch (Exception ex)
            {
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoaclWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (LoaclWebCamsCollection.Count != 0)
            {
                // add all devices to combo
                foreach (FilterInfo device in LoaclWebCamsCollection)
                {
                    camerasBox.Items.Add(device.Name);
                }
            }
            else
            {
                camerasBox.Items.Add("Nie znaleziono kamer");
            }

            camerasBox.SelectedIndex = 0;

            LocalWebCam = new VideoCaptureDevice(LoaclWebCamsCollection[0].MonikerString);
            LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);

            LocalWebCam.Start();
        }

    }
}
