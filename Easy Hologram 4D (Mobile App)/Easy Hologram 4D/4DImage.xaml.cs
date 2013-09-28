using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using Microsoft.Phone;
using System.Windows.Media;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Media;
using Microsoft.Live;

namespace Easy_Hologram_4D
{
    public partial class _4DImage : PhoneApplicationPage
    {
        string WorkingDesignName = string.Empty;

        public _4DImage()
        {
            InitializeComponent();
           
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                if (this.NavigationContext.QueryString.ContainsKey("FileName"))
                {
                    WorkingDesignName = string.Empty;
                    App.ExitCall = false;

                    OpenFlyer(this.NavigationContext.QueryString["FileName"]);
                }
               
            }
            else if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                if (App.SaveFileName != "" && App.FromWhere == "SaveHolo")
                {
                    if (App.SaveType == "H") // Save as Hologram
                    {
                        SaveFlyer(App.SaveFileName);

                        WorkingDesignName = App.SaveFileName;
                        //ApplicationTitle.Text = "DESIGNER :: " + App.SaveFileName;

                        if (App.ExitCall == true)
                        {
                            App.ExitCall = false;
                            NavigationService.GoBack();
                        }
                    }
                    else if (App.SaveType == "I") // Save as Image
                    {
                        MemoryStream ms = new MemoryStream();
                        WriteableBitmap wbmpThumbnail = new WriteableBitmap(this.ContentPanel, null);
                        wbmpThumbnail.SaveJpeg(ms, wbmpThumbnail.PixelWidth, wbmpThumbnail.PixelHeight, 0, 100);

                        ms.Seek(0, 0);
                        MediaLibrary mediaLibrary = new MediaLibrary();
                        Picture pic = mediaLibrary.SavePicture(App.SaveFileName + ".jpg", ms);

                        mediaLibrary.Dispose();
                        pic.Dispose();
                        ms.Flush();
                        ms.Close();
                        ms.Dispose();
                    }
                    else if (App.SaveType == "C") // Save in Cloud
                    {
                        if (App.SaveFileName.EndsWith(".4dh"))
                            WorkingDesignName = App.SaveFileName;
                        else
                            WorkingDesignName = App.SaveFileName + ".4dh";

                        SaveFlyer(WorkingDesignName);
                        
                        //ApplicationTitle.Text = "DESIGNER :: " + App.SaveFileName;

                        if (App.ExitCall == true)
                        {
                            App.ExitCall = false;
                            NavigationService.GoBack();
                        }

                        using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(WorkingDesignName, FileMode.Open,FileAccess.Read,FileShare.Read))
                            {
                                try{
                                    //LiveOperationResult res = await App.client.BackgroundUploadAsync("me/skydrive", new Uri("/shared/transfers/" + WorkingDesignName, UriKind.Relative), OverwriteOption.Overwrite);
                                    var res = await App.client.UploadAsync("me/skydrive", WorkingDesignName, stream, OverwriteOption.Overwrite);
                                    
                                    //InfoText.Text = "File " + WorkingDesignName + " uploaded";

                                    myIsolatedStorage.DeleteFile(WorkingDesignName);
                                }
                                catch (Exception){}
                            }
                        }

                    }

                }
                else if (App.SaveFileName == "" && App.FromWhere == "SaveFlyer")
                {
                    if (App.ExitCall == true)
                    {
                        App.ExitCall = false;
                        //App.ChangesSaved = false;

                        NavigationService.GoBack();
                    }
                }
            }

            App.SaveType = "H";
            App.FromWhere = "";
            base.OnNavigatedTo(e);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Do you want to save this image set?", "Save Hologram Images", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                App.ExitCall = true;
                btnSave_Click(sender, e);

            }
            else
            {
              //  e.Cancel=true; 
            }
        }

       //-----------Capture Images from Camera------------------
        CameraCaptureTask cameraCaptureTask;
        ExifLib.JpegInfo info;
        WriteableBitmap wbmp90;
        WriteableBitmap wbmpNeg90;
        WriteableBitmap wbmpHVFlip;

        private void img0Deg_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {// Top Image


            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Img0Deg_Completed);
            cameraCaptureTask.Show();

        }

        private void cameraCaptureTask_Img0Deg_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(e.ChosenPhoto);
                img0Deg.Source = bitmap;

            }
            else if (e.TaskResult == TaskResult.Cancel)
                return;// MessageBox.Show("Photo was not captured - operation was cancelled", "Photo not captured", MessageBoxButton.OK); 
            else
                MessageBox.Show("Error while capturing photo:\n" + e.Error.Message, "Fail", MessageBoxButton.OK);
        }

       
        private void img90Deg_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        { // Right Image
            
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Img90Deg_Completed);
            cameraCaptureTask.Show();

        }

        private void cameraCaptureTask_Img90Deg_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                info = ExifLib.ExifReader.ReadJpeg(e.ChosenPhoto, e.OriginalFileName);

                //wbmp90 = new WriteableBitmap(info.Width, info.Height);
                wbmp90 = new WriteableBitmap(1282, 720);

                e.ChosenPhoto.Position = 0;
                wbmp90.LoadJpeg(e.ChosenPhoto);
                img90Deg.Source = wbmp90;

                //BitmapImage bitmap = new BitmapImage();
                //bitmap.SetSource(e.ChosenPhoto);
                //img90Deg.Source = bitmap;

            }
            else if (e.TaskResult == TaskResult.Cancel)
                return;// MessageBox.Show("Photo was not captured - operation was cancelled", "Photo not captured", MessageBoxButton.OK); 
            else
                MessageBox.Show("Error while capturing photo:\n" + e.Error.Message, "Fail", MessageBoxButton.OK);
        }

        
        private void imgHV_Flip_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {// Bottom Image
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_ImgHVFlip_Completed);
            cameraCaptureTask.Show();
        }

        private void cameraCaptureTask_ImgHVFlip_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                info = ExifLib.ExifReader.ReadJpeg(e.ChosenPhoto, e.OriginalFileName);

                //wbmpHVFlip = new WriteableBitmap(info.Width, info.Height);
                wbmpHVFlip = new WriteableBitmap(1282, 720);

                e.ChosenPhoto.Position = 0;
                wbmpHVFlip.LoadJpeg(e.ChosenPhoto);
                imgHV_Flip.Source = wbmpHVFlip;

                //BitmapImage bitmap = new BitmapImage();
                //bitmap.SetSource(e.ChosenPhoto);
                //imgHV_Flip.Source = bitmap;

            }
            else if (e.TaskResult == TaskResult.Cancel)
                return;// MessageBox.Show("Photo was not captured - operation was cancelled", "Photo not captured", MessageBoxButton.OK); 
            else
                MessageBox.Show("Error while capturing photo:\n" + e.Error.Message, "Fail", MessageBoxButton.OK);
        }


        private void imgNeg90Deg_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {//Left Image
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_ImgNeg90Deg_Completed);
            cameraCaptureTask.Show();
        }

        private void cameraCaptureTask_ImgNeg90Deg_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                info = ExifLib.ExifReader.ReadJpeg(e.ChosenPhoto, e.OriginalFileName);

                //wbmpNeg90 = new WriteableBitmap(info.Width, info.Height);
                wbmpNeg90 = new WriteableBitmap(1282, 720);

                e.ChosenPhoto.Position = 0;
                wbmpNeg90.LoadJpeg(e.ChosenPhoto);
                imgNeg90Deg.Source = wbmpNeg90;

                //BitmapImage bitmap = new BitmapImage();
                //bitmap.SetSource(e.ChosenPhoto);
                //imgNeg90Deg.Source = bitmap;

            }
            else if (e.TaskResult == TaskResult.Cancel)
                return;// MessageBox.Show("Photo was not captured - operation was cancelled", "Photo not captured", MessageBoxButton.OK); 
            else
                MessageBox.Show("Error while capturing photo:\n" + e.Error.Message, "Fail", MessageBoxButton.OK);
        }



        //-----------END - Capture Images from Camera------------------




        // ---------------App bar commands ------------------

        private void btnNew_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Do you want to save this image set?", "Save Hologram Images", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                btnSave_Click(sender,e);

            }
            else
            {
               
            }

            this.txtImageName.Text = "My 4D Image (4D)";

            this.img0Deg.Source = new BitmapImage(new Uri("Assets/0Deg Image.png", UriKind.Relative));

            this.img90Deg.Source = new BitmapImage(new Uri("Assets/90Degrees Image.png", UriKind.Relative));

            this.imgHV_Flip.Source = new BitmapImage(new Uri("Assets/FlipHV Image.png", UriKind.Relative));

            this.imgNeg90Deg.Source = new BitmapImage(new Uri("Assets/Neg90Deg Image.png", UriKind.Relative)); 

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SavePage.xaml?HoloName=" + WorkingDesignName, UriKind.Relative));
        }

        private void btnHologram_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = sender as ApplicationBarIconButton;

            if (btn.Text == "Hologram")
            {
                btn.Text = "Normal";
                GenerateHologram();
            }
            else if (btn.Text == "Normal")
            {
                btn.Text = "Hologram";
                HologramToNormal();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {

        }


        // ---------------End - App bar commands ------------------

        private void GenerateHologram()
        {
            if (wbmp90 == null)
                wbmp90 = new WriteableBitmap(img90Deg.Source as BitmapSource);
            if (wbmpHVFlip == null)
                wbmpHVFlip = new WriteableBitmap(imgHV_Flip.Source as BitmapSource);
            if (wbmpNeg90 == null)
                wbmpNeg90 = new WriteableBitmap(imgNeg90Deg.Source as BitmapSource);

            //wbmp90.LoadJpeg(e.ChosenPhoto);
            
            img90Deg.Source = wbmp90.Rotate(90);
            //imgHV_Flip.Source = wbmpHVFlip.Flip(WriteableBitmapExtensions.FlipMode.Horizontal);
            //imgHV_Flip.Source = wbmpHVFlip.Flip(WriteableBitmapExtensions.FlipMode.Vertical);
            imgHV_Flip.Source = wbmpHVFlip.Rotate(180);
            imgNeg90Deg.Source = wbmpNeg90.Rotate(270);
        }

        private void HologramToNormal()
        {
            img90Deg.Source = wbmp90;
            imgHV_Flip.Source = wbmpHVFlip;
            imgNeg90Deg.Source = wbmpNeg90;
        }


        private void GenerateHologram_Old()
        {
            //1. Don't Touch First Picture

            //2. Roate 90 Degree
            //img90Deg.Source  = Utility.RotateDynaImage(img90Deg.Source ,90);
            WriteableBitmap wbmp;
            //wbmp.LoadJpeg()
            if (typeof(WriteableBitmap).Equals(img90Deg.Source))
            {
                wbmp = (WriteableBitmap)img90Deg.Source;
               // img90Deg.Stretch = Stretch.UniformToFill;
            }
            else
                wbmp = new WriteableBitmap(img90Deg.Source as BitmapSource);
  
            wbmp = WriteableBitmapExtensions.Rotate(wbmp, 90);
            img90Deg.Source = wbmp;
            wbmp = null;


            //3. Flip Horizontal and Flip Vertical

            WriteableBitmap wbmp1;
            if (typeof(WriteableBitmap).Equals(imgHV_Flip.Source))
                wbmp1 = (WriteableBitmap)imgHV_Flip.Source;
            else
                wbmp1 = new WriteableBitmap(imgHV_Flip.Source as BitmapSource);

            //wbmp = WriteableBitmapExtensions.Flip(wbmp, System.Windows.Media.Imaging.WriteableBitmapExtensions.FlipMode.Horizontal);
            wbmp1 = WriteableBitmapExtensions.Rotate(wbmp1, 180);
            imgHV_Flip.Source = wbmp1;
            wbmp1 = null;

            //if (typeof(WriteableBitmap).Equals(imgHV_Flip.Source))
            //    wbmp = (WriteableBitmap)imgHV_Flip.Source;
            //else
            //    wbmp = new WriteableBitmap(imgHV_Flip.Source as BitmapSource);

            //wbmp = WriteableBitmapExtensions.Flip(wbmp, System.Windows.Media.Imaging.WriteableBitmapExtensions.FlipMode.Vertical);
            //imgHV_Flip.Source = wbmp;
            //wbmp = null;

            //4. Rotate -90 Degree
           //imgNeg90Deg.Source =  Utility.RotateDynaImage(imgNeg90Deg.Source ,-90);

            WriteableBitmap wbmp2;
            if (typeof(WriteableBitmap).Equals(imgNeg90Deg.Source))
                wbmp2 = (WriteableBitmap)imgNeg90Deg.Source;
            else
                wbmp2 = new WriteableBitmap(imgNeg90Deg.Source as BitmapSource);

            wbmp2 = WriteableBitmapExtensions.Rotate(wbmp2, 270);
            imgNeg90Deg.Source = wbmp2;
            wbmp2 = null;
        }

        private void HologramToNormal_Old()
        {
            //1. Don't Touch First Picture

            //2. Roate 90 Degree
            //img90Deg.Source  = Utility.RotateDynaImage(img90Deg.Source ,90);
            WriteableBitmap wbmp;

            if (typeof(WriteableBitmap).Equals(img90Deg.Source))
                wbmp = (WriteableBitmap)img90Deg.Source;
            else
                wbmp = new WriteableBitmap(img90Deg.Source as BitmapSource);

            wbmp = WriteableBitmapExtensions.Rotate(wbmp, 270);
            img90Deg.Source = wbmp;
            wbmp = null;

            //3. Flip Horizontal and Flip Vertical
            WriteableBitmap wbmp1;
            if (typeof(WriteableBitmap).Equals(imgHV_Flip.Source))
                wbmp1 = (WriteableBitmap)imgHV_Flip.Source;
            else
                wbmp1 = new WriteableBitmap(imgHV_Flip.Source as BitmapSource);

            //wbmp = WriteableBitmapExtensions.Flip(wbmp, System.Windows.Media.Imaging.WriteableBitmapExtensions.FlipMode.Horizontal);
            //wbmp = WriteableBitmapExtensions.Flip(wbmp, System.Windows.Media.Imaging.WriteableBitmapExtensions.FlipMode.Vertical);
            wbmp1 = WriteableBitmapExtensions.Rotate(wbmp1, 180);
            imgHV_Flip.Source = wbmp1;
            wbmp1 = null;

            //4. Rotate -90 Degree
            //imgNeg90Deg.Source =  Utility.RotateDynaImage(imgNeg90Deg.Source ,-90);
            WriteableBitmap wbmp2;
            if (typeof(WriteableBitmap).Equals(imgNeg90Deg.Source))
                wbmp2 = (WriteableBitmap)imgNeg90Deg.Source;
            else
                wbmp2 = new WriteableBitmap(imgNeg90Deg.Source as BitmapSource);

            wbmp2 = WriteableBitmapExtensions.Rotate(wbmp2, 90);
            imgNeg90Deg.Source = wbmp2;
            wbmp2 = null;
        }

        // ------ Save Hologgram ----

        private void SaveFlyer(string FileName)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(FileName, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Design>));
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, GeneratePersonData());
                    }
                }
            }

            txtImageName.Text = FileName;
        }

        private List<Design> GeneratePersonData()
        {
            List<Design> data = new List<Design>();

            byte[] b = null;
            WriteableBitmap wbmp = new WriteableBitmap(img0Deg.Source as BitmapSource);

            using (MemoryStream ms = new MemoryStream())
            {
                wbmp.SaveJpeg(ms, wbmp.PixelWidth, wbmp.PixelHeight, 0, 70);
                ms.Seek(0, SeekOrigin.Begin);
                b = ms.GetBuffer();
            }

            data.Add(new Design()
            {
                ImageData = b, //wbmp.ToByteArray(),
                ImageCtrlWidth = img0Deg.Width,
                ImageCtrlHeight = img0Deg.Height,
                ImagePixelWidth = wbmp.PixelWidth,
                ImagePixelHeight = wbmp.PixelHeight,
                ImageType = "img0Deg"
            });
            wbmp = null;

            if (wbmp90 == null)
                wbmp90 = new WriteableBitmap(img90Deg.Source as BitmapSource);

            b = null;
            using (MemoryStream ms = new MemoryStream())
            {
                wbmp90.SaveJpeg(ms, wbmp90.PixelWidth, wbmp90.PixelHeight, 0, 70);
                ms.Seek(0, SeekOrigin.Begin);
                b = ms.GetBuffer();
            }

            data.Add(new Design()
            {
                ImageData = b, //wbmp90.ToByteArray(),
                ImageCtrlWidth = img90Deg.Width,
                ImageCtrlHeight = img90Deg.Height,
                ImagePixelWidth = wbmp90.PixelWidth,
                ImagePixelHeight = wbmp90.PixelHeight,
                ImageType = "img90Deg"
            });

            if (wbmpHVFlip == null)
                wbmpHVFlip = new WriteableBitmap(imgHV_Flip.Source as BitmapSource);

            b = null;
            using (MemoryStream ms = new MemoryStream())
            {
                wbmpHVFlip.SaveJpeg(ms, wbmpHVFlip.PixelWidth, wbmpHVFlip.PixelHeight, 0, 70);
                ms.Seek(0, SeekOrigin.Begin);
                b = ms.GetBuffer();
            }

            data.Add(new Design()
            {
                ImageData = b, //wbmpHVFlip.ToByteArray(),
                ImageCtrlWidth = imgHV_Flip.Width,
                ImageCtrlHeight = imgHV_Flip.Height,
                ImagePixelWidth = wbmpHVFlip.PixelWidth,
                ImagePixelHeight = wbmpHVFlip.PixelHeight,
                ImageType = "imgHV_Flip"
            });

            if (wbmpNeg90 == null)
                wbmpNeg90 = new WriteableBitmap(imgNeg90Deg.Source as BitmapSource);

            b = null;
            using (MemoryStream ms = new MemoryStream())
            {
                wbmpNeg90.SaveJpeg(ms, wbmpNeg90.PixelWidth, wbmpNeg90.PixelHeight, 0, 70);
                ms.Seek(0, SeekOrigin.Begin);
                b = ms.GetBuffer();
            }

            data.Add(new Design()
            {
                ImageData = b, //wbmpNeg90.ToByteArray(),
                ImageCtrlWidth = imgNeg90Deg.Width,
                ImageCtrlHeight = imgNeg90Deg.Height,
                ImagePixelWidth = wbmpNeg90.PixelWidth,
                ImagePixelHeight = wbmpNeg90.PixelHeight,
                ImageType = "imgNeg90Deg"
            });


            

            return data;
        }


        // ------ Open Hologgram ----

        private void OpenFlyer(String FileName)
        {
            try
            {
                txtImageName.Text = FileName;
                WorkingDesignName = FileName;
                //ApplicationTitle.Text = "DESIGNER :: " + FileName;

                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(FileName, FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Design>));
                        List<Design> data = (List<Design>)serializer.Deserialize(stream);

                        for (int i = 0; i < data.Count; i++)
                        {
                            try
                            {
                                Design imgcls = data[i];
                                MemoryStream msbg = new MemoryStream(imgcls.ImageData);

                                BitmapImage bitImagebg = new BitmapImage();
                                bitImagebg.SetSource(msbg);

                                if (imgcls.ImageType == "img0Deg")
                                    img0Deg.Source = bitImagebg; //wbmp;
                                else if (imgcls.ImageType == "img90Deg")
                                    img90Deg.Source = bitImagebg;
                                else if (imgcls.ImageType == "imgHV_Flip")
                                    imgHV_Flip.Source = bitImagebg;
                                else if (imgcls.ImageType == "imgNeg90Deg")
                                    imgNeg90Deg.Source = bitImagebg;


                            }
                            catch (Exception)
                            {
                                //add some code here
                            }
                        }
                    }
                }
            }
            catch
            {
                //add some code here
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