using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Easy_Hologram_4D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFlyer(@"k:\Second Test");
        }

     

        private void PyramidBase_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (txtInstructions.Text == "Tap Here and place holograpic pyramid on this rectangle")
            {
                this.txtInstructions.Text = "Tap Here to view images in normal view";
                ConvertToHologram();
            }
            else
            {
                txtInstructions.Text = "Tap Here and place holograpic pyramid on this rectangle";
                BackToNormal();
            }
        }


        private void ConvertToHologram()
        {
            TransformedBitmap bmp90 = new TransformedBitmap();
            bmp90.BeginInit();
            bmp90.Source = (BitmapSource)img90Deg.Source;

            RotateTransform transform90 = new RotateTransform(90);
            bmp90.Transform = transform90;
            bmp90.EndInit();
            img90Deg.Source = bmp90;

            bmp90 = null;
            transform90 = null;

            //---------------------------
            TransformedBitmap bmpHVFlip = new TransformedBitmap();
            bmpHVFlip.BeginInit();
            bmpHVFlip.Source = (BitmapSource)imgHV_Flip.Source;

            ScaleTransform transformFlip = new ScaleTransform();
            transformFlip.ScaleX = -1;
            transformFlip.ScaleY = -1;
            bmpHVFlip.Transform = transformFlip;
            bmpHVFlip.EndInit();
            imgHV_Flip.Source = bmpHVFlip;

            bmpHVFlip = null;
            transformFlip = null;

            //-------------------------------------------
            TransformedBitmap bmpNeg90 = new TransformedBitmap();
            bmpNeg90.BeginInit();
            bmpNeg90.Source = (BitmapSource)imgNeg90Deg.Source;

            RotateTransform transformNeg90 = new RotateTransform(-90);
            bmpNeg90.Transform = transformNeg90;
            bmpNeg90.EndInit();
            imgNeg90Deg.Source = bmpNeg90;

            bmpNeg90 = null;
            transformNeg90 = null;
        }

        private void BackToNormal()
        {
            TransformedBitmap bmp90 = new TransformedBitmap();
            bmp90.BeginInit();
            bmp90.Source = (BitmapSource)img90Deg.Source;

            RotateTransform transform90 = new RotateTransform(-90);
            bmp90.Transform = transform90;
            bmp90.EndInit();
            img90Deg.Source = bmp90;

            bmp90 = null;
            transform90 = null;

            //---------------------------
            TransformedBitmap bmpHVFlip = new TransformedBitmap();
            bmpHVFlip.BeginInit();
            bmpHVFlip.Source = (BitmapSource)imgHV_Flip.Source;

            ScaleTransform transformFlip = new ScaleTransform();
            transformFlip.ScaleX = -1;
            transformFlip.ScaleY = -1;
            bmpHVFlip.Transform = transformFlip;
            bmpHVFlip.EndInit();
            imgHV_Flip.Source = bmpHVFlip;

            bmpHVFlip = null;
            transformFlip = null;

            //-------------------------------------------
            TransformedBitmap bmpNeg90 = new TransformedBitmap();
            bmpNeg90.BeginInit();
            bmpNeg90.Source = (BitmapSource)imgNeg90Deg.Source;

            RotateTransform transformNeg90 = new RotateTransform(90);
            bmpNeg90.Transform = transformNeg90;
            bmpNeg90.EndInit();
            imgNeg90Deg.Source = bmpNeg90;

            bmpNeg90 = null;
            transformNeg90 = null;
        }

        private void OpenFlyer(String FileName)
        {
            try
            {
                using (FileStream fs = File.Open(FileName,FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Design>));
                    List<Design> data = (List<Design>)serializer.Deserialize(fs);

                    for (int i = 0; i < data.Count; i++)
                    {
                        try
                        {
                            Design imgcls = data[i];

                            var image = new BitmapImage();
                            using (var mem = new MemoryStream(imgcls.ImageData))
                            {
                                mem.Position = 0;
                                image.BeginInit();
                                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.UriSource = null;
                                image.StreamSource = mem;
                                image.EndInit();
                            }
                            image.Freeze();

                            if (imgcls.ImageType == "img0Deg")
                                img0Deg.Source = image;
                            else if (imgcls.ImageType == "img90Deg")
                                img90Deg.Source = image;
                            else if (imgcls.ImageType == "imgHV_Flip")
                                imgHV_Flip.Source = image;
                            else if (imgcls.ImageType == "imgNeg90Deg")
                                imgNeg90Deg.Source = image;


                        }
                        catch (Exception)
                        {
                            //add some code here
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }

    public class Design
    {
        byte[] _ImageData;
        double _ImageCtrlWidth;
        double _ImageCtrlHeight;
        double _ImagePixelWidth;
        double _ImagePixelHeight;
        string _imageType;

        public byte[] ImageData
        {
            get { return _ImageData; }
            set { _ImageData = value; }
        }

        public double ImageCtrlWidth
        {
            get { return _ImageCtrlWidth; }
            set { _ImageCtrlWidth = value; }
        }

        public double ImageCtrlHeight
        {
            get { return _ImageCtrlHeight; }
            set { _ImageCtrlHeight = value; }
        }

        public double ImagePixelWidth
        {
            get { return _ImagePixelWidth; }
            set { _ImagePixelWidth = value; }
        }

        public double ImagePixelHeight
        {
            get { return _ImagePixelHeight; }
            set { _ImagePixelHeight = value; }
        }

        public string ImageType
        {
            get { return _imageType; }
            set { _imageType = value; }
        }
    }

}
