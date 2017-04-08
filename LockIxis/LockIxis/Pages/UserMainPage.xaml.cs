using LockIxis.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace LockIxis.Pages
{
	public partial class UserMainPage : MasterDetailPage
	{
        User _user;
        QRCodeScanningPage _qrscanpageOnActionLock;
        QRCodeScanningPage _qrscanpageOnGenerateTransaction;

        public UserMainPage(User u)
		{
            _user = u;
            _qrscanpageOnActionLock = new QRCodeScanningPage(ActionLockClickedScanDelegate);
            _qrscanpageOnGenerateTransaction = new QRCodeScanningPage(GenerateTransactionClickedScanDelegate);
            BindingContext = new UserMainViewModel(_user);
			InitializeComponent ();
		}

        private void ShowMasterButtonClick(object sender, EventArgs e)
        {
            IsPresented = !IsPresented;
        }

        void ViewOwnLocksClicked(object sender, EventArgs e)
        {
        }

        void ViewCurrentTransactionsClicked(object sender, EventArgs e)
        {
        }

        async void ActionLockClicked(object sender, EventArgs eventargs)
        {
            NavigationPage.SetHasNavigationBar(_qrscanpageOnActionLock, false);
            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(_qrscanpageOnActionLock);
        }

        async void GenerateTransactionClicked(object sender, EventArgs e)
        {
            NavigationPage.SetHasNavigationBar(_qrscanpageOnActionLock, false);
            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(_qrscanpageOnGenerateTransaction);
        }

        public void ActionLockClickedScanDelegate(Result r)
        {
            try
            {
                _qrscanpageOnActionLock.IsScanning = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await LockIxisApp.GetInstance().MainPage.Navigation.PopAsync();
                    string lock_address = r.Text;
                    if (lock_address.Length > 0)
                    {
                        var answer = await DisplayAlert("Do you want to action this lock", r.Text, "Yes", "No");
                        if (answer == true)
                        {
                            await DisplayAlert("", String.Format("Connecting to {0}", lock_address), "OK");
                        }
                    }
                });
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GenerateTransactionClickedScanDelegate(Result r)
        {
            try
            {
                _qrscanpageOnActionLock.IsScanning = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await LockIxisApp.GetInstance().MainPage.Navigation.PopAsync();
                    string lock_address = r.Text;
                    if (lock_address.Length > 0)
                    {
                        var answer = await DisplayAlert("Do you want to generate a transaction for this lock", r.Text, "Yes", "No");
                        if (answer == true)
                        {
                            //await DisplayAlert("", String.Format("Connecting to {0}", lock_address), "OK");
                            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(new GenerateTransactionPage());
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
