using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using Xamarin.Forms;

namespace LockIxis.Pages
{
	public partial class LockActionsPagexaml : ContentPage
	{
        QRCodeScanningPage _qrscanpageOnActionLock;
        QRCodeScanningPage _qrscanpageOnGenerateTransaction;
        MasterDetailPage _rootpage;

        public LockActionsPagexaml (MasterDetailPage mdp)
		{
            _rootpage = mdp;
            _qrscanpageOnActionLock = new QRCodeScanningPage(ActionLockClickedScanDelegate);
            _qrscanpageOnGenerateTransaction = new QRCodeScanningPage(GenerateTransactionClickedScanDelegate);
            InitializeComponent();
		}

        public void ShowMasterButtonClick(object sender, EventArgs eventargs)
        {
            _rootpage.IsPresented = !_rootpage.IsPresented;
        }

        async void ActionLockClicked(object sender, EventArgs eventargs)
        {
            NavigationPage.SetHasNavigationBar(_qrscanpageOnActionLock, false);
            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(_qrscanpageOnActionLock);
        }

        async void GenerateTransactionClicked(object sender, EventArgs e)
        {
            NavigationPage.SetHasNavigationBar(_qrscanpageOnGenerateTransaction, false);
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GenerateTransactionClickedScanDelegate(Result r)
            
        {
            try
            {
                _qrscanpageOnGenerateTransaction.IsScanning = false;
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
                            var generateTransactionPage = new GenerateTransactionPage(lock_address);
                            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(generateTransactionPage);
                            //if(await generateTransactionPage.TransactionIsGenerated())
                            //{
                            await generateTransactionPage.OnGenerateTransaction(null,);
                            //}

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
