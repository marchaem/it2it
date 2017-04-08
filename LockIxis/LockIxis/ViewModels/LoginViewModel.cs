using LockIxis.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using static ZXing.Net.Mobile.Forms.ZXingScannerPage;

namespace LockIxis.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        string _username;
        private RegisterPage _page; // HERE

        Command logInUserCommand;

        public LoginViewModel(RegisterPage page)
        {
            _page = page;
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged("Username"); }
        }


        public Command RegisterUserCommand
        {
            get
            {
                if (logInUserCommand == null)
                {
                    logInUserCommand = new Command(() => ExecuteSignUpUserCommand());
                }
                return logInUserCommand;
                //return logInUserCommand ?? (logInUserCommand = new Command(async () => await ExecuteSignInUserCommand()));
            }
        }

        public void ExecuteSignUpUserCommand()
        {
            var user = new User(Username);
            string _scannedQR = "";

            #region QR options
            var options = new MobileBarcodeScanningOptions
            {
                TryInverted = true,
                AutoRotate = false,
                UseFrontCameraIfAvailable = false,
                TryHarder = true,
                PossibleFormats = new List<ZXing.BarcodeFormat>
                {
                    ZXing.BarcodeFormat.QR_CODE
                }
            };
            #endregion

            var scanPage = new ZXingScannerPage(options)
            {
                DefaultOverlayTopText = "Align the barcode within the frame",
                DefaultOverlayBottomText = "Bottom text",
                DefaultOverlayShowFlashButton = true,
            };
            scanPage.OnScanResult += (result) =>
            {
                // Stop scanning
                scanPage.IsScanning = false;
                // Pop the page and show the result
                _page.Navigation.PopAsync();
                Device.BeginInvokeOnMainThread(() =>
                {
                    _scannedQR = result.Text;
                    if (_scannedQR.Length > 0)
                    {
                        user.InitialiseCryptoParameters(_scannedQR);
                        LockIxisApp.Current.MainPage = LockIxisApp.FetchMainUI(user);
                    }
                });
            };

            NavigationPage.SetHasNavigationBar(scanPage, false);
            _page.Navigation.PushAsync(scanPage);
        }



    }
}
