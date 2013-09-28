using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Live;
using Microsoft.Live.Controls;
using System.IO.IsolatedStorage;
using System.IO;

namespace Easy_Hologram_4D
{
    public partial class SavePage : PhoneApplicationPage
    {
        public SavePage()
        {
            InitializeComponent();
            this.Loaded += SavePage_Loaded;
        }

        void SavePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.ExitCall == true)
                btnCancel.Content = "Don't Save";
            else
                btnCancel.Content = "Cancel";

            txtFlyerName.Focus();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("HoloName"))
            {
                txtFlyerName.Text = this.NavigationContext.QueryString["HoloName"];
            }
        }

        private void btnSaveFlyer_Click(object sender, RoutedEventArgs e)
        {
            if (txtFlyerName.Text != string.Empty)
            {
                if (this.rbtnSaveDesign.IsChecked == true)
                    App.SaveType = "H";
                else if (this.rbtnSaveImage.IsChecked == true)
                    App.SaveType = "I";
                //else if (this.rbtnSaveInCloud.IsChecked == true)
                //    App.SaveType = "C";

                App.FromWhere = "SaveHolo";
                App.SaveFileName = txtFlyerName.Text;

                NavigationService.GoBack();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            App.FromWhere = "SaveHolo";
            App.SaveFileName = "";

            NavigationService.GoBack();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            App.FromWhere = "SaveHolo";
            App.SaveFileName = "";
        }

      
        
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            App.SaveType = "C";
            App.FromWhere = "SaveHolo";
            App.SaveFileName = txtFlyerName.Text;
            NavigationService.GoBack();
        }

        private void skydrive_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (e != null && e.Status == LiveConnectSessionStatus.Connected)
            {
                App.client = new LiveConnectClient(e.Session);
                this.GetAccountInformations();
                btnUpload.IsEnabled = true;
            }
            else
            {
                btnUpload.IsEnabled = false;
                App.client = null;

                if (rbtnSaveInCloud.IsChecked == true)
                    InfoText.Text = e.Error != null ? e.Error.Message : string.Empty;
            }
        }

        private async void GetAccountInformations()
        {
            try
            {
                LiveOperationResult operationResult = await App.client.GetAsync("me");
                var jsonResult = operationResult.Result as dynamic;
                string firstName = jsonResult.first_name ?? string.Empty;
                string lastName = jsonResult.last_name ?? string.Empty;
                InfoText.Text = "Welcome " + firstName + " " + lastName;
            }
            catch (Exception e)
            {
                InfoText.Text = e.ToString();
            }
        }


        private void rbtnSaveInCloud_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                SkyDrive.Visibility = Visibility.Visible;
                LocalDrive.Visibility = Visibility.Collapsed;
            }
            catch (Exception) { }
        }

        private void rbtnSaveDesign_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                LocalDrive.Visibility = Visibility.Visible;
                SkyDrive.Visibility = Visibility.Collapsed;
            }
            catch (Exception) { }
        }

        private void rbtnSaveImage_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                LocalDrive.Visibility = Visibility.Visible;
                SkyDrive.Visibility = Visibility.Collapsed;
            }
            catch (Exception) { }
        }


    }
}