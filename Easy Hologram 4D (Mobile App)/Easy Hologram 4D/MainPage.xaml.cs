using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.IO;

namespace Easy_Hologram_4D
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            //if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            //{
            //    if (App.ChangesSaved)
            //    {
            //        LoadDesigns();
            //    }
            //}
            //else
            //{
            LoadDesigns();
            //}

        }

        private void New4D_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/4Dimage.xaml", UriKind.Relative));
        }

        private void btnTileOpenFlyer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/4DImage.xaml?FileName=" + ((e.OriginalSource as Button).Content as TextBlock).Text, UriKind.Relative));
        }

        private void ContextMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HologramData DelTile = (sender as MenuItem).DataContext as HologramData;
                if (!lstStd.Remove(DelTile))
                {
                    foreach (HologramData std in lstStd)
                    {
                        if (std.Name == DelTile.Name)
                        {
                            lstStd.RemoveAt(lstStd.IndexOf(std));
                            break;
                        }
                    }
                }


                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    myIsolatedStorage.DeleteFile(DelTile.Name);
                }

                lstData.ItemsSource = null;
                lstData.ItemsSource = lstStd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Designer :: Design deleting error", MessageBoxButton.OK);
            }
        }

        private void LoadDesigns()
        {
            try
            {
                lstData.ItemsSource = null;
                lstData.ItemsSource = GetAllDesigns();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Designer :: Design Loading error", MessageBoxButton.OK);
            }
        }

        List<HologramData> lstStd = new List<HologramData> { };
        private List<HologramData> GetAllDesigns()
        {
            lstStd.Clear();
            string[] Flyers;
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            Flyers = myIsolatedStorage.GetFileNames();

            foreach (string FlyerName in Flyers)
            {
                ImageBrush Imgb = new ImageBrush();
                
                try
                {
                    IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(FlyerName, FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Design>));
                    List<Design> data = (List<Design>)serializer.Deserialize(stream);

                    Design imgclsThumbnail = data[0];
                    MemoryStream msbg = new MemoryStream(imgclsThumbnail.ImageData);

                    BitmapImage bitImagebg = new BitmapImage();
                    bitImagebg.SetSource(msbg);

                    Imgb.ImageSource = bitImagebg;

                    msbg.Flush();
                    msbg.Close();
                    msbg.Dispose();
                    imgclsThumbnail = null;

                    stream.Flush();
                    stream.Close();
                    stream.Dispose();
                }
                catch (Exception)
                {
                    Imgb.ImageSource = new BitmapImage(new Uri("Assets\\Tiles\\0Deg Image.png", UriKind.RelativeOrAbsolute));
                }


                lstStd.Add(new HologramData { Name = FlyerName, FirstImage = Imgb });
            }


            myIsolatedStorage.Dispose();
            return lstStd;
        }
        
    }


    public class HologramData
    {
        public string Name { get; set; }
        public ImageBrush FirstImage { get; set; }
    }

}